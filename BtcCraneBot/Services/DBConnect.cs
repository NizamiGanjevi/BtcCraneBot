using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;
using System.Windows.Forms;

namespace BtcCraneBot
{
    class DBConnect
    {        
        public static string dbName = @"\BtcCraneBotUsers.db3";
        public static string dbSource = Application.StartupPath.ToString() + dbName;
        public static bool dbIsCreate = false;
        static string sourceString = "Data Source = " + dbSource;
        static SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
        static SQLiteCommand command = new SQLiteCommand((SQLiteConnection)Connection());
        

        static object Connection()
        {
            SQLiteConnection sQLiteConnection = (SQLiteConnection)factory.CreateConnection();
            sQLiteConnection.ConnectionString = sourceString;
            sQLiteConnection.Open();
            return sQLiteConnection;
        }        

        public static void DataBaseCreate()
        {           
            if(File.Exists(dbSource) != true)
            {
                SQLiteConnection.CreateFile(dbName);
                dbIsCreate = true;
            }
            else
            {
                dbIsCreate = true;
            }
            
        }
        
        public static void ExecuteNonQuery(string sqlCommand)
        {
            try
            {                
                command.CommandType = CommandType.Text;
                command.CommandText = sqlCommand;
                command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static object ExecuteScalar(string sqlCommand)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sqlCommand;
                object user_id = command.ExecuteScalarAsync().Result;
                return user_id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }
    }
}
