using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace StockToDatabase
{
    class FileScanner
    {
        List <string>  fileHeaderFormat = new List<string> { };

        public FileScanner()
        {
            Console.WriteLine("Created instance of FileScanner!");
        }

        public void scanFolderForValidFiles(String folder, DateTime startDate, DateTime endDate) {
            Console.WriteLine("In FS: scanFolderForValidFiles \n Getting list of files in folder:");
            String[] filePaths = Directory.GetFiles(folder);

            String fileDate;
            DateTime convertedDate;// = DateTime.Parse(fileDate);
            int totalNbrOfStocks = 0;

            //CALL METHOD FOR ANALYZING THE FILE
            

            foreach (string f in filePaths) {
                // Is this a csv file?
                if(Path.GetExtension(f).Equals(".csv"))
                {
                    fileDate = Path.GetFileNameWithoutExtension(f);
                    // Is the format yyyymmdd?     YYYY  M            M  D                     D
                    if (Regex.IsMatch(fileDate, @"^\d{4}(0?[1-9]|1[012])(0?[1-9]|[12][0-9]|3[01])$"))
                    {
                        // Convert the date to ISO 8601 format
                        fileDate = fileDate.Insert(4, "-").Insert(7, "-");

                        convertedDate = DateTime.Parse(fileDate);
                        if (startDate <= convertedDate && convertedDate <= endDate)
                        {
                            totalNbrOfStocks +=analyzeFile(f);
                        }
                    }
                }
            }
            Console.WriteLine("-----------------------------------------------");
            foreach(string f in fileHeaderFormat)
            {
                Console.WriteLine(f);
            }
            Console.WriteLine(totalNbrOfStocks);
            
        }

        /*
         * @ return number of stocks records
         */
        public int analyzeFile(String fileName) {
            int stockCounter = 0;
            
            String line;

            // Read the file and display it line by line.  
            TextReader tr = new StreamReader(fileName, Encoding.GetEncoding(1252), true);
            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace(", ", ","); 
                // ANALYZE ONLY FIRST THREE ROWS FOR NOW
                if (stockCounter < 4) {
                    // Save the header
                    if (stockCounter == 0) {
                        if (!fileHeaderFormat.Any(line.Contains)){
                            Console.WriteLine("New header for " + fileName + "\n " + line);
                            fileHeaderFormat.Add(line);
                        }
                    }
                }
                if (line.Length > 10) { stockCounter++; }
            }
            tr.Close();
            return stockCounter - 1; //First row doesn't count.
        }
    }
}
