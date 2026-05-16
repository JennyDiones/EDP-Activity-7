using System;
using System.Drawing;
using System.Windows.Forms;

namespace BookstoreIS.Forms
{
    public class AboutForm : Form
    {
        public AboutForm()
        {
            Text            = "DashboardForm";
            Size            = new Size(500, 420);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            Font            = new Font("Segoe UI", 9f);
            BackColor       = Color.White;

            // ── Header banner ────────────────────────────────────────────────
            var pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 90,
                BackColor = Color.FromArgb(0, 50, 120)
            };
            pnlHeader.Controls.Add(new Label
            {
                Text      = "Bookstore Information System",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = false,
                Size      = new Size(460, 40),
                Location  = new Point(20, 10),
                TextAlign = ContentAlignment.MiddleCenter
            });
            pnlHeader.Controls.Add(new Label
            {
                Text      = "Information System - Version 1.0.0",
                ForeColor = Color.FromArgb(180, 200, 255),
                AutoSize  = false,
                Size      = new Size(460, 22),
                Location  = new Point(20, 50),
                TextAlign = ContentAlignment.MiddleCenter
            });
            pnlHeader.Controls.Add(new Label
            {
                Text      = "Developed for Academic Purposes",
                ForeColor = Color.FromArgb(160, 180, 220),
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                AutoSize  = false,
                Size      = new Size(460, 20),
                Location  = new Point(20, 68),
                TextAlign = ContentAlignment.MiddleCenter
            });

            // ── Description ──────────────────────────────────────────────────
            var pnlDesc = new Panel
            {
                Location  = new Point(20, 105),
                Size      = new Size(445, 50),
                BackColor = Color.FromArgb(245, 245, 250),
                Padding   = new Padding(10)
            };
            pnlDesc.Controls.Add(new Label
            {
                Text     = "This Information System was developed as an activity using C# .NET Windows Form with Microsoft Visual Studio and that's all.",
                AutoSize = false,
                Size     = new Size(425, 40),
                Location = new Point(10, 8)
            });

            // ── Info rows ────────────────────────────────────────────────────
            var info = new[]
            {
                ("Application:", "Bookstore Information System"),
                ("Version:",     "1.0.0 (Build 2026)"),
                ("Platform:",    ".NET Framework 4.8 / Windows Form"),
                ("Developer:",   "Jenny Diones"),
                ("Course:",      "Event Driven Programming"),
                ("Technologies:","C#, MS Excel Interop, MySQL"),
                ("Copyright:",   "(C) 2024 All Rights Reserved")
            };

            int y = 165;
            foreach (var (label, value) in info)
            {
                Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = Color.FromArgb(0,50,120), Location = new Point(30, y), AutoSize = true });
                Controls.Add(new Label { Text = value, Location = new Point(160, y), AutoSize = true });
                y += 24;
            }

            // ── OK button ────────────────────────────────────────────────────
            var btnOk = new Button
            {
                Text      = "OK",
                Size      = new Size(80, 32),
                Location  = new Point(195, 350),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                DialogResult = DialogResult.OK,
                Cursor    = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;

            Controls.Add(pnlHeader);
            Controls.Add(pnlDesc);
            Controls.Add(btnOk);
            AcceptButton = btnOk;
        }
    }
}
