using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LogPreprocessor
{
    public class LogCrawler
    {
        public List<string> CrawlLogStatements(string projectPath)
        {
            List<string> logStatements = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(projectPath);
            List<FileInfo> sourceFiles = new List<FileInfo>();
            
            BrowseAllSourceFile(directoryInfo, sourceFiles);

            foreach(var file in sourceFiles)
            {
                foreach(var line in File.ReadAllLines(file.FullName))
                {
                    string statement = line.Trim(' ');
                    string logStatementIdentifier = "logger.Info(";
                    if (statement.Contains(logStatementIdentifier))
                    {
                        statement = statement.Remove(0, logStatementIdentifier.Length);
                        statement = statement.Substring(0, statement.Length - 1);
                        logStatements.Add(statement);
                    }
                }
            }

          //  CheckLogStatement(logStatements);

            return logStatements;
        }

        private void BrowseAllSourceFile(DirectoryInfo directory, List<FileInfo> sourceFiles)
        {
            
            foreach(var file in directory.GetFiles())
            {
                if (file.Extension.Equals(".cs"))
                {
                    sourceFiles.Add(file);
                }
            }

            foreach(var dir in directory.GetDirectories())
            {
                BrowseAllSourceFile(dir, sourceFiles);
            }
        }

        private void CheckLogStatement(List<string>logStatements)
        {
            foreach (var stat in logStatements)
            {
                if (stat.Contains(":"))
                {
                    Console.WriteLine(stat);
                }
                string id = stat.Split(' ')[0].Split('=')[1];
                if (int.Parse(id.Trim(' ')) != logStatements.IndexOf(stat) + 1)
                {
                    Console.WriteLine(stat);
                }
            }
        }


    }
}
