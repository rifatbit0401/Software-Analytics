using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogPreprocessor
{
    class Program
    {
        static void Main(string[] args)
        {
           /* var crawler = new LogCrawler();
            crawler.CrawlLogStatements(@"D:\Rifat\software analytics\Data\IITDU.SIUSC\CVAnalyzer");
            */
            FileUtility fileUtility = new FileUtility();
            fileUtility.ConvertToJsonFile(@"D:\Rifat\software analytics\Data\logdata\transaction.txt", @"D:\Rifat\software analytics\Data\logdata\transaction_1.json");
            Console.ReadLine();
        }
    }
}
