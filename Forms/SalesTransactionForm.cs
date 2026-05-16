using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using BookstoreIS.Database;
using MySql.Data.MySqlClient;

namespace BookstoreIS.Forms
{
    public partial class SalesTransactionForm : Form
    {
        private DataGridView dgvBooks = null!;
        private DataGridView dgvCart = null!;
        private TextBox txtSearch = null!;
        private TextBox txtQuantity = null!;
        private Label lblTotal = null!;
        private Button btnAddToCart = null!;
        private Button btnRemove = null!;
        private Button btnCompleteSale = null!;
        private Button btnClearCart = null!;

        private decimal totalAmount = 0;
        private readonly int currentUserId;
        private readonly List<CartItem> cart = new();

        public SalesTransactionForm(int loggedInUserId)
        {
            currentUserId = loggedInUserId;
            InitializeComponent();
            LoadAvailableBooks();
        }

        private void InitializeComponent()
        {
            this.Text = "Sales Transaction - Point of Sale";
            this.Size = new Size(1150, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);

            // ── Left: Available Books ─────────────────────────────────────────
            var lblBooks = new Label
            {
                Text = "Available Books",
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                AutoSize = true
            };

            txtSearch = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(400, 32),
                PlaceholderText = "Search by title or author…"
            };

            dgvBooks = new DataGridView
            {
                Location = new Point(20, 90),
                Size = new Size(500, 520),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };

            // ── Right: Shopping Cart ──────────────────────────────────────────
            var lblCart = new Label
            {
                Text = "Shopping Cart",
                Location = new Point(550, 20),
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                AutoSize = true
            };

            dgvCart = new DataGridView
            {
                Location = new Point(550, 90),
                Size = new Size(560, 400),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            txtQuantity = new TextBox
            {
                Text = "1",
                Location = new Point(550, 510),
                Size = new Size(80, 32)
            };

            btnAddToCart = new Button
            {
                Text = "Add to Cart",
                Location = new Point(640, 508),
                Size = new Size(130, 38),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            btnRemove = new Button
            {
                Text = "Remove Item",
                Location = new Point(780, 508),
                Size = new Size(130, 38),
                BackColor = Color.OrangeRed,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            lblTotal = new Label
            {
                Text = "Total: ₱ 0.00",
                Location = new Point(550, 560),
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                AutoSize = true
            };

            btnClearCart = new Button
            {
                Text = "Clear Cart",
                Location = new Point(550, 610),
                Size = new Size(140, 55),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            btnCompleteSale = new Button
            {
                Text = "COMPLETE SALE",
                Location = new Point(720, 610),
                Size = new Size(180, 55),
                BackColor = Color.Blue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            Controls.AddRange(new Control[]
            {
                lblBooks, txtSearch, dgvBooks,
                lblCart, dgvCart,
                txtQuantity, btnAddToCart, btnRemove,
                lblTotal, btnClearCart, btnCompleteSale
            });

            txtSearch.TextChanged += (s, e) => LoadAvailableBooks(txtSearch.Text.Trim());
            btnAddToCart.Click += BtnAddToCart_Click;
            btnRemove.Click += BtnRemove_Click;
            btnCompleteSale.Click += BtnCompleteSale_Click;
            btnClearCart.Click += (s, e) => ClearCart();
        }

        private void LoadAvailableBooks(string search = "")
        {
            try
            {
                string sql = @"SELECT book_id AS ID, title AS Title, author AS Author,
                                      price AS Price, stock_quantity AS Stock
                               FROM books
                               WHERE stock_quantity > 0";

                dgvBooks.DataSource = string.IsNullOrWhiteSpace(search)
                    ? DatabaseHelper.GetDataTable(sql)
                    : DatabaseHelper.GetDataTable(
                        sql + " AND (title LIKE @s OR author LIKE @s)",
                        new MySqlParameter("@s", $"%{search}%"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading books: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddToCart_Click(object? sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvBooks.SelectedRows[0];
            int bookId = Convert.ToInt32(row.Cells["ID"].Value);
            string title = row.Cells["Title"].Value?.ToString() ?? "";
            decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
            int stock = Convert.ToInt32(row.Cells["Stock"].Value);

            if (!int.TryParse(txtQuantity.Text.Trim(), out int qty) || qty <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check total qty already in cart for this book
            int alreadyInCart = 0;
            foreach (var c in cart)
                if (c.BookId == bookId) alreadyInCart += c.Quantity;

            if (qty + alreadyInCart > stock)
            {
                MessageBox.Show($"Not enough stock! Available: {stock - alreadyInCart}", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If book already in cart, just increase quantity
            var existing = cart.Find(c => c.BookId == bookId);
            if (existing != null)
                existing.Quantity += qty;
            else
                cart.Add(new CartItem { BookId = bookId, Title = title, Price = price, Quantity = qty });

            RefreshCart();
        }

        // ── FIX: use cart index stored in the hidden Book ID column ──────────
        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            if (dgvCart.CurrentRow == null || dgvCart.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select an item in the cart to remove.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // The cart list and the DataTable rows are in the same order.
            // Use CurrentRow.Index (not SelectedRows[0].Index) — more reliable.
            int rowIndex = dgvCart.CurrentRow.Index;

            if (rowIndex >= 0 && rowIndex < cart.Count)
            {
                string removed = cart[rowIndex].Title;
                cart.RemoveAt(rowIndex);
                RefreshCart();
                MessageBox.Show($"\"{removed}\" removed from cart.", "Removed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshCart()
        {
            var dt = new DataTable();
            dt.Columns.Add("Book ID", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("Subtotal", typeof(decimal));

            totalAmount = 0;
            foreach (var item in cart)
            {
                decimal subtotal = item.Price * item.Quantity;
                dt.Rows.Add(item.BookId, item.Title, item.Price, item.Quantity, subtotal);
                totalAmount += subtotal;
            }

            dgvCart.DataSource = dt;
            lblTotal.Text = $"Total: ₱ {totalAmount:N2}";
        }

        private void ClearCart()
        {
            cart.Clear();
            RefreshCart();
        }

        private void BtnCompleteSale_Click(object? sender, EventArgs e)
        {
            if (cart.Count == 0)
            {
                MessageBox.Show("Cart is empty!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Complete sale for ₱{totalAmount:N2}?",
                    "Confirm Sale", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                // 1. Insert into sales
                int saleId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(
                    @"INSERT INTO sales (user_id, total_amount, sale_date)
                      VALUES (@uid, @total, NOW());
                      SELECT LAST_INSERT_ID();",
                    new MySqlParameter("@uid", currentUserId),
                    new MySqlParameter("@total", totalAmount)));

                foreach (var item in cart)
                {
                    // 2. Insert sale items
                    DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO sale_items (sale_id, book_id, quantity, unit_price, subtotal)
                          VALUES (@sid, @bid, @qty, @price, @sub)",
                        new MySqlParameter("@sid", saleId),
                        new MySqlParameter("@bid", item.BookId),
                        new MySqlParameter("@qty", item.Quantity),
                        new MySqlParameter("@price", item.Price),
                        new MySqlParameter("@sub", item.Price * item.Quantity));

                    // 3. Deduct stock
                    DatabaseHelper.ExecuteNonQuery(
                        "UPDATE books SET stock_quantity = stock_quantity - @qty WHERE book_id = @bid",
                        new MySqlParameter("@qty", item.Quantity),
                        new MySqlParameter("@bid", item.BookId));
                }

                // 4. Record in purchase_orders for history tracking
                int poId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(
                    @"INSERT INTO purchase_orders (supplier_id, user_id, total_cost, status, order_date)
                      VALUES (NULL, @uid, @total, 'Sold', NOW());
                      SELECT LAST_INSERT_ID();",
                    new MySqlParameter("@uid", currentUserId),
                    new MySqlParameter("@total", totalAmount)));

                foreach (var item in cart)
                {
                    DatabaseHelper.ExecuteNonQuery(
                        @"INSERT INTO po_items (po_id, book_id, quantity, unit_cost, subtotal)
                          VALUES (@po, @bid, @qty, @price, @sub)",
                        new MySqlParameter("@po", poId),
                        new MySqlParameter("@bid", item.BookId),
                        new MySqlParameter("@qty", item.Quantity),
                        new MySqlParameter("@price", item.Price),
                        new MySqlParameter("@sub", item.Price * item.Quantity));
                }

                MessageBox.Show(
                    $"Sale completed!\nTotal: ₱{totalAmount:N2}\nSale ID: #{saleId}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearCart();
                LoadAvailableBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error completing sale:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}