namespace Lumia_EmoTunes
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            panel2 = new Panel();
            musicPlayer = new Microsoft.Web.WebView2.WinForms.WebView2();
            panel3 = new Panel();
            lblMoodDisplay = new Label();
            picFeedbackBox = new PictureBox();
            lblFeedbackInstruction = new Label();
            lblPretest = new Label();
            lblWarning = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)musicPlayer).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picFeedbackBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.None;
            pictureBox1.Location = new Point(-3, -3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(640, 480);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.None;
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(34, 43);
            panel1.Name = "panel1";
            panel1.Size = new Size(640, 480);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.None;
            panel2.Controls.Add(panel1);
            panel2.Location = new Point(-335, 59);
            panel2.Name = "panel2";
            panel2.Size = new Size(720, 547);
            panel2.TabIndex = 3;
            // 
            // musicPlayer
            // 
            musicPlayer.AllowExternalDrop = true;
            musicPlayer.CreationProperties = null;
            musicPlayer.DefaultBackgroundColor = Color.White;
            musicPlayer.Dock = DockStyle.Fill;
            musicPlayer.Location = new Point(0, 0);
            musicPlayer.Name = "musicPlayer";
            musicPlayer.Size = new Size(853, 547);
            musicPlayer.TabIndex = 4;
            musicPlayer.ZoomFactor = 1D;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.None;
            panel3.Controls.Add(musicPlayer);
            panel3.Location = new Point(600, -200);
            panel3.Name = "panel3";
            panel3.Size = new Size(853, 547);
            panel3.TabIndex = 5;
            // 
            // lblMoodDisplay
            // 
            lblMoodDisplay.Anchor = AnchorStyles.None;
            lblMoodDisplay.AutoSize = true;
            lblMoodDisplay.Location = new Point(-14, -10);
            lblMoodDisplay.Name = "lblMoodDisplay";
            lblMoodDisplay.Size = new Size(0, 20);
            lblMoodDisplay.TabIndex = 10;
            // 
            // picFeedbackBox
            // 
            picFeedbackBox.Anchor = AnchorStyles.None;
            picFeedbackBox.BackColor = Color.Transparent;
            picFeedbackBox.Image = Properties.Resources.Lumia_DisclaimerWhite;
            picFeedbackBox.Location = new Point(185, 59);
            picFeedbackBox.Name = "picFeedbackBox";
            picFeedbackBox.Size = new Size(724, 431);
            picFeedbackBox.SizeMode = PictureBoxSizeMode.Zoom;
            picFeedbackBox.TabIndex = 6;
            picFeedbackBox.TabStop = false;
            picFeedbackBox.Visible = false;
            // 
            // lblFeedbackInstruction
            // 
            lblFeedbackInstruction.Anchor = AnchorStyles.None;
            lblFeedbackInstruction.AutoSize = true;
            lblFeedbackInstruction.BackColor = Color.White;
            lblFeedbackInstruction.Font = new Font("Iceberg", 22.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblFeedbackInstruction.Location = new Point(235, 227);
            lblFeedbackInstruction.Name = "lblFeedbackInstruction";
            lblFeedbackInstruction.Size = new Size(607, 92);
            lblFeedbackInstruction.TabIndex = 7;
            lblFeedbackInstruction.Text = "Do you like this Genre? \r\n(Nod: Keep Genre | Shake: Skip Genre)";
            lblFeedbackInstruction.Visible = false;
            // 
            // lblPretest
            // 
            lblPretest.Anchor = AnchorStyles.None;
            lblPretest.AutoSize = true;
            lblPretest.BackColor = Color.White;
            lblPretest.Font = new Font("Iceberg", 22.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPretest.Location = new Point(207, 220);
            lblPretest.Name = "lblPretest";
            lblPretest.Size = new Size(687, 138);
            lblPretest.TabIndex = 8;
            lblPretest.Text = "Pre-test starting. Please look at the camera \r\nwith your 'Normal' face for a moment\r\n\r\n";
            lblPretest.Visible = false;
            // 
            // lblWarning
            // 
            lblWarning.Anchor = AnchorStyles.None;
            lblWarning.AutoSize = true;
            lblWarning.BackColor = Color.White;
            lblWarning.Font = new Font("Iceberg", 22.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblWarning.ForeColor = Color.Firebrick;
            lblWarning.Location = new Point(255, 273);
            lblWarning.Name = "lblWarning";
            lblWarning.Size = new Size(587, 46);
            lblWarning.TabIndex = 9;
            lblWarning.Text = "WARNING: Please look at the camera";
            lblWarning.Visible = false;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1061, 533);
            Controls.Add(lblWarning);
            Controls.Add(lblPretest);
            Controls.Add(lblFeedbackInstruction);
            Controls.Add(picFeedbackBox);
            Controls.Add(lblMoodDisplay);
            Controls.Add(panel3);
            Controls.Add(panel2);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmMain";
            Text = "frmMain1";
            WindowState = FormWindowState.Maximized;
            Load += frmMain_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)musicPlayer).EndInit();
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picFeedbackBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Panel panel1;
        private Panel panel2;
        private Microsoft.Web.WebView2.WinForms.WebView2 musicPlayer;
        private Panel panel3;
        private PictureBox picFeedbackBox;
        private Label lblFeedbackInstruction;
        private Label lblPretest;
        private Label lblWarning;
        private Label lblMoodDisplay;
    }
}