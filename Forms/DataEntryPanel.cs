using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;
using MySql.Data.MySqlClient;

namespace BookstoreIS.Forms
{
    // ── ComboItem Class ─────────────────────────────────────────────────────
    public class ComboItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public ComboItem(int id, string? text)
        {
            Id = id;
            Text = text ?? string.Empty;
        }

        public override string ToString() => Text;
    }

    // ── Main Panel ──────────────────────────────────────────────────────────
    public class DataEntryPanel : Panel
    {
        private TabControl tabEntry;

        public DataEntryPanel()
        {
            BackColor = Color.FromArgb(255, 248, 220);
            Build();
        }

        private void Build()
        {
            var lbl = new Label
            {
                Text = "Data Entry",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            tabEntry = new TabControl
            {
                Location = new Point(0, 35),
                Size = new Size(770, 430)
            };

            tabEntry.TabPages.Add(BuildBookTab());
            tabEntry.TabPages.Add(BuildAuthorTab());
            tabEntry.TabPages.Add(BuildCategoryTab());
            tabEntry.TabPages.Add(BuildStockTab());

            Controls.AddRange(new Control[] { lbl, tabEntry });
        }

        // ═══════════════════════════════════════════════════════════════
        //  BOOK ENTRY TAB
        // ═══════════════════════════════════════════════════════════════
        private TabPage BuildBookTab()
        {
            var tp = new TabPage("Add Book");
            int y = 15;

            var txtTitle = AddField(tp, "Title *", 10, ref y, 400);
            var txtIsbn = AddField(tp, "ISBN", 10, ref y, 200);
            var txtPrice = AddField(tp, "Price (₱) *", 10, ref y, 150);
            var txtStock = AddField(tp, "Stock Quantity *", 10, ref y, 100);

            // Category and Author as plain-text ComboBoxes (matches your DB)
            var cboCat = AddCombo(tp, "Category *", 10, ref y, 350);
            var cboAuth = AddCombo(tp, "Author *", 10, ref y, 350);

            // Populate Category from DB
            LoadCombo(cboCat,
                      "SELECT category_id, category_name FROM categories ORDER BY category_name",
                      "categories",
                      fallbackSql: "SELECT category_id, name FROM categories ORDER BY name");

            // Populate Author from DB
            LoadCombo(cboAuth,
                      "SELECT author_id, CONCAT(first_name,' ',last_name) AS name FROM authors ORDER BY last_name",
                      "authors");

            var btnSave = MakeBtn("Save Book", Color.FromArgb(0, 120, 215));
            var btnClear = MakeBtn("Clear", Color.FromArgb(100, 100, 100));

            btnSave.Location = new Point(10, y);
            btnClear.Location = new Point(130, y);

            tp.Controls.AddRange(new Control[] { btnSave, btnClear });

            btnClear.Click += (sender, e) =>
            {
                txtTitle.Clear(); txtIsbn.Clear();
                txtPrice.Clear(); txtStock.Clear();
            };

            btnSave.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    MessageBox.Show("Title and Price are required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboCat.SelectedItem == null)
                {
                    MessageBox.Show("Please select a Category.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboAuth.SelectedItem == null)
                {
                    MessageBox.Show("Please select an Author.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Price must be a valid number (e.g. 199.99).",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // ✅ Matches your actual books columns:
                    // title, author, isbn, price, stock_quantity, category
                    string authorName = cboCat.SelectedItem?.ToString() ?? "";
                    string categoryName = cboCat.SelectedItem?.ToString() ?? "";
                    int stock = int.TryParse(txtStock.Text, out int sq) ? sq : 0;

                    // Get selected author name text directly
                    string selectedAuthor = cboAuth.SelectedItem?.ToString() ?? "";
                    string selectedCategory = cboCat.SelectedItem?.ToString() ?? "";

                    DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO books (title, author, isbn, price, stock_quantity, category)
                          VALUES (@t, @a, @i, @p, @s, @c)",
                        new MySqlParameter("@t", txtTitle.Text.Trim()),
                        new MySqlParameter("@a", selectedAuthor),
                        new MySqlParameter("@i", txtIsbn.Text.Trim()),
                        new MySqlParameter("@p", price),
                        new MySqlParameter("@s", stock),
                        new MySqlParameter("@c", selectedCategory));

                    MessageBox.Show("Book added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtTitle.Clear(); txtIsbn.Clear();
                    txtPrice.Clear(); txtStock.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving book:\n\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            return tp;
        }

        // ═══════════════════════════════════════════════════════════════
        //  SIMPLE ENTRY TABS
        // ═══════════════════════════════════════════════════════════════

        private TabPage BuildAuthorTab()
        {
            var tp = new TabPage("Add Author");
            int y = 20;

            var txtFirst = AddField(tp, "First Name *", 20, ref y, 200);
            var txtLast = AddField(tp, "Last Name *", 20, ref y, 200);
            var txtNat = AddField(tp, "Nationality", 20, ref y, 200);
            var txtBirth = AddField(tp, "Birth Year", 20, ref y, 80);

            var btnSave = MakeBtn("Save Author", Color.FromArgb(0, 120, 215));
            var btnClear = MakeBtn("Clear", Color.FromArgb(100, 100, 100));
            btnSave.Location = new Point(20, y);
            btnClear.Location = new Point(140, y);
            tp.Controls.AddRange(new Control[] { btnSave, btnClear });

            btnClear.Click += (s, e) => { txtFirst.Clear(); txtLast.Clear(); txtNat.Clear(); txtBirth.Clear(); };

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtFirst.Text) ||
                    string.IsNullOrWhiteSpace(txtLast.Text))
                {
                    MessageBox.Show("First Name and Last Name are required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    int birth = int.TryParse(txtBirth.Text, out int b) ? b : 0;

                    DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO authors (first_name, last_name, nationality, birth_year)
                          VALUES (@fn, @ln, @nat, @by)",
                        new MySqlParameter("@fn", txtFirst.Text.Trim()),
                        new MySqlParameter("@ln", txtLast.Text.Trim()),
                        new MySqlParameter("@nat", txtNat.Text.Trim()),
                        new MySqlParameter("@by", birth));

                    MessageBox.Show("Author added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtFirst.Clear(); txtLast.Clear(); txtNat.Clear(); txtBirth.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving author:\n\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            return tp;
        }

        private TabPage BuildCategoryTab()
        {
            var tp = new TabPage("Add Category");
            int y = 20;

            var txtName = AddField(tp, "Category Name *", 20, ref y, 250);
            var txtDesc = AddField(tp, "Description", 20, ref y, 350);

            var btnSave = MakeBtn("Save Category", Color.FromArgb(0, 120, 215));
            var btnClear = MakeBtn("Clear", Color.FromArgb(100, 100, 100));
            btnSave.Location = new Point(20, y);
            btnClear.Location = new Point(150, y);
            tp.Controls.AddRange(new Control[] { btnSave, btnClear });

            btnClear.Click += (s, e) => { txtName.Clear(); txtDesc.Clear(); };

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Category Name is required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // ✅ Try category_name column first, fall back to name
                    try
                    {
                        DatabaseHelper.ExecuteNonQuery(
                            "INSERT INTO categories (category_name, description) VALUES (@n, @d)",
                            new MySqlParameter("@n", txtName.Text.Trim()),
                            new MySqlParameter("@d", txtDesc.Text.Trim()));
                    }
                    catch
                    {
                        DatabaseHelper.ExecuteNonQuery(
                            "INSERT INTO categories (name, description) VALUES (@n, @d)",
                            new MySqlParameter("@n", txtName.Text.Trim()),
                            new MySqlParameter("@d", txtDesc.Text.Trim()));
                    }

                    MessageBox.Show("Category added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtName.Clear(); txtDesc.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving category:\n\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            return tp;
        }

        private TabPage BuildStockTab()
        {
            var tp = new TabPage("Update Stock");
            int y = 20;

            var lblBookId = new Label { Text = "Book ID:", Location = new Point(20, y), AutoSize = true };
            var txtBookId = new TextBox { Location = new Point(140, y - 2), Size = new Size(150, 26) };
            y += 40;

            var lblQty = new Label { Text = "New Quantity:", Location = new Point(20, y), AutoSize = true };
            var txtQty = new TextBox { Location = new Point(140, y - 2), Size = new Size(150, 26) };
            y += 40;

            var btnUpdate = MakeBtn("Update Stock", Color.FromArgb(0, 120, 215));
            btnUpdate.Location = new Point(20, y);

            var lblStatus = new Label
            {
                Location = new Point(20, y + 50),
                AutoSize = true,
                ForeColor = Color.Green
            };

            btnUpdate.Click += (sender, e) =>
            {
                if (!int.TryParse(txtBookId.Text, out int bookId) ||
                    !int.TryParse(txtQty.Text, out int qty))
                {
                    MessageBox.Show("Please enter valid numbers.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    int rows = DatabaseHelper.ExecuteNonQuery(
                        "UPDATE books SET stock_quantity = @qty WHERE book_id = @id",
                        new MySqlParameter("@qty", qty),
                        new MySqlParameter("@id", bookId));

                    if (rows == 0)
                        lblStatus.Text = $"No book found with ID {bookId}.";
                    else
                        lblStatus.Text = $"✔ Stock updated for Book ID {bookId}.";

                    txtBookId.Clear();
                    txtQty.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating stock:\n\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            tp.Controls.AddRange(new Control[]
                { lblBookId, txtBookId, lblQty, txtQty, btnUpdate, lblStatus });
            return tp;
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════

        private static TextBox AddField(Control parent, string labelText,
                                        int x, ref int y, int width)
        {
            parent.Controls.Add(new Label
            {
                Text = labelText,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            });

            var tb = new TextBox
            {
                Location = new Point(x + 150, y - 2),
                Size = new Size(width, 26)
            };

            parent.Controls.Add(tb);
            y += 35;
            return tb;
        }

        private static ComboBox AddCombo(Control parent, string labelText,
                                         int x, ref int y, int width)
        {
            parent.Controls.Add(new Label
            {
                Text = labelText,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            });

            var cbo = new ComboBox
            {
                Location = new Point(x + 150, y - 2),
                Size = new Size(width, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            parent.Controls.Add(cbo);
            y += 35;
            return cbo;
        }

        private static Button MakeBtn(string text, Color bg)
        {
            return new Button
            {
                Text = text,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 32),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
        }

        /// <summary>
        /// Loads a ComboBox from the DB. If the primary SQL fails, tries fallbackSql.
        /// Shows a visible error message instead of silently failing.
        /// </summary>
        private static void LoadCombo(ComboBox cbo, string sql, string tableName,
                                      string? fallbackSql = null)
        {
            cbo.Items.Clear();

            DataTable? dt = null;

            // 1️⃣ Try primary SQL
            try
            {
                dt = DatabaseHelper.GetDataTable(sql);
            }
            catch { }

            // 2️⃣ Try fallback SQL if primary failed
            if ((dt == null || dt.Rows.Count == 0) && fallbackSql != null)
            {
                try
                {
                    dt = DatabaseHelper.GetDataTable(fallbackSql);
                }
                catch { }
            }

            // 3️⃣ Show error if still no data
            if (dt == null)
            {
                MessageBox.Show(
                    $"Could not load '{tableName}' dropdown.\n\n" +
                    $"Make sure the '{tableName}' table exists in your database.",
                    "Dropdown Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 4️⃣ Populate ComboBox
            foreach (DataRow r in dt.Rows)
            {
                int id = Convert.ToInt32(r[0]);
                string text = r[1]?.ToString() ?? string.Empty;
                cbo.Items.Add(new ComboItem(id, text));
            }

            if (cbo.Items.Count > 0)
                cbo.SelectedIndex = 0;
            else
                MessageBox.Show(
                    $"The '{tableName}' table is empty.\n\n" +
                    $"Add some records to '{tableName}' first using the matching tab.",
                    "No Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }
    }
}