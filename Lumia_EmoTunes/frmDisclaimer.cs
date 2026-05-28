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
    public partial class frmDisclaimer : Form
    {
        private RoundedButton roundedButton1;
        private RoundedButton roundedButton2;

        public frmDisclaimer()
        {
            InitializeComponent();
            this.ResizeRedraw = true;
            roundedButton1.Parent = pictureBox1;
            roundedButton2.Parent = pictureBox1;
            roundedButton1.BringToFront();
            roundedButton2.BringToFront();

            roundedButton1.Click += RoundedButton1_Click;
            roundedButton2.Click += RoundedButton2_Click;

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image == null) return;
            Rectangle imgRect = GetImageDisplayRectangle(pictureBox1);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            StringFormat centerFormat = new StringFormat();
            centerFormat.Alignment = StringAlignment.Center;

            using (Font titleFont = new Font("Iceberg", 44, FontStyle.Bold))
            using (Font subFont = new Font("Iceberg", 17, FontStyle.Regular))
            {
                string title = "Disclaimer";
                string subtitle = "EmoTunes uses your camera to match your music to your expression.\n" +
                          "Granting access allows us to create a truly personalized listening experience just for you.";

                float centerX = imgRect.X + (imgRect.Width / 2);
                float startY = imgRect.Y + 100;

                e.Graphics.DrawString(title, titleFont, Brushes.Black, centerX, startY, centerFormat);

                float subtextY = startY + 150;
                e.Graphics.DrawString(subtitle, subFont, Brushes.Black, centerX, subtextY, centerFormat);
            }
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


        private void frmDisclaimer_Resize(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;

            Rectangle imgRect = GetImageDisplayRectangle(pictureBox1);
            int centerX = imgRect.X + (imgRect.Width / 2);

            int yPosFromTopOfImage = 450;


            int gapBetweenButtons = 100;

            int horizontalNudge = 0;

            int finalY = imgRect.Y + yPosFromTopOfImage;
            roundedButton1.Location = new Point(
                (centerX + horizontalNudge) - roundedButton1.Width - (gapBetweenButtons / 2),
                finalY
            );

            roundedButton2.Location = new Point(
                (centerX + horizontalNudge) + (gapBetweenButtons / 2),
                finalY
            );
        }

        private void RoundedButton1_Click(object sender, EventArgs e)
        {
            frmUserDetection nextForm = new frmUserDetection();
            nextForm.Show();
            this.Hide();
        }

        private void RoundedButton2_Click(object sender, EventArgs e)
        {
            frmStart nextForm = new frmStart(true);

            nextForm.Show();
            this.Hide();
        }

        private void frmDisclaimer_Load(object sender, EventArgs e)
        {
            roundedButton1.Cursor = Cursors.Hand;
            roundedButton2.Cursor = Cursors.Hand;
        }
    }
}
