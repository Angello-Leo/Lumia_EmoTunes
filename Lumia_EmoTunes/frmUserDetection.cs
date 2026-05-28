using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Face;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Lumia_EmoTunes
{
    public partial class frmUserDetection : Form
    {
        private VideoCapture capture;
        private Mat frame;
        private System.Windows.Forms.Timer loginCameraTimer;
        private CascadeClassifier faceCascade;

        private Label lblUserFound;
        private Label lblUserNotFound;
        private Label lblInstructions;
        private TextBox textBoxUsername;
        private PictureBox picLoginPreview;

        private LBPHFaceRecognizer recognizer;
        private bool isTrained = false;
        private bool isTransitioning = false;

        private string lastDetectedUser = "";
        private int consecutiveUserMatchCount = 0;
        private const int RequiredConsecutiveFrames = 5;

        private int cameraWarmUpFrames = 0;
        private const int WarmUpThreshold = 45;

        private const double StrictConfidenceCutoff = 46.0; 

        private bool isCapturing = false;
        private bool isRegisteringNewUser = false;
        private int captureCount = 0;
        private const int MaxCaptures = 15;
        private List<Mat> trainingImages = new List<Mat>();
        private string currentNewUserName = "";
        private readonly string trainingDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TrainingData");

        public frmUserDetection()
        {
            InitializeComponent();
            InitializeDatabase();

            if (File.Exists("haarcascade_frontalface_default.xml"))
                faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");

            //LayoutPictureBox();
            CreateManualControls();

            recognizer = LBPHFaceRecognizer.Create(1, 8, 8, 8, 80.0);
            LoadTrainedModel();

            if (pictureBox1 != null) pictureBox1.SendToBack();
        }

        private void LoadTrainedModel()
        {
            if (File.Exists("face_model.yml"))
            {
                recognizer.Read("face_model.yml");
                isTrained = true;
            }
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=EmoTunes.db";
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            {
                connection.Open();
                var pragmaCommand = connection.CreateCommand();
                pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
                pragmaCommand.ExecuteNonQuery();

                var command = connection.CreateCommand();
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
                command.ExecuteNonQuery();

                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS UserPreferences (
                    UserId INTEGER,
                    Mood TEXT,
                    TriesCount INTEGER DEFAULT 0,
                    PreferredGenre TEXT,
                    LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (UserId, Mood),
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );";
                command.ExecuteNonQuery();

                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS FeedbackHistory (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER,
                    Mood TEXT,
                    Genre TEXT,
                    IsPositive BOOLEAN,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );";
                command.ExecuteNonQuery();
            }
        }

        private string GetNameFromDb(int label)
        {
            if (label < 0) return "Unknown";

            string connectionString = "Data Source=EmoTunes.db";
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Name FROM Users WHERE Id = @id";
                command.Parameters.AddWithValue("@id", label);

                var result = command.ExecuteScalar();
                return result != null ? result.ToString() : "Unknown";
            }
        }

        private void frmUserDetection_Load(object sender, EventArgs e)
        {
            SetupLoginCamera();
        }

        private void SetupLoginCamera()
        {
            cameraWarmUpFrames = 0;
            capture = new VideoCapture(0);
            capture.FrameWidth = 640;
            capture.FrameHeight = 480;
            frame = new Mat();

            loginCameraTimer = new System.Windows.Forms.Timer();
            loginCameraTimer.Interval = 33;
            loginCameraTimer.Tick += LoginCameraTimer_Tick;
            loginCameraTimer.Start();
        }

        private void CreateManualControls()
        {
            picLoginPreview = new PictureBox();
            picLoginPreview.Anchor = AnchorStyles.None;
            picLoginPreview.Location = new System.Drawing.Point(50, 50);
            picLoginPreview.Name = "picLoginPreview";
            picLoginPreview.Size = new System.Drawing.Size(640, 480);
            picLoginPreview.TabIndex = 1;
            picLoginPreview.TabStop = false;
            picLoginPreview.BorderStyle = BorderStyle.FixedSingle;
            picLoginPreview.BringToFront();

            this.Controls.Add(picLoginPreview);

            lblUserFound = new Label();
            lblUserFound.Anchor = AnchorStyles.None;
            lblUserFound.AutoSize = true;
            lblUserFound.BackColor = Color.Transparent;
            lblUserFound.Font = new Font("Iceberg", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUserFound.Location = new System.Drawing.Point(720, 140);
            lblUserFound.Name = "lblUserFound";
            lblUserFound.Text = "Hey there, Your dashboard is ready.";
            lblUserFound.Visible = false;

            lblUserNotFound = new Label();
            lblUserNotFound.Anchor = AnchorStyles.None;
            lblUserNotFound.AutoSize = true;
            lblUserNotFound.BackColor = Color.Transparent;
            lblUserNotFound.Font = new Font("Iceberg", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUserNotFound.Location = new System.Drawing.Point(720, 140);
            lblUserNotFound.Name = "lblUserNotFound";
            lblUserNotFound.Text = "Hmm.. I can't recognize you";
            lblUserNotFound.Visible = false;

            lblInstructions = new Label();
            lblInstructions.Anchor = AnchorStyles.None;
            lblInstructions.AutoSize = true;
            lblInstructions.BackColor = Color.Transparent;
            lblInstructions.Font = new Font("Iceberg", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblInstructions.Location = new System.Drawing.Point(720, 219);
            lblInstructions.Name = "lblInstructions";
            lblInstructions.Text = "How can I address you?";

            textBoxUsername = new TextBox();
            textBoxUsername.Anchor = AnchorStyles.None;
            textBoxUsername.Font = new Font("Iceberg", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxUsername.Location = new System.Drawing.Point(720, 306);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.PlaceholderText = "Enter name here";
            textBoxUsername.Size = new System.Drawing.Size(396, 44);
            textBoxUsername.Visible = false;

            this.Controls.Add(lblUserFound);
            this.Controls.Add(lblUserNotFound);
            this.Controls.Add(lblInstructions);
            this.Controls.Add(textBoxUsername);
            textBoxUsername.KeyDown += TextBoxUsername_KeyDown;
            picLoginPreview.BringToFront();
        }

        private void LoginCameraTimer_Tick(object sender, EventArgs e)
        {
            if (capture != null && capture.IsOpened())
            {
                capture.Read(frame);
                if (!frame.Empty())
                {
                    Cv2.Flip(frame, frame, FlipMode.Y);

                    picLoginPreview.Image?.Dispose();
                    picLoginPreview.Image = BitmapConverter.ToBitmap(frame);

                    if (isCapturing)
                    {
                        ProcessCapture(frame);
                    }
                    else if (cameraWarmUpFrames >= WarmUpThreshold)
                    {
                        DetectUser(frame);
                    }
                    else
                    {
                        cameraWarmUpFrames++;
                    }
                }
            }
        }

        private void DetectUser(Mat image)
        {
            if (faceCascade == null || isCapturing || isTransitioning || isRegisteringNewUser) return;

            using (var gray = new Mat())
            {
                Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

                // Quality Check: If the frame average brightness is near pitch black (camera initializing), skip it
                Scalar meanBrightness = Cv2.Mean(gray);
                if (meanBrightness.Val0 < 15.0) return;

                var faces = faceCascade.DetectMultiScale(gray, 1.1, 6, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(70, 70));

                if (faces.Length > 0)
                {
                    Rect primaryFace = faces[0];
                    foreach (var rect in faces)
                    {
                        if ((rect.Width * rect.Height) > (primaryFace.Width * primaryFace.Height))
                        {
                            primaryFace = rect;
                        }
                    }

                    if (isTrained)
                    {
                        using (Mat faceROI = new Mat(gray, primaryFace))
                        using (Mat resizedFace = new Mat())
                        {
                            Cv2.Resize(faceROI, resizedFace, new OpenCvSharp.Size(200, 200));
                            Cv2.EqualizeHist(resizedFace, resizedFace);

                            recognizer.Predict(resizedFace, out int label, out double confidence);

                            // Fixed: Use StrictConfidenceCutoff to prevent false positives on blurry/warming up frames
                            if (label != -1 && confidence < StrictConfidenceCutoff)
                            {
                                string username = GetNameFromDb(label);
                                if (username != "Unknown")
                                {
                                    EvaluateTemporalMatch(username);
                                    return;
                                }
                            }

                            EvaluateTemporalMatch("Unknown");
                        }
                    }
                    else
                    {
                        EvaluateTemporalMatch("Unknown");
                    }
                }
                else
                {
                    EvaluateTemporalMatch("");
                }
            }
        }


        private void EvaluateTemporalMatch(string detectedUser)
        {
            if (detectedUser == "")
            {
                lastDetectedUser = "";
                consecutiveUserMatchCount = 0;
                UpdateUIOnDetection(false, "");
                return;
            }

            if (detectedUser == lastDetectedUser)
            {
                consecutiveUserMatchCount++;
            }
            else
            {
                lastDetectedUser = detectedUser;
                consecutiveUserMatchCount = 1;
            }

            if (consecutiveUserMatchCount >= RequiredConsecutiveFrames)
            {
                if (detectedUser == "Unknown")
                {
                    isRegisteringNewUser = true;
                    UpdateUIOnDetection(true, "Unknown");
                }
                else
                {
                    UpdateUIOnDetection(true, detectedUser);
                }
            }
            else
            {
                this.BeginInvoke(new Action(() =>
                {
                    lblInstructions.Text = "Verifying identity... hold still";
                }));
            }
        }

        private void UpdateUIOnDetection(bool userPresent, string username)
        {
            if (cameraWarmUpFrames < WarmUpThreshold || isCapturing || this.IsDisposed) return;

            this.BeginInvoke(new Action(() =>
            {
                if (userPresent)
                {
                    if (username == "Unknown")
                    {
                        lblUserNotFound.Visible = true;
                        lblUserFound.Visible = false;
                        textBoxUsername.Visible = true;
                        lblInstructions.Text = "I don't recognize you. What's your name?";
                        textBoxUsername.Focus();
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(username))
                        {
                            lblUserFound.Visible = true;
                            lblUserNotFound.Visible = false;
                            textBoxUsername.Visible = false;
                            lblInstructions.Text = $"Welcome back, {username}!";

                            TransitionToMainProgram(username);
                        }
                    }
                }
                else
                {
                    lblUserFound.Visible = false;
                    lblUserNotFound.Visible = false;
                    textBoxUsername.Visible = false;
                    lblInstructions.Text = "Searching for a face...";
                }
            }));
        }

        private void TextBoxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                currentNewUserName = textBoxUsername.Text.Trim();

                if (!string.IsNullOrEmpty(currentNewUserName))
                {
                    if (UserExists(currentNewUserName))
                    {
                        TransitionToMainProgram(currentNewUserName);
                    }
                    else
                    {
                        isCapturing = true;
                        captureCount = 0;
                        trainingImages.Clear();

                        lblUserNotFound.Visible = false;
                        lblUserFound.Visible = false;
                        textBoxUsername.Visible = false;
                        lblInstructions.Text = "Hold still... capturing your face.";
                    }
                }
                else
                {
                    isRegisteringNewUser = false;
                }
                e.SuppressKeyPress = true;
            }
        }

        private void TransitionToMainProgram(string userName)
        {
            if (isTransitioning) return;
            isTransitioning = true;

            loginCameraTimer.Stop();
            capture?.Dispose();

            this.BeginInvoke(new Action(() =>
            {
                frmLoading loading = new frmLoading(userName);
                loading.Show();
                this.Hide();
            }));
        }

        private void ProcessCapture(Mat currentFrame)
        {
            using (var gray = new Mat())
            {
                Cv2.CvtColor(currentFrame, gray, ColorConversionCodes.BGR2GRAY);
                var faces = faceCascade.DetectMultiScale(gray, 1.1, 4, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(40, 40));

                if (faces.Length > 0 && captureCount < MaxCaptures)
                {
                    Rect primaryFace = faces[0];
                    foreach (var rect in faces)
                    {
                        if ((rect.Width * rect.Height) > (primaryFace.Width * primaryFace.Height))
                        {
                            primaryFace = rect;
                        }
                    }

                    using (Mat face = new Mat(gray, primaryFace))
                    {
                        Mat resizedFace = new Mat();
                        Cv2.Resize(face, resizedFace, new OpenCvSharp.Size(200, 200));

                        trainingImages.Add(resizedFace);
                        captureCount++;

                        this.BeginInvoke(new Action(() => {
                            lblInstructions.Text = $"Capturing: {captureCount}/{MaxCaptures}";
                        }));
                    }
                }

                if (captureCount >= MaxCaptures)
                {
                    isCapturing = false;
                    this.BeginInvoke(new Action(() =>
                    {
                        CompleteRegistration();
                    }));
                }
            }
        }

        private int SaveNewUserToDb(string name)
        {
            string connectionString = "Data Source=EmoTunes.db";
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Users (Name) VALUES (@name); SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@name", name);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void CompleteRegistration()
        {
            int newUserId = SaveNewUserToDb(currentNewUserName);

            if (!Directory.Exists(trainingDataDir))
            {
                Directory.CreateDirectory(trainingDataDir);
            }

            for (int i = 0; i < trainingImages.Count; i++)
            {
                string filePath = Path.Combine(trainingDataDir, $"{newUserId}_{i}_{DateTime.Now.Ticks}.png");
                trainingImages[i].SaveImage(filePath);
            }

            RebuildAndTrainModel();

            foreach (var img in trainingImages)
            {
                img.Dispose();
            }
            trainingImages.Clear();

            isRegisteringNewUser = false;
            TransitionToMainProgram(currentNewUserName);
        }

        private void RebuildAndTrainModel()
        {
            if (!Directory.Exists(trainingDataDir)) return;

            var allImages = new List<Mat>();
            var allLabels = new List<int>();
            string[] files = Directory.GetFiles(trainingDataDir, "*.png");

            foreach (var file in files)
            {
                try
                {
                    string fileName = Path.GetFileName(file);
                    string[] parts = fileName.Split('_');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int userId))
                    {
                        Mat faceImg = Cv2.ImRead(file, ImreadModes.Grayscale);
                        if (!faceImg.Empty())
                        {
                            allImages.Add(faceImg);
                            allLabels.Add(userId);
                        }
                    }
                }
                catch
                {
                    // Skip corrupt files safely
                }
            }

            if (allImages.Count > 0)
            {
                recognizer?.Dispose();
                recognizer = LBPHFaceRecognizer.Create(1, 8, 8, 8, 80.0);

                recognizer.Train(allImages, allLabels);
                recognizer.Write("face_model.yml");
                isTrained = true;
            }

            foreach (var mat in allImages)
            {
                mat.Dispose();
            }
        }

        private bool UserExists(string name)
        {
            string connectionString = "Data Source=EmoTunes.db";
            using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM Users WHERE Name = @name";
                command.Parameters.AddWithValue("@name", name);

                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }

        private void LayoutPictureBox()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;
            this.Padding = new Padding(50);
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.BackColor = Color.White;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(picLoginPreview);
            pictureBox1.SendToBack();
        }
    }
}