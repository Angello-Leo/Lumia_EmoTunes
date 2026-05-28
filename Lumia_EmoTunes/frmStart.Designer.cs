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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStart));
            roundedButton1 = new RoundedButton();
            txtLumia = new Label();
            pictureBoxPopup = new PictureBox();
            lblPopupMessage = new Label();
            lblClose = new Label();
            label1 = new Label();
            lblClosingValidaion = new Label();
            btnClose = new RoundedButton();
            btnNo = new RoundedButton();
            roundedButton2 = new RoundedButton();
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
            roundedButton1.Location = new Point(509, 446);
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
            txtLumia.Location = new Point(440, 73);
            txtLumia.Name = "txtLumia";
            txtLumia.Size = new Size(710, 112);
            txtLumia.TabIndex = 2;
            txtLumia.Text = "Lu     ia:E     oTunes";
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
            lblPopupMessage.Location = new Point(152, 216);
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
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Iceberg", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(454, 196);
            label1.Name = "label1";
            label1.Size = new Size(842, 204);
            label1.TabIndex = 6;
            label1.Text = resources.GetString("label1.Text");
            // 
            // lblClosingValidaion
            // 
            lblClosingValidaion.Anchor = AnchorStyles.None;
            lblClosingValidaion.AutoSize = true;
            lblClosingValidaion.Font = new Font("Iceberg", 16.5F);
            lblClosingValidaion.Location = new Point(128, 176);
            lblClosingValidaion.Name = "lblClosingValidaion";
            lblClosingValidaion.Size = new Size(563, 34);
            lblClosingValidaion.TabIndex = 8;
            lblClosingValidaion.Text = "Are you sure you want to close the application?";
            lblClosingValidaion.Visible = false;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.None;
            btnClose.BackColor = Color.LimeGreen;
            btnClose.BorderColor = Color.Purple;
            btnClose.BorderRadius = 20;
            btnClose.BorderSize = 0F;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Iceberg", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(201, 255);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(188, 50);
            btnClose.TabIndex = 9;
            btnClose.Text = "Yeah, I'm sure";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Visible = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnNo
            // 
            btnNo.Anchor = AnchorStyles.None;
            btnNo.BackColor = Color.Brown;
            btnNo.BorderColor = Color.Purple;
            btnNo.BorderRadius = 20;
            btnNo.BorderSize = 0F;
            btnNo.FlatAppearance.BorderSize = 0;
            btnNo.FlatStyle = FlatStyle.Flat;
            btnNo.Font = new Font("Iceberg", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnNo.ForeColor = Color.White;
            btnNo.Location = new Point(440, 255);
            btnNo.Name = "btnNo";
            btnNo.Size = new Size(188, 50);
            btnNo.TabIndex = 10;
            btnNo.Text = "Wait, hang on";
            btnNo.UseVisualStyleBackColor = false;
            btnNo.Visible = false;
            btnNo.Click += btnNo_Click;
            // 
            // roundedButton2
            // 
            roundedButton2.Anchor = AnchorStyles.None;
            roundedButton2.BackColor = Color.DarkRed;
            roundedButton2.BorderColor = Color.PaleVioletRed;
            roundedButton2.BorderRadius = 20;
            roundedButton2.BorderSize = 0F;
            roundedButton2.Cursor = Cursors.Hand;
            roundedButton2.FlatAppearance.BorderSize = 0;
            roundedButton2.FlatStyle = FlatStyle.Flat;
            roundedButton2.Font = new Font("Iceberg", 37F);
            roundedButton2.ForeColor = Color.White;
            roundedButton2.Location = new Point(900, 446);
            roundedButton2.Name = "roundedButton2";
            roundedButton2.Size = new Size(309, 96);
            roundedButton2.TabIndex = 11;
            roundedButton2.Text = "EXIT";
            roundedButton2.UseVisualStyleBackColor = false;
            roundedButton2.Click += roundedButton2_Click;
            // 
            // frmStart
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(800, 450);
            Controls.Add(roundedButton2);
            Controls.Add(btnNo);
            Controls.Add(btnClose);
            Controls.Add(lblClosingValidaion);
            Controls.Add(lblClose);
            Controls.Add(lblPopupMessage);
            Controls.Add(pictureBoxPopup);
            Controls.Add(roundedButton1);
            Controls.Add(label1);
            Controls.Add(txtLumia);
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
        private Label label1;
        private Label lblClosingValidaion;
        private RoundedButton btnClose;
        private RoundedButton btnNo;
        private RoundedButton roundedButton2;
    }
}
