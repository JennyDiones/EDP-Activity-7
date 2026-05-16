using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace BookstoreIS.Forms
{
    public class UserManagementPanel : Panel
    {
        private DataGridView dgvUsers;
        private TextBox txtSearch;
        private ComboBox cboStatusFilter;
        private Button btnRefresh, btnAdd, btnEdit;

        public UserManagementPanel()
        {
            this.Dock = DockStyle.Fill; // Ensure it fills the parent container
            BackColor = Color.White;
            Build();
            LoadUsers();
        }

        private void Build()
        {
            // ── Header ───────────────────────────────────────────────
            var lblHead = new Label
            {
                Text = "User Management",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            // ── Row 1: Search + Filter + Buttons ─────────────────────
            txtSearch = new TextBox
            {
                Location = new Point(0, 38),
                Size = new Size(220, 30),
                PlaceholderText = "Search users...",
                Font = new Font("Segoe UI", 9.5f)
            };
            txtSearch.TextChanged += FilterChanged;

            var lblFilter = new Label
            {
                Text = "Status:",
                Location = new Point(228, 42),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f)
            };

            cboStatusFilter = new ComboBox
            {
                Location = new Point(278, 38),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cboStatusFilter.Items.AddRange(new string[] { "All", "Active", "Inactive" });
            cboStatusFilter.SelectedIndex = 0;
            cboStatusFilter.SelectedIndexChanged += FilterChanged;

            // ── Buttons ──────────────────────────────────────────────
            btnRefresh = MakeButton("⟳ Refresh", Color.FromArgb(64, 64, 64));
            btnAdd = MakeButton("+ Add User", Color.FromArgb(0, 160, 80));
            btnEdit = MakeButton("✎ Edit User", Color.FromArgb(160, 140, 0));

            var pnlBtns = new FlowLayoutPanel
            {
                Location = new Point(440, 34),
                Size = new Size(400, 40),
                WrapContents = false,
                AutoSize = true
            };
            btnRefresh.Margin = new Padding(0, 0, 6, 0);
            btnAdd.Margin = new Padding(0, 0, 6, 0);
            btnEdit.Margin = new Padding(0, 0, 0, 0);
            pnlBtns.Controls.AddRange(new Control[] { btnRefresh, btnAdd, btnEdit });

            btnRefresh.Click += (s, e) => LoadUsers();
            btnAdd.Click += (s, e) => BtnAdd_Click();
            btnEdit.Click += (s, e) => BtnEdit_Click();

            // ── DataGridView ─────────────────────────────────────────
            dgvUsers = new DataGridView
            {
                Location = new Point(0, 82),
                Size = new Size(870, 400),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.FixedSingle,
                GridColor = Color.FromArgb(220, 220, 220)
            };

            // Fix for the FormatException: Handle DataError event
            dgvUsers.DataError += (s, e) => { e.ThrowException = false; };

            // Attach formatting event once
            dgvUsers.CellFormatting += DgvUsers_CellFormatting;

            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 40, 60);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);

            Controls.AddRange(new Control[]
            {
                lblHead, txtSearch, lblFilter, cboStatusFilter,
                pnlBtns, dgvUsers
            });
        }

        private static Button MakeButton(string text, Color bg)
        {
            return new Button
            {
                Text = text,
                Size = new Size(110, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
        }

        private void FilterChanged(object? sender, EventArgs e)
            => LoadUsers(txtSearch.Text.Trim());

        private void LoadUsers(string search = "")
        {
            try
            {
                string status = cboStatusFilter.SelectedItem?.ToString() ?? "All";

                string sql = @"SELECT user_id   AS ID,
                                      username   AS Username,
                                      full_name  AS `Full Name`,
                                      email      AS Email,
                                      role       AS Role,
                                      is_active  AS Status
                               FROM   users
                               WHERE  1=1";

                var parameters = new List<MySqlParameter>();

                if (!string.IsNullOrEmpty(search))
                {
                    sql += " AND (username LIKE @s OR full_name LIKE @s OR email LIKE @s)";
                    parameters.Add(new MySqlParameter("@s", $"%{search}%"));
                }

                if (status == "Active") sql += " AND is_active = 1";
                if (status == "Inactive") sql += " AND is_active = 0";

                sql += " ORDER BY username";

                // Rebind data
                dgvUsers.DataSource = DatabaseHelper.GetDataTable(sql, parameters.ToArray());

                // ── Column sizing & Type Override ──────────────────────
                if (dgvUsers.Columns.Count > 0)
                {
                    dgvUsers.Columns["ID"].Width = 45;
                    dgvUsers.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

                    if (dgvUsers.Columns.Contains("Status"))
                    {
                        // Explicitly tell the column it will display strings to prevent formatting errors
                        dgvUsers.Columns["Status"].ValueType = typeof(string);
                        dgvUsers.Columns["Status"].Width = 100;
                        dgvUsers.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        dgvUsers.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    dgvUsers.Columns["Role"].Width = 90;
                    dgvUsers.Columns["Role"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUsers_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            // Safety checks
            if (e.RowIndex < 0 || dgvUsers.Columns.Count == 0) return;

            string colName = dgvUsers.Columns[e.ColumnIndex].Name;
            if (colName == "Status" && e.Value != null)
            {
                try
                {
                    bool isActive = Convert.ToBoolean(e.Value);
                    e.Value = isActive ? "✔ Active" : "✘ Inactive";
                    e.FormattingApplied = true;

                    e.CellStyle.ForeColor = isActive ? Color.DarkGreen : Color.Crimson;
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                catch
                {
                    // Fallback if value isn't a simple boolean/int
                }
            }
        }

        private void BtnAdd_Click()
        {
            using var editor = new UserEditorForm();
            if (editor.ShowDialog() == DialogResult.OK)
                LoadUsers(txtSearch.Text.Trim());
        }

        private void BtnEdit_Click()
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to edit.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["ID"].Value);

            using var editor = new UserEditorForm(userId);
            if (editor.ShowDialog() == DialogResult.OK)
                LoadUsers(txtSearch.Text.Trim());
        }
    }
}