using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Lumia_EmoTunes
{
    internal class DatabaseHelper
    {
        private const string DbName = "emotunes_data.db";

        public static void InitializeDatabase()
        {
            // This creates the .db file in your debug folder if it doesn't exist
            if (!File.Exists(DbName))
            {
                File.Create(DbName).Close();
            }

            using (var connection = new SqliteConnection($"Data Source={DbName}"))
            {
                connection.Open();

                // Create a table to store your session data
                // We store the 'Emotion' as text and 'Details' as a JSON string
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS UserSessions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                    DetectedEmotion TEXT,
                    JsonData TEXT
                );";

                tableCommand.ExecuteNonQuery();
            }
        }

        public static string GetConnectionString()
        {
            return $"Data Source={DbName}";
        }
    }
}
