using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;
using MySql.Data.MySqlClient;

namespace BookstoreIS.Forms
{
    public class UserEditorForm : Form
    {
        private TextBox txtUsername, txtFullName, txtEmail, txtPassword;
        private ComboBox cboRole;
        private CheckBox chkActive;
        private Button btnSave, btnCancel;

        private int? _userId = null;   // null = Add mode, has value = Edit mode

        // Constructor for Add New User
        public UserEditorForm()
        {
            InitializeComponent();
            Text = "Add New User";
            txtPassword.PasswordChar = '●';
            chkActive.Checked = true;
        }

        // Constructor for Edit User
        public UserEditorForm(int userId)
        {
            InitializeComponent();
            _userId = userId;
            Text = "Edit User";
            LoadUserData(userId);
        }

        private void InitializeComponent()
        {
            this.Text = "User Editor";
            this.Size = new Size(480, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.75f);

            int y = 40;

            txtUsername = AddField("Username", ref y);
            txtFullName = AddField("Full Name", ref y);
            txtEmail = AddField("Email", ref y);

            // Password
            var lblPassword = new Label { Text = "Password", Location = new Point(30, y), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(160, y - 3), Size = new Size(250, 28) };
            txtPassword.PasswordChar = '●';
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            y += 45;

            // Role
            var lblRole = new Label { Text = "Role", Location = new Point(30, y), AutoSize = true };
            cboRole = new ComboBox
            {
                Location = new Point(160, y - 3),
                Size = new Size(250, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboRole.Items.AddRange(new string[] { "Admin", "Manager", "Staff" });
            cboRole.SelectedIndex = 2;
            Controls.Add(lblRole);
            Controls.Add(cboRole);
            y += 45;

            // Active
            chkActive = new CheckBox
            {
                Text = "Active Account",
                Location = new Point(160, y),
                Checked = true,
                Font = new Font("Segoe UI", 10f)
            };
            Controls.Add(chkActive);
            y += 50;

            // Buttons
            btnSave = new Button
            {
                Text = "Save",
                Size = new Size(120, 40),
                Location = new Point(160, y),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold)
            };

            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(110, 40),
                Location = new Point(290, y),
                FlatStyle = FlatStyle.Flat
            };

            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
        }

        private TextBox AddField(string label, ref int y)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(30, y),
                AutoSize = true
            };

            var tb = new TextBox
            {
                Location = new Point(160, y - 3),
                Size = new Size(250, 28)
            };

            Controls.Add(lbl);
            Controls.Add(tb);
            y += 45;
            return tb;
        }

        private void LoadUserData(int userId)
        {
            try
            {
                string sql = @"SELECT username, full_name, email, role, is_active 
                               FROM users WHERE user_id = @id";

                var parameters = new MySqlParameter[] { new MySqlParameter("@id", userId) };
                DataTable dt = DatabaseHelper.GetDataTable(sql, parameters);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    txtUsername.Text = row["username"].ToString();
                    txtFullName.Text = row["full_name"].ToString();
                    txtEmail.Text = row["email"].ToString();
                    cboRole.Text = row["role"].ToString();
                    chkActive.Checked = Convert.ToBoolean(row["is_active"]);

                    txtUsername.Enabled = false; // Username usually can't be changed
                    txtPassword.PlaceholderText = "(Leave blank to keep current password)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading user data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Username and Full Name are required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_userId == null && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Password is required when creating a new user.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success;

                if (_userId == null) // Add new user
                {
                    success = DatabaseHelper.AddUser(
                        txtUsername.Text.Trim(),
                        txtPassword.Text.Trim(),
                        txtFullName.Text.Trim(),
                        txtEmail.Text.Trim(),
                        cboRole.SelectedItem?.ToString() ?? "Staff",
                        chkActive.Checked);
                }
                else // Update existing user
                {
                    success = DatabaseHelper.UpdateUser(
                        _userId.Value,
                        txtFullName.Text.Trim(),
                        txtEmail.Text.Trim(),
                        cboRole.SelectedItem?.ToString() ?? "Staff",
                        chkActive.Checked,
                        string.IsNullOrWhiteSpace(txtPassword.Text) ? null : txtPassword.Text.Trim());
                }

                if (success)
                {
                    MessageBox.Show("User saved successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Failed to save user.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Save Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}