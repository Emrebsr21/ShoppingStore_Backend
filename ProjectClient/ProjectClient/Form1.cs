using ProjectClient;
using System;
using System.Windows.Forms;

namespace ShoppingClient
{
    public partial class LoginForm : Form
    {
        private ServerConnection _serverConnection;

        public LoginForm()
        {
            InitializeComponent();
            _serverConnection = new ServerConnection();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string hostname = txtHostname.Text;
            string password = txtPassword.Text;

            if (int.TryParse(password, out int accountNumber))
            {
                if (_serverConnection.Connect(hostname, 55055, accountNumber, password, out string userName))
                {
                    OperationalForm operationalForm = new OperationalForm(_serverConnection, userName);
                    operationalForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Failed to connect to the server or login failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid account number. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
