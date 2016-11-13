using System.Collections;
using SDAProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApplication1
{
    public class ARFFileWriter
    {
        /*public void TestARFFileWriter()
        {
            var controller = new Controller();
            controller.Initiate();
        }*/

        public void WriteARFFile(string filePath, Concept parentConcept, Concept childConcept,
            List<Transaction> transactions, List<LogModel> logModels)
        {
            for (int i = 0; i < parentConcept.Intents.Count; i++)
            {
                parentConcept.Intents[i] = parentConcept.Intents[i].Replace('\'', ' ').Replace('l', ' ').Trim(' ');
            }
            for (int i = 0; i < parentConcept.Extents.Count; i++)
            {
                parentConcept.Extents[i] = parentConcept.Extents[i].Replace('\'', ' ').Replace('t', ' ').Trim(' ');
            }
            for (int i = 0; i < childConcept.Intents.Count; i++)
            {
                childConcept.Intents[i] = childConcept.Intents[i].Replace('\'', ' ').Replace('l', ' ').Trim(' ');
            }
            for (int i = 0; i < childConcept.Extents.Count; i++)
            {
                childConcept.Extents[i] = childConcept.Extents[i].Replace('\'', ' ').Replace('t', ' ').Trim(' ');
            }

            var parentTransactions = transactions.Where(t => parentConcept.Extents.Contains(t.Id)).ToList();
            var childTransactions = transactions.Where(t => childConcept.Extents.Contains(t.Id)).ToList();
            var commonTransactions = new List<Transaction>();


            foreach (var childTransaction in childTransactions)
            {
                var foundTransaction = parentTransactions.FirstOrDefault(t => t.Id.Equals(childTransaction.Id));
                if (foundTransaction != null)
                {
                    commonTransactions.Add(foundTransaction);
                }
            }


            var parentLogModels = logModels.Where(l => parentConcept.Intents.Contains(l.Id)).ToList();

            var map = BuildAttributeDictionary(commonTransactions, parentLogModels);
           
            string txt = "@RELATION iris\n\n";
            foreach (var parentLogModel in parentLogModels)
            {
                foreach (var cf in parentLogModel.ContextualFactors)
                {
                    /*int num;
                    txt = txt + "@ATTRIBUTE " + cf;
                    if (int.TryParse(cf, out num))
                    {
                        txt += "\t" + "NUMERIC";
                    }
                    else
                    {
                        txt += "\t" + "string";
                    }
                    txt += "\n";*/
                    var modifiedCf = cf + "_" + parentLogModel.Id;
                    txt += "@ATTRIBUTE " + modifiedCf + "{";

                    foreach (var value in map[modifiedCf])
                    {
                        txt += "'" + value + "',";
                    }

                    txt = txt.Remove(txt.Length - 1);
                    txt += "}\n";
                }
            }
            // txt += "@ATTRIBUTE class\t{success,failure}\n";
            txt += "\n@DATA\n";

            foreach (var commonTransaction in commonTransactions)
            {
                Dictionary<string, bool> used = new Dictionary<string, bool>();
                string row = "";
                foreach (var logData in commonTransaction.LogDataList)
                {
                    if(parentLogModels.FirstOrDefault(l=>l.Id.Equals(logData.LogModel.Id))==null)
                        continue;
                    int id = 0;
                    foreach (var contextualValue in logData.ContextualValues)
                    {
                        var cf = logData.LogModel.ContextualFactors.ToArray()[id]+"_"+logData.LogModel.Id;
                        if (!used.ContainsKey(cf))
                        {
                            row += "'" + contextualValue + "',";
                        }
                        used[cf] = true;
                        id++;
                    }
                }
                //   txt += "success";
                row = row.Remove(row.Length - 1);
                row += "\n";
                txt += row;
            }

            File.WriteAllText(filePath, txt);
        }

        public Dictionary<string, HashSet<string>> BuildAttributeDictionary(List<Transaction> transactions, List<LogModel>logModels )
        {
            var map = new Dictionary<string, HashSet<string>>();
            foreach (var logData in transactions.First().LogDataList)
            {
                if(logModels.FirstOrDefault(l=>l.Id.Equals(logData.LogModel.Id)) == null)
                    continue;
                foreach (var contextualFactor in logData.LogModel.ContextualFactors)
                {
                    var modifiedCf = contextualFactor + "_" + logData.LogModel.Id;
                    if (!map.ContainsKey(modifiedCf))
                    {
                        map.Add(modifiedCf, new HashSet<string>());
                    }
                }
            }

            foreach (var transaction in transactions)
            {
                foreach (var logData in transaction.LogDataList)
                {
                    if (logModels.FirstOrDefault(l => l.Id.Equals(logData.LogModel.Id)) == null)
                        continue;
                
                    int id = 0;
                    foreach (var contextualValue in logData.ContextualValues)
                    {
                        var key = logData.LogModel.ContextualFactors.ToArray()[id] + "_" + logData.LogModel.Id;
                        map[key].Add(contextualValue);
                        id++;
                    }
                }
            }
            return map;
        }


    }
}
