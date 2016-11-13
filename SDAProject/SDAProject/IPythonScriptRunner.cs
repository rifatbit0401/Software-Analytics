using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDAProject
{
    public interface IPythonScriptRunner
    {
        List<String> RunFCAPythonScript(string inputDataFilePathInUnixFormat);
        List<String> RunCLPythonScript(string inputDataFilePathInUnixFormat);
    }
}
