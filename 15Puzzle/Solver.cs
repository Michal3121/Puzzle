using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _15Puzzle
{
    

    public class Solver
    {
        private List<MatrixNode> OpenList = new List<MatrixNode>();
        private List<MatrixNode> ClosedList = new List<MatrixNode>();
        private List<MatrixNode> nodeCurrentAllSucc;

        private MatrixNode matrixStart;
        private MatrixManager matrixManager = new MatrixManager();

        public MatrixNode MatrixStart
        {
            get
            {
                return matrixStart;
            }
            set
            {
                matrixStart = value;
            }
        }

        public Solver(MatrixNode matrixStart)
        {
            this.matrixStart = matrixStart;
            OpenList.Add(this.matrixStart);
        }

        public void FindSolution()
        {
            int currentValueFuncG = 0;
            int aux = 0;
            while (this.OpenList.Any()) // false ak neobsahuje
            {
                Console.WriteLine(aux + ". prechod -----------------------------------------");
                nodeCurrentAllSucc = new List<MatrixNode>();
                currentValueFuncG++;
                MatrixNode nodeCurrent = new MatrixNode(this.OpenList.First().Matrix);
                nodeCurrent.FunctionG = this.OpenList.First().FunctionG;
                if (nodeCurrent.getDistanceH() == 0)
                {
                    Console.WriteLine("Solution has been found");
                    return;
                }
                this.ClosedList.Add(this.OpenList.First());
                this.OpenList.Remove(nodeCurrent);

                nodeCurrentAllSucc.AddRange(this.matrixManager.getAllNodeSuccessors(nodeCurrent));

                nodeCurrentAllSucc.ForEach(e => e.FunctionG = nodeCurrent.FunctionG + 1);
                nodeCurrentAllSucc.OrderBy(e => e.FunctionF);

                /*
                Console.WriteLine("ListCurrentSucc size " + nodeCurrentAllSucc.Count);
                nodeCurrentAllSucc.ForEach(e =>
                {
                    this.matrixManager.ActualMatrixToString(e);  // pomocny vypis matic v liste
                    Console.WriteLine("F: " + e.FunctionF);
                    Console.WriteLine("G: " + e.FunctionG);
                    Console.Write(Environment.NewLine);
                });
                */

                nodeCurrentAllSucc.ForEach(e =>
                {
                    if ((!OpenList.Contains(e)) && (!ClosedList.Contains(e)))
                    {
                        OpenList.Add(e);
                    }
                });

                List<MatrixNode> sortedOpenList = OpenList.OrderBy(e => e.FunctionF).ToList();
                OpenList = sortedOpenList;
                /*
                Console.WriteLine("OpenListCurrent");
                OpenList.ForEach(e =>
                {
                    this.matrixManager.ActualMatrixToString(e);
                    Console.WriteLine("F: " + e.FunctionF);
                    Console.WriteLine("G: " + e.FunctionG);
                    Console.Write(Environment.NewLine);
                });

                Console.WriteLine("ClosedListCurrent");
                ClosedList.ForEach(e =>
                {
                    this.matrixManager.ActualMatrixToString(e);
                    Console.WriteLine("F: " + e.FunctionF);
                    Console.WriteLine("G: " + e.FunctionG);
                    Console.Write(Environment.NewLine);
                });
                */
                Console.WriteLine("NodeCurrentFirst:");
                this.matrixManager.ActualMatrixToString(OpenList.First()); /*
                Console.WriteLine("F: " + OpenList.First().FunctionF);
                Console.WriteLine("G: " + OpenList.First().FunctionG);

                Console.WriteLine("OpenList size " + OpenList.Count);
                Console.WriteLine("CloseList size " + ClosedList.Count);
                Console.WriteLine("CurrentValueG " + currentValueFuncG);*/
                aux++;
            }

        }
    }
}
