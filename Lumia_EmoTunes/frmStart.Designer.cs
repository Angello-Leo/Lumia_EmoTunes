namespace Lumia_EmoTunes
{
    partial class frmStart
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            roundedButton1 = new RoundedButton();
            txtLumia = new Label();
            pictureBoxPopup = new PictureBox();
            lblPopupMessage = new Label();
            lblClose = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPopup).BeginInit();
            SuspendLayout();
            // 
            // roundedButton1
            // 
            roundedButton1.Anchor = AnchorStyles.None;
            roundedButton1.BackColor = Color.FromArgb(134, 76, 204);
            roundedButton1.BorderColor = Color.PaleVioletRed;
            roundedButton1.BorderRadius = 20;
            roundedButton1.BorderSize = 0F;
            roundedButton1.Cursor = Cursors.Hand;
            roundedButton1.FlatAppearance.BorderSize = 0;
            roundedButton1.FlatStyle = FlatStyle.Flat;
            roundedButton1.Font = new Font("Iceberg", 37F);
            roundedButton1.ForeColor = Color.White;
            roundedButton1.Location = new Point(590, 325);
            roundedButton1.Name = "roundedButton1";
            roundedButton1.Size = new Size(309, 96);
            roundedButton1.TabIndex = 1;
            roundedButton1.Text = "START";
            roundedButton1.UseVisualStyleBackColor = false;
            roundedButton1.Click += roundedButton1_Click;
            // 
            // txtLumia
            // 
            txtLumia.Anchor = AnchorStyles.None;
            txtLumia.AutoSize = true;
            txtLumia.BackColor = Color.Transparent;
            txtLumia.Font = new Font("Iceberg", 55F);
            txtLumia.ForeColor = SystemColors.Control;
            txtLumia.Location = new Point(440, 200);
            txtLumia.Name = "txtLumia";
            txtLumia.Size = new Size(690, 112);
            txtLumia.TabIndex = 2;
            txtLumia.Text = "Lumia:EmoTunes";
            // 
            // pictureBoxPopup
            // 
            pictureBoxPopup.Anchor = AnchorStyles.None;
            pictureBoxPopup.BackColor = Color.Transparent;
            pictureBoxPopup.Image = Properties.Resources.Lumia_DisclaimerWhite;
            pictureBoxPopup.Location = new Point(103, 75);
            pictureBoxPopup.Name = "pictureBoxPopup";
            pictureBoxPopup.Size = new Size(607, 320);
            pictureBoxPopup.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPopup.TabIndex = 3;
            pictureBoxPopup.TabStop = false;
            pictureBoxPopup.Visible = false;
            // 
            // lblPopupMessage
            // 
            lblPopupMessage.Anchor = AnchorStyles.None;
            lblPopupMessage.AutoSize = true;
            lblPopupMessage.Font = new Font("Iceberg", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPopupMessage.Location = new Point(152, 229);
            lblPopupMessage.Name = "lblPopupMessage";
            lblPopupMessage.Size = new Size(510, 37);
            lblPopupMessage.TabIndex = 4;
            lblPopupMessage.Text = "Please allow camera access to continue.";
            lblPopupMessage.Visible = false;
            lblPopupMessage.Click += lblPopupMessage_Click;
            // 
            // lblClose
            // 
            lblClose.Anchor = AnchorStyles.None;
            lblClose.AutoSize = true;
            lblClose.Cursor = Cursors.Hand;
            lblClose.Font = new Font("Iceberg", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblClose.ForeColor = Color.Red;
            lblClose.Location = new Point(651, 86);
            lblClose.Name = "lblClose";
            lblClose.Size = new Size(33, 37);
            lblClose.TabIndex = 5;
            lblClose.Text = "X";
            lblClose.Visible = false;
            lblClose.Click += label1_Click;
            // 
            // frmStart
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Lumia_Bg_Start;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(800, 450);
            Controls.Add(lblClose);
            Controls.Add(lblPopupMessage);
            Controls.Add(pictureBoxPopup);
            Controls.Add(txtLumia);
            Controls.Add(roundedButton1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmStart";
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            Load += frmStart_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxPopup).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RoundedButton roundedButton1;
        private Label txtLumia;
        private PictureBox pictureBoxPopup;
        private Label lblPopupMessage;
        private Label lblClose;
    }
}
