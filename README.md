# Bookstore Information System
**Developer:** Jenny A. Diones | **Course:** Event Driven Programming | **Date:** 04-19-26

---

## Prerequisites

| Tool | Version |
|------|---------|
| Visual Studio | 2022 (Community or higher) |
| .NET SDK | 6.0+ |
| MySQL Server | 8.0+ |
| MySQL Workbench | (optional, for DB management) |

---

## Step 1 – Set up the Database

1. Open **MySQL Workbench** (or any MySQL client).
2. Run **`Diones_Activity2.sql`** – this creates the `bookstore` schema, tables, views, function, and stored procedure.
3. Run **`seed_data.sql`** – this inserts 10 publishers, 10 categories, 10 authors, and 10 books so the app has data to display.

---

## Step 2 – Configure the Connection String

Open `Database/DatabaseHelper.cs` and update these constants to match your MySQL setup:

```csharp
private const string Server   = "localhost";   // your MySQL host
private const string Port     = "3306";
private const string DbName   = "bookstore";
private const string UserId   = "root";
private const string Password = "";            // ← your MySQL root password
```

---

## Step 3 – Open in Visual Studio

1. Open Visual Studio 2022.
2. **File → Open → Project/Solution** → select `BookstoreIS.csproj`.
3. Visual Studio will restore the **MySql.Data** NuGet package automatically (requires internet the first time).
4. Press **F5** (or **Ctrl+F5**) to run.

---

## Login Credentials

| Username | Password | Note |
|----------|----------|------|
| `admin`  | `admin123` | Built-in demo account |

---

## Application Features

### Forms / Panels

| Screen | Description |
|--------|-------------|
| **Login Form** | Username + password, Remember Me, Forgot Password link |
| **Password Recovery** | Email-based reset (demo mode) |
| **Dashboard** | Live stats (total books, authors, categories, low-stock count) + recent records grid |
| **Records** | Browse/search/add/edit/delete Books, Authors, Publishers, Categories (4 tabs) |
| **Report Generator** | 5 report types, date range filter, column selector, CSV export, print |
| **Data Entry** | Add books (with author link), authors, publishers, categories, and update stock |
| **Settings** | Test the DB connection |
| **About** | Application info panel |

### Database Objects Used

| Object | Usage |
|--------|-------|
| `vw_book_details` | Full book info view |
| `vw_books_by_category_count` | Category summary view |
| `vw_low_stock_alert` | Low stock report |
| `get_author_full_name()` | Author name function |
| `sp_AddStockToBook` | Stock update stored procedure (Data Entry → Update Stock tab) |

---

## Troubleshooting

**"Unable to connect to any of the specified MySQL hosts"**
- Make sure MySQL Server is running (check Windows Services or XAMPP).
- Verify the password in `DatabaseHelper.cs`.

**NuGet restore failed**
- Go to **Tools → NuGet Package Manager → Manage NuGet Packages for Solution** and install `MySql.Data` version 8.3.0.

**App runs but shows sample data instead of real data**
- The app gracefully falls back to dummy data when the DB is unreachable, so you can still see the UI. Fix the connection string to get live data.
