using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15Puzzle
{
    public class MatrixNode
    {
        private int[,] _goalMatrix;
        private int[,] _matrix;
        private int _functionG;

        public int[,] Matrix
        {
            get { return this._matrix; }
        }

        public int[,] GoalMatrix
        {
            get { return this._goalMatrix; }
            set { this._goalMatrix = value; }
        }

        public int FunctionG
        {
            get { return this._functionG; }
            set { this._functionG = value; }
        }

        public int FunctionF
        {
            get { return this.getDistanceH() + this._functionG; }
        }

        public MatrixNode(int[,] matrix)
        {
            _goalMatrix = new int[4, 4] { { 0, 1, 2, 3 }, { 4, 5, 6, 7 }, { 8, 9, 10, 11 }, { 12, 13, 14, 15 } };
            this._matrix = (int[,])matrix.Clone();
            _functionG = 0;
        }


        public int getDistanceH()
        {
            int rowLength = _matrix.GetLength(0);
            int colLength = _matrix.GetLength(1);
            int distance = 0;

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (_matrix[i, j] != _goalMatrix[i, j] && _matrix[i, j] != 15)
                    {
                        distance++;

                    }
                }
            }
            return distance;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is MatrixNode))
            {
                return false;
            }

            MatrixNode otherMatrix = (MatrixNode)obj;

            int rowLength = otherMatrix.Matrix.GetLength(0);
            int colLength = otherMatrix.Matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (!(otherMatrix.Matrix[i, j] == this.Matrix[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 7;

            int rowLength = _matrix.GetLength(0);
            int colLength = _matrix.GetLength(1);

            for (int i = 0; i < colLength - 1; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    hash = hash * 37 + _matrix[i, j];
                }
            }

            return hash;
        }
    }

    public class MatrixManager
    {
        public int[,] FillMatrixWithRandomNumbers(int colLength, int rowLength)
        {
            int[,] randomMatrix = new int[4, 4];
            Random randNum = new Random();
            int aux = 0;
            this.InitMatrix(-1, randomMatrix);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    aux = randNum.Next(0, 16);
                    while (this.NumberExist(aux, randomMatrix))
                    {
                        aux = randNum.Next(0, 16);
                    }
                    randomMatrix[i, j] = aux;
                }
            }

            //return randomMatrix;
            return new int[4, 4] { { 4, 8, 3, 7 }, { 9, 15, 5, 2 }, { 12, 1, 13, 11 }, { 14, 0, 10, 6 } };
        }

        private bool NumberExist(int number, int[,] matrix)
        {
            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (matrix[i, j] == number)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void InitMatrix(int number, int[,] matrix)
        {
            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    matrix[i, j] = number;
                }
            }
        }


        public List<MatrixNode> getAllNodeSuccessors(MatrixNode currentMatrixNode)
        {
            List<MatrixNode> successors = new List<MatrixNode>();

            if (this.CanNodeMoveDown(currentMatrixNode))
            {
                successors.Add(this.FindNodeDownSuccessor(currentMatrixNode));
            }

            if (this.CanNodeMoveUp(currentMatrixNode))
            {
                successors.Add(this.FindNodeUpSuccessor(currentMatrixNode));
            }

            if (this.CanNodeMoveRight(currentMatrixNode))
            {
                successors.Add(this.FindNodeRightSuccessor(currentMatrixNode));
            }

            if (this.CanNodeMoveLeft(currentMatrixNode))
            {
                successors.Add(this.FindNodeLeftSuccessor(currentMatrixNode));
            }

            return successors;
        }

        public bool CanNodeMoveDown(MatrixNode currentMatrixNode)
        {
            int rowLength = currentMatrixNode.Matrix.GetLength(0);
            int colLength = currentMatrixNode.Matrix.GetLength(1);

            for (int i = 0; i < colLength - 1; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (currentMatrixNode.Matrix[i + 1, j] == 15)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanNodeMoveUp(MatrixNode currentMatrixNode)
        {
            int rowLength = currentMatrixNode.Matrix.GetLength(0);
            int colLength = currentMatrixNode.Matrix.GetLength(1);

            for (int i = 1; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (currentMatrixNode.Matrix[i - 1, j] == 15)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanNodeMoveRight(MatrixNode currentMatrixNode)
        {
            int rowLength = currentMatrixNode.Matrix.GetLength(0);
            int colLength = currentMatrixNode.Matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 1; j < rowLength; j++)
                {
                    if (currentMatrixNode.Matrix[i, j - 1] == 15)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanNodeMoveLeft(MatrixNode currentMatrixNode)
        {
            int rowLength = currentMatrixNode.Matrix.GetLength(0);
            int colLength = currentMatrixNode.Matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength - 1; j++)
                {
                    if (currentMatrixNode.Matrix[i, j + 1] == 15)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public MatrixNode FindNodeDownSuccessor(MatrixNode currentMatrixNode)
        {
            MatrixNode nodeSuccessor = new MatrixNode(currentMatrixNode.Matrix);
            int rowLength = nodeSuccessor.Matrix.GetLength(0);
            int colLength = nodeSuccessor.Matrix.GetLength(1);

            for (int i = 0; i < colLength - 1; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (nodeSuccessor.Matrix[i + 1, j] == 15)
                    {
                        nodeSuccessor.Matrix[i + 1, j] = nodeSuccessor.Matrix[i, j];
                        nodeSuccessor.Matrix[i, j] = 15;
                        return nodeSuccessor;
                    }
                }
            }
            return nodeSuccessor;
        }

        public MatrixNode FindNodeUpSuccessor(MatrixNode currentMatrixNode)
        {
            MatrixNode nodeSuccessor = new MatrixNode(currentMatrixNode.Matrix);
            int rowLength = nodeSuccessor.Matrix.GetLength(0);
            int colLength = nodeSuccessor.Matrix.GetLength(1);

            for (int i = 1; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (nodeSuccessor.Matrix[i - 1, j] == 15)
                    {
                        nodeSuccessor.Matrix[i - 1, j] = nodeSuccessor.Matrix[i, j];
                        nodeSuccessor.Matrix[i, j] = 15;
                        return nodeSuccessor;
                    }
                }
            }
            return nodeSuccessor;
        }

        public MatrixNode FindNodeRightSuccessor(MatrixNode currentMatrixNode)
        {
            MatrixNode nodeSuccessor = new MatrixNode(currentMatrixNode.Matrix);
            int rowLength = nodeSuccessor.Matrix.GetLength(0);
            int colLength = nodeSuccessor.Matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 1; j < rowLength; j++)
                {
                    if (nodeSuccessor.Matrix[i, j - 1] == 15)
                    {
                        nodeSuccessor.Matrix[i, j - 1] = nodeSuccessor.Matrix[i, j];
                        nodeSuccessor.Matrix[i, j] = 15;
                        return nodeSuccessor;
                    }
                }
            }
            return nodeSuccessor;
        }

        public MatrixNode FindNodeLeftSuccessor(MatrixNode currentMatrixNode)
        {
            MatrixNode nodeSuccessor = new MatrixNode(currentMatrixNode.Matrix);
            int rowLength = nodeSuccessor.Matrix.GetLength(0);
            int colLength = nodeSuccessor.Matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength - 1; j++)
                {
                    if (nodeSuccessor.Matrix[i, j + 1] == 15)
                    {
                        nodeSuccessor.Matrix[i, j + 1] = nodeSuccessor.Matrix[i, j];
                        nodeSuccessor.Matrix[i, j] = 15;
                        return nodeSuccessor;
                    }
                }
            }
            return nodeSuccessor;
        }

        public void ActualMatrixToString(MatrixNode matrixNode)
        {
            int rowLength = matrixNode.Matrix.GetLength(0);
            int colLength = matrixNode.Matrix.GetLength(1);

            for (int i = 0; i < colLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (matrixNode.Matrix[i, j] != 15)
                    {
                        Console.Write(String.Format("{0} ", matrixNode.Matrix[i, j]));
                    }
                    else
                    {
                        Console.Write("X_CanvasPosition ");
                    }
                }
                Console.Write(Environment.NewLine);
            }
        }

    }
}
