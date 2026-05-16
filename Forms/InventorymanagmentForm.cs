using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BookstoreIS.Database;

namespace BookstoreIS.Forms
{
    public class InventoryManagementForm : Form
    {
        // ── Controls ──────────────────────────────────────────────────────────
        private DataGridView dgvInventory = null!;
        private TextBox txtSearch = null!;
        private Label lblStatus = null!;
        private Panel pnlTop = null!;
        private Panel pnlActions = null!;

        // ── Theme colours ─────────────────────────────────────────────────────
        private static readonly Color ColPrimary = Color.FromArgb(30, 136, 229);
        private static readonly Color ColSuccess = Color.FromArgb(56, 142, 60);
        private static readonly Color ColWarning = Color.FromArgb(245, 124, 0);
        private static readonly Color ColDanger = Color.FromArgb(211, 47, 47);
        private static readonly Color ColBg = Color.FromArgb(245, 248, 252);
        private static readonly Color ColPanel = Color.FromArgb(255, 255, 255);
        private static readonly Color ColRowLow = Color.FromArgb(255, 165, 0);   // qty ≤ 5  → orange
        private static readonly Color ColRowOut = Color.FromArgb(220, 53, 69);   // qty = 0  → red
        private static readonly Font FontUI = new Font("Segoe UI", 10f);
        private static readonly Font FontBold = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font FontTitle = new Font("Segoe UI", 14f, FontStyle.Bold);

        // ─────────────────────────────────────────────────────────────────────
        public InventoryManagementForm()
        {
            InitializeComponent();
            LoadInventoryData();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UI SETUP
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            this.Text = "Inventory Management";
            this.Size = new Size(1180, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ColBg;
            this.MinimumSize = new Size(900, 600);

            // ── Top bar ───────────────────────────────────────────────────────
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(21, 101, 192),
                Padding = new Padding(16, 0, 16, 0)
            };

            var lblTitle = new Label
            {
                Text = "Inventory Management",
                ForeColor = Color.White,
                Font = FontTitle,
                AutoSize = true,
                Location = new Point(16, 20)
            };

            lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(200, 230, 255),
                Font = FontUI,
                AutoSize = true,
                Location = new Point(16, 46)
            };

            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(lblStatus);

            // ── Search / refresh bar ──────────────────────────────────────────
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = ColPanel,
                Padding = new Padding(16, 10, 16, 0)
            };

            var lblSearch = new Label
            {
                Text = "🔍 Search:",
                Location = new Point(16, 16),
                AutoSize = true,
                Font = FontUI
            };

            txtSearch = new TextBox
            {
                Location = new Point(95, 13),
                Size = new Size(320, 28),
                Font = FontUI,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += (s, e) => LoadInventoryData(txtSearch.Text.Trim());

            var btnRefresh = MakeButton("Refresh", ColPrimary, new Point(430, 12), new Size(110, 30));
            btnRefresh.Click += (s, e) => { txtSearch.Clear(); LoadInventoryData(); };

            var lblLegend = new Label
            {
                Text = "  🔴 Out of stock (0)   🟠 Low stock (≤ 5)",
                Location = new Point(560, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray
            };

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnRefresh);
            pnlSearch.Controls.Add(lblLegend);

            // ── Action buttons bar ────────────────────────────────────────────
            pnlActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = ColPanel,
                Padding = new Padding(16, 10, 16, 0)
            };

            int bx = 16;
            var btnAdd = MakeButton("➕ Add New Book", ColSuccess, new Point(bx, 12), new Size(145, 34));
            btnAdd.Click += BtnAdd_Click;

            bx += 155;
            var btnAddStock = MakeButton("Add Stock", ColPrimary, new Point(bx, 12), new Size(130, 34));
            btnAddStock.Click += BtnAddStock_Click;

            bx += 140;
            var btnEditPrice = MakeButton("Edit Price", ColWarning, new Point(bx, 12), new Size(130, 34));
            btnEditPrice.Click += BtnEditPrice_Click;

            bx += 140;
            var btnEditBook = MakeButton("Edit Book", Color.FromArgb(100, 100, 200), new Point(bx, 12), new Size(130, 34));
            btnEditBook.Click += BtnEditBook_Click;

