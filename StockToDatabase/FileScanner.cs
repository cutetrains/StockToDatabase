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
        List <string> stockNames = new List<string> { };
        DbParser db = new DbParser();
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
                            Counters.fileCounter++;
                            if (db.checkForDate(Convert.ToDateTime(fileDate))){
                                Console.WriteLine(fileDate + " already has a record!");
                            } else {
                                totalNbrOfStocks += analyzeFile(f, fileDate);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("-----------------------------------------------");
            foreach(string f in fileHeaderFormat)
            {
                Console.WriteLine(f);
            }
            Console.WriteLine("Total number of analyzed files: " + Counters.fileCounter);
            Console.WriteLine("Total number of analyzed stocks: " + Counters.stockRecordCounter);
            Console.WriteLine("Unknown records: " + Counters.unknownCounter);
            Console.WriteLine("Type 1:  " + Counters.type1Counter);
            Console.WriteLine("Type 2:  " + Counters.type2Counter);
            Console.WriteLine("Type 3:  " + Counters.type3Counter);
        }

        /*
         * @ return number of stocks records
         */
        public int analyzeFile(String fileName, String fileDate) {
            int stockCounter = 0;
            int response = 0;
            String line;
            String fileHeader = "";

            // Read the file and display it line by line.  
            TextReader tr = new StreamReader(fileName, Encoding.GetEncoding(1252), true);
            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace(", ", ","); 
                // ANALYZE ONLY FIRST THREE ROWS FOR NOW
                if (stockCounter < 4) {
                    // Save the header
                    if (stockCounter == 0)
                    {
                        if (!fileHeaderFormat.Any(line.Contains))
                        {
                            Console.WriteLine("New header for " + fileName + "\n " + line);
                            fileHeaderFormat.Add(line);
                        }
                        fileHeader = line;
                    }
                    else {
                        parseRecordToDatabase(line, fileHeader, fileDate);
                    }
                }
                if (line.Length > 10) { stockCounter++; }//Ignore empty lines
            }
            tr.Close();
            return stockCounter - 1; //First row doesn't count.
        }

        public Nullable<float> string2Float(String s)
        {
            Nullable<float> value = null;
            try //TODO: Make this a separate function with index and expected variable
            {
                value = Convert.ToSingle(s.Replace(".", ","));
            }
            catch (FormatException)
            {
                Console.WriteLine($"Unable to parse '{s}'");
            }
            return value;
        }

        public int parseRecordToDatabase(String record, String fileHeader, String fileDate)
        {

            Boolean identified = false ;
            Boolean errorFound = false;
            Console.WriteLine(fileHeader + "\nAnalyzing line: \n " + record + "   " + fileDate);
            string sql = null;
            List<string> recordElements;
            float epsilon = 0.1F; // Error tolearance 
            // DB means that this will go into the database
            string name = "";            //                     DB
            Nullable<float> P = null;    // Price per share     DB
            Nullable<float> E = null;    // Earning per share  
            Nullable<float> PE = null;   // Price per earning P/E  DB
            Nullable<float> C = null;    // Capital per share        
            Nullable<float> PC = null;   // Price per capital P/JEK  DB
            Nullable<float> D = null;    // Dividend 
            Nullable<float> DP = null;   // Dividend per price (Direktavkastning)  DB
            Nullable<float> Pm = null;   // Profit Margin       DB
            Nullable<float> RSI = null;    //                     DB
            String nextReport = "";      //                     DB 
            String nextDividend = "";    //                     DB
            Nullable<int> TA = null;     //                     DB
            Counters.stockRecordCounter++;
            //For simplicity, use a switch case for different headers.
            switch (fileHeader) {
                /*      0    1    2    3              4    5                 6         7                 8               */
                case "Namn;Namn;Kurs;Vinst per aktie;P E;Kapital per aktie;P JEK;Direktavkastning;Utdelning per aktie;" +
                "Vinstmarginal;RSI;Nasta rapport;Utdelningsdatum;TA":
                    /*     9       10      11           12           13  */
                    Console.WriteLine("-----------\n" + fileHeader + "\n???????\nNamn;Namn;Kurs;Vinst per aktie;P E;Kapital per aktie;P JEK;Direktavkastning;Utdelning per aktie;" +
                "Vinstmarginal;RSI;Nasta rapport;Utdelningsdatum;TA\n----------");
                    Counters.type1Counter++;
                    Console.WriteLine("Found in case");
                    recordElements = record.Split(';').ToList<string>();
                    name = recordElements[1];
                    Console.WriteLine(name);
                    if (!recordElements[0].Equals(recordElements[1])) {
                        Console.WriteLine("Name mismatch for " + name );
                        errorFound = true;
                    }
                    P = string2Float(recordElements[2]);
                    E = string2Float(recordElements[3]);
                    PE = string2Float(recordElements[4]);
                    Console.WriteLine($"Checking results: '{P}'/'{E}'='{PE}', is that '{P/E}'?");
                    if (Math.Abs(Convert.ToDouble(P/E)) < epsilon) {
                        Console.WriteLine("Error, mismatch! ");
                    }
                    C = string2Float(recordElements[5]);
                    PC = string2Float(recordElements[6]);
                    Console.WriteLine($"Checking results: '{P}'/'{C}'='{PC}', is that '{P / C}'?");
                    DP = string2Float(recordElements[7]);
                    D = string2Float(recordElements[8]);
                    //TODO ELABORATE ABOUT PERCENT V S FRACTION?
                    Console.WriteLine($"Checking results: '{D}'/'{P}'='{DP}', is that '{D / P}'?");
                    Pm = string2Float(recordElements[9]);
                    int i = 0;
                    Console.WriteLine("---" + recordElements[10] + "---");
                    if (recordElements[10] == "tbd")
                    {
                        RSI = -1;// -1 indicates that this value is null
                    }
                    else {
                        RSI = string2Float(recordElements[10]);
                    }
                    nextReport = recordElements[11];
                    nextDividend = recordElements[12];
                    //TODO CHECK LENGTH OF recordEleents before doing this
                    if (recordElements.Count == 14)
                    {
                        if (Int32.TryParse(recordElements[10], out i))
                        {
                            TA = i;
                        }
                    }
                    else
                    {
                        Console.WriteLine("TA not detected");
                    }

                    sql = "INSERT INTO StockTable(RecordDate, StockName, Price, PricePerEarning," +
                  " PricePerCapital, Yield, ProfitMargin, RSI, DividendDate, ReportDate) VALUES('" +
                    fileDate + "', '" + name + "', " + P.ToString().Replace(",",".") + ", " + 
                    PE.ToString().Replace(",", ".") + ", " + PC.ToString().Replace(",", ".") + ", " + 
                    DP.ToString().Replace(",", ".") + ", " + Pm.ToString().Replace(",", ".") + ", " + 
                    RSI.ToString().Replace(",", ".") + ", '" + nextDividend + "', '" + nextReport + "');";
                    Console.WriteLine("INSERT INTO StockTable(RecordDate, StockName, Price, PricePerEarning," +
                  " PricePerCapital, Yield, ProfitMargin, RSI, DividendDate, ReportDate) VALUES('1970-01-01', 'GodFather', 10.0, 7, 8, 9, 10, 11, 12, 13, 14);");
                    Console.WriteLine(sql);
                    db.launchSqlCommand(sql);
                    db.writeSummareyToConsole();
                    identified = true;//DELETE
                    //PE = recordElements[3];
                    break;
                //case "Namn,Namn,Kurs,P / E,P / S,P / JEK,Direktavkastning,Vinstmarginal,Marknadsvärde,Utdelningsdatum,Senaste årsbokslut, " +
                //"Utveckling sedan årsskiftet, Vinst per aktie, P/ E - tal,Kapital per aktie,Betalkurs / JEK,Direktavkastning,Vinstmarginal,RSI,TA":
                case "Namn,Namn,Kurs,P / E,P / S,P / JEK,Direktavkastning,Vinstmarginal,MarknadsvÃ¤rde,Utdelningsdatum,Senaste Ã¥rsbokslut," +
                "Utveckling sedan Ã¥rsskiftet,Vinst per aktie,P / E - tal,Kapital per aktie,Betalkurs / JEK,Direktavkastning,Vinstmarginal,RSI,TA":

                    Counters.type2Counter++;
                    Console.WriteLine("Type 2 found!");
                    identified = true;
                    break;
                case "Namn,Namn,Kurs,P / E,Kursrekord,Marknadsvärde,Förändring under året,Vinst per aktie,P / E - tal,Kapital per aktie,Betalkurs / JEK," +
                "Direktavkastning,Vinstmarginal,RSI,TA":
                    Counters.type3Counter++;
                    Console.WriteLine("Type 3 found!");
                    identified = true;
                    break;
                default:
                    Console.WriteLine("Recordheader not identified " + fileDate );
                    Counters.unknownCounter++;
                    break;
            }
            if (identified == true) {
                if (errorFound == true)
                {
                    return -1;
                }
                else {
                    return 1;
                }
            }
            else {
                return 0;
            }
        }

    }

}
