using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Antropod.MathParser;

namespace Antropod.MatrixMethod
{

    [Serializable]
    public class Stat
    {
        [NonSerialized]
        public double[,] matrix;

        [NonSerialized]
        public double[,] newMatrix;

        [NonSerialized]
        public double[] calculatedData;

        [NonSerialized]
        public int total;

        [NonSerialized]
        public int count;

        [NonSerialized]
        public double probability;

        private List<ElementNominal> elements = new List<ElementNominal>();

        [DisplayName("Номиналы")]
        [Description("Номиналы элементов схемы")]
        public ReadOnlyCollection<ElementNominal> Elements
        {
            get { return elements.AsReadOnly(); }
        }

        [DisplayName("Выражение")]
        [Description("Выражение для вычисления входного параметра")]
        public string Expression { get; set; }
        //public double RNominal { get; set; }
        [DisplayName("Нижняя граница")]
        [Description("Нижняя граница поля допуска для выходного параметра, %")]
        public double LowPrecent { get; set; }

        [DisplayName("Верхняя граница")]
        [Description("Верхняя граница поля допуска для выходного параметра, %")]
        public double HighPrecent { get; set; }

        [NonSerialized]
        private double _nominal = 0;
        [DisplayName("Номинал")]
        [Description("Номинальное значение выходного параметра")]
        public double Nominal
        {
            get { return _nominal; }
        }

        [NonSerialized]
        private double _lowValue = 0;
        [DisplayName("Нижнее значение")]
        [Description("Нижняя граница поля допуска для выходного параметра, в единицах")]
        public double LowValue
        {
            get { return _lowValue; }
        }

        [NonSerialized]
        private double _highValue = 0;
        [DisplayName("Верхнее значение")]
        [Description("Верхняя граница поля допуска для выходного параметра, в единицах")]
        public double HighValue
        {
            get { return _highValue; }
        }

        [NonSerialized]
        private int _within = 0;
        [DisplayName("Попаданий")]
        [Description("Количество значений, попавших в поле допуска")]
        public double Within
        {
            get { return _within; }
        }

        [NonSerialized]
        private int _total = 0;
        [DisplayName("Всего")]
        [Description("Всего значений")]
        public double Total
        {
            get { return _total; }
        }

        [NonSerialized]
        private double _probability = 0;
        [DisplayName("Вероятность")]
        [Description("Отношение Попаданий / Всего")]
        public double Probability
        {
            get { return _probability; }
        }

        public Stat(IEnumerable<string> names)
        {
            Expression = "";
            LowPrecent = -5;
            HighPrecent = 5;
            foreach (var name in names)
                elements.Add(new ElementNominal(name, 0));
        }

        public void Recalculate(double[,] matrix)
        {
            newMatrix = new double[matrix.GetLength(0), matrix.GetLength(1) + 1];
            calculatedData = new double[matrix.GetLength(0)];

            for (int i = 0; i < matrix.GetLength(0); ++i)
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    newMatrix[i, j] = (1 + matrix[i, j] / 100) * elements[j].Value;

            int count = 0;
            var parser = new MathParser.MathParser(Expression);

            for (int i = 0; i < elements.Count; ++i)
            {
                parser.variables[elements[i].Name] = elements[i].Value;
            }

            var nominal = parser.Parse();
            var low = nominal * (1 + LowPrecent / 100);
            var high = nominal * (1 + HighPrecent / 100);

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < elements.Count; ++j)
                {
                    parser.variables[elements[j].Name] = newMatrix[i, j];
                }
                var result = parser.Parse();
                newMatrix[i, matrix.GetLength(1) + 0] = result;
                bool within = (result >= low) && (result <= high);
                if (within) count++;
                calculatedData[i] = result;
            }

            _nominal = nominal;
            _total = matrix.GetLength(0);
            _within = count;
            _probability = ((double)count / (double)_total);
            _lowValue = low;
            _highValue = high;
        }
    }

    [Serializable]
    public class ElementNominal
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }
        public double Value { get; set; }

        public ElementNominal(string name, double value)
        {
            _name = name;
            Value = value;
        }
    }
}