            bx += 140;
            var btnDelete = MakeButton("Delete Book", ColDanger, new Point(bx, 12), new Size(135, 34));
            btnDelete.Click += BtnDelete_Click;

            pnlActions.Controls.Add(btnAdd);
            pnlActions.Controls.Add(btnAddStock);
            pnlActions.Controls.Add(btnEditPrice);
            pnlActions.Controls.Add(btnEditBook);
            pnlActions.Controls.Add(btnDelete);

            // ── Grid ──────────────────────────────────────────────────────────
            dgvInventory = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                BackgroundColor = ColBg,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 32 },
                GridColor = Color.FromArgb(220, 220, 230)
            };

            StyleGrid(dgvInventory);
            dgvInventory.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) BtnEditBook_Click(s, e); };
            dgvInventory.DataBindingComplete += DgvInventory_DataBindingComplete;

            // ── Layout ────────────────────────────────────────────────────────
            this.Controls.Add(dgvInventory);   // Fill (between Tops and Bottom)
            this.Controls.Add(pnlActions);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlTop);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DATA
        // ══════════════════════════════════════════════════════════════════════
        private void LoadInventoryData(string search = "")
        {
            try
            {
                string query = @"
                    SELECT
                        book_id        AS `ID`,
                        title          AS `Title`,
                        author         AS `Author`,
                        price          AS `Price (₱)`,
                        stock_quantity AS `Stock`,
                        isbn           AS `ISBN`
                    FROM books";

                MySql.Data.MySqlClient.MySqlParameter[] parms = Array.Empty<MySql.Data.MySqlClient.MySqlParameter>();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query += " WHERE title LIKE @search OR author LIKE @search OR isbn LIKE @search";
                    parms = new[] { new MySql.Data.MySqlClient.MySqlParameter("@search", "%" + search + "%") };
                }

                query += " ORDER BY book_id DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, parms);
                dgvInventory.DataSource = dt;

                int total = dt.Rows.Count;
                int outOf = 0;
                int low = 0;

                foreach (DataRow r in dt.Rows)
                {
                    int qty = Convert.ToInt32(r["Stock"]);
                    if (qty == 0) outOf++;
                    else if (qty <= 5) low++;
                }

                lblStatus.Text = $"{total} book(s) found   |   🔴 {outOf} out of stock   |   🟡 {low} low stock";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory:\n" + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Row colouring after bind ──────────────────────────────────────────
        private void DgvInventory_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                if (row.DataBoundItem is not DataRowView drv) continue;
                int qty = Convert.ToInt32(drv["Stock"]);

                if (qty == 0)
                {
                    row.DefaultCellStyle.BackColor = ColRowOut;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else if (qty <= 5)
                {
                    row.DefaultCellStyle.BackColor = ColRowLow;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  BUTTON HANDLERS
        // ══════════════════════════════════════════════════════════════════════

        // ── Add New Book ──────────────────────────────────────────────────────
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new BookDialog("Add New Book");
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                string sql = @"
                    INSERT INTO books (title, author, price, stock_quantity, isbn)
                    VALUES (@title, @author, @price, @stock, @isbn)";

                DatabaseHelper.ExecuteNonQuery(sql,
                    P("@title", dlg.BookTitle),
                    P("@author", dlg.Author),
                    P("@price", dlg.Price),
                    P("@stock", dlg.Stock),
                    P("@isbn", dlg.Isbn));

                LoadInventoryData();
                MessageBox.Show("Book added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding book:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Add Stock ─────────────────────────────────────────────────────────
        private void BtnAddStock_Click(object? sender, EventArgs e)
        {
            if (!TryGetSelectedId(out int id, out string title)) return;

            using var dlg = new StockDialog(title);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE books SET stock_quantity = stock_quantity + @qty WHERE book_id = @id",
                    P("@qty", dlg.Quantity),
                    P("@id", id));

                LoadInventoryData(txtSearch.Text.Trim());
                MessageBox.Show($"Added {dlg.Quantity} unit(s) to \"{title}\".", "Stock Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Edit Price ────────────────────────────────────────────────────────
        private void BtnEditPrice_Click(object? sender, EventArgs e)
        {
            if (!TryGetSelectedId(out int id, out string title)) return;

            // Read current price
            decimal currentPrice = 0;
            if (dgvInventory.CurrentRow?.DataBoundItem is DataRowView drv)
                currentPrice = Convert.ToDecimal(drv["Price (₱)"]);

            using var dlg = new PriceDialog(title, currentPrice);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE books SET price = @price WHERE book_id = @id",
                    P("@price", dlg.NewPrice),
                    P("@id", id));

                LoadInventoryData(txtSearch.Text.Trim());
                MessageBox.Show($"Price updated to ₱{dlg.NewPrice:N2} for \"{title}\".", "Price Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating price:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Edit Book ─────────────────────────────────────────────────────────
        private void BtnEditBook_Click(object? sender, EventArgs e)
        {
            if (!TryGetSelectedId(out int id, out string _)) return;

            if (dgvInventory.CurrentRow?.DataBoundItem is not DataRowView drv) return;

            using var dlg = new BookDialog("Edit Book",
                drv["Title"].ToString()!,
                drv["Author"].ToString()!,
                Convert.ToDecimal(drv["Price (₱)"]),
                Convert.ToInt32(drv["Stock"]),
                drv["ISBN"].ToString()!);

            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery(@"
                    UPDATE books
                    SET title=@title, author=@author,
                        price=@price, stock_quantity=@stock, isbn=@isbn
                    WHERE book_id=@id",
                    P("@title", dlg.BookTitle),
                    P("@author", dlg.Author),
                    P("@price", dlg.Price),
                    P("@stock", dlg.Stock),
                    P("@isbn", dlg.Isbn),
                    P("@id", id));

                LoadInventoryData(txtSearch.Text.Trim());
                MessageBox.Show("Book updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating book:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Delete Book ───────────────────────────────────────────────────────
        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (!TryGetSelectedId(out int id, out string title)) return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete \"{title}\"?\n\nThis action cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM books WHERE book_id = @id",
                    P("@id", id));

                LoadInventoryData(txtSearch.Text.Trim());
                MessageBox.Show($"\"{title}\" has been deleted.", "Deleted",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting book:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HELPERS
        // ══════════════════════════════════════════════════════════════════════
        private bool TryGetSelectedId(out int id, out string title)
        {
            id = 0;
            title = "";

            if (dgvInventory.CurrentRow == null ||
                dgvInventory.CurrentRow.DataBoundItem is not DataRowView drv)
            {
                MessageBox.Show("Please select a book first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            id = Convert.ToInt32(drv["ID"]);
            title = drv["Title"].ToString()!;
            return true;
        }

        private static MySql.Data.MySqlClient.MySqlParameter P(string name, object value) =>
            new MySql.Data.MySqlClient.MySqlParameter(name, value);

        private static Button MakeButton(string text, Color back, Point loc, Size sz)
        {
            var b = new Button
            {
                Text = text,
                Location = loc,
                Size = sz,
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private static void StyleGrid(DataGridView g)
        {
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            g.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(21, 101, 192);
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 255);

            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(21, 101, 192);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            g.EnableHeadersVisualStyles = false;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  DIALOG — Add / Edit Book
    // ══════════════════════════════════════════════════════════════════════════
    internal class BookDialog : Form
    {
        public string BookTitle => txtTitle.Text.Trim();
        public string Author => txtAuthor.Text.Trim();
        public decimal Price => decimal.Parse(txtPrice.Text);
        public int Stock => (int)numStock.Value;
        public string Isbn => txtIsbn.Text.Trim();

        private TextBox txtTitle = null!;
        private TextBox txtAuthor = null!;
        private TextBox txtPrice = null!;
        private NumericUpDown numStock = null!;
        private TextBox txtIsbn = null!;

        public BookDialog(string caption,
            string title = "", string author = "",
            decimal price = 0, int stock = 0, string isbn = "")
        {
            this.Text = caption;
            this.Size = new Size(420, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 248, 252);

            int y = 20;
            txtTitle = AddRow("Title:", title, ref y);
            txtAuthor = AddRow("Author:", author, ref y);
            txtPrice = AddRow("Price (₱):", price.ToString("F2"), ref y);

            // Stock uses NumericUpDown
            var lblS = new Label { Text = "Stock:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10f) };
            numStock = new NumericUpDown
            {
                Location = new Point(130, y - 2),
                Size = new Size(240, 26),
                Minimum = 0,
                Maximum = 99999,
                Value = stock,
                Font = new Font("Segoe UI", 10f)
            };
            this.Controls.Add(lblS);
            this.Controls.Add(numStock);
            y += 36;

            txtIsbn = AddRow("ISBN:", isbn, ref y);

            var btnOk = new Button
            {
                Text = "✔ Save",
                Location = new Point(130, y + 10),
                Size = new Size(100, 34),
                BackColor = Color.FromArgb(30, 136, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += BtnOk_Click;

            var btnCancel = new Button
            {
                Text = "✖ Cancel",
                Location = new Point(240, y + 10),
                Size = new Size(100, 34),
                BackColor = Color.FromArgb(180, 180, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.Add(btnOk);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }

        private TextBox AddRow(string label, string value, ref int y)
        {
            var lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 10f) };
            var txt = new TextBox { Location = new Point(130, y - 2), Size = new Size(240, 26), Text = value, Font = new Font("Segoe UI", 10f) };
            this.Controls.Add(lbl);
            this.Controls.Add(txt);
            y += 36;
            return txt;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) { Warn("Title is required."); return; }
            if (string.IsNullOrWhiteSpace(txtAuthor.Text)) { Warn("Author is required."); return; }
            if (!decimal.TryParse(txtPrice.Text, out decimal p) || p < 0) { Warn("Enter a valid price."); return; }
            this.DialogResult = DialogResult.OK;
        }

        private void Warn(string msg) =>
            MessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  DIALOG — Add Stock
    // ══════════════════════════════════════════════════════════════════════════
    internal class StockDialog : Form
    {
        public int Quantity => (int)numQty.Value;
        private NumericUpDown numQty = null!;

        public StockDialog(string bookTitle)
        {
            this.Text = "Add Stock";
            this.Size = new Size(360, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 248, 252);

            var lbl = new Label
            {
                Text = $"Book: {bookTitle}\nQuantity to add:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            };

            numQty = new NumericUpDown
            {
                Location = new Point(20, 70),
                Size = new Size(300, 30),
                Minimum = 1,
                Maximum = 9999,
                Value = 1,
                Font = new Font("Segoe UI", 12f)
            };

            var btnOk = new Button
            {
                Text = "✔ Add Stock",
                Location = new Point(20, 115),
                Size = new Size(140, 34),
                BackColor = Color.FromArgb(30, 136, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text = "✖ Cancel",
                Location = new Point(170, 115),
                Size = new Size(140, 34),
                BackColor = Color.FromArgb(180, 180, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] { lbl, numQty, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  DIALOG — Edit Price
    // ══════════════════════════════════════════════════════════════════════════
    internal class PriceDialog : Form
    {
        public decimal NewPrice => decimal.Parse(txtPrice.Text);
        private TextBox txtPrice = null!;

        public PriceDialog(string bookTitle, decimal currentPrice)
        {
            this.Text = "Edit Price";
            this.Size = new Size(360, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 248, 252);

            var lbl = new Label
            {
                Text = $"Book: {bookTitle}\nNew Price (₱):",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            };

            txtPrice = new TextBox
            {
                Location = new Point(20, 70),
                Size = new Size(300, 30),
                Text = currentPrice.ToString("F2"),
                Font = new Font("Segoe UI", 12f)
            };

            var btnOk = new Button
            {
                Text = "✔ Update Price",
                Location = new Point(20, 115),
                Size = new Size(145, 34),
                BackColor = Color.FromArgb(245, 124, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) =>
            {
                if (!decimal.TryParse(txtPrice.Text, out decimal p) || p < 0)
                { MessageBox.Show("Enter a valid price.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                this.DialogResult = DialogResult.OK;
            };

            var btnCancel = new Button
            {
                Text = "✖ Cancel",
                Location = new Point(175, 115),
                Size = new Size(145, 34),
                BackColor = Color.FromArgb(180, 180, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] { lbl, txtPrice, btnOk, btnCancel });
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
    }
}