using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDAProject
{
    public interface ILogProcessor
    {
        List<Transaction> ParseTransactions(string logStatementFilePath, string transactionFilePath);
        List<LogModel> ParseLogModels(string logStatementFilePath);
    }
}
