using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
namespace PrivacyFinalProject.Helpers
{
    public static class DataBase
    {

        public static void CreateDatabase()
        {
            string dbFilePath = @"..\..\..\DB\PrivacyDB.sqlite";

            if (!File.Exists(dbFilePath))
            {
                SQLiteConnection.CreateFile(dbFilePath);

                using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    m_dbConnection.Open();
                    string dropTableQuery = "DROP TABLE IF EXISTS userinfo";
                    string sql = "CREATE TABLE userinfo (username VARCHAR(50), password VARCHAR(200))";
                    using (SQLiteCommand dropTableCommand = new SQLiteCommand(dropTableQuery, m_dbConnection))
                    using (SQLiteCommand createTableCommand = new SQLiteCommand(sql, m_dbConnection))
                    {
                        dropTableCommand.ExecuteNonQuery();
                        createTableCommand.ExecuteNonQuery();
                    }
                }
            }
        }


        public static void InsertData(string firstName, string lastName, string password)
        {
            string username = $"{firstName} {lastName}";

            string dbFilePath = @"..\..\..\DB\PrivacyDB.sqlite";

            if (File.Exists(dbFilePath))
            {
                using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                {
                    m_dbConnection.Open();
                    string sql = "INSERT INTO userinfo (username, password) VALUES (@username, @password)";
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                CreateDatabase();
            }
        }


        public static bool CheckPassword(string firstName, string lastName, string password)
        {
            string username = $"{firstName} {lastName}";

            string dbFilePath = @"..\..\..\DB\PrivacyDB.sqlite";

            if (File.Exists(dbFilePath))
            {
                try
                {
                    using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                    {
                        m_dbConnection.Open();
                        string sql = "SELECT password FROM userinfo WHERE username = @username";
                        using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                        {
                            command.Parameters.AddWithValue("@username", username);
                            using (SQLiteDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string storedPassword = reader["password"].ToString();
                                    return password == storedPassword;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    // Log or handle the error as needed
                    return false;
                }
            }
            else
            {
                CreateDatabase();
            }

            return false;
        }

        public static bool ResetPassword(string firstName, string lastName, string oldPassword, string newPassword)
        {
            string username = $"{firstName} {lastName}";

            string dbFilePath = @"..\..\..\DB\PrivacyDB.sqlite";

            if (File.Exists(dbFilePath))
            {
                try
                {
                    using (SQLiteConnection m_dbConnection = new SQLiteConnection($"Data Source={dbFilePath};Version=3;"))
                    {
                        m_dbConnection.Open();

                        // Check if the old password matches the one stored in the database
                        string sqlCheckPassword = "SELECT password FROM userinfo WHERE username = @username";
                        using (SQLiteCommand commandCheckPassword = new SQLiteCommand(sqlCheckPassword, m_dbConnection))
                        {
                            commandCheckPassword.Parameters.AddWithValue("@username", username);
                            object result = commandCheckPassword.ExecuteScalar();
                            if (result != null && result.ToString() == oldPassword)
                            {
                                // Old password matches, update to new password
                                string sqlUpdatePassword = "UPDATE userinfo SET password = @newPassword WHERE username = @username";
                                using (SQLiteCommand commandUpdatePassword = new SQLiteCommand(sqlUpdatePassword, m_dbConnection))
                                {
                                    commandUpdatePassword.Parameters.AddWithValue("@newPassword", newPassword);
                                    commandUpdatePassword.Parameters.AddWithValue("@username", username);
                                    int rowsAffected = commandUpdatePassword.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        // Password updated successfully
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    // Log or handle the error as needed
                    return false;
                }
            }
            else
            {
                CreateDatabase();
            }

            // If any step fails or old password doesn't match, return false
            return false;
        }


    }
}
