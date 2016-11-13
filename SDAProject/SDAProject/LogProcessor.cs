using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SDAProject
{
    public class LogProcessor : ILogProcessor
    {

        public void TestLogProcessor()
        {
            string logStatementFilePath = @"I:\Saif NDC\log.txt";
            string transactionFilePath = @"I:\Saif NDC\transaction.json";
            var logModels = CreateLogModelList(logStatementFilePath);
            CreateTransactionList(transactionFilePath, logModels);

            //  var logData = new LogData("id=1 signin check username=iit, password=iit123", logModels);
        }

        public List<LogModel> CreateLogModelList(string logStatemtFilePath)
        {
            var logModels = new List<LogModel>();
            var logStatements = File.ReadAllLines(logStatemtFilePath);
            foreach (var logStatement in logStatements)
            {
                var logModel = new LogModel(logStatement);
                logModels.Add(logModel);
            }
            return logModels;
        }

        public List<Transaction> CreateTransactionList(string transactionJsonFilePath, List<LogModel> logModels)
        {
            var transactions = new List<Transaction>();
            var transactionJsonModels =
                JsonConvert.DeserializeObject<List<TransactionFileJsonModel>>(File.ReadAllText(transactionJsonFilePath));
            foreach (var transactionFileJsonModel in transactionJsonModels)
            {
                var transaction = new Transaction();
                transaction.Id = transactionFileJsonModel.Id;
                transaction.Status = transactionFileJsonModel.Status;
                foreach (var logString in transactionFileJsonModel.LogStringList)
                {
                    var logData = new LogData(logString, logModels);
                    transaction.LogDataList.Add(logData);
                }
                transactions.Add(transaction);
            }

            return transactions;
        }


        public List<Transaction> ParseTransactions(string logStatementFilePath, string transactionFilePath)
        {
            var logModels = CreateLogModelList(logStatementFilePath);
            return CreateTransactionList(transactionFilePath, logModels);
        }

        public List<LogModel> ParseLogModels(string logStatementFilePath)
        {
            return CreateLogModelList(logStatementFilePath);
        }
    }

    public class LogModel
    {
        public string Id { get; set; }
        public string Statement { get; set; }
        public List<string> ContextualFactors { get; set; }

        public LogModel(string logStatement)
        {
            ContextualFactors = new List<string>();
            InitializeObject(logStatement);
        }

        private void InitializeObject(string logStatement)
        {
            this.Id = ParseLogId(logStatement);
            this.Statement = logStatement;

            int count = 0;
            string prunedLogStatement = "";
            for (int i = 0; i < logStatement.Length; i++)
            {
                if (logStatement[i] == '"')
                {
                    count++;
                    prunedLogStatement += "#";
                    continue;
                }

                if (count % 2 == 1)
                    continue;

                prunedLogStatement += logStatement[i];
            }
            prunedLogStatement = prunedLogStatement.Replace('+', '#');
            foreach (var contextualFactor in prunedLogStatement.Split('#'))
            {
                var trimmedContextualFactor = contextualFactor.Trim(' ');
                if (trimmedContextualFactor == "")
                    continue;
                this.ContextualFactors.Add(trimmedContextualFactor);
            }
        }

        public string ParseLogId(string logStatement)
        {
            return logStatement.Split('"')[1].Split('=')[1].Trim();
        }
    }

    public class LogData
    {
        public LogModel LogModel { get; set; }
        public string LogTracedString { get; set; }
        public List<string> ContextualValues { get; set; }
        public LogData(string logString, List<LogModel> logModels)
        {
            ContextualValues = new List<string>();
            InitializeObject(logString, logModels);
        }

        private void InitializeObject(string logString, List<LogModel> logModels)
        {
            string logId = ParseLogId(logString);
            this.LogModel = logModels.First(lg => lg.Id.Equals(logId));
            this.LogTracedString = logString;

            ParseContextualValues(logString);
        }

        private void ParseContextualValues(string logString)
        {
            var staticStringList =
                LogModel.Statement.Split('+').Where(s => s.Trim().StartsWith("\"") && s.Trim().EndsWith("\""));
            foreach (var str in staticStringList)
            {
                logString = logString.Replace(str.Trim('"'), "#");
            }
            foreach (var contextualValue in logString.Split('#'))
            {
                if (contextualValue == "")
                    continue;
                this.ContextualValues.Add(contextualValue);
            }
            // Console.WriteLine(logString);
        }

        private string ParseLogId(string logString)
        {
            return logString.Trim().Split(' ')[0].Split('=')[1];
        }



    }


    public class Transaction
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public List<LogData> LogDataList { get; set; }
        public Transaction()
        {
            LogDataList = new List<LogData>();
        }
    }

}
