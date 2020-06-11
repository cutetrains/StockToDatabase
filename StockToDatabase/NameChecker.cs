using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StockToDatabase
{
    class NameChecker
    {
        List<string> unknownList = new List<string>();
        
        public const int XML_WHITE_LIST = 1;
        public const int XML_BLACK_LIST = 2;
        XmlDocument doc = new XmlDocument();
        Dictionary<string, string> whiteDict =
                  new Dictionary<string, string>();

        Dictionary<string, string> blackDict =
                  new Dictionary<string, string>();

        public NameChecker() { 
            Console.WriteLine("Initiated NameChecker");
            //XmlDocument doc = new XmlDocument();
            
            doc.PreserveWhitespace = true;
            //TODO Remove hard coded values later?
            try {
                // C:\Users\gusta\Dropbox\Ekonomi\results\namesAlias.xml
                doc.Load("C:\\Users\\gusta\\Dropbox\\Ekonomi\\results\\namesAlias.xml");
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Failed to load namesAlias");
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf - 8\"?> " +
                            " < stockRecords > " +
                            "   < whiteList > " +
                            "   </ whiteList > " +
                            "   < blackList > " +
                            "   </ blackList > " +
                            " </ stockRecords > ");
            }

        }
        

        /**********************************************************************
         * findStockName checks whether the combination of the expected stock 
         * name and returned stock names are found 
         * in the dictionaries.
         * 
         * @param expectedName
         * @param returnedName
         * @param mode
         * 
         * @return int, 1 means that the combination has been found
         *              in the white list. The record can be sent to database
         *              0 means that the combination hasn't been found. A XML
         *              proposal string is shown in the consolse
         *              -1 means that the combination has been found in the 
         *              black list and that record shall not be sent to 
         *              the database
         *********************************************************************/
        public int findStockName(string expectedName, string returnedName) {

            // VALID NAMES ARE INCORRECTLY MARKED AS UNKNOWN
            returnedName = returnedName.Replace("&", "&amp;");
            expectedName = expectedName.Replace("&", "&amp;");
            //Console.WriteLine("Is :" + returnedName + "--- same as \n +++"+ expectedName + "?" + 
            //    whiteDict.ContainsKey(returnedName) + " " +  whiteDict.ContainsKey(expectedName));
            if (whiteDict.ContainsKey(expectedName) && whiteDict[expectedName].Contains(returnedName))
            {
                return 1;
            }
            else if (blackDict.ContainsKey(expectedName) && blackDict[expectedName].Contains(returnedName)) {
                return -1;
            } else
            {
                if (!unknownList.Contains("    <stock name = \"" + returnedName + "\" alias = \"" + expectedName + "\" /> "))
                {
                    unknownList.Add("    <stock name = \"" + returnedName + "\" alias = \"" + expectedName + "\" /> ");
                }
                return 0;
            }
        }
          
        public void importDictionaries()
        {
            int currentListType = 0;
            Console.WriteLine("In importDictionaries");

            XmlNode root = doc.DocumentElement;
            XmlNodeList listTypes = root.ChildNodes;
            Console.WriteLine(listTypes.Count);
            
            string stockName;
            string stockAlias;
            string[] stockNames;
            List<string> types = new List<string>();
            types.Add("white");
            types.Add("black");
            foreach (XmlNode thisNode in listTypes)
            {


                if (thisNode.NodeType != XmlNodeType.Whitespace)
                {
                    currentListType++;
                    switch (currentListType)
                    {
                        case XML_WHITE_LIST:
                            Console.WriteLine("White List/Allowed combinations:");
                            break;

                        case XML_BLACK_LIST:
                            Console.WriteLine("Black List/Not allowed combinations:");
                            break;

                        default:
                            Console.WriteLine("Invalid part of XML file");
                            break;
                    }
                    XmlNodeList listElements = thisNode.ChildNodes;
                    foreach (XmlNode pairNode in listElements)
                    {
                        if (pairNode.NodeType != XmlNodeType.Whitespace)
                        {
                            stockNames = pairNode.OuterXml.Split('\"');
                            Console.WriteLine("   " + stockNames[1] + "..." + stockNames[3]);

                            //TODO: ALERT IF THE RECORD ALREADY EXIST
                            if (currentListType == XML_WHITE_LIST) {
                                //CHECK IF KEY AND VALUE PAIR EXISTS
                                //if (whiteDict.ContainsKey(stockNames[3]) && myDic[stockNames[3]].Equals(stockNames[1]))
                                whiteDict[stockNames[3]]= stockNames[1];
                                //whiteDict.Add(stockNames[3], stockNames[1]);
                            }
                            else if (currentListType == XML_BLACK_LIST) {
                                blackDict.Add(stockNames[3], stockNames[1]);
                            }
                            //Console.WriteLine(currentListType);
                        }
                    }
                }
            }
            //TODO make function to print contents.
            Console.WriteLine("White:");
            whiteDict.ToList().ForEach(x => Console.WriteLine(x.Key + " -> " + x.Value));
            Console.WriteLine("Black:");
            blackDict.ToList().ForEach(x => Console.WriteLine(x.Key + " -> " + x.Value));
        }

        public void exportDictionaries()
        {
            Console.WriteLine("In exportDictionaries");
        }

        //TODO DELETE!
        public void findNode(string str) {
            Console.WriteLine("Finding " + str + " in xml tree");
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/4d47d0b4-6522-41f4-ba29-63e38ba340ef/how-to-select-child-node-in-xmldocument-by-attribute-value?forum=csharpgeneral
            XmlNode root = doc.DocumentElement;
            if (root != null)
            {
                Console.WriteLine(root.ToString());
                //XmlNode node = doc.DocumentElement.SelectSingleNode("//node[@label='ABB']/node[@label='Web.Config']");
                //XmlNode node = root.SelectSingleNode("//stock[@name='Getinge']/stock[@name='ABB']");
                //XmlNode node = root.SelectSingleNode("//node[@name=\"Getinge\"]/node[@name=\"ABB\"]");

                XmlNode node = doc.GetElementsByTagName("stock")[1];
                //XmlNode node = doc.GetElementById("Getinge");
                if (node != null)
                {
                    //Process the node 
                    Console.WriteLine("\n Found: \n  " + node.ToString() + "\n end \n");
                    Console.WriteLine(node.OuterXml);
                }
                else {
                    Console.WriteLine("No matches");
                }


            }
        }

        public void showUnknownCombinations() {
            Console.WriteLine("The following combinations were not found in nameAlias.xml. Check whether to update them or not!");
            foreach (string s in unknownList) {
                Console.WriteLine(s);
            }
            
            Console.WriteLine("White:");
            foreach (KeyValuePair<string, string> kvp in whiteDict)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine("Black:");
            foreach (KeyValuePair<string, string> kvp in blackDict)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            

        }

    }
}
