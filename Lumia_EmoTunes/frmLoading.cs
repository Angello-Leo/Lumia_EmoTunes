    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using static System.Windows.Forms.DataFormats;

    namespace Lumia_EmoTunes
    {
        public partial class frmLoading : Form
        {
            private int progressValue = 0;
            private string _userName;

        public frmLoading(string userName)
            {
                InitializeComponent();
                _userName = userName;
            }

            private void pictureBox1_Paint(object sender, PaintEventArgs e)
            {
                if (pictureBox1.Image == null) return;
                Rectangle imgRect = GetImageDisplayRectangle(pictureBox1);

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Smooths out the bar corners

                StringFormat centerFormat = new StringFormat();
                centerFormat.Alignment = StringAlignment.Center;

                float centerX = imgRect.X + (imgRect.Width / 2);

                using (Font titleFont = new Font("Iceberg", 44, FontStyle.Bold))
                {
                    string title = "Initializing System"; // Changed for the loading screen feel
                    float startY = imgRect.Y + 100;
                    e.Graphics.DrawString(title, titleFont, Brushes.Black, centerX, startY, centerFormat);
                }

                using (Font userFont = new Font("Iceberg", 20, FontStyle.Italic))
                {
                    string welcomeMsg = $"Welcome back, {_userName}";
                    // Position it nicely above the loading bar
                    e.Graphics.DrawString(welcomeMsg, userFont, Brushes.DimGray, centerX, imgRect.Y + 275, centerFormat);
                }

                int barY = imgRect.Y + 350;
                int barWidth = 450;
                int barHeight = 24;
                int radius = 10; // For rounded bar edges

                // Create the background path (Dark Gray)
                Rectangle bgRect = new Rectangle((int)centerX - (barWidth / 2), barY, barWidth, barHeight);
                using (System.Drawing.Drawing2D.GraphicsPath pathBg = GetRoundedRect(bgRect, radius))
                {
                    e.Graphics.FillPath(Brushes.DimGray, pathBg);
                }

                int fillWidth = (int)((progressValue / 100.0) * barWidth);
                if (fillWidth > 5) // Only draw if there's actual progress
                {
                    Rectangle fillRect = new Rectangle((int)centerX - (barWidth / 2), barY, fillWidth, barHeight);
                    using (System.Drawing.Drawing2D.GraphicsPath pathFill = GetRoundedRect(fillRect, radius))
                    using (SolidBrush purpleBrush = new SolidBrush(Color.FromArgb(134, 76, 204)))
                    {
                        e.Graphics.FillPath(purpleBrush, pathFill);
                    }
                }
                string statusText = $"Scanning for Hardware... {progressValue}%";
                e.Graphics.DrawString(statusText, new Font("Iceberg", 14), Brushes.Black, centerX, barY + 35, centerFormat);
            }
            private Rectangle GetImageDisplayRectangle(PictureBox pbox)
            {
                double ratio = Math.Min((double)pbox.Width / pbox.Image.Width, (double)pbox.Height / pbox.Image.Height);
                int w = (int)(pbox.Image.Width * ratio);
                int h = (int)(pbox.Image.Height * ratio);
                int x = (pbox.Width - w) / 2;
                int y = (pbox.Height - h) / 2;
                return new Rectangle(x, y, w, h);
            }

            private System.Drawing.Drawing2D.GraphicsPath GetRoundedRect(Rectangle rect, int radius)
            {
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                float d = radius * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                return path;
            }

            private void loadingTimer_Tick(object sender, EventArgs e)
            {
                if (progressValue < 100)
                {
                    progressValue += 2; // Speed of the loading bar
                    pictureBox1.Invalidate(); // Redraw the bar
                }
                else
                {
                    loadingTimer.Stop();
                    StartCameraAndMoveToNextForm();
                }
            }

            private void StartCameraAndMoveToNextForm()
            {
                // 1. Logic to initialize your camera goes here
                // 2. Open the next form
                frmMain mainApp = new frmMain();
                mainApp.Show();
                this.Hide();
            }
        }
    }
