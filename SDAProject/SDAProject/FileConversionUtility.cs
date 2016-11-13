using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDAProject
{
    public class FileConversionUtility
    {
        public void TestFilConversionUtility()
        {
            string logStatementFilePath = @"I:\Saif NDC\log.txt";
            string transactionFilePath = @"I:\Saif NDC\transaction.json";
            string filePath = @"I:\Saif NDC\sample_fca.csv";
            ILogProcessor logProcessor = new LogProcessor();
            var logModels = logProcessor.ParseLogModels(logStatementFilePath);
            var transactions = logProcessor.ParseTransactions(logStatementFilePath, transactionFilePath);
            CreateCSVFile(filePath, transactions, logModels);
        }

        public void CreateCSVFile(string filePath, List<Transaction> transactions, List<LogModel> logModels)
        {
            String text = "";
            foreach (var logModel in logModels)
            {
                text += ",l" + logModel.Id;
            }
            text += "\n";
            foreach (var transaction in transactions)
            {
                // text += transaction.Id;
                String row = "";
                for (int i = 0; i < logModels.Count(); i++)
                {
                    row += ",";
                }
                foreach (var logData in transaction.LogDataList)
                {
                    int index = logModels.FindIndex(log => log.Id.Equals(logData.LogModel.Id));
                    int position = 2 * index + 1;
                    if (position < row.Length)
                    {
                        row = row.Insert(position, "X");
                    }
                    else if (position == row.Length)
                    {
                        row += "X";
                    }
                }
                row = "t" + transaction.Id + row + "\n";
                text += row;
            }
            File.WriteAllText(filePath, text);
        }
    }
}
