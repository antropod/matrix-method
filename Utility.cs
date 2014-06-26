using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Antropod.MatrixMethod
{
    class Utility
    {
        public static void ClearDataGrid(DataGridView dataGridView)
        {
            dataGridView.Rows.Clear();
        }

        public static void FeedDataToGrid(DataGridView dataGridView, double[] data)
        {
            dataGridView.ColumnCount = 1;
            dataGridView.Rows.Clear();
            if (data == null)
                return;
            dataGridView.RowCount = data.Length;
            for (int i = 0; i < data.Length; ++i)
                dataGridView.Rows[i].SetValues(data[i]);
        }

        public static double[] RowSum(double[,] arr)
        {
            var result = new double[arr.GetLength(0)];
            for (int i = 0; i < arr.GetLength(0); ++i)
            {
                result[i] = 0;
                for (int j = 0; j < arr.GetLength(1); ++j)
                    result[i] += arr[i, j];
            }
            return result;
        }

        public static double[,] Product(double[][] values)
        {
            int numCols = values.Length;
            int[] lengths = new int[numCols];
            int numRows = 1;
            for (int i = 0; i < values.Length; ++i)
            {
                if (values[i] == null) throw new InterfaceException("Недостаточно данных");
                var len = values[i].Length;
                numRows *= len;
                lengths[i] = len;
            }

            var cmul = new int[numCols];
            var cprod = 1;
            for (int i = 0; i < numCols; ++i)
            {
                cmul[i] = cprod;
                cprod *= lengths[i];
            }

            var result = new double[numRows, numCols];

            for (int i = 0; i < numRows; ++i)
                for (int j = 0; j < numCols; ++j)
                {
                    var x = (i / cmul[j]) % lengths[j];
                    result[i, j] = values[j][x];
                }
            
            return result;
        }

        public struct Point
        {
            public double x, y;
            public string toString()
            {
                return String.Format("({0}, {1})", x, y);
            }
        }

        public static Point[] Histogram(double[] vdata, int nbins, bool normalized = false)
        {
            if (vdata == null) throw new InterfaceException("vdata must be not null");
            if (nbins < 1) throw new InterfaceException("Количество интервалов должно быть не меньше 1");

            var data = new double[vdata.Length];
            Array.Copy(vdata, data, data.Length);
            Array.Sort(data);
            double lowest = data[0];
            double highest = data[data.Length - 1];
            double size = (highest - lowest) / (nbins - 1);
            double leftmost = lowest - size / 2;

            double[] borders = new double[nbins+1];
            for (int i = 0; i < borders.Length; ++i)
            {
                borders[i] = leftmost + size * i;
            }

            int[] csum = new int[nbins + 1];
            for (int i = 0; i < data.Length; ++i)
            {
                for (int j = 0; j < borders.Length; ++j )
                {
                    if (data[i] < borders[j]) csum[j]++;
                }
            }

            var result = new Point[nbins];
            for (int i = 0; i < result.Length; ++i )
            {
                result[i].y = csum[i + 1] - csum[i];
                result[i].x = (borders[i] + borders[i + 1]) / 2;
            }

            if (normalized)
                for (int i = 0; i < result.Length; ++i)
                    result[i].y /= data.Length;

             return result;
        }


        public static double[] EvenStride(double[] data, int number)
        {
            if (data == null) throw new InterfaceException("Необходимы данные");
            if (number <= 0) throw new InterfaceException("Число квантов должно быть больше 0");

            var result = new double[number];

            int blockSize = data.Length / number;
            int halfSize = blockSize / 2;
            bool isEven = (blockSize % 2) == 0;
            for (int i = 0; i < number; ++i)
            {
                int x = i * blockSize + halfSize;
                result[i] = isEven ? (data[x] + data[x-1]) / 2 : data[x];
            }

            return result;
        }

        public static void ExceptionMessageBox(Exception exc)
        {
            MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Serialize(Object obj, string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
            }
        }

        public static object Deserialize(string filename)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return formatter.Deserialize(stream);
            }
        }

    }

    public class InterfaceException : Exception
    {
        public InterfaceException()
        {}

        public InterfaceException(string message)
            :base(message)
        {}
    }

}
