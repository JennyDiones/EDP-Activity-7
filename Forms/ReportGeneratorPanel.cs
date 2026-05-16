using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookstoreIS.Database;
using ClosedXML.Excel;

namespace BookstoreIS.Forms
{
    public class ReportGeneratorPanel : Panel
    {
        private ComboBox cboReport = null!;
        private RadioButton rdoExcel = null!;
        private RadioButton rdoPdf = null!;
        private RadioButton rdoCsv = null!;
        private DateTimePicker dtpFrom = null!;
        private DateTimePicker dtpTo = null!;
        private CheckBox chkID = null!;
        private CheckBox chkName = null!;
        private CheckBox chkDate = null!;
        private CheckBox chkStatus = null!;
        private CheckBox chkRemarks = null!;
        private DataGridView dgvPreview = null!;
        private Button btnClear = null!;
        private Button btnPrint = null!;
        private Button btnExcel = null!;
        private Button btnGenerate = null!;

        private PictureBox picLogo = null!;
        private Label lblSignedBy = null!;
        private Panel pnlSignatureLine = null!;

        public ReportGeneratorPanel()
        {
            BackColor = Color.White;
            Build();
            Preview();
        }

        private void Build()
        {
            // ... [All UI code remains the same - only Preview part changed] ...
            var lblHead = new Label { Text = "Report Generator", Font = new Font("Segoe UI", 13f, FontStyle.Bold), AutoSize = true, Location = new Point(0, 0) };

            var grpType = new GroupBox { Text = "Report Type", Location = new Point(0, 35), Size = new Size(770, 80) };
            grpType.Controls.Add(new Label { Text = "Select Report:", Location = new Point(10, 25), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) });

            cboReport = new ComboBox { Location = new Point(10, 45), Size = new Size(200, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cboReport.Items.AddRange(new object[] { "Monthly Summary Report", "Low Stock Alert", "Books by Category", "Full Book Inventory", "Author List" });
            cboReport.SelectedIndex = 0;
            cboReport.SelectedIndexChanged += (s, e) => Preview();

            grpType.Controls.Add(new Label { Text = "Format:", Location = new Point(230, 25), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) });
            rdoExcel = new RadioButton { Text = "Excel(.xlsx)", Location = new Point(230, 45), AutoSize = true, Checked = true };
            rdoPdf = new RadioButton { Text = "Pdf", Location = new Point(340, 45), AutoSize = true };
            rdoCsv = new RadioButton { Text = "CSV", Location = new Point(400, 45), AutoSize = true };
            grpType.Controls.AddRange(new Control[] { cboReport, rdoExcel, rdoPdf, rdoCsv });

            var grpDate = new GroupBox { Text = "Date Range", Location = new Point(0, 125), Size = new Size(770, 70) };
            grpDate.Controls.Add(new Label { Text = "From", Location = new Point(10, 25), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) });
            dtpFrom = new DateTimePicker { Location = new Point(10, 42), Size = new Size(180, 26), Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) };
            grpDate.Controls.Add(new Label { Text = "To", Location = new Point(210, 25), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) });
            dtpTo = new DateTimePicker { Location = new Point(210, 42), Size = new Size(180, 26), Value = DateTime.Today };
            grpDate.Controls.AddRange(new Control[] { dtpFrom, dtpTo });

            var grpCols = new GroupBox { Text = "Include Columns", Location = new Point(0, 205), Size = new Size(770, 50) };
            chkID = new CheckBox { Text = "ID / Record no.", Location = new Point(10, 20), AutoSize = true, Checked = true };
            chkName = new CheckBox { Text = "Name", Location = new Point(145, 20), AutoSize = true, Checked = true };
            chkDate = new CheckBox { Text = "Date", Location = new Point(220, 20), AutoSize = true, Checked = true };
            chkStatus = new CheckBox { Text = "Status", Location = new Point(280, 20), AutoSize = true, Checked = true };
            chkRemarks = new CheckBox { Text = "Remarks", Location = new Point(355, 20), AutoSize = true };
            grpCols.Controls.AddRange(new Control[] { chkID, chkName, chkDate, chkStatus, chkRemarks });

            var lblPrev = new Label { Text = "Preview", Font = new Font("Segoe UI", 10f, FontStyle.Bold), AutoSize = true, Location = new Point(0, 265) };
            dgvPreview = new DataGridView
            {
                Location = new Point(0, 287),
                Size = new Size(770, 155),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            btnClear = MakeBtn("Clear", Color.FromArgb(100, 100, 100), 420, 452);
            btnPrint = MakeBtn("Print", Color.FromArgb(80, 80, 80), 500, 452);
            btnExcel = MakeBtn("Export Excel", Color.FromArgb(0, 130, 80), 580, 452);
            btnGenerate = MakeBtn("Generate Report", Color.FromArgb(0, 120, 215), 680, 452);

            btnClear.Click += (s, e) => ResetForm();
            btnPrint.Click += (s, e) => MessageBox.Show("Print sent to printer (demo).", "Print");
            btnExcel.Click += (s, e) => ExportToExcel();
            btnGenerate.Click += (s, e) => { Preview(); MessageBox.Show("Report generated successfully!", "Success"); };

            picLogo = new PictureBox { Size = new Size(140, 50), Location = new Point(25, 305), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            LoadLogo();

            lblSignedBy = new Label { Text = "Signed by:", Location = new Point(280, 320), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            pnlSignatureLine = new Panel { Location = new Point(450, 545), Size = new Size(280, 2), BackColor = Color.Black };

            Controls.AddRange(new Control[] { lblHead, grpType, grpDate, grpCols, lblPrev, dgvPreview,
                btnClear, btnPrint, btnExcel, btnGenerate, picLogo, lblSignedBy, pnlSignatureLine });
        }

        private void LoadLogo()
        {
            string[] paths = { "logo.png", @"Images\logo.png", Path.Combine(Application.StartupPath, "logo.png"), Path.Combine(Application.StartupPath, @"Images\logo.png") };
            foreach (string path in paths)
                if (File.Exists(path)) { try { picLogo.Image = Image.FromFile(path); return; } catch { } }
        }

        private void ResetForm()
        {
            cboReport.SelectedIndex = 0;
            dtpFrom.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpTo.Value = DateTime.Today;
            Preview();
        }

        private static Button MakeBtn(string text, Color bg, int x, int y)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                MinimumSize = new Size(80, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void Preview()
        {
            try
            {
                string sql = GetReportSql();
                DataTable dt = DatabaseHelper.GetDataTable(sql);

                dgvPreview.DataSource = dt;

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No records found.\nTry adding some books in Data Entry.",
                                  "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading preview:\n" + ex.Message, "Database Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgvPreview.DataSource = null;
            }

            ApplyColumnVisibility();
        }

        private string GetReportSql()
        {
            // Simplified query matching your actual books table structure
            return cboReport.SelectedIndex switch
            {
                1 => "SELECT title AS Title, isbn AS ISBN, stock_quantity AS Stock, price AS Price, author AS Author " +
                     "FROM books WHERE stock_quantity <= 40 ORDER BY stock_quantity LIMIT 50",

                _ => "SELECT book_id AS ID, title AS Title, author AS Author, price AS Price, " +
                     "stock_quantity AS Stock, category AS Category " +
                     "FROM books ORDER BY book_id LIMIT 100"
            };
        }

        private void ApplyColumnVisibility()
        {
            if (dgvPreview.Columns["ID"] != null) dgvPreview.Columns["ID"].Visible = chkID.Checked;
            if (dgvPreview.Columns["Title"] != null) dgvPreview.Columns["Title"].Visible = chkName.Checked;
            if (dgvPreview.Columns["Author"] != null) dgvPreview.Columns["Author"].Visible = chkName.Checked;
            if (dgvPreview.Columns["Price"] != null) dgvPreview.Columns["Price"].Visible = true;
            if (dgvPreview.Columns["Stock"] != null) dgvPreview.Columns["Stock"].Visible = true;
        }

        private void ExportToExcel()
        {
            if (dgvPreview.DataSource 
                is not DataTable dt || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Warning");
                return;
            }

            using SaveFileDialog sfd = new() { Filter = "Excel Files (*.xlsx)|*.xlsx", FileName = $"BookstoreReport_{DateTime.Now:yyyyMMdd_HHmmss}" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Report");

                string logoPath = FindLogoPath();
                if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                    ws.AddPicture(logoPath).MoveTo(ws.Cell("A1")).Scale(0.40);

                ws.Cell(8, 1).InsertTable(dt, true);

                int lastRow = dt.Rows.Count + 12;
                ws.Cell(lastRow, 1).Value = "Signed by:_______________________________";

                wb.SaveAs(sfd.FileName);
                MessageBox.Show($"Successfully exported to:\n{sfd.FileName}", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed: " + ex.Message, "Error");
            }
        }

        private string FindLogoPath()
        {
            string[] paths = { "logo.png", @"Images\logo.png", Path.Combine(Application.StartupPath, "logo.png"), Path.Combine(Application.StartupPath, @"Images\logo.png") };
            return paths.FirstOrDefault(File.Exists) ?? string.Empty;
        }
    }
}