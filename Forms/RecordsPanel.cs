using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;
using MySql.Data.MySqlClient;

namespace BookstoreIS.Forms
{
    public class RecordsPanel : Panel
    {
        private TabControl tabMain;
        private DataGridView dgvBooks, dgvAuthors, dgvPublishers, dgvCategories;
        private TextBox txtSearch;
        private Button btnSearch, btnRefresh, btnAdd, btnEdit, btnDelete;

        public RecordsPanel()
        {
            BackColor = Color.White;
            Build();
            LoadAll();
        }

        private void Build()
        {
            var lblHead = new Label
            {
                Text = "Records",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0)
            };

            txtSearch = new TextBox
            {
                Location = new Point(0, 35),
                Size = new Size(250, 26),
                PlaceholderText = "Search books…",
                Font = new Font("Segoe UI", 9.5f)
            };

            btnSearch = MakeBtn("Search", 260, 34, Color.FromArgb(0, 120, 215));
            btnRefresh = MakeBtn("Refresh", 345, 34, Color.FromArgb(80, 80, 80));
            btnAdd = MakeBtn("+ Add", 440, 34, Color.FromArgb(0, 160, 80));
            btnEdit = MakeBtn("Edit", 520, 34, Color.FromArgb(200, 140, 0));
            btnDelete = MakeBtn("Delete", 590, 34, Color.FromArgb(200, 0, 0));

            btnSearch.Click += (s, e) => SearchBooks();
            btnRefresh.Click += (s, e) => LoadAll();
            btnAdd.Click += (s, e) => OpenBookEditor(null);
            btnEdit.Click += (s, e) => EditSelectedBook();
            btnDelete.Click += (s, e) => DeleteSelectedBook();

            tabMain = new TabControl
            {
                Location = new Point(0, 70),
                Size = new Size(770, 380)
            };

            dgvBooks = MakeGrid();
            dgvAuthors = MakeGrid();
            dgvPublishers = MakeGrid();
            dgvCategories = MakeGrid();

            tabMain.TabPages.Add(MakeTab("Books", dgvBooks));
            tabMain.TabPages.Add(MakeTab("Authors", dgvAuthors));
            tabMain.TabPages.Add(MakeTab("Publishers", dgvPublishers));
            tabMain.TabPages.Add(MakeTab("Categories", dgvCategories));

            Controls.AddRange(new Control[] { lblHead, txtSearch, btnSearch, btnRefresh,
                btnAdd, btnEdit, btnDelete, tabMain });
        }

        private static TabPage MakeTab(string title, DataGridView dgv)
        {
            var tp = new TabPage(title);
            dgv.Dock = DockStyle.Fill;
            tp.Controls.Add(dgv);
            return tp;
        }

        private static DataGridView MakeGrid()
        {
            var g = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(30,40,60),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                },
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                GridColor = Color.FromArgb(220, 220, 220)
            };
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            return g;
        }

        private static Button MakeBtn(string text, int x, int y, Color bg)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(70, 27),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5f)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void LoadAll()
        {
            LoadBooks();
            LoadAuthors();
            LoadPublishers();
            LoadCategories();
        }

        private void LoadBooks(string filter = "")
        {
            try
            {
                string sql = @"SELECT b.book_id AS ID, b.title AS Title, b.isbn AS ISBN, 
                               CONCAT(a.first_name,' ',a.last_name) AS Author, 
                               p.name AS Publisher, c.name AS Category, 
                               b.price AS Price, b.stock_quantity AS Stock, 
                               b.publication_year AS Year 
                               FROM books b 
                               JOIN book_authors ba ON b.book_id = ba.book_id 
                               JOIN authors a ON ba.author_id = a.author_id 
                               JOIN publishers p ON b.publisher_id = p.publisher_id 
                               JOIN categories c ON b.category_id = c.category_id";

                if (!string.IsNullOrEmpty(filter))
                    sql += " WHERE b.title LIKE @f OR a.first_name LIKE @f OR a.last_name LIKE @f";

                var parameters = string.IsNullOrEmpty(filter)
                    ? Array.Empty<MySqlParameter>()
                    : new[] { new MySqlParameter("@f", $"%{filter}%") };

                dgvBooks.DataSource = DatabaseHelper.GetDataTable(sql, parameters);
            }
            catch
            {
                dgvBooks.DataSource = SampleBooksTable();
            }
        }

        private void LoadAuthors()
        {
            try
            {
                dgvAuthors.DataSource = DatabaseHelper.GetDataTable(
                    "SELECT author_id AS ID, first_name AS First, last_name AS Last, " +
                    "nationality AS Nationality, birth_year AS BirthYear FROM authors ORDER BY last_name");
            }
            catch
            {
                dgvAuthors.DataSource = SampleTable(new[] { "ID", "First", "Last", "Nationality", "BirthYear" },
                    1, "John", "Doe", "Filipino", 1990);
            }
        }

        private void LoadPublishers()
        {
            try
            {
                dgvPublishers.DataSource = DatabaseHelper.GetDataTable(
                    "SELECT publisher_id AS ID, name AS Name, country AS Country, " +
                    "founded_year AS Founded FROM publishers ORDER BY name");
            }
            catch
            {
                dgvPublishers.DataSource = SampleTable(new[] { "ID", "Name", "Country", "Founded" },
                    1, "Sample Press", "Philippines", 2000);
            }
        }

        private void LoadCategories()
        {
            try
            {
                dgvCategories.DataSource = DatabaseHelper.GetDataTable(
                    "SELECT category_id AS ID, name AS Name, description AS Description " +
                    "FROM categories ORDER BY name");
            }
            catch
            {
                dgvCategories.DataSource = SampleTable(new[] { "ID", "Name", "Description" },
                    1, "Fiction", "Fiction books");
            }
        }

        private void SearchBooks() => LoadBooks(txtSearch.Text.Trim());

        private void OpenBookEditor(DataGridViewRow? row)
        {
            using var dlg = new BookEditorDialog(row);
            if (dlg.ShowDialog() == DialogResult.OK)
                LoadBooks();
        }

        private void EditSelectedBook()
        {
            if (tabMain.SelectedIndex != 0 || dgvBooks.SelectedRows.Count == 0) return;
            OpenBookEditor(dgvBooks.SelectedRows[0]);
        }

        private void DeleteSelectedBook()
        {
            if (tabMain.SelectedIndex != 0 || dgvBooks.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvBooks.SelectedRows[0].Cells["ID"].Value);

            if (MessageBox.Show($"Delete book ID {id}?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM books WHERE book_id=@id",
                        new MySqlParameter("@id", id));
                    LoadBooks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ── Sample Data Methods ─────────────────────────────────────────────
        private static DataTable SampleBooksTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Title");
            dt.Columns.Add("ISBN");
            dt.Columns.Add("Author");
            dt.Columns.Add("Publisher");
            dt.Columns.Add("Category");
            dt.Columns.Add("Price");
            dt.Columns.Add("Stock");
            dt.Columns.Add("Year");

            dt.Rows.Add(1, "Clean Code", "9780132350884", "Robert Martin", "Prentice Hall", "Programming", "450.00", 25, 2008);
            dt.Rows.Add(2, "The Pragmatic Programmer", "9780135957059", "Andrew Hunt", "Addison-Wesley", "Programming", "520.00", 15, 2019);
            return dt;
        }

        private static DataTable SampleTable(string[] cols, params object[] row)
        {
            var dt = new DataTable();
            foreach (var c in cols)
                dt.Columns.Add(c);

            if (row != null && row.Length > 0)
            {
                if (row.Length > cols.Length)
                    Array.Resize(ref row, cols.Length);
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}