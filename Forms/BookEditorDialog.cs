using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;

namespace BookstoreIS.Forms
{
    public class BookEditorDialog : Form
    {
        private int? _bookId;
        private TextBox txtTitle = null!;
        private TextBox txtIsbn = null!;
        private TextBox txtPrice = null!;
        private TextBox txtStock = null!;
        private TextBox txtYear = null!;
        private ComboBox cboPublisher = null!;
        private ComboBox cboCategory = null!;
        private ComboBox cboAuthor = null!;

        public BookEditorDialog(DataGridViewRow? row = null)
        {
            _bookId = row != null ? Convert.ToInt32(row.Cells["ID"].Value) : null;
            InitializeComponent();
            LoadComboBoxes();

            if (row != null)
                LoadExistingData(row);
        }

        private void InitializeComponent()
        {
            Text = _bookId.HasValue ? "Edit Book" : "Add New Book";
            Size = new Size(540, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(255, 248, 220);   // Matching your required background
            Font = new Font("Segoe UI", 9.5f);

            int y = 25;

            txtTitle = AddTextField("Title *", ref y);
            txtIsbn = AddTextField("ISBN", ref y);
            txtPrice = AddTextField("Price (₱) *", ref y);
            txtStock = AddTextField("Stock Quantity", ref y);
            txtYear = AddTextField("Publication Year", ref y);

            cboPublisher = AddComboField("Publisher *", ref y);
            cboCategory = AddComboField("Category *", ref y);
            cboAuthor = AddComboField("Author *", ref y);

            var btnSave = new Button
            {
                Text = "Save",
                Size = new Size(110, 38),
                Location = new Point(280, y + 20),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(110, 38),
                Location = new Point(400, y + 20),
                FlatStyle = FlatStyle.Flat
            };

            Controls.Add(btnSave);
            Controls.Add(btnCancel);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        private TextBox AddTextField(string label, ref int y)
        {
            var lbl = new Label { Text = label, Location = new Point(30, y), AutoSize = true };
            var tb = new TextBox { Location = new Point(190, y - 3), Size = new Size(300, 28) };
            Controls.Add(lbl);
            Controls.Add(tb);
            y += 48;
            return tb;
        }

        private ComboBox AddComboField(string label, ref int y)
        {
            var lbl = new Label { Text = label, Location = new Point(30, y), AutoSize = true };
            var cbo = new ComboBox
            {
                Location = new Point(190, y - 3),
                Size = new Size(300, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Controls.Add(lbl);
            Controls.Add(cbo);
            y += 48;
            return cbo;
        }

        private void LoadComboBoxes()
        {
            LoadIntoCombo(cboPublisher, "SELECT publisher_id, name FROM publishers ORDER BY name");
            LoadIntoCombo(cboCategory, "SELECT category_id, name FROM categories ORDER BY name");
            LoadIntoCombo(cboAuthor, "SELECT author_id, CONCAT(first_name, ' ', last_name) AS name FROM authors ORDER BY last_name");
        }

        private void LoadIntoCombo(ComboBox cbo, string sql)
        {
            try
            {
                var dt = DatabaseHelper.GetDataTable(sql);
                foreach (DataRow r in dt.Rows)
                {
                    cbo.Items.Add(new ComboItem(Convert.ToInt32(r[0]), r[1].ToString()));
                }
                if (cbo.Items.Count > 0) cbo.SelectedIndex = 0;
            }
            catch { }
        }

        private void LoadExistingData(DataGridViewRow row)
        {
            txtTitle.Text = row.Cells["Title"]?.Value?.ToString() ?? "";
            txtIsbn.Text = row.Cells["ISBN"]?.Value?.ToString() ?? "";
            txtPrice.Text = row.Cells["Price"]?.Value?.ToString() ?? "";
            txtStock.Text = row.Cells["Stock"]?.Value?.ToString() ?? "";
            txtYear.Text = row.Cells["Year"]?.Value?.ToString() ?? "";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Book saved successfully! (Demo)", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
    }
}