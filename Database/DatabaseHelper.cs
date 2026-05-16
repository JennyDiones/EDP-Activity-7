using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace BookstoreIS.Database
{
    public class DatabaseHelper
    {
        // ================== CONNECTION SETTINGS ==================
        private const string Server = "localhost";
        private const string Port = "3307";
        private const string DbName = "bookstoreis";
        private const string UserId = "root";
        private const string Password = "";

        private static string ConnectionString =>
            $"Server={Server};Port={Port};Database={DbName};Uid={UserId};Pwd={Password};" +
            "CharSet=utf8mb4;AllowUserVariables=True;";

        // ================== CONNECTION ==================
        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        // ================== CORE METHODS ==================
        public static DataTable ExecuteQuery(string sql, params MySqlParameter[] parms)
        {
            return GetDataTable(sql, parms);
        }

        public static object? ExecuteScalar(string sql, params MySqlParameter[] parms)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parms != null && parms.Length > 0)
                cmd.Parameters.AddRange(parms);
            return cmd.ExecuteScalar();
        }

        public static int ExecuteNonQuery(string sql, params MySqlParameter[] parms)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parms != null && parms.Length > 0)
                cmd.Parameters.AddRange(parms);
            return cmd.ExecuteNonQuery();
        }

        public static DataTable GetDataTable(string sql, params MySqlParameter[] parms)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(sql, conn);
            if (parms != null && parms.Length > 0)
                cmd.Parameters.AddRange(parms);

            using var adapter = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        // ================== PASSWORD HASHING ==================
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }

        // ================== LOGIN ==================
        public static bool ValidateLogin(string username, string password,
            out string role, out int userId, out string fullName)
        {
            role = fullName = "";
            userId = 0;

            string hashed = HashPassword(password);
            string sql = @"SELECT user_id, role, full_name FROM users 
                           WHERE username=@u AND password=@p AND is_active=1";

            try
            {
                using var conn = GetConnection();
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", hashed);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userId = Convert.ToInt32(reader["user_id"]);
                    role = reader["role"]?.ToString() ?? "";
                    fullName = reader["full_name"]?.ToString() ?? "";
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Error: " + ex.Message);
            }
            return false;
        }

        // ================== USER MANAGEMENT ==================
        public static bool AddUser(string username, string password, string fullName,
            string email, string role, bool isActive = true)
        {
            string sql = @"INSERT INTO users (username, password, full_name, email, role, is_active, created_at) 
                           VALUES (@u, @p, @fn, @e, @r, @a, NOW())";

            try
            {
                ExecuteNonQuery(sql,
                    new MySqlParameter("@u", username),
                    new MySqlParameter("@p", HashPassword(password)),
                    new MySqlParameter("@fn", fullName),
                    new MySqlParameter("@e", email ?? ""),
                    new MySqlParameter("@r", role),
                    new MySqlParameter("@a", isActive));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("AddUser Error: " + ex.Message);
                return false;
            }
        }

        public static bool UpdateUser(int userId, string fullName, string email,
            string role, bool isActive, string? newPassword = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    string sql = @"UPDATE users SET full_name=@fn, email=@e, role=@r, 
                                   is_active=@a, password=@p WHERE user_id=@id";
                    ExecuteNonQuery(sql,
                        new MySqlParameter("@fn", fullName),
                        new MySqlParameter("@e", email ?? ""),
                        new MySqlParameter("@r", role),
                        new MySqlParameter("@a", isActive),
                        new MySqlParameter("@p", HashPassword(newPassword)),
                        new MySqlParameter("@id", userId));
                }
                else
                {
                    string sql = @"UPDATE users SET full_name=@fn, email=@e, role=@r, 
                                   is_active=@a WHERE user_id=@id";
                    ExecuteNonQuery(sql,
                        new MySqlParameter("@fn", fullName),
                        new MySqlParameter("@e", email ?? ""),
                        new MySqlParameter("@r", role),
                        new MySqlParameter("@a", isActive),
                        new MySqlParameter("@id", userId));
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("UpdateUser Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable GetAllUsers(string search = "")
        {
            string sql = @"SELECT user_id AS ID, username AS Username, full_name AS 'Full Name',
                                 email AS Email, role AS Role, is_active AS Active 
                           FROM users";

            if (!string.IsNullOrWhiteSpace(search))
                sql += " WHERE username LIKE @s OR full_name LIKE @s OR email LIKE @s";

            return GetDataTable(sql, new MySqlParameter("@s", $"%{search}%"));
        }
    }
}