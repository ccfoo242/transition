using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.Common
{
    public class Matrix
    {
        public ComplexDecimal[,] Data { get; }

        public int QuantityOfRows => Data.GetLength(0);
        public int QuantityOfColumns => Data.GetLength(1);

        public bool IsSquare => QuantityOfColumns == QuantityOfRows;

        public Matrix(int QuantityOfRows, int QuantityOfColumns)
        {
            Data = new ComplexDecimal[QuantityOfRows, QuantityOfColumns];
        }

        public Matrix(int QuantityOfRowsAndColumns)
        {
            Data = new ComplexDecimal[QuantityOfRowsAndColumns, QuantityOfRowsAndColumns];
        }

        public void Multiply(ComplexDecimal value)
        {
            for (int x = 0; x < QuantityOfRows; x++)
                for (int y = 0; y < QuantityOfColumns; y++)
                    Data[x, y] *= value;
        }

        public Matrix Solve(Matrix bVector)
        {
            //LU

            if (!IsSquare) throw new InvalidOperationException("Matrix non square");
            
            if (bVector.QuantityOfColumns != 1) throw new InvalidOperationException("B Matrix must have one column");

            var L = new Matrix(QuantityOfRows);
            var U = new Matrix(QuantityOfRows);

            var Z = new Matrix(QuantityOfRows, 1);
            var X = new Matrix(QuantityOfRows, 1);

            int n = QuantityOfRows;

            for (int k = 0; k <= (n - 1); k++)
            {
                L.Data[k, k] = 1;
                for (int j = k; j <= (n - 1); j++)
                {
                    U.Data[k, j] = Data[k, j];
                    for (int s = 0; s <= (k - 1); s++)
                        U.Data[k, j] -= L.Data[k, s] * U.Data[s, j];
                }
                for (int i = k + 1; i <= (n - 1); i++)
                {
                    L.Data[i, k] = Data[i, k];
                    for (int s = 0; s <= (k - 1); s++)
                        L.Data[i, k] -= L.Data[i, s] * U.Data[s, k];

                    L.Data[i, k] /= U.Data[k, k];
                }
            }

            Z.Data[0, 0] = bVector.Data[0, 0];

            for (int i = 1; i <= (n - 1); i++)
            {
                Z.Data[i, 0] = bVector.Data[i, 0];
                for (int j = 0; j <= (i - 1); j++)
                    Z.Data[i, 0] -= L.Data[i, j] * Z.Data[j, 0];
            }

            X.Data[n - 1, 0] = Z.Data[n - 1, 0] / U.Data[n - 1, n - 1];

            for (int i = n - 2; i >= 0; i--)
            {
                X.Data[i, 0] = Z.Data[i, 0];
                for (int j = i + 1; j <= (n - 1); j++)
                    X.Data[i, 0] -= U.Data[i, j] * X.Data[j, 0];
                X.Data[i, 0] /= U.Data[i, i];

            }

            return X;
        }

        public override string ToString()
        {
            string output = "";

            for (int x = 0; x < QuantityOfRows; x++)
            {
                for (int y = 0; y < QuantityOfColumns; y++)
                {
                    output += Data[x, y].ToString();
                    if (y < (QuantityOfColumns - 1))
                        output += " , ";
                    else
                        output += " || ";
                }
                output += Environment.NewLine;
            }

            return output;

        }

    }
}
