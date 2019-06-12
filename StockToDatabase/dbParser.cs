using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockToDatabase
{
    class DbParser
    {

        string connectionString;
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        string sql = null;
        int result = 0;

        public DbParser() {
            Console.WriteLine("Created instance of dbParser!");

            connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\gusta" +
                "\\source\\repos\\StockToDatabase\\StockToDatabase\\StockRecordDb.mdf; Integrated Security = True";
            connection =  new SqlConnection(connectionString);

        }

        public bool checkForDate(DateTime stockDate) {
            sql = "SELECT * FROM StockTable WHERE  RecordDate = '" + stockDate + "';";
            Console.WriteLine("EXECUTING: " + sql);
            bool result = true;
            try
            {
                connection.Open();

                command = new SqlCommand(sql, connection);
                reader = command.ExecuteReader();
                result = reader.HasRows ? true : false;
                Console.WriteLine(result);
                reader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! (WriteSummary)");
                Console.WriteLine("EXCEPTION: ");
                Console.WriteLine(ex);
            }
            return result;
        }

        public void clearDb() {
            Console.WriteLine("TODO Implement clear database code!");
            Console.WriteLine("TODO Print contents to console");
            sql = "DELETE FROM StockTable;"; ;
            try
            {
                connection.Open();

                command = new SqlCommand(sql, connection);
                result = command.ExecuteNonQuery();
                Console.WriteLine("Deleted " + result + " records");

                sql = "DBCC CHECKIDENT('stockTable', RESEED, 0)";
                command = new SqlCommand(sql, connection);
                result = command.ExecuteNonQuery();

                command.Dispose();
                connection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection (ClearDB)");
                Console.WriteLine("EXCEPTION: ");
                Console.WriteLine(ex);
            }
        }

        public int launchSqlCommand(String sql)
        {
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
                Console.WriteLine("EXCEPTION: ");
                Console.WriteLine(ex);
            }
            return 0;
        }


    public void writeSummareyToConsole() {
            Console.WriteLine("TODO Print contents to console");
            sql = "SELECT * FROM StockTable;";
            try
            {
                connection.Open();

                command = new SqlCommand(sql, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader.GetValue(0) + " - " + 
                        reader.GetValue(1) + " - " + reader.GetValue(2));
                }
                reader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! (WriteSummary)");
                Console.WriteLine("EXCEPTION: ");
                Console.WriteLine(ex);
            }
        }

        public void scanStocksToDb()
        {
            Console.WriteLine("TODO Implement code for scanning stocks!");
        }
    }
}
