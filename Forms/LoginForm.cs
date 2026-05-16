using System;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;

namespace BookstoreIS.Forms
{
    public class LoginForm : Form
    {
        private Label lblTitle;
        private Label lblUsername, lblPassword;
        private TextBox txtUsername, txtPassword;
        private CheckBox chkRemember;
        private LinkLabel lnkForgot;
        private Button btnLogin, btnCancel;

        public string CurrentFullName { get; private set; } = "";
        public string CurrentRole { get; private set; } = "";
        public int CurrentUserId { get; private set; } = 0;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Form Settings
            Text = "Bookstore Information System - Login";
            Size = new Size(460, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(255, 248, 220);

            // Main Title
            lblTitle = new Label
            {
                Text = "Bookstore Information System",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 112),
                AutoSize = true,
                Location = new Point(45, 30)
            };

            // Username
            lblUsername = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(60, 100),
                AutoSize = true
            };

            txtUsername = new TextBox
            {
                Location = new Point(60, 125),
                Size = new Size(340, 32),
                Font = new Font("Segoe UI", 10.5f)
            };

            // Password
            lblPassword = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(60, 170),
                AutoSize = true
            };

            txtPassword = new TextBox
            {
                Location = new Point(60, 195),
                Size = new Size(340, 32),
                Font = new Font("Segoe UI", 10.5f),
                PasswordChar = '●'
            };

            // Remember Me
            chkRemember = new CheckBox
            {
                Text = "Remember me",
                Location = new Point(60, 245),
                Font = new Font("Segoe UI", 9.5f),
                AutoSize = true
            };

            // Forgot Password
            lnkForgot = new LinkLabel
            {
                Text = "Forgot Password?",
                Location = new Point(260, 245),
                Font = new Font("Segoe UI", 9.5f),
                LinkColor = Color.FromArgb(0, 102, 204),
                AutoSize = true
            };
            lnkForgot.Click += (s, e) => new PasswordRecoveryForm(this).ShowDialog();

            // Buttons
            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(120, 40),
                Location = new Point(60, 290),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                Font = new Font("Segoe UI", 10f)
            };

            btnLogin = new Button
            {
                Text = "Login",
                Size = new Size(120, 40),
                Location = new Point(280, 290),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            // Add controls
            Controls.AddRange(new Control[]
            {
                lblTitle, lblUsername, txtUsername, lblPassword, txtPassword,
                chkRemember, lnkForgot, btnCancel, btnLogin
            });

            // Event Handlers
            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += (s, e) => this.Close();        // Changed: Close instead of Exit
            AcceptButton = btnLogin;     // Enter key = Login
            CancelButton = btnCancel;    // Esc key = Cancel
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isValid = DatabaseHelper.ValidateLogin(username, password,
                out string role, out int userId, out string fullName);

            if (isValid)
            {
                CurrentFullName = fullName;
                CurrentRole = role;
                CurrentUserId = userId;

                MessageBox.Show($"Welcome, {fullName}!", "Login Successful",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                var dashboard = new DashboardForm(fullName, role, userId);
                dashboard.Show();
                this.Hide();        // Hide login form instead of closing (allows logout later)
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}