using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace LogPreprocessor
{
    public class FileUtility
    {
        public void ConvertToJsonFile(string transactionTextFilePath, string outputJsonFilePath)
        {
            var lines = File.ReadAllLines(transactionTextFilePath);
            int count = 1;
            List<TransactionJsonModel> transactionJsonModels = new List<TransactionJsonModel>();
            var transactionJsonModel = new TransactionJsonModel();
            foreach(var line in lines)
            {
                if (line.Equals(""))
                    continue;

                if (line.ToLower().Trim(' ').StartsWith("transaction#"))
                {
                    transactionJsonModel = new TransactionJsonModel();
                    transactionJsonModel.Id = count.ToString();
                    count++;
                    transactionJsonModel.Status = line;
                    transactionJsonModels.Add(transactionJsonModel);
                    continue;
                }
                transactionJsonModel.Logs.Add(line);
            }

            var jsonText = JsonConvert.SerializeObject(transactionJsonModels);
            File.WriteAllText(outputJsonFilePath,jsonText);
            
        }
    }

    public class TransactionJsonModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public List<string> Logs { get; set; }

        public TransactionJsonModel()
        {
            Logs = new List<string>();
        }

    }
}
