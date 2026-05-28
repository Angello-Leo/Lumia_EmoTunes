using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lumia_EmoTunes
{
    public partial class FrmClosing : Form
    {
        private System.Windows.Forms.Timer closeTimer;
        public FrmClosing()
        {
            InitializeComponent();

            closeTimer = new System.Windows.Forms.Timer();
            closeTimer.Interval = 5000; // 5000 milliseconds = 5 seconds
            closeTimer.Tick += CloseTimer_Tick;
            closeTimer.Start();
        }
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop();
            Application.Exit();     
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            StringFormat centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;       // Horizontal center
            centerFormat.LineAlignment = StringAlignment.Center;  // Vertical center

            using (Font titleFont = new Font("Iceberg", 44, FontStyle.Bold))
            using (Font subFont = new Font("Iceberg", 25, FontStyle.Regular))
            using (Font exitFont = new Font("Iceberg", 18, FontStyle.Italic)) // Smaller font for the exit message
            {
                string title = "Rest in the Rhythm";
                string subtitle = "Whatever you’re feeling, we’re here for the harmony. See you next time.";
                string exitMessage = "Thank you for using Lumia:EmoTunes! Exiting Application...";

                float midX = pictureBox1.Width / 2;
                float midY = pictureBox1.Height / 2;

                // Title
                e.Graphics.DrawString(title, titleFont, Brushes.Black, midX, midY - 100, centerFormat);

                // Draw Subtitle (at the center)
                e.Graphics.DrawString(subtitle, subFont, Brushes.Black, midX, midY, centerFormat);

                // Draw Exit Message (pushed down from center)
                e.Graphics.DrawString(exitMessage, exitFont, Brushes.DimGray, midX, midY + 100, centerFormat);
            }
        }
    }
}
