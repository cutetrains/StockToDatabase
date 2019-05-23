using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace StockToDatabase
{
    public partial class Form1 : Form
    {

        DbParser dbParser = new DbParser();
        FileScanner fileScanner = new FileScanner();
        private FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

        // Bad design, the inputPath and the text in the folderButton are redundant.
        private String inputPath = @"C:\Users\gusta\Dropbox\Ekonomi\results";
                
        public Form1()
        {
            InitializeComponent();
            Console.WriteLine(inputPath);
            if (!Directory.Exists(inputPath))
            {
                inputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                Console.WriteLine(inputPath);
            }
            folderButton.Text = inputPath;
            /*DISABLING TEMPORARILY TO REDUCE DATES UNDER DEVELOPMENT
            DateTimePicker toDatePicker = new DateTimePicker();
            toDateTimePicker.Value = DateTime.Now;*/
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Click on the link below to continue learning how to build a desktop app using WinForms!
            System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

        }

        /*
         TODO: DELETE CODE BELOW!
            */
        private void button1_Click(object sender, EventArgs e)
        {
            
            string connetionString = null;
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader reader;
            string sql = null;
            
            connetionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = C:\\Users\\gusta"+
                "\\source\\repos\\StockToDatabase\\StockToDatabase\\StockRecordDb.mdf; Integrated Security = True";
            Console.WriteLine(connetionString);
            sql = "INSERT INTO StockTable(RecordDate, StockName, Price, EarningPerShare, PricePerEarning, CapitalPerShare," + 
                  " PricePerCapital, Dividend, Yield, ProfitMargin, ROI)"+ 
                  " VALUES('1970-01-01', 'GodFather', 10.0, 7, 8, 9, 10, 11, 12, 13, 14);";
            connection = new SqlConnection(connetionString);

            Console.WriteLine("Try to connect to database...");

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                
                sql = "SELECT * FROM StockTable;";
                command = new SqlCommand(sql, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show(reader.GetValue(0) + " - " + reader.GetValue(1) + " - " + reader.GetValue(2));
                }
                reader.Close();
                command.Dispose();
                
                MessageBox.Show(" ExecuteNonQuery in SqlCommand executed !!");
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
                Console.WriteLine("EXCEPTION: ");
                Console.WriteLine(ex);
            }

            Console.WriteLine("All done. Press any key to finish...");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Check  database info");
            dbParser.writeSummareyToConsole();
            dbParser.clearDb();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Scan Stocks in folder:" + inputPath);
            Console.WriteLine(fromDateTimePicker.Text);
            Console.WriteLine(toDateTimePicker.Text);
            fileScanner.scanFolderForValidFiles(inputPath, fromDateTimePicker.Value, toDateTimePicker.Value);
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                inputPath = folderBrowserDialog1.SelectedPath;
                folderButton.Text = inputPath;
            }
        }
    }
}
