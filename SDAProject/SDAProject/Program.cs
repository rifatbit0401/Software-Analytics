using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDAProject
{
    class Program
    {
        static void Main(string[] args)
        {
            new PythonScriptRunner(@"I:\ASR Analytics\python_script.py").TestPythonScriptRunner();
            //new FileConversionUtility().TestFilConversionUtility();
           // new LogProcessor().TestLogProcessor();
           // TestAlgorithmClass();
            Console.ReadLine();
        }

        private static void TestAlgorithmClass()
        {
            var algorithm = new Algorithm();
            string fcaFilePath = @"E:\MSSE Program\MSSE 2nd Semester\Software Analytics\Code\my_input.txt";
            string latticeFilePath = @"E:\MSSE Program\MSSE 2nd Semester\Software Analytics\Code\lattice_input.txt";
            algorithm.CreateConceptLattice(algorithm.CreateConceptDictorionary(fcaFilePath), latticeFilePath);
        }
    }


    public class Algorithm : IConceptLattice
    {

        private const String EXTENTS = "extents:";
        private const String INTENTS = "intents";
        private const String EDGES = "edges";
        private const char INPUT_SEPERATOR = ';';
        private List<String> _extents = new List<string>();
        private List<String> _intents = new List<string>();


        public List<Concept> ConstructConceptLattice(string fcaFilePath, string latticeFilePath)
        {
            var conceptMap = CreateConceptDictorionary(fcaFilePath);
            CreateConceptLattice(conceptMap, latticeFilePath);
            return conceptMap.Values.ToList();
        }

        public List<Concept> FormalConceptAnalysis()
        {
            return null;
        }

        public Dictionary<string, Concept> CreateConceptDictorionary(String filePath)
        {
            var conceptMap = new Dictionary<string, Concept>();
            var concepts = new List<Concept>();
            foreach (var line in File.ReadAllLines(filePath))
            {
                String extentStr = line.Substring(1, line.IndexOf(')') - 1);
                String intentStr = line.Substring(line.LastIndexOf('(') + 1, line.Length - line.LastIndexOf('(') - 2);
                var concept = new Concept();

                foreach (var extent in extentStr.Split(','))
                {
                    concept.Extents.Add(extent.Trim());
                }
                foreach (var intent in intentStr.Split(','))
                {
                    concept.Intents.Add(intent.Trim());
                }
                concepts.Add(concept);
            }

            concepts.Reverse();
            String ch = "c";

            foreach (var concept in concepts)
            {
                String nodeLabel = ch + concepts.IndexOf(concept).ToString();
                conceptMap.Add(nodeLabel, concept);
            }

            return conceptMap;
        }


        public void CreateConceptLattice(Dictionary<String, Concept> conceptMap, string filePath)
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                if(line=="")
                    continue;
                string parent = line.Split(' ')[1];
                string child = line.Split(' ')[0];
                if (parent == child)
                    continue;
                var parentConcept = conceptMap[parent];
                var childConcept = conceptMap[child];

                if (parentConcept.ChildList.Contains(childConcept))
                    continue;
                parentConcept.ChildList.Add(childConcept);
            }

        }

        private String Remove(String str, char ch)
        {
            string newStr = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ch)
                    continue;
                newStr += str[i];
            }

            return newStr;
        }



    }

    public class Concept
    {
        public List<String> Intents { get; set; }
        public List<String> Extents { get; set; }
        public List<Concept> ChildList { get; set; }

        public Concept()
        {
            Intents = new List<string>();
            Extents = new List<string>();
            ChildList = new List<Concept>();
        }

        public void Print()
        {
            String outputStr = "";
            outputStr += "(";
            foreach (var extent in Extents)
            {
                outputStr += extent + " ";
            }
            outputStr += ") (";

            foreach (var intent in Intents)
            {
                outputStr += intent + " ";
            }
            outputStr += ")";

            Console.WriteLine(outputStr);
        }

    }



}
