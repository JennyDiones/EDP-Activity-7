using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;
using MySql.Data.MySqlClient;

namespace BookstoreIS.Forms
{
    public partial class PurchaseOrderForm : Form
    {
        private DataGridView dgvOrders = null!;
        private DataGridView dgvItems = null!;
        private Button btnRefresh = null!;
        private Button btnReadyToShip = null!;
        private Button btnMarkReceived = null!;
        private Button btnCancelOrder = null!;
        private Label lblItemsHeader = null!;
        private Panel itemsPanel = null!;

        private readonly int currentUserId;
        private int selectedPoId = -1;

        public PurchaseOrderForm(int loggedInUserId)
        {
            currentUserId = loggedInUserId;
            InitializeComponent();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.Text = "Purchase Order & Sales History";
            this.Size = new Size(1300, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);

            // Header
            var header = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.FromArgb(34, 139, 34) };
            header.Controls.Add(new Label
            {
                Text = "Purchase Order & Sales History",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Location = new Point(20, 13),
                AutoSize = true
            });

            // Buttons
            btnRefresh = CreateStyledButton("🔄 Refresh", 10, Color.FromArgb(40, 167, 69));
            btnReadyToShip = CreateStyledButton("📦 Mark as Ready to Ship", 150, Color.Orange);
            btnMarkReceived = CreateStyledButton("✅ Mark as Received", 370, Color.FromArgb(40, 167, 69));
            btnCancelOrder = CreateStyledButton("❌ Cancel Order", 580, Color.FromArgb(220, 53, 69));

            btnRefresh.Click += (s, e) => RefreshAll();
            btnReadyToShip.Click += BtnReadyToShip_Click;
            btnMarkReceived.Click += BtnMarkReceived_Click;
            btnCancelOrder.Click += BtnCancelOrder_Click;

            // Legend
            var legendPanel = new Panel
            {
                Location = new Point(760, 68),
                Size = new Size(520, 32),
                BackColor = Color.White
            };
            AddLegendDot(legendPanel, Color.Orange, "Ready to Ship", 0);
            AddLegendDot(legendPanel, Color.LightGreen, "Received", 130);
            AddLegendDot(legendPanel, Color.LightCoral, "Cancelled", 240);
            AddLegendDot(legendPanel, Color.LightSkyBlue, "Sold", 350);

            // Orders Grid
            dgvOrders = new DataGridView
            {
                Location = new Point(10, 120),
                Size = new Size(1270, 310),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                RowHeadersVisible = false,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(240, 255, 240)
                }
            };

            // Use DataBindingComplete for reliable row coloring
            dgvOrders.DataBindingComplete += DgvOrders_DataBindingComplete;
            dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;

            // Items Panel
            itemsPanel = new Panel
            {
                Location = new Point(10, 440),
                Size = new Size(1270, 295),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 252, 248)
            };

            lblItemsHeader = new Label
            {
                Text = "▼ Select an order above to see its books",
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                AutoSize = true
            };

            dgvItems = new DataGridView
            {
                Location = new Point(5, 38),
                Size = new Size(1255, 248),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false
            };

            itemsPanel.Controls.Add(lblItemsHeader);
            itemsPanel.Controls.Add(dgvItems);

            Controls.Add(header);
            Controls.Add(btnRefresh);
            Controls.Add(btnReadyToShip);
            Controls.Add(btnMarkReceived);
            Controls.Add(btnCancelOrder);
            Controls.Add(legendPanel);
            Controls.Add(dgvOrders);
            Controls.Add(itemsPanel);
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private Button CreateStyledButton(string text, int x, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 68),
                Size = new Size(200, 42),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void AddLegendDot(Panel parent, Color color, string text, int x)
        {
            parent.Controls.Add(new Panel { Location = new Point(x, 8), Size = new Size(16, 16), BackColor = color });
            parent.Controls.Add(new Label { Text = text, Location = new Point(x + 20, 6), AutoSize = true, Font = new Font("Segoe UI", 9f) });
        }

        // ── Safe column reader ────────────────────────────────────────────────
        private string GetCellValue(DataGridViewRow row, string columnName)
        {
            // Check via the grid's Columns collection (accepts string name)
            if (dgvOrders.Columns.Contains(columnName))
                return row.Cells[dgvOrders.Columns[columnName].Index].Value?.ToString() ?? "";
            return "";
        }

        // ── Data loading ──────────────────────────────────────────────────────
        private void LoadOrders()
        {
            string sql = @"
                SELECT 
                    po.po_id        AS `PO ID`,
                    CASE 
                        WHEN po.supplier_id IS NULL AND po.status = 'Sold' THEN 'Walk-in'
                        ELSE 'Online Order'
                    END             AS `Order Type`,
                    po.order_date   AS `Order Date`,
                    po.status       AS `Status`,
                    CONCAT('₱ ', FORMAT(po.total_cost, 2)) AS `Total Cost`,
                    u.username      AS `Created By`,
                    COUNT(pi.book_id) AS `# Books`
                FROM purchase_orders po
                LEFT JOIN suppliers s  ON po.supplier_id = s.supplier_id
                JOIN  users u          ON po.user_id     = u.user_id
                LEFT JOIN po_items pi  ON po.po_id       = pi.po_id
                GROUP BY po.po_id
                ORDER BY po.order_date DESC";

            dgvOrders.DataSource = null;
            dgvOrders.Columns.Clear();
            dgvOrders.DataSource = DatabaseHelper.GetDataTable(sql);
        }

        private void RefreshAll()
        {
            LoadOrders();
            ClearItemsPanel();
        }

        // ── Row colouring — fires AFTER data is fully bound ───────────────────
        private void DgvOrders_DataBindingComplete(object? sender,
            DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvOrders.Rows)
            {
                string status = GetCellValue(row, "Status");
                row.DefaultCellStyle.BackColor = status switch
                {
                    "ReadyToShip" => Color.Orange,
                    "Ready to Ship" => Color.Orange,
                    "Received" => Color.LightGreen,
                    "Cancelled" => Color.LightCoral,
                    "Sold" => Color.LightSkyBlue,
                    _ => Color.Gold
                };

                // Keep text readable on all backgrounds
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        // ── Selection changed ─────────────────────────────────────────────────
        private void DgvOrders_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                selectedPoId = -1;
                return;
            }

            var row = dgvOrders.SelectedRows[0];

            // Safe reads using helper
            string poIdStr = GetCellValue(row, "PO ID");
            string status = GetCellValue(row, "Status");
            string type = GetCellValue(row, "Order Type");

            if (!int.TryParse(poIdStr, out selectedPoId))
            {
                selectedPoId = -1;
                return;
            }

            lblItemsHeader.Text = $"PO #{selectedPoId}  |  {type}  |  Status: {status}";
            LoadOrderItems(selectedPoId);
        }

        private void LoadOrderItems(int poId)
        {
            string sql = @"
                SELECT 
                    b.title   AS `Book Title`,
                    b.author  AS `Author`,
                    pi.quantity                             AS `Qty`,
                    CONCAT('₱ ', FORMAT(pi.unit_cost, 2))  AS `Unit Cost`,
                    CONCAT('₱ ', FORMAT(pi.subtotal,  2))  AS `Subtotal`
                FROM po_items pi
                JOIN books b ON pi.book_id = b.book_id
                WHERE pi.po_id = @po
                ORDER BY b.title";

            dgvItems.DataSource = DatabaseHelper.GetDataTable(sql,
                new MySqlParameter("@po", poId));
        }

        private void ClearItemsPanel()
        {
            dgvItems.DataSource = null;
            lblItemsHeader.Text = "▼ Select an order above to see its books";
            selectedPoId = -1;
        }

        // ── Status update ─────────────────────────────────────────────────────
        private void BtnReadyToShip_Click(object? sender, EventArgs e)
            => UpdateStatus("Ready to Ship", "Ready to Ship");

        private void BtnMarkReceived_Click(object? sender, EventArgs e)
            => UpdateStatus("Received", "Received", updateStock: true);

        private void UpdateStatus(string newStatus, string displayName,
            bool updateStock = false)
        {
            if (selectedPoId <= 0)
            {
                MessageBox.Show("Please select an order first!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string current = GetCellValue(dgvOrders.SelectedRows[0], "Status");

            if (current == "Received" || current == "Cancelled")
            {
                MessageBox.Show($"This order is already '{current}'.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Change status to '{displayName}'?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                if (updateStock)
                {
                    var items = DatabaseHelper.GetDataTable(
                        "SELECT book_id, quantity FROM po_items WHERE po_id = @po",
                        new MySqlParameter("@po", selectedPoId));

                    foreach (DataRow r in items.Rows)
                    {
                        DatabaseHelper.ExecuteNonQuery(
                            "UPDATE books SET stock_quantity = stock_quantity + @qty WHERE book_id = @bid",
                            new MySqlParameter("@qty", Convert.ToInt32(r["quantity"])),
                            new MySqlParameter("@bid", Convert.ToInt32(r["book_id"])));
                    }
                }

                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE purchase_orders SET status = @st WHERE po_id = @id",
                    new MySqlParameter("@st", newStatus),
                    new MySqlParameter("@id", selectedPoId));

                MessageBox.Show($"Order #{selectedPoId} is now '{displayName}'!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelOrder_Click(object? sender, EventArgs e)
        {
            if (selectedPoId <= 0)
            {
                MessageBox.Show("Please select an order first!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string current = GetCellValue(dgvOrders.SelectedRows[0], "Status");
            if (current == "Received" || current == "Cancelled")
            {
                MessageBox.Show($"Cannot cancel — order is already '{current}'.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Cancel Order #{selectedPoId}?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE purchase_orders SET status = 'Cancelled' WHERE po_id = @id",
                    new MySqlParameter("@id", selectedPoId));

                MessageBox.Show("Order cancelled.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
        }
    }
}