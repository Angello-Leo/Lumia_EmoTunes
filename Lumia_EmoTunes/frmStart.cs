using System.Windows.Forms;

namespace Lumia_EmoTunes
{
    public partial class frmStart : Form
    {
        private bool _shouldShowPopup;
        public frmStart(bool showPopup)
        {
            InitializeComponent();
            _shouldShowPopup = showPopup;
        }

        private void roundedButton1_Click(object sender, EventArgs e)
        {
            frmDisclaimer frmDisclaimer = new frmDisclaimer();
            frmDisclaimer.Show();
            this.Hide();
        }



        private void frmStart_Load(object sender, EventArgs e)
        {
            if (_shouldShowPopup)
            {
                pictureBoxPopup.Visible = true;
                pictureBoxPopup.BringToFront();
                lblPopupMessage.Visible = true;
                lblPopupMessage.BringToFront();
                lblClose.Visible = true;
                lblClose.BringToFront();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            pictureBoxPopup.Visible = false;
            lblPopupMessage.Visible = false;
            lblClose.Visible = false;
        }

        private void lblPopupMessage_Click(object sender, EventArgs e)
        {

        }

        private void lblCoseApp_Click(object sender, EventArgs e)
        {

            lblClosingValidaion.Visible = true;
            pictureBoxPopup.Visible = true;
            btnClose.Visible = true;
            btnNo.Visible = true;
            lblClosingValidaion.BringToFront();
            btnClose.BringToFront();
            btnNo.BringToFront();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            lblClosingValidaion.Visible = false;
            pictureBoxPopup.Visible = false;
            btnClose.Visible = false;
            btnNo.Visible = false;
        }

        private void roundedButton2_Click(object sender, EventArgs e)
        {
            lblClosingValidaion.Visible = true;
            pictureBoxPopup.Visible = true;
            btnClose.Visible = true;
            btnNo.Visible = true;
            lblClosingValidaion.BringToFront();
            btnClose.BringToFront();
            btnNo.BringToFront();
        }
    }
}
    