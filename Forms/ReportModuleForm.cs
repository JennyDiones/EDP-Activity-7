using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BookstoreIS.Database;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing.Chart;

namespace BookstoreIS.Forms
{
    public partial class ReportModuleForm : Form
    {
        private DataGridView dgvReport = null!;
        private ComboBox cboReportType = null!;
        private DateTimePicker dtpFrom = null!;
        private DateTimePicker dtpTo = null!;
        private Button btnLoad = null!;
        private Button btnExport = null!;
        private Label lblTitle = null!;
        private Label lblFrom = null!;
        private Label lblTo = null!;
        private Label lblType = null!;

        private const string StoreCompanyName = "Jenny Bookstore";
        private const string CompanyAddress = "123 Rizal Street, Naga City, Camarines Sur";
        private const string CompanyContact = "Tel: (054) 123-4567 | info@jennybookstore.ph";
        private const string LogoPath = @"Resources\logo.png";

        private readonly int currentUserId;
        private readonly string currentUserName;

        public ReportModuleForm(int loggedInUserId, string loggedInUserName = "System Administrator")
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            currentUserId = loggedInUserId;
            currentUserName = loggedInUserName;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Report Generation Module - Jenny Bookstore";
            this.Size = new Size(1180, 740);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);

            lblTitle = new Label
            {
                Text = "📊 REPORT GENERATION MODULE",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(34, 139, 34)
            };

            lblType = new Label { Text = "Report Type:", Location = new Point(20, 75), AutoSize = true };
            cboReportType = new ComboBox
            {
                Location = new Point(120, 72),
                Size = new Size(280, 32),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboReportType.Items.AddRange(new string[] { "Sales Report", "Purchase Orders Report", "Inventory Summary", "Low Stock Items" });
            cboReportType.SelectedIndex = 0;

            lblFrom = new Label { Text = "From:", Location = new Point(430, 75), AutoSize = true };
            dtpFrom = new DateTimePicker { Location = new Point(480, 72), Size = new Size(160, 30), Value = DateTime.Now.AddMonths(-1) };

            lblTo = new Label { Text = "To:", Location = new Point(660, 75), AutoSize = true };
            dtpTo = new DateTimePicker { Location = new Point(690, 72), Size = new Size(160, 30), Value = DateTime.Now };

            btnLoad = new Button { Text = "🔍 Load Report", Location = new Point(870, 70), Size = new Size(130, 38), BackColor = Color.DodgerBlue, ForeColor = Color.White };
            btnExport = new Button { Text = "📤 Export to Excel", Location = new Point(1010, 70), Size = new Size(140, 38), BackColor = Color.SeaGreen, ForeColor = Color.White };

            dgvReport = new DataGridView
            {
                Location = new Point(20, 130),
                Size = new Size(1130, 550),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.AliceBlue }
            };

            btnLoad.Click += new EventHandler(BtnLoad_Click);
            btnExport.Click += new EventHandler(BtnExport_Click);

            Controls.Add(lblTitle);
            Controls.AddRange(new Control[] { lblType, cboReportType, lblFrom, dtpFrom, lblTo, dtpTo, btnLoad, btnExport, dgvReport });
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            string reportType = cboReportType.SelectedItem?.ToString() ?? "Sales Report";
            DataTable dt = reportType switch
            {
                "Sales Report" => GetSalesReport(),
                "Purchase Orders Report" => GetPurchaseOrdersReport(),
                "Inventory Summary" => GetInventorySummary(),
                "Low Stock Items" => GetLowStockReport(),
                _ => new DataTable()
            };

            dgvReport.DataSource = dt;
            lblTitle.Text = $"📊 {reportType} ({dt.Rows.Count} records)";
        }

        // Report methods (shortened for brevity)
        private DataTable GetSalesReport() => DatabaseHelper.GetDataTable("SELECT 'Sample Data' AS Info", null);
        private DataTable GetPurchaseOrdersReport() => DatabaseHelper.GetDataTable("SELECT 'Sample Data' AS Info", null);
        private DataTable GetInventorySummary() => DatabaseHelper.GetDataTable("SELECT 'Sample Data' AS Info", null);
        private DataTable GetLowStockReport() => DatabaseHelper.GetDataTable("SELECT 'Sample Data' AS Info", null);

        // ====================== FIXED EXPORT ======================
        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dgvReport.DataSource is not DataTable dt || dt.Rows.Count == 0)
            {
                MessageBox.Show("Please load a report first.", "Warning");
                return;
            }

            string reportName = cboReportType.SelectedItem?.ToString() ?? "Report";

            using SaveFileDialog sfd = new()
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"{reportName.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using ExcelPackage package = new();
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Report");

                int row = 1;

                // LOGO
                string logoFullPath = Path.Combine(Application.StartupPath, LogoPath);
                bool logoFound = File.Exists(logoFullPath);

                if (logoFound)
                {
                    var logo = ws.Drawings.AddPicture("CompanyLogo", new FileInfo(logoFullPath));
                    logo.SetPosition(0, 5, 0, 5);
                    logo.SetSize(140, 100);
                }

                // HEADER
                ws.Cells[row, 3].Value = StoreCompanyName;
                ws.Cells[row, 3].Style.Font.Size = 18;
                ws.Cells[row, 3].Style.Font.Bold = true;
                row++;

                ws.Cells[row, 3].Value = CompanyAddress; row++;
                ws.Cells[row, 3].Value = CompanyContact; row += 2;

                ws.Cells[row, 3].Value = reportName.ToUpper();
                ws.Cells[row, 3].Style.Font.Size = 14;
                ws.Cells[row, 3].Style.Font.Bold = true;
                row++;

                ws.Cells[row, 3].Value = $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                row++;
                ws.Cells[row, 3].Value = $"Period: {dtpFrom.Value:yyyy-MM-dd} to {dtpTo.Value:yyyy-MM-dd}";
                row += 3;

                // DATA
                ws.Cells[row, 1].LoadFromDataTable(dt, true);
                ws.Cells.AutoFitColumns();

                // SIGNATURE
                row += dt.Rows.Count + 5;
                ws.Cells[row, 1].Value = "Prepared By:";
                ws.Cells[row + 1, 1].Value = currentUserName;
                ws.Cells[row + 3, 1].Value = "_______________________________";
                ws.Cells[row + 4, 1].Value = "Signature Over Printed Name";
                ws.Cells[row + 5, 1].Value = $"Date: {DateTime.Now:yyyy-MM-dd}";

                // SHEET 2
                ExcelWorksheet chartSheet = package.Workbook.Worksheets.Add("Chart");
                var chart = chartSheet.Drawings.AddChart("SummaryChart", eChartType.ColumnClustered);
                chart.Title.Text = $"{reportName} Summary";
                chart.SetPosition(1, 0, 1, 0);
                chart.SetSize(1000, 550);

                package.SaveAs(new FileInfo(sfd.FileName));

                string msg = "✅ Export Successful!\n\n";
                msg += logoFound ? "✓ Logo Loaded\n" : "⚠ Logo not found (check Resources folder)\n";
                msg += "✓ Company Name\n";
                msg += "✓ Signature";

                MessageBox.Show(msg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}