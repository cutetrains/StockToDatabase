using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace StockToDatabase
{
    
    class FileScanner
    {
        List <string>  fileHeaderFormat = new List<string> { };
        List <string> stockNames = new List<string> { };
        DbParser db = new DbParser();
        NameChecker nc = new NameChecker();

        Form1 form1;
        
        public FileScanner()
        {
            Console.WriteLine("Created instance of FileScanner!");
            //nc.showStatus();
            //nc.findNode("ABB");
            nc.importDictionaries();
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
            Console.WriteLine("\n-----------------------------------------------");
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
            Console.WriteLine("Errors detected:  " + Counters.errorCounter);
            nc.showUnknownCombinations();
        }
        
        /*
         * @ return number of stocks records
         */
        public int analyzeFile(String fileName, String fileDate) {
            int stockCounter = 0;
            int response = 0;
            String line;
            String fileHeader = "";
            int numOfLines = 500;

            // Read the file and display it line by line.  
            TextReader tr = new StreamReader(fileName, Encoding.GetEncoding(1252), true);
            Console.WriteLine("\n" + fileDate);
            while ((line = tr.ReadLine()) != null)
            {
                line = line.Replace(", ", ","); 
                // ANALYZE ONLY FIRST THREE ROWS FOR NOW
                if (stockCounter < numOfLines ) {
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
            if (s != "") {
                try //TODO: Make this a separate function with index and expected variable
                {
                    value = Convert.ToSingle(s.Replace(".", ","));
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Unable to parse '{s}'");
                }
            }
            return value;
        }

        public int parseRecordToDatabase(String record, String fileHeader, String fileDate)
        {
            string[] formats = {"yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-M-d"};
            DateTime dateValue;
            DateTime fileDateValue;
            Boolean identified = false ;
            Boolean errorFound = false;
            Boolean nameStatus = true;
            //Console.WriteLine(fileHeader + "\nAnalyzing line: \n " + record + "   " + fileDate);
            string sql = null;
            List<string> recordElements;
            float epsilon = 0.1F; // Error tolearance 
            // DB means that this will go into the database
            string expectedName = "";
            string name = "";            //                     DB
            Nullable<float> P = null;    // Price per share     DB
            Nullable<bool> priceMissingInRecord = null;
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
            DateTime.TryParseExact(fileDate, formats,
                                  new CultureInfo("en-US"),
                                  DateTimeStyles.None,
                                  out fileDateValue);
            //For simplicity, use a switch case for different headers.
            switch (fileHeader) {
                /*      0    1    2    3              4    5                 6         7                 8               */
                case "Namn;Namn;Kurs;Vinst per aktie;P E;Kapital per aktie;P JEK;Direktavkastning;Utdelning per aktie;" +
                   "Vinstmarginal;RSI;Nasta rapport;Utdelningsdatum;TA":
                case "Namn;Namn;Kurs;Vinst per aktie;P E;Kapital per aktie;P JEK;Direktavkastning;Utdelning per aktie;" +
                   "Vinstmarginal;RSI;Nasta rapport;Utdelningsdatum;TA;":
                    /*     9       10      11           12           13  */

                    //Console.WriteLine("-----------\n" + fileHeader + "\n???????\nNamn;Namn;Kurs;Vinst per aktie;P E;Kapital per aktie;P JEK;Direktavkastning;Utdelning per aktie;" +
                    //"Vinstmarginal;RSI;Nasta rapport;Utdelningsdatum;TA\n----------");
                    Counters.type1Counter++;
                    //errorFound = false;
                    //Console.WriteLine("Found in case");
                    recordElements = record.Split(';').ToList<string>();
                    if (recordElements.Count < 11)
                    {
                        //Console.WriteLine("ERROR! Less than 13 elements");
                        errorFound = true;
                    }
                    else if (recordElements[0].Equals("---Void---"))
                    {
                        //Console.WriteLine("Ignoring ---Void---");
                        errorFound = true;
                    }
                    else {
                        expectedName = recordElements[0];
                        name = recordElements[1];
                        //Check whether name is in white list, unknown or in black list:
                        //Console.WriteLine(expectedName + " - > " + name);
                        nameStatus = (1 == nc.findStockName(expectedName, name));
                        //Console.Write(" " + name + " ");
                        if (!recordElements[0].Equals(recordElements[1]))
                        {
                            /* UNCOMMENT BELOW !!!
                             * 
                             * 
                             * */
                            //Console.WriteLine("Name mismatch for " + name );
                            //errorFound = true;
                        }
                        //Console.WriteLine("AAAA1");
                        try { P = string2Float(recordElements[2]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("P format exception: " + recordElements[2]);
                        }

                        try { E = string2Float(recordElements[3]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("E format exception: " + recordElements[2]);
                        }
                        //Console.WriteLine(E);
                        try { PE = string2Float(recordElements[4]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("PE format exception: " + recordElements[2]);
                        }
                        checkPE();
                        //Console.WriteLine(P);
                        //Console.WriteLine(errorFound);
                        //TODO CHANGE THIS TO CHECK IF PERCENT OR NORMAL!!!
                        /*if (Math.Abs(Convert.ToDouble(P/E)) < epsilon) {
                            Console.WriteLine("Error, mismatch! ");
                        }*/
                        //Console.WriteLine("Before C for " + name);

                        try { C = string2Float(recordElements[5]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("C error! " + name);
                        }
                        //Console.WriteLine("Before PC for " + name);

                        try { PC = string2Float(recordElements[6]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("PC Error! " + name);
                        }
                        checkPC();
                        //Console.WriteLine($"Checking results: '{P}'/'{C}'='{PC}', is that '{P / C}'?");
                        //Console.WriteLine("Before DP for " + name);

                        try { DP = string2Float(recordElements[7]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("DP error! " + name);
                        }
                        //Console.WriteLine("Before D for " + name);

                        try { D = string2Float(recordElements[8]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("Failed to convert dividend to float: " + recordElements[8]);
                            //    Console.WriteLine("Try this instead: " + recordElements[8].Replace(".", ","));

                        }
                        checkYieldDivident();
                        //TODO ELABORATE ABOUT PERCENT V S FRACTION?
                        //Console.WriteLine($"Checking results: '{D}'/'{P}'='{DP}', is that '{D / P}'?");
                        //Console.WriteLine(recordElements.Count() + " " + name);
                        try { Pm = string2Float(recordElements[9]); }
                        catch (FormatException)
                        {
                            Console.WriteLine("Pm error! " + name);
                        }
                        //Console.WriteLine("After Pm " + name);

                        int i = 0;
                        //Console.WriteLine("---" + recordElements[10] + "---");
                        if (recordElements[10].Equals("tbd"))
                        {
                            RSI = -1;// -1 indicates that this value is null
                        }
                        else
                        {
                            RSI = string2Float(recordElements[10]);
                        }
                        //Console.WriteLine("After RSI " + name);

                        //Console.WriteLine("Length: " + recordElements.Count);
                        if (recordElements.Count == 12)
                        {
                            //Console.WriteLine("Dates seem to be missing?");
                            nextDividend = "NULL";
                            nextReport = "NULL";
                            if (Int32.TryParse(recordElements[12], out i))
                            {
                                TA = i;
                            }

                        }
                        else
                        {
                            //REFACTOR TO FUNCTION!
                            nextReport = recordElements[11];
                            //Console.WriteLine("Checking date");
                            //if(recordElements == "0")
                            if (!DateTime.TryParseExact(nextReport, formats,
                                  new CultureInfo("en-US"),
                                  DateTimeStyles.None,
                                  out dateValue))
                            {
                                //Console.WriteLine("NULLING NEXT REPORT");
                                nextReport = "NULL";
                            }
                            else
                            {
                                nextReport = "'" + nextReport + "'";
                            }

                            nextDividend = recordElements[12];
                            if (!DateTime.TryParseExact(nextDividend, formats,
                                  new CultureInfo("en-US"),
                                  DateTimeStyles.None,
                                  out dateValue))
                            {
                                //Console.WriteLine("NULLING NEXT DIVIDEND DATE");
                                nextDividend = "NULL";
                            }
                            else
                            {
                                nextDividend = "'" + nextDividend + "'";
                            }

                            //TODO CHECK LENGTH OF recordEleents before doing this
                            if (recordElements.Count == 14)
                            {
                                if (Int32.TryParse(recordElements[13], out i))
                                {
                                    TA = i;
                                }
                            }
                            else
                            {
                                //Console.WriteLine("TA not detected");
                            }
                        }
                        //Console.WriteLine("Error found: " + errorFound);
                        if (errorFound == false && 
                            nameStatus == true &&
                            !db.checkStockDate(fileDateValue, name)
                            )
                        {
                            //Console.WriteLine("Checkking name " + name + " for date " + fileDate + ": " + db.checkStockDate(fileDateValue, name));
                            //"TODO AVOID ADDING DUPLICATE ENTRIES!";
                            sql = "INSERT INTO StockTable(RecordDate, StockName, Price, PricePerEarning," +
                          " PricePerCapital, Yield, ProfitMargin, RSI, DividendDate, ReportDate, PriceMissing) VALUES('" +
                            fileDate + "', '" +
                            name + "', " +
                            P.ToString().Replace(",", ".") + ", " +
                            PE.ToString().Replace(",", ".") + ", " +
                            PC.ToString().Replace(",", ".") + ", " +
                            DP.ToString().Replace(",", ".") + ", " +
                            (Pm == null ? "NULL" : Pm.ToString().Replace(",", ".")) + ", " +
                            RSI.ToString().Replace(",", ".") + ", " +
                            nextDividend + ", " +
                            nextReport + ", '" +
                            (priceMissingInRecord == true ? 1 : 0) + "');";
                            //Console.WriteLine("INSERT INTO StockTable(RecordDate, StockName, Price, PricePerEarning," +
                            //" PricePerCapital, Yield, ProfitMargin, RSI, DividendDate, ReportDate) VALUES('1970-01-01', 'GodFather', 10.0, 7, 8, 9, 10, 11, 12, 13, 14);");
                            //Console.WriteLine(sql);
                            //Console.Write("+");

                            //Console.WriteLine(sql);
                            db.launchSqlCommand(sql);
                        }
                        else
                        {
                            //Console.WriteLine("\nSkipping record for " + name + " on " + fileDate);
                        }
                        //db.writeSummareyToConsole();
                        identified = true;//DELETE
                                          //PE = recordElements[3];
                    }
                    break;
                //case "Namn,Namn,Kurs,P / E,P / S,P / JEK,Direktavkastning,Vinstmarginal,Marknadsvärde,Utdelningsdatum,Senaste årsbokslut, " +
                //"Utveckling sedan årsskiftet, Vinst per aktie, P/ E - tal,Kapital per aktie,Betalkurs / JEK,Direktavkastning,Vinstmarginal,RSI,TA":
                case "Namn,Namn,Kurs,P / E,P / S,P / JEK,Direktavkastning,Vinstmarginal,MarknadsvÃ¤rde,Utdelningsdatum,Senaste Ã¥rsbokslut," +
                "Utveckling sedan Ã¥rsskiftet,Vinst per aktie,P / E - tal,Kapital per aktie,Betalkurs / JEK,Direktavkastning,Vinstmarginal,RSI,TA":

                    Counters.type2Counter++;
                    Console.Write("2");
                    identified = true;
                    break;
                case "Namn,Namn,Kurs,P / E,Kursrekord,Marknadsvärde,Förändring under året,Vinst per aktie,P / E - tal,Kapital per aktie,Betalkurs / JEK," +
                "Direktavkastning,Vinstmarginal,RSI,TA":
                    Counters.type3Counter++;
                    Console.Write("3");
                    identified = true;
                    break;
                default:
                    Console.Write("?");
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

                                

            void checkPE() {
                /*TODO: ERROR WHEN POPULATING PartnerTech 2015-10-10. Nulled data added*/
                /* Sum up the missing values */
                int nrOfNulls = (P == null ? 1 : 0) + (PE == null ? 1 : 0) + (E == null ? 1 : 0);
                if (P == null) { errorFound = true; }
                //Console.WriteLine("PE: NULLS: " + nrOfNulls);
                if (nrOfNulls > 1) {
                    //Console.WriteLine(" Too many nulls, can't recover!");
                    errorFound = true;
                } else if (nrOfNulls == 0) {
                    //Console.WriteLine(" No nulls, no need to recover!");
                    //Console.WriteLine($"Checking results: '{P}'/'{E}'='{PE}', is that '{P / E}'?");
                }
                else {
                    if (P == null) {
                        priceMissingInRecord = true;
                        P = PE * E;
                    } else if (PE == null) {
                        PE = P / E;
                    } else {
                        E = P / PE;
                    }
                    //Console.WriteLine($"Checking results: '{P}'/'{E}'='{PE}', is that '{P / E}'?");
                }
            }

            void checkPC() {
                /* Sum up the missing values */
                int nrOfNulls = (P == null ? 1 : 0) + (PC == null ? 1 : 0) + (C == null ? 1 : 0);

                //Console.WriteLine("PC: NULLS: " + nrOfNulls);
                if (nrOfNulls > 1)
                {
                    //Console.WriteLine(" Too many nulls, can't recover!");
                    errorFound = true;
                }
                else if (nrOfNulls == 0)
                {
                    //Console.WriteLine(" No nulls, no need to recover!");
                    //Console.WriteLine($"Checking results: '{P}'/'{C}'='{PC}', is that '{P / C}'?");
                }
                else
                {
                    if (P == null)
                    {
                        priceMissingInRecord = true;
                        P = PC * C;
                    }
                    else if (PC == null)
                    {
                        PC = P / C;
                    }
                    else
                    {
                        C = P / PC;
                    }
                    //Console.WriteLine($"Checking results: '{P}'/'{C}'='{PC}', is that '{P / C}'?");
                }

            }

            void checkYieldDivident() {
                if (D == null && DP == null)
                {
                    D = 0;
                    DP = 0;
                }
                else if (D == null && DP != null)
                {
                    D = DP * P;
                }
                else if (D != null && DP == null) {
                    DP = D / P;
                }
                //TODO CHECK THAT DP IS ALMOST D / P
            }


            int checkForErrors() {
                    return -1;
            }
        }

    }

}
