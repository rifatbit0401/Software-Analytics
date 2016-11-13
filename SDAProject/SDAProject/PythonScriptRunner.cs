using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDAProject
{
    public class PythonScriptRunner : IPythonScriptRunner
    {
        private string _pythonScriptFilePath;

        public PythonScriptRunner(string pythonScriptFilePath)
        {
            _pythonScriptFilePath = pythonScriptFilePath;
        }

        public void TestPythonScriptRunner()
        {
            string inputDataFilePathInUnixFormat = "I:/Saif NDC/sample_fca.csv";
            RunFCAPythonScript(inputDataFilePathInUnixFormat);
            var x = RunCLPythonScript(inputDataFilePathInUnixFormat);
           // File.WriteAllLines(@"I:\Saif NDC\cl_output.txt",x);
        }


        public List<String> RunFCAPythonScript(string inputDataFilePathInUnixFormat)
        {
            return RunPythonScript(CreatePythonScriptForFCA(inputDataFilePathInUnixFormat));
        }

        public List<String> RunCLPythonScript(string inputDataFilePathInUnixFormat)
        {
            return RunPythonScript(CreatePythonScriptForCL(inputDataFilePathInUnixFormat));
        }

        private List<String> RunPythonScript(List<String> pythonScipt)
        {
            var outputString = new List<string>();
           // _pythonScriptFilePath = @"I:\Saif NDC\python_script.py";
            File.WriteAllLines(_pythonScriptFilePath, pythonScipt);
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = @"C:\Anaconda2\python.exe";
            processStartInfo.Arguments = "\"" + _pythonScriptFilePath + "\"";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            var process = Process.Start(processStartInfo);
            var output = process.StandardOutput;

            while (!output.EndOfStream)
            {
                //Console.WriteLine(output.ReadLine());
                outputString.Add(output.ReadLine());
            }

            return outputString;
        }

        private List<String> CreatePythonScriptForFCA(string inputDataFilePathInUnixFormat)
        {
            var script = new List<string>();
            script.Add("from concepts import Context");
            script.Add("c = Context.fromfile('" + inputDataFilePathInUnixFormat + "', frmat='csv')");
            //            script.Add("c = Context.fromfile('I:/Saif NDC/relations.csv', frmat='csv')");
            script.Add("for extent, intent in c.lattice:");
            script.Add("     print('%r %r' % (extent, intent))");
            script.Add("\n");
            //script.Add("print c.lattice.graphviz().source");
            //script.Add("\n");
            return script;
        }

        private List<String> CreatePythonScriptForCL(string inputDataFilePathInUnixFormat)
        {
            var script = new List<string>();
            script.Add("from concepts import Context");
            script.Add("c = Context.fromfile('" + inputDataFilePathInUnixFormat + "', frmat='csv')");
            //            script.Add("c = Context.fromfile('I:/Saif NDC/relations.csv', frmat='csv')");
            //script.Add("for extent, intent in c.lattice:");
            //script.Add("     print('%r %r' % (extent, intent))");
            //script.Add("\n");
            script.Add("print c.lattice.graphviz().source");
            script.Add("\n");
            return script;
        }
    }
}
