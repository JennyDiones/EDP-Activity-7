using System;
using System.Drawing;
using System.Windows.Forms;

namespace BookstoreIS.Forms
{
    public class PasswordRecoveryForm : Form
    {
        private readonly Form _parent;

        // Fields initialized with null-forgiving operator to suppress CS8618
        private Label lblTitle = null!;
        private Label lblSub = null!;
        private Label lblEmail = null!;
        private Label lblUser = null!;
        private Label lblTips = null!;
        private Label lblTip1 = null!;
        private Label lblTip2 = null!;
        private TextBox txtEmail = null!;
        private TextBox txtUsername = null!;
        private Button btnBack = null!;
        private Button btnSend = null!;
        private Panel pnlCard = null!;
        private Panel pnlTips = null!;

        public PasswordRecoveryForm(Form parent)
        {
            _parent = parent;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Password Recovery";
            Size = new Size(480, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(240, 240, 245);
            Font = new Font("Segoe UI", 9f);

            pnlCard = new Panel
            {
                Size = new Size(430, 340),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(255, 252, 220),
                Padding = new Padding(20)
            };

            lblTitle = new Label
            {
                Text = "Password Recoverry",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 100, 0),
                AutoSize = true,
                Location = new Point(20, 18)
            };

            lblSub = new Label
            {
                Text = "Enter your details below to receive password reset link",
                AutoSize = true,
                Location = new Point(20, 48)
            };

            lblEmail = new Label
            {
                Text = "Email Address:",
                Location = new Point(20, 80),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            txtEmail = new TextBox
            {
                Size = new Size(390, 26),
                Location = new Point(20, 100),
                BackColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10f)
            };

            lblUser = new Label
            {
                Text = "Username (optional)",
                Location = new Point(20, 135),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            txtUsername = new TextBox
            {
                Size = new Size(390, 26),
                Location = new Point(20, 155),
                BackColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10f)
            };

            // Tips panel
            pnlTips = new Panel
            {
                Size = new Size(390, 80),
                Location = new Point(20, 190),
                BackColor = Color.FromArgb(255, 252, 200)
            };

            lblTips = new Label
            {
                Text = "Recovery Tips",
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(8, 8)
            };

            lblTip1 = new Label
            {
                Text = "1. Enter your registered email address above.",
                AutoSize = true,
                Location = new Point(8, 28)
            };

            lblTip2 = new Label
            {
                Text = "2. Check your inbox for the reset link.",
                AutoSize = true,
                Location = new Point(8, 48)
            };

            pnlTips.Controls.AddRange(new Control[] { lblTips, lblTip1, lblTip2 });

            btnBack = new Button
            {
                Text = "Back to Login",
                Size = new Size(110, 30),
                Location = new Point(20, 295),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderColor = Color.Gray;
            btnBack.Click += (s, e) => Close();

            btnSend = new Button
            {
                Text = "Send Reset Link",
                Size = new Size(140, 35),
                Location = new Point(270, 290),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSend.FlatAppearance.BorderColor = Color.FromArgb(0, 90, 180);
            btnSend.Click += BtnSend_Click;   // Fixed: No more nullability warning

            pnlCard.Controls.AddRange(new Control[]
            {
                lblTitle, lblSub, lblEmail, txtEmail,
                lblUser, txtUsername, pnlTips,
                btnBack, btnSend
            });

            Controls.Add(pnlCard);
            AcceptButton = btnSend;
        }

        private void BtnSend_Click(object? sender, EventArgs e)   // Fixed: object? 
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.",
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"A password reset link has been sent to:\n{email}\n\n(Demo mode – no actual email sent.)",
                            "Reset Link Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }
    }
}