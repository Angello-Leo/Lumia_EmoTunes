using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.Web.WebView2.Core;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;

namespace Lumia_EmoTunes
{
    public partial class frmMain : Form
    {
        private readonly IConfiguration _configuration;

        private bool _hasCalibrated = false;
        private VideoCapture capture;
        private Mat frame;
        private System.Windows.Forms.Timer cameraTimer;
        private CascadeClassifier faceCascade;
        private InferenceSession _onnxSession;
        private string[] _emotions = { "Neutral", "Happy", "Surprise", "Sad", "Angry", "Disgust", "Fear", "Contempt" };

        private bool isCalibrating = false;
        private string baselineEmotion = "";
        private int calibrationFrames = 0;
        private Dictionary<string, int> calibrationCounts = new Dictionary<string, int>();
        private DateTime _calibrationEndTime = DateTime.MaxValue;
        private bool _isWaitingAfterCalibration = false;

        string _currentlyPlayingGenre = "Lofi";
        string _currentPlayingEmotion = "";
        DateTime _nextAllowedSearchTime = DateTime.MinValue;

        private string[] _testGenres = { "Lofi", "Jazz", "Acoustic", "Electronic", "Rock", "Classical" };
        private bool _isGenreSelectionActive = false;
        private int currentUserId = 1;

        private List<Point2f> _headPath = new List<Point2f>();
        private const int PathBufferLimit = 30;
        private bool _gestureProcessed = false;

        private Dictionary<string, int> _feedbackPopupCounts = new Dictionary<string, int>
        {
            { "Happy", 0 }, { "Sad", 0 }, { "Neutral", 0 }, { "Angry", 0 }
        };

        private Stack<string> _playHistory = new Stack<string>();
        private string _currentVideoId = "";
        private bool _isPreTestComplete = false;

        private DateTime _lastGestureTime = DateTime.MinValue;
        private const int GestureCooldownMs = 1500;

        CascadeClassifier eyeCascade = new CascadeClassifier("haarcascade_eye.xml");
        DateTime? _eyesClosedStartTime = null;
        DateTime? _lookingAwayStartTime = null;

        private  DateTime _preTestMusicStartTime = DateTime.MinValue;
        private const int PreTestCooldownSeconds = 30;

        public frmMain()
        {
            InitializeComponent();
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            InitializeBrowser();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            SetupCamera();
        }

        private async void InitializeBrowser()
        {
            try
            {
                var options = new CoreWebView2EnvironmentOptions("--autoplay-policy=no-user-gesture-required");
                var env = await CoreWebView2Environment.CreateAsync(null, null, options);

                await musicPlayer.EnsureCoreWebView2Async(env);

                musicPlayer.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "emotunes.local",
                    Application.StartupPath,
                    CoreWebView2HostResourceAccessKind.Allow);

                musicPlayer.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

                musicPlayer.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
                musicPlayer.CoreWebView2.WebResourceRequested += (s, args) =>
                {
                    args.Request.Headers.SetHeader("Referer", "https://emotunes.local/");
                    args.Request.Headers.SetHeader("Origin", "https://emotunes.local");
                };
            }
            catch (Exception ex) { MessageBox.Show("Browser Init Failed: " + ex.Message); }
        }

        private void SetupCamera()
        {
            _onnxSession = new InferenceSession("emotion-ferplus-8.onnx");
            faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

            capture = new VideoCapture(0);
            capture.FrameWidth = 640;
            capture.FrameHeight = 480;
            frame = new Mat();

            cameraTimer = new System.Windows.Forms.Timer();
            cameraTimer.Interval = 33;
            cameraTimer.Tick += CameraTimer_Tick;
            cameraTimer.Start();
            StartCalibration();
        }

        private async void StartCalibration()
        {
            isCalibrating = true;
            calibrationFrames = 0;
            calibrationCounts.Clear();
            calibrationCounts["Happy"] = 0;
            calibrationCounts["Sad"] = 0;
            calibrationCounts["Angry"] = 0;
            calibrationCounts["Neutral"] = 0;

            picFeedbackBox.Visible = true;
            lblPretest.Visible = true;
            lblPretest.BringToFront();

            await Task.Delay(3500);

            lblPretest.Visible = false;
            lblPretest.SendToBack();
        }

        private void CameraTimer_Tick(object sender, EventArgs e)
        {
            if (capture != null && capture.IsOpened())
            {
                capture.Read(frame);
                if (!frame.Empty())
                {
                    Cv2.Flip(frame, frame, FlipMode.Y);
                    DetectEmotion(frame);

                    if (pictureBox1.Visible)
                    {
                        try
                        {
                            Bitmap bitmap = BitmapConverter.ToBitmap(frame);
                            var oldImage = pictureBox1.Image;
                            pictureBox1.Image = bitmap;
                            oldImage?.Dispose();
                        }
                        catch { }
                    }
                }
            }
        }

        private void DetectHeadGesture(Rect faceRect)
        {
            if ((DateTime.Now - _lastGestureTime).TotalMilliseconds < GestureCooldownMs)
            {
                _headPath.Clear();
                return;
            }

            Point2f currentCenter = new Point2f(faceRect.X + faceRect.Width / 2, faceRect.Y + faceRect.Height / 2);
            _headPath.Add(currentCenter);

            if (_headPath.Count > PathBufferLimit) _headPath.RemoveAt(0);

            if (_headPath.Count == PathBufferLimit && !_gestureProcessed)
            {
                float deltaX = _headPath.Max(p => p.X) - _headPath.Min(p => p.X);
                float deltaY = _headPath.Max(p => p.Y) - _headPath.Min(p => p.Y);

                if (deltaY > 55 && deltaY > deltaX * 2.0)
                {
                    ExecuteGesture(true);
                }
                else if (deltaX > 55 && deltaX > deltaY * 2.0)
                {
                    ExecuteGesture(false);
                }
            }
        }

        private void ExecuteGesture(bool isNod)
        {
            _lastGestureTime = DateTime.Now;
            _headPath.Clear();

            HandleGestureFeedback(isNod);

            if (isNod)
            {
                Console.WriteLine("Confirmed: Action Taken");
            }
            else
            {
                Console.WriteLine("Confirmed: Skipping Song");
            }
        }

        private void HandleGestureFeedback(bool isNod)
        {
            if (_gestureProcessed) return;
            _gestureProcessed = true;

            if (!_isPreTestComplete)
            {
                if (isNod)
                {
                    Debug.WriteLine("Pre-Test: Genre Accepted.");
                    _isPreTestComplete = true;
                    _isGenreSelectionActive = false;

                    SaveInitialPreferences(_currentlyPlayingGenre);
                    ShowFeedbackUI(false);
                    TriggerSong(_currentPlayingEmotion);
                }
                else
                {
                    double elapsed = (DateTime.Now - _preTestMusicStartTime).TotalSeconds;

                    if (elapsed >= PreTestCooldownSeconds || _preTestMusicStartTime == DateTime.MinValue)
                    {
                        RotateToNextGenre();
                        TriggerNextSong();
                        _preTestMusicStartTime = DateTime.Now;
                    }
                    else
                    {
                        Debug.WriteLine($"Shake ignored: {PreTestCooldownSeconds - (int)elapsed}s remaining.");
                    }
                }
            }
            else
            {
                if (isNod) { LogFeedback(_currentPlayingEmotion, _currentlyPlayingGenre, true); TriggerNextSong(); }
                else { LogFeedback(_currentPlayingEmotion, _currentlyPlayingGenre, false); PlayPreviousSong(); }

                ShowFeedbackUI(false);
            }

            Task.Run(async () => {
                await Task.Delay(GestureCooldownMs);
                this.Invoke(new Action(() => { _gestureProcessed = false; }));
            });
        }

        private async void TriggerNextSong()
        {
            if (!string.IsNullOrEmpty(_currentVideoId))
                _playHistory.Push(_currentVideoId);

            string videoId = await GetMoodVideoId(_currentPlayingEmotion);
            _currentVideoId = videoId;
            TriggerSongWithId(videoId);
        }

        private void PlayPreviousSong()
        {
            if (_playHistory.Count > 0)
            {
                string prevId = _playHistory.Pop();
                _currentVideoId = prevId;
                TriggerSongWithId(prevId);
            }
            else
            {
                Debug.WriteLine("No previous songs in history.");
            }
        }

        private void DetectEmotion(Mat img)
        {
            using (var gray = new Mat())
            {
                Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
                var faces = faceCascade.DetectMultiScale(gray, 1.1, 4);

                if (faces.Length == 0)
                {
                    _eyesClosedStartTime = null;
                    CheckForLookingAway(img);
                    return;
                }

                if (_lookingAwayStartTime != null)
                {
                    _lookingAwayStartTime = null;
                    this.BeginInvoke(new Action(() =>
                    {
                        picFeedbackBox.Visible = false;
                        lblWarning.Visible = false;
                    }));
                }

                var rect = faces.OrderByDescending(f => f.Width * f.Height).First();

                Cv2.Rectangle(img, rect, Scalar.LimeGreen, 2);
                CheckForEyesClosedExit(img, rect);

                if (!_hasCalibrated && !isCalibrating) StartCalibration();

                using (Mat faceROI = new Mat(gray, rect))
                using (Mat resizedFace = new Mat())
                {
                    Cv2.Resize(faceROI, resizedFace, new OpenCvSharp.Size(64, 64));

                    var inputTensor = new DenseTensor<float>(new[] { 1, 1, 64, 64 });
                    for (int y = 0; y < 64; y++)
                        for (int x = 0; x < 64; x++)
                            inputTensor[0, 0, y, x] = (float)resizedFace.At<byte>(y, x);

                    var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("Input3", inputTensor) };

                    using (var results = _onnxSession.Run(inputs))
                    {
                        var output = results.First().AsEnumerable<float>().ToArray();

                        int neutralIndex = Array.IndexOf(_emotions, "Neutral");
                        int sadIndex = Array.IndexOf(_emotions, "Sad");
                        int angryIndex = Array.IndexOf(_emotions, "Angry");
                        int happyIndex = Array.IndexOf(_emotions, "Happy");

                        if (output[sadIndex] > (output[neutralIndex] * 0.4f))
                        {
                            output[sadIndex] += 10.0f;
                            output[neutralIndex] -= 5.0f;
                        }
                        else if (output[angryIndex] > (output[neutralIndex] - 1.0f))
                        {
                            output[angryIndex] += 5.0f;
                        }

                        int maxIndex = Array.IndexOf(output, output.Max());
                        string finalEmotion = MapToFourEmotions(_emotions[maxIndex]);

                        HandleAppStates(img, rect, finalEmotion);
                    }
                }
                DetectHeadGesture(rect);
            }
        }

        private void CheckForLookingAway(Mat img)
        {
            if (_lookingAwayStartTime == null)
            {
                _lookingAwayStartTime = DateTime.Now;
            }
            else
            {
                double elapsed = (DateTime.Now - _lookingAwayStartTime.Value).TotalSeconds;
                if (elapsed >= 1.5)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        if (!lblWarning.Visible)
                        {
                            picFeedbackBox.Visible = true;
                            lblWarning.Visible = true;
                            picFeedbackBox.BringToFront();
                            lblWarning.BringToFront();

                            System.Media.SystemSounds.Exclamation.Play();

                        }
                    }));
                }
            }
        }

        private void HandleAppStates(Mat img, Rect rect, string finalEmotion)
        {
            if (isCalibrating)
            {
                if (calibrationCounts.ContainsKey(finalEmotion)) calibrationCounts[finalEmotion]++;
                calibrationFrames++;
                int progress = (calibrationFrames * 100) / 50;

                Cv2.PutText(img, $"Tuning Face: {progress}%", new OpenCvSharp.Point(rect.X, rect.Y - 10),
                    HersheyFonts.HersheyComplex, 0.5, Scalar.Yellow, 1);

                if (calibrationFrames >= 50)
                {
                    isCalibrating = false;
                    _hasCalibrated = true;
                    baselineEmotion = calibrationCounts.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                    if (HasCompletedPreTest(currentUserId, "Neutral"))
                    {
                        _currentlyPlayingGenre = GetSavedGenre(currentUserId, "Neutral");
                        _isWaitingAfterCalibration = false;
                        _nextAllowedSearchTime = DateTime.Now;
                        _isPreTestComplete = true;
                    }
                    else
                    {
                        _isWaitingAfterCalibration = true;
                        _calibrationEndTime = DateTime.Now.AddSeconds(5);
                    }
                }
                return;
            }
            else if (_isWaitingAfterCalibration)
            {
                double secondsLeft = (_calibrationEndTime - DateTime.Now).TotalSeconds;
                if (secondsLeft > 0)
                {
                    Cv2.PutText(img, $"Get Ready: {Math.Ceiling(secondsLeft)}s", new OpenCvSharp.Point(rect.X, rect.Y - 10),
                        HersheyFonts.HersheyComplex, 0.5, Scalar.Cyan, 1);
                }
                else
                {
                    _isWaitingAfterCalibration = false;
                    _isPreTestComplete = false;
                    _isGenreSelectionActive = true;
                    _currentlyPlayingGenre = _testGenres[0];
                    _preTestMusicStartTime = DateTime.Now;

                    ShowFeedbackUI(true);
                    _nextAllowedSearchTime = DateTime.Now;
                    TriggerNextSong();
                }
                return;
            }
            else
            {
                if (!_isPreTestComplete)
                {
                    double elapsed = (DateTime.Now - _preTestMusicStartTime).TotalSeconds;

                    if (elapsed < PreTestCooldownSeconds && _preTestMusicStartTime != DateTime.MinValue)
                    {
                        int secondsLeft = PreTestCooldownSeconds - (int)elapsed;
                        Cv2.PutText(img, $"Listen to {_currentlyPlayingGenre}: {secondsLeft}s left",
                            new OpenCvSharp.Point(rect.X, rect.Y - 10),
                            HersheyFonts.HersheyComplex, 0.5, Scalar.LimeGreen, 1);
                    }
                    else
                    {
                        ShowMood(img, rect, finalEmotion);
                    }
                }
                else
                {
                    ShowMood(img, rect, finalEmotion);
                }
            }
        }

        private void ShowMood(Mat img, Rect rect, string finalEmotion)
        {

            this.Invoke((MethodInvoker)delegate {
                lblMoodDisplay.Visible = true;
                lblMoodDisplay.Text = $"Mood: {finalEmotion}";
                lblMoodDisplay.ForeColor = Color.Black;
                lblMoodDisplay.BackColor = Color.Transparent;
                lblMoodDisplay.Font = new Font("Iceberg", 20, FontStyle.Bold);

                lblMoodDisplay.Location = new System.Drawing.Point(1030, 23);
            });
            Cv2.PutText(img, $"Mood: {finalEmotion}",
                new OpenCvSharp.Point(rect.X, rect.Y - 10),
                HersheyFonts.HersheyComplex, 0.5, Scalar.LimeGreen, 1);

            if (finalEmotion != _currentPlayingEmotion && DateTime.Now >= _nextAllowedSearchTime)
            {
                TriggerSong(finalEmotion);
            }
        }

        private async void TriggerSong(string mood)
        {
            _currentPlayingEmotion = mood;
            _nextAllowedSearchTime = DateTime.Now.AddMinutes(3);

            if (_feedbackPopupCounts.ContainsKey(mood) && _feedbackPopupCounts[mood] < 3)
            {
                ShowFeedbackUI(true);
                _feedbackPopupCounts[mood]++;
            }
            else
            {
                ShowFeedbackUI(false);
            }

            string videoId = await GetMoodVideoId(mood);
            if (!string.IsNullOrEmpty(videoId))
            {
                TriggerSongWithId(videoId);
            }
        }

        private void ShowFeedbackUI(bool visible)
        {
            this.Invoke(new Action(() => {
                picFeedbackBox.Visible = visible;
                lblFeedbackInstruction.Visible = visible;

                if (visible)
                {
                    picFeedbackBox.BringToFront();
                    lblFeedbackInstruction.BringToFront();
                }
            }));
        }

        private void TriggerSongWithId(string videoId)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (musicPlayer?.CoreWebView2 != null)
                {
                    string html = LoadYoutubeHtml(videoId);
                    musicPlayer.CoreWebView2.NavigateToString(html);
                }
            }));
        }

        private string LoadYoutubeHtml(string videoId)
        {
            return "<!DOCTYPE html>" +
                   "<html>" +
                   "<head>" +
                   "    <style>" +
                   "        body, html { margin: 0; padding: 0; height: 100%; width: 100%; background-color: black; overflow: hidden; }" +
                   "        #player { position: absolute; top: 0; left: 0; width: 100%; height: 100%; }" +
                   "    </style>" +
                   "</head>" +
                   "<body>" +
                   "    <div id='player'></div>" +
                   "    <script src='https://www.youtube.com/iframe_api'></script>" +
                   "    <script>" +
                   "        var player;" +
                   "        function onYouTubeIframeAPIReady() {" +
                   "            player = new YT.Player('player', {" +
                   "                videoId: '" + videoId + "'," +
                   "                playerVars: {" +
                   "                    'autoplay': 1," +
                   "                    'mute': 1," +
                   "                    'controls': 0," +
                   "                    'modestbranding': 1," +
                   "                    'rel': 0," +
                   "                    'enablejsapi': 1" +
                   "                }," +
                   "                events: {" +
                   "                    'onReady': (event) => {" +
                   "                        event.target.playVideo();" +
                   "                        setTimeout(() => event.target.unMute(), 100);" +
                   "                    }," +
                   "                    'onStateChange': (event) => {" +
                   "                        if (event.data === YT.PlayerState.PAUSED) { event.target.playVideo(); }" +
                   "                    }" +
                   "                }" +
                   "            });" +
                   "        }" +
                   "    </script>" +
                   "</body>" +
                   "</html>";
        }

        private string MapToFourEmotions(string raw)
        {
            switch (raw)
            {
                case "Happy": case "Surprise": return "Happy";
                case "Sad": case "Fear": return "Sad";
                case "Angry": case "Disgust": return "Angry";
                default: return "Neutral";
            }
        }

        private void SaveInitialPreferences(string selectedGenre)
        {
            string connectionString = "Data Source=EmoTunes.db";
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = @"
            INSERT OR REPLACE INTO UserPreferences (UserId, Mood, PreferredGenre, TriesCount)
            VALUES (@userId, @mood, @genre, 0);";

                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@mood", "Neutral");
                command.Parameters.AddWithValue("@genre", selectedGenre);

                command.ExecuteNonQuery();
            }

            MessageBox.Show($"Great! We'll start your journey with some {selectedGenre}.");
            _isGenreSelectionActive = false;
        }

        private void LogFeedback(string currentMood, string currentGenre, bool liked)
        {
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=EmoTunes.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO FeedbackHistory (UserId, Mood, Genre, IsPositive)
            VALUES (@userId, @mood, @genre, @liked);";

                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@mood", currentMood);
                command.Parameters.AddWithValue("@genre", currentGenre);
                command.Parameters.AddWithValue("@liked", liked);

                command.ExecuteNonQuery();
            }
        }

        private async Task<string> GetMoodVideoId(string emotion)
        {
            string apiKey = _configuration["YouTubeSettings:ApiKey"];
            string appName = _configuration["YouTubeSettings:ApplicationName"];

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = appName
            });

            string specificVibe = "";

            switch (emotion)
            {
                case "Happy":
                    specificVibe = "Trending Global Pop Billboard Hits high energy";
                    break;
                case "Sad":
                    specificVibe = "OPM Relapse Hugot playlist";
                    break;
                case "Angry":
                    specificVibe = "Modern Rock Metal high intensity official audio";
                    break;
                default:
                    specificVibe = $"{_currentlyPlayingGenre} aesthetic vibe Chill";
                    break;
            }

            string query = $"{specificVibe}";

            var searchRequest = youtubeService.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.Type = "video";
            searchRequest.MaxResults = 10;
            searchRequest.VideoCategoryId = "10";
            searchRequest.RegionCode = "PH";

            var orders = new[] {
                SearchResource.ListRequest.OrderEnum.Relevance,
                SearchResource.ListRequest.OrderEnum.ViewCount
            };

            Random rng = new Random();
            searchRequest.Order = orders[rng.Next(orders.Length)];

            var response = await searchRequest.ExecuteAsync();
                
            if (response.Items.Count > 0)
            {
                int randomIndex = rng.Next(response.Items.Count);
                return response.Items[randomIndex].Id.VideoId;
            }

            return null;
        }

        private void RotateToNextGenre()
        {
            int currentIndex = Array.IndexOf(_testGenres, _currentlyPlayingGenre);
            int nextIndex = (currentIndex + 1) % _testGenres.Length;
            _currentlyPlayingGenre = _testGenres[nextIndex];

            UpdateDatabaseGenre(_currentPlayingEmotion, _currentlyPlayingGenre);
            Debug.WriteLine($"System Rotated Genre to: {_currentlyPlayingGenre}");
        }

        private void UpdateDatabaseGenre(string mood, string newGenre)
        {
            string connectionString = "Data Source=EmoTunes.db";
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE UserPreferences 
            SET PreferredGenre = @genre, TriesCount = 0 
            WHERE UserId = @userId AND Mood = @mood;";

                command.Parameters.AddWithValue("@userId", currentUserId);
                command.Parameters.AddWithValue("@mood", mood);
                command.Parameters.AddWithValue("@genre", newGenre);

                command.ExecuteNonQuery();
            }
        }

        private bool HasCompletedPreTest(int userId, string mood)
        {
            string connectionString = "Data Source=EmoTunes.db";
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM UserPreferences WHERE UserId = @uid AND Mood = @mood AND PreferredGenre IS NOT NULL";
                command.Parameters.AddWithValue("@uid", userId);
                command.Parameters.AddWithValue("@mood", mood);

                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }

        private string GetSavedGenre(int userId, string mood)
        {
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=EmoTunes.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT PreferredGenre FROM UserPreferences WHERE UserId = @uid AND Mood = @mood";
                command.Parameters.AddWithValue("@uid", userId);
                command.Parameters.AddWithValue("@mood", mood);
                return command.ExecuteScalar()?.ToString() ?? _testGenres[0];
            }
        }

        private void CheckForEyesClosedExit(Mat img, Rect faceRect)
        {
            Rect safeRect = faceRect.Intersect(new Rect(0, 0, img.Width, img.Height));
            if (safeRect.Width <= 0 || safeRect.Height <= 0) return;

            using (Mat faceRegion = new Mat(img, safeRect))
            {
                var eyes = eyeCascade.DetectMultiScale(
                    faceRegion,
                    1.1,
                    10,
                    HaarDetectionTypes.DoRoughSearch,
                    new OpenCvSharp.Size(30, 30)
                );

                bool eyesVisible = eyes.Length > 0;

                if (!eyesVisible)
                {
                    if (_eyesClosedStartTime == null)
                    {
                        _eyesClosedStartTime = DateTime.Now;
                    }
                    else
                    {
                        double elapsed = (DateTime.Now - _eyesClosedStartTime.Value).TotalSeconds;
                        string countdownText = $"Closing in: {Math.Max(0, 5 - (int)elapsed)}s";

                        Cv2.PutText(img, countdownText, new OpenCvSharp.Point(faceRect.X, faceRect.Y - 20),
                            HersheyFonts.HersheyComplex, 0.6, Scalar.Red, 2);

                        // Fixed: Shut down application gracefully after a full 5 seconds
                        if (elapsed >= 5.0)
                        {
                            _eyesClosedStartTime = null;
                            this.Invoke(new Action(() => {
                                cameraTimer?.Stop();
                                capture?.Release();
                                new FrmClosing().Show();
                                this.Hide();
                            }));
                        }
                    }
                }
                else
                {
                    _eyesClosedStartTime = null;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            cameraTimer?.Stop();
            capture?.Release();
            frame?.Dispose();
            _onnxSession?.Dispose();
            base.OnFormClosing(e);
        }
    }
}