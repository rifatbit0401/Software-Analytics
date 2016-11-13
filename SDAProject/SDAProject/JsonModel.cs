using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDAProject
{
    class JsonModel
    {
    }

    public class TransactionFileJsonModel
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public List<String> LogStringList { get; set; }
    }


}
