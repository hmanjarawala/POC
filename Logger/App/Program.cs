//using SampleLogger;
using System;
using AAUtility;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            //ILogger logger = LogManager.GetLogger(string.Format(@"C:\Users\himanshu.manjarawala\Documents\C#\Sample Logger\HistoryLog-{0}.log", DateTime.Now.ToString("dd-MM-yyyy")));
            //logger.LogInfo("Hello Moto....");
            //logger.LogError("Hello Moto....");
            //logger.LogDebug("Hello Moto....");
            //logger.LogWarn("Hello Moto....");
            //logger.LogFatal("Hello Moto....");
            string jsonString = "{" +
                                    "\"store\": {" +
                                                "\"book\": [" +
                                                   " {" +
                                                "\"category\": \"reference\"," +
                                                        "\"author\": \"Nigel Rees\"," +
                                                        "\"title\": \"Sayings of the Century\"," +
                                                        "\"price\": 8.95" +
                                            "}," +
                                            "{" +
                                                "\"category\": \"fiction\"," +
                                                "\"author\": \"Evelyn Waugh\"," +
                                                "\"title\": \"Sword of Honour\"," +
                                                "\"price\": 12.99" +
                                            "}," +
                                            "{" +
                                                "\"category\": \"fiction\"," +
                                                "\"author\": \"Herman Melville\"," +
                                                "\"title\": \"Moby Dick\"," +
                                                "\"isbn\": \"0-553-21311-3\"," +
                                                "\"price\": 8.99" +
                                            "}," +
                                            "{" +
                                                "\"category\": \"fiction\"," +
                                                "\"author\": \"J. R. R. Tolkien\"," +
                                                "\"title\": \"The Lord of the Rings\"," +
                                                "\"isbn\": \"0-395-19395-8\"," +
                                                "\"price\": 22.99" +
                                            "}" +
                                        "]," +
                                        "\"bicycle\": {" +
                                            "\"color\": \"red\"," +
                                            "\"price\": 19.95" +
                                        "}" +
                                    "}," +
                                    "\"expensive\": 10" +
                                "}";

            /**
            
                JsonPath Result


                $.store.book[*].author                  The authors of all books 
                $..author                               All authors 
                $.store.*                               All things, both books and bicycles 
                $.store..price                          The price of everything 
                $..book[2]                              The third book 
                $..book[-2]                             The second to last book 
                $..book[0,1]                            The first two books 
                $..book[:2]                             All books from index 0 (inclusive) until index 2 (exclusive) 
                $..book[1:2]                            All books from index 1 (inclusive) until index 2 (exclusive) 
                $..book[-2:]                            Last two books 
                $..book[2:]                             Book number two from tail 
                $..book[?(@.isbn)]                      All books with an ISBN number 
                $.store.book[?(@.price < 10)]           All books in store cheaper than 10 
                $.expensive                             Give me value of "expensive" 
                $..*                                    Give me every thing 

            **/

            UtilityFasad uf = new UtilityFasad();
            Console.WriteLine(uf.GetDateTimeFromDateTime("25/11/2019", "dd/MM/yyyy", "en-US", "dd-MM-yyyy"));
            Console.WriteLine(uf.GetNthDayOfWeek("25/11/2019 06:12 AM", "dd/MM/yyyy hh:mm tt", "en-IN", "dd-MM-yyyy", 2, "Saturday"));
            Console.WriteLine(uf.GetMatchingGroup("4", @"[0-9]+", 1, 1, false, false));
            Console.WriteLine(uf.GetLongestMatchedString("C0232K19DOI43967/HDFC BANK LIMITED", "ABCC0232K19DOI43967"));
            Console.WriteLine(uf.GetLongestMatchedString("cov_basic_as_cov_x_gt_y_rna_geness_w1000000", "cov_rna15pcs_as_cov_x_gt_y_rna_genes_w1000000"));
            Console.WriteLine(uf.GetFutureDate("09-01-2019", "dd-MM-yyyy", 5, "False", "en-US"));
            uf.Initialize(@"C:\Users\himanshu.manjarawala\Documents\C#\Configuration_Settings.xml", @"C:\Users\himanshu.manjarawala\Documents\C#");
            var jsonNode = uf.GetMultipleNodes(jsonString, "$.expensive");
            uf.UpdateActivityLog("Task Start", "Mainbot.atmx");
            uf.LogError("Error occur in task, Kindly check the Task", "Mainbot.atmx");
            uf.UpdateActivityLog("Task Stop", "Mainbot.atmx");
            Console.WriteLine(uf.GetConfigurationValue("CUSTOM_ERROR_MESSAGE"));
            string input = "12ABC 16DEF AB14D";
            Console.WriteLine(uf.GetWeekOfTheMonth("29-04-2019", "dd-MM-yyyy", "en-US"));
            Console.WriteLine(uf.GetMatchCount(input, "[A-Z]+", 1, false, false));
            Console.WriteLine(uf.GetMatchingGroup("The the quick brown fox  fox jumps over the lazy dog dog.", @"\b(?<word>\w+)\s+(\k<word>)\b", 1, 3, false, false));
            Console.WriteLine(uf.GetMatchingStrings("The the quick brown fox  fox jumps over the lazy dog dog.", @"\b(?<word>\w+)\s+(\k<word>)\b", 1, false, false));
            Console.WriteLine(uf.ReplaceMatchingString(input, "[A-Z]+", "XX", 12, 3, false, false));
            Console.Read();
        }
    }
}
