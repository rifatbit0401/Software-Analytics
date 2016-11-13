using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDAProject;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private List<Node> nodes = new List<Node>();
        private int _heightUnit = 100;
        private int _totalWidth = 1000;
        int rectHeight = 100;
        int rectWidth = 100;
        // private int numSelectedRect = 0;
        private List<NodeConcept> _nodeConcepts = new List<NodeConcept>();
        private Controller _controller;

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {

            _controller = new Controller();
            _controller.Initiate();
            IConceptLattice conceptLattice = new Algorithm();
            /*string fcaFilePath = @"E:\MSSE Program\MSSE 2nd Semester\Software Analytics\Code\my_input.txt";
            string latticeFilePath = @"E:\MSSE Program\MSSE 2nd Semester\Software Analytics\Code\lattice_input.txt";
            var concepts = conceptLattice.ConstructConceptLattice(fcaFilePath, latticeFilePath);
             */
            var concepts = conceptLattice.ConstructConceptLattice(_controller.FCAFilePath, _controller.LatticeFilePath);
            nodes = ConvertToNodes(concepts).Values.ToList();
            AddLabel(nodes[0]);

            int maxLevel = nodes.Max(node => node.Level);

            for (int i = 0; i <= maxLevel; i++)
            {
                var sameLevelNodes = nodes.Where(node => node.Level.Equals(i)).ToArray();
                for (int j = 0; j < sameLevelNodes.Count(); j++)
                {
                    Node nd = sameLevelNodes[j];
                    nd.X = this.Size.Width / sameLevelNodes.Count() * (j);
                    nd.Y = 150 * i;
                }
            }

            foreach (var node in nodes)
            {
                DrawIt(node);
                foreach (var child in node.ChildNodes)
                {
                    DrawEdge(node, child);
                }
            }

        }

        private Dictionary<Concept, Node> ConvertToNodes(List<Concept> concepts)
        {
            var map = new Dictionary<Concept, Node>();
            foreach (var concept in concepts)
            {
                String txt = "(";
                foreach (var intent in concept.Intents)
                {
                    txt = txt + intent + " ";
                }
                txt += ")\n(";
                foreach (var extent in concept.Extents)
                {
                    txt = txt + extent + " ";
                }
                txt += ")";
                Node node = new Node { Text = txt };
                map[concept] = node;
                _nodeConcepts.Add(new NodeConcept { Concept = concept, Node = node });
            }

            foreach (var concept in concepts)
            {
                foreach (var childConcept in concept.ChildList)
                {
                    map[concept].ChildNodes.Add(map[childConcept]);
                }
            }
            // return map.Values.ToList();
            return map;
        }

        private void DrawEdge(Node parent, Node child)
        {
            Graphics graphics = this.CreateGraphics();
            float x1 = parent.X + rectHeight / 2;
            float y1 = parent.Y + rectWidth;
            float x2 = child.X + rectHeight / 2;
            float y2 = child.Y;
            graphics.DrawLine(Pens.Brown, x1, y1, x2, y2);
        }

        private void DrawIt(Node node)
        {
            Graphics graphics = this.CreateGraphics();
            Rectangle rectangle = new Rectangle(node.X, node.Y, rectHeight, rectWidth);
            TextFormatFlags flags = TextFormatFlags.WordBreak;
            TextRenderer.DrawText(graphics, node.Text, new Font("Arial", 10), rectangle, Color.Blue, flags);
            //graphics.DrawString(node.Text,new Font("Arial",10), Brushes.Blue, rectangle);
            // graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
            graphics.DrawRectangle(System.Drawing.Pens.Red, rectangle);

        }

        public void AddLabel(Node root)
        {
            root.Level = 0;
            var stack = new Stack<Node>();
            stack.Push(root);

            while (stack.Count != 0)
            {
                var parentNode = stack.Pop();
                foreach (var childNode in parentNode.ChildNodes)
                {
                    childNode.Level = parentNode.Level + 1;
                    stack.Push(childNode);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int parentNodeId = int.Parse(textBox1.Text)-1;
            int childNodeId = int.Parse(textBox2.Text)-1;
            textBox1.Clear();
            textBox2.Clear();
            saveFileDialog1.ShowDialog();
            var filePath = saveFileDialog1.FileName;
            new ARFFileWriter().WriteARFFile(filePath
                                             , _nodeConcepts.ToArray()[parentNodeId].Concept,
                                             _nodeConcepts.ToArray()[childNodeId].Concept,
                                             _controller._transactions, _controller._logModels);

        }


        /* private Rectangle SelectedRectangle1;
         private Rectangle SelectedRectangle2;
        */
        /* private void Form1_Click(object sender, EventArgs e)
       {
           Point cursorPos = this.PointToClient(Cursor.Position);
           foreach (var node in nodes)
           {
               var rectangle = new Rectangle(node.X, node.Y, rectWidth, rectHeight);
               if(rectangle.Contains(cursorPos))
               {
                   this.CreateGraphics().FillRectangle(Brushes.CadetBlue,rectangle);
                   numSelectedRect++;
                   SelectedRectangle1 = rectangle;

               }
               if(numSelectedRect%2==0)
               {
                   CreateGraphics().FillRectangle(Brushes.White, SelectedRectangle1);
                    
               }
           }
       }*/

    }

    public class NodeConcept
    {
        public Node Node { get; set; }
        public Concept Concept { get; set; }
    }

    public class Node
    {
        public string Text { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Level { get; set; }
        public List<Node> ChildNodes { get; set; }
        public Node()
        {
            ChildNodes = new List<Node>();
        }
    }


    public class Controller
    {
        private ILogProcessor _logProcessor;
        private IPythonScriptRunner _pythonScriptRunner;
        private FileConversionUtility _fileConversionUtility;
        private IConceptLattice _conceptLattice;
        public string LogStatementFilePath { get; set; }
        public string TransactionFilePath { get; set; }
        public string PythonScriptFilePath { get; set; }
        public string PythonInputDataCSVFilePath { get; set; }
        public string PythonInputDataCSVFilePathInUnixFormat { get; set; }
        public string FCAFilePath { get; set; }
        public string LatticeFilePath { get; set; }
        public List<LogModel> _logModels;
        public List<Transaction> _transactions;
        private List<string> _fCAPythonOutput;
        private List<String> _cLPythonOutput;


        public Controller()
        {
            ConfigurePath();
            _logProcessor = new LogProcessor();
            _pythonScriptRunner = new PythonScriptRunner(PythonScriptFilePath);
            _fileConversionUtility = new FileConversionUtility();
            _conceptLattice = new Algorithm();
        }

        public void TestController()
        {
            Initiate();
        }
        public void Initiate()
        {
            _logModels = _logProcessor.ParseLogModels(LogStatementFilePath);
            _transactions = _logProcessor.ParseTransactions(LogStatementFilePath, TransactionFilePath);
            _fileConversionUtility.CreateCSVFile(PythonInputDataCSVFilePath, _transactions, _logModels);
            _fCAPythonOutput = _pythonScriptRunner.RunFCAPythonScript(PythonInputDataCSVFilePathInUnixFormat);
            _cLPythonOutput = _pythonScriptRunner.RunCLPythonScript(PythonInputDataCSVFilePathInUnixFormat);

            File.WriteAllLines(FCAFilePath, _fCAPythonOutput);
            File.WriteAllLines(LatticeFilePath, FilterCLPythonOutput(_cLPythonOutput));
        }

        private void ConfigurePath()
        {
            string basePath = @"I:\ASR Analytics\Analytics\";
            LogStatementFilePath = basePath + "log.txt"; //user given
            TransactionFilePath = basePath + "transaction.json"; //user given and should be in json format

            PythonInputDataCSVFilePath = basePath + "pythoninputdata.csv";
            PythonInputDataCSVFilePathInUnixFormat = "I:/ASR Analytics/Analytics/" + "pythoninputdata.csv";
            FCAFilePath = basePath + "fca.txt";
            LatticeFilePath = basePath + "lattice.txt";
            PythonScriptFilePath = basePath + "python_script.py";
        }

        private List<string> FilterCLPythonOutput(List<string> CLPythonOutput)
        {
            var filteredOutput = CLPythonOutput.Where(line => line.Contains("->"));
            var cLFileInputData = new List<string>();
            foreach (var line in filteredOutput)
            {
                var prunedLine = line.Trim(' ');
                if (line.Contains("["))
                    prunedLine = prunedLine.Remove(line.IndexOf('['));
                prunedLine = prunedLine.Replace("->", ",");
                string row = "";
                foreach (var splittedStr in prunedLine.Split(','))
                {
                    if (splittedStr.Trim(' ') == "")
                        continue;

                    if (row != "")
                        row += " ";
                    row = row + splittedStr.Trim(' ').Trim('\t');
                }
                cLFileInputData.Add(row + "\n");
            }

            return cLFileInputData;
        }


    }


}
