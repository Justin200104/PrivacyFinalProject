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
            if (!File.Exists(@"..\..\..\DB\PrivacyDB.sqlite"))
            {
                SQLiteConnection.CreateFile(@"..\..\..\DB\PrivacyDB.sqlite");
                SQLiteConnection m_dbConnection;
                m_dbConnection = new SQLiteConnection("Data Source=PrivacyDB.sqlite;Version=3;");
                m_dbConnection.Open();
                string dropTableQuery = "DROP TABLE IF EXISTS userinfo";
                string sql = "create table userinfo (username varchar(20), password varchar(200))";
                SQLiteCommand dropTableCommand = new SQLiteCommand(dropTableQuery, m_dbConnection);
                SQLiteCommand createTableCommand = new SQLiteCommand(sql, m_dbConnection);

                dropTableCommand.ExecuteNonQuery();
                createTableCommand.ExecuteNonQuery();

                m_dbConnection.Close();
            }

        }

        public static void InsertData(string username, string password)
        {
            if (File.Exists(@"..\..\..\DB\PrivacyDB.sqlite"))
            {
                SQLiteConnection m_dbConnection;
                m_dbConnection = new SQLiteConnection("Data Source=PrivacyDB.sqlite;Version=3;");
                m_dbConnection.Open();
                string sql = $"insert into userinfo (username, password) values ('{username}', '{password}')";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                m_dbConnection.Close();
            }
        }

        public static bool CheckPassword(string username, string password)
        {
            if (File.Exists(@"..\..\..\DB\PrivacyDB.sqlite"))
            {
                SQLiteConnection m_dbConnection;
                m_dbConnection = new SQLiteConnection("Data Source=PrivacyDB.sqlite;Version=3;");
                m_dbConnection.Open();
                string sql = $"select password from userinfo where username = '{username}'";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string storedPassword = reader["password"].ToString();
                    m_dbConnection.Close();
                    return password == storedPassword;
                }

                m_dbConnection.Close();
            }
            return false;

        }
    }
}
