using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Lumia_EmoTunes
{
    public partial class frmMain : Form
    {
        private VideoCapture capture;
        private Mat frame;
        private System.Windows.Forms.Timer cameraTimer;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            SetupCamera();
        }

        private void SetupCamera()
        {
            capture = new VideoCapture(0); // 0 = default camera
            capture.FrameWidth = 640;
            capture.FrameHeight = 480;
            frame = new Mat();

            // Setup the loop to pull frames
            cameraTimer = new System.Windows.Forms.Timer();
            cameraTimer.Interval = 33; // ~30 Frames Per Second
            cameraTimer.Tick += CameraTimer_Tick;
            cameraTimer.Start();
        }

        private void CameraTimer_Tick(object sender, EventArgs e)
        {
            if (capture != null && capture.IsOpened())
            {
                capture.Read(frame); // Grab the hardware frame

                if (!frame.Empty())
                {
                    // Flip the frame so it acts like a mirror
                    Cv2.Flip(frame, frame, FlipMode.Y);

                    // Convert to Bitmap and show in PictureBox
                    Bitmap bitmap = BitmapConverter.ToBitmap(frame);

                    // Clean up the old image to prevent memory leaks
                    var oldImage = pictureBox1.Image;
                    pictureBox1.Image = bitmap;
                    oldImage?.Dispose();
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            cameraTimer?.Stop();
            capture?.Release();
            frame?.Dispose();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
