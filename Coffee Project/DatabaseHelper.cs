using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Coffee_Project
{
    public class DatabaseHelper
    {
        private static string connectionString = GetConnectionString();

        private static string GetConnectionString()
        {
            try
            {
                // Try to get connection string from App.config
                string configConnectionString = ConfigurationManager.ConnectionStrings["CoffeeProjectConnection"]?.ConnectionString;
                if (!string.IsNullOrEmpty(configConnectionString))
                {
                    return configConnectionString;
                }
            }
            catch
            {
                // If reading from config fails, use default
            }
            
            // Default connection string - try common SQL Server configurations
            // Try localhost first, then localhost\SQLEXPRESS (common for SQL Server Express)
            return @"Data Source=localhost\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;";
        }

        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public static bool TestConnection()
        {
            // Try multiple connection string patterns
            string[] connectionStrings = new string[]
            {
                connectionString, // Try configured connection string first
                @"Data Source=localhost\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=localhost;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=.\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=(local)\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=.;Initial Catalog=CoffeeProjectDB;Integrated Security=True;"
            };

            foreach (string connStr in connectionStrings)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        // If successful, update the connection string
                        connectionString = connStr;
                        return true;
                    }
                }
                catch
                {
                    // Try next connection string
                    continue;
                }
            }

            // If all fail, show helpful error message
            MessageBox.Show(
                "Cannot connect to SQL Server. Please check:\n\n" +
                "1. SQL Server is running\n" +
                "2. Database 'CoffeeProjectDB' exists\n" +
                "3. Connection string in App.config is correct\n\n" +
                "Common connection strings:\n" +
                "- localhost\\SQLEXPRESS (SQL Server Express)\n" +
                "- localhost (Default SQL Server)\n" +
                "- .\\SQLEXPRESS (Local SQL Server Express)\n\n" +
                "Update App.config with the correct connection string.",
                "Connection Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return false;
        }

        public static bool RegisterUser(string username, string name, string password, string role)
        {
            // Try to establish connection with multiple connection strings
            SqlConnection conn = null;

            // List of connection strings to try
            string[] connectionStrings = new string[]
            {
                connectionString,
                @"Data Source=localhost\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=localhost;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=.\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=(local)\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=.;Initial Catalog=CoffeeProjectDB;Integrated Security=True;"
            };

            // Try each connection string
            foreach (string connStr in connectionStrings)
            {
                try
                {
                    conn = new SqlConnection(connStr);
                    conn.Open();
                    connectionString = connStr; // Update for future use
                    break;
                }
                catch
                {
                    if (conn != null)
                    {
                        conn.Dispose();
                        conn = null;
                    }
                    continue;
                }
            }

            if (conn == null || conn.State != ConnectionState.Open)
            {
                MessageBox.Show(
                    "Cannot connect to SQL Server. Please check:\n\n" +
                    "1. SQL Server is running (check Services)\n" +
                    "2. Database 'CoffeeProjectDB' exists (run CreateDatabase.sql)\n" +
                    "3. Update App.config with correct connection string\n\n" +
                    "Common connection strings:\n" +
                    "- Data Source=localhost\\SQLEXPRESS;... (SQL Server Express)\n" +
                    "- Data Source=localhost;... (Default SQL Server)\n" +
                    "- Data Source=.\\SQLEXPRESS;... (Local Express)",
                    "Database Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            try
            {
                using (conn)
                {
                    
                    
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int count = (int)checkCmd.ExecuteScalar();
                        
                        if (count > 0)
                        {
                            MessageBox.Show("Username already exists. Please choose a different username.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    string insertQuery = "INSERT INTO Users (Username, Name, Password, Role, CreatedDate) VALUES (@Username, @Name, @Password, @Role, @CreatedDate)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Password", password); 
                        cmd.Parameters.AddWithValue("@Role", role);
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        
                        int result = cmd.ExecuteNonQuery();
                        
                        if (result > 0)
                        {
                            MessageBox.Show("Registration successful! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return true;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2 || sqlEx.Number == 53 || sqlEx.Number == -1)
                {
                    MessageBox.Show(
                        "Cannot connect to SQL Server. Please check:\n\n" +
                        "1. SQL Server is running\n" +
                        "2. Server name/instance is correct\n" +
                        "3. Update App.config connection string\n\n" +
                        "Error: " + sqlEx.Message,
                        "Database Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else if (sqlEx.Number == 4060)
                {
                    MessageBox.Show(
                        "Database 'CoffeeProjectDB' does not exist.\n\n" +
                        "Please run the CreateDatabase.sql script in SQL Server Management Studio.",
                        "Database Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else if (sqlEx.Number == 208 || sqlEx.Message.Contains("Invalid object name"))
                {
                    MessageBox.Show(
                        "The 'Users' table does not exist in the database.\n\n" +
                        "Please run the CreateDatabase.sql script in SQL Server Management Studio to create the database and table.\n\n" +
                        "The script is located in your project folder:\n" +
                        "Coffee Project\\CreateDatabase.sql",
                        "Table Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Registration failed: " + sqlEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            return false;
        }

        public static bool LoginUser(string username, string password, out string role, out string name)
        {
            role = "";
            name = "";
            
           
            SqlConnection conn = null;
            string[] connectionStrings = new string[]
            {
                connectionString,
                @"Data Source=localhost\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=localhost;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=.\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=(local)\SQLEXPRESS;Initial Catalog=CoffeeProjectDB;Integrated Security=True;",
                @"Data Source=.;Initial Catalog=CoffeeProjectDB;Integrated Security=True;"
            };

           
            foreach (string connStr in connectionStrings)
            {
                try
                {
                    conn = new SqlConnection(connStr);
                    conn.Open();
                    connectionString = connStr; 
                    break;
                }
                catch
                {
                    if (conn != null)
                    {
                        conn.Dispose();
                        conn = null;
                    }
                    continue;
                }
            }

            if (conn == null || conn.State != ConnectionState.Open)
            {
                MessageBox.Show(
                    "Cannot connect to SQL Server. Please check:\n\n" +
                    "1. SQL Server is running\n" +
                    "2. Database 'CoffeeProjectDB' exists\n" +
                    "3. Update App.config with correct connection string",
                    "Database Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            try
            {
                using (conn)
                {
                    
                    string query = "SELECT Role, Name FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                role = reader["Role"].ToString();
                                name = reader["Name"].ToString();
                                return true;
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2 || sqlEx.Number == 53 || sqlEx.Number == -1)
                {
                    MessageBox.Show(
                        "Cannot connect to SQL Server. Please check your connection settings.",
                        "Database Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else if (sqlEx.Number == 4060)
                {
                    MessageBox.Show(
                        "Database 'CoffeeProjectDB' does not exist.\n\n" +
                        "Please run the CreateDatabase.sql script in SQL Server Management Studio.",
                        "Database Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else if (sqlEx.Number == 208 || sqlEx.Message.Contains("Invalid object name"))
                {
                    MessageBox.Show(
                        "The 'Users' table does not exist in the database.\n\n" +
                        "Please run the CreateDatabase.sql script in SQL Server Management Studio to create the database and table.\n\n" +
                        "The script is located in your project folder:\n" +
                        "Coffee Project\\CreateDatabase.sql",
                        "Table Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Login failed: " + sqlEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            return false;
        }
    }
}

