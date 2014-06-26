using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace Antropod.MatrixMethod
{
    [Serializable]
    public abstract class Quant
    {
        public abstract String GetName();
        public virtual double[] GetQuants() { return null; }
        public abstract Quant Clone();
    }

    [Serializable]
    public class UserTypedQuant : Quant
    {
        private List<double> _data;

        public UserTypedQuant()
        {
            _data = new List<double>();
        }

        public UserTypedQuant(UserTypedQuant src)
        {
            _data = new List<double>(src._data);
        }

        public override string GetName()
        {
            return "Пользовательский список";
        }

        public override double[] GetQuants()
        {
            return _data.ToArray();
        }

        public override Quant Clone()
        {
            return new UserTypedQuant(this);
        }


        [Description("Список значений мат. ожиданий.")]
        [DisplayName("Данные")]
        public List<double> Data
        {
            get { return _data; }
        }
    }

    [Serializable]
    public class UniformQuant : Quant
    {
        protected double _low = 0, _high = 0;
        protected int _number = 12;

        public UniformQuant()
        {
        }

        public UniformQuant(UniformQuant src)
        {
            this._low = src._low;
            this._high = src._high;
            this._number = src._number;
        }

        public override Quant Clone()
        {
            return new UniformQuant(this);
        }

        [Description("Нижняя граница поля допуска")]
        [DisplayName("Дн")]
        public double Low
        {
            get { return _low; }
            set { _low = value; }
        }


        [Description("Верхняя граница поля допуска")]
        [DisplayName("Дв")]
        public double High
        {
            get { return _high; }
            set { _high = value; }
        }

        [Description("Количество квантов")]
        [DisplayName("N")]
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public override String GetName()
        {
            return "Полосовой равновероятный";
        }

        protected virtual double[] uniformQuant(double low, double high, int number)
        {
            if (high == low) throw new InterfaceException("Верхняя и нижняя граница должны различаться");
            if (number <= 0) throw new InterfaceException("Количество интервалов должно быть больше 0");

            var step = (high - low) / number;
            var M = new double[number];
            for (int i = 0; i < number; ++i)
            {
                M[i] = low + step * (i + 0.5);
            }
            return M;
        }

        public override double[] GetQuants()
        {
            var M = uniformQuant(Low, High, Number);
            Array.Sort(M);
            return M;
        }
    }

    [Serializable]
    public class UniformBiQuant : UniformQuant
    {
        public UniformBiQuant()
            : base()
        {}

        public UniformBiQuant(UniformBiQuant src)
        {
            this._low = src._low;
            this._high = src._high;
            this._number = src._number;
        }

        public override Quant Clone()
        {
            return new UniformBiQuant(this);
        }

        public override String GetName()
        {
            return "Двухполосный равновероятный";
        }

        protected override double[] uniformQuant(double low, double high, int number)
        {
            if (number % 2 != 0) throw new InterfaceException("n должно быть четным");
            if (Math.Sign(low) != Math.Sign(high)) throw new InterfaceException("Знак Дн и Дв должен быть одинаковым");

            var M1 = base.uniformQuant(low, high, number / 2);
            var M2 = base.uniformQuant(-high, -low, number / 2);

            var M = new double[M1.Length + M2.Length];
            M1.CopyTo(M, 0);
            M2.CopyTo(M, M1.Length);
            return M;
        }
    }

    [Serializable]
    public class NormalQuant : UniformQuant
    {
        public NormalQuant()
        {
        }

        public NormalQuant(NormalQuant src)
        {
            this._low = src._low;
            this._high = src._high;
            this._number = src._number;
            this._loc = src._loc;
            this._scale = src._scale;
        }

        public override Quant Clone()
        {
            return new NormalQuant(this);
        }

        public override String GetName()
        {
            return "Полосовой нормальный";
        }

        protected double _loc = 0, _scale = 1;

        [Description("Мат. ожидание")]
        [DisplayName("MО")]
        public double Loc
        {
            get { return _loc; }
            set { _loc = value; }
        }

        [Description("Среднеквадратическое отклонение")]
        [DisplayName("СКО")]
        public double Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        protected virtual double[] normalQuant(double low, double high, double loc, double scale, int number)
        {
            if (high == low) throw new InterfaceException("Верхняя и нижняя граница должны различаться");
            if (number <= 0) throw new InterfaceException("Количество интервалов должно быть больше 0");

            var rv = new Normal(loc, scale);
            var Plow = rv.CumulativeDistribution(low);
            var Phigh = rv.CumulativeDistribution(high);
            var P = Phigh - Plow;
            var Pkv = P / number;
            var M = new double[number];
            for (int i = 0; i < number; ++i)
            {
                var Pi = Pkv * (i + .5) + Plow;
                M[i] = rv.InverseCumulativeDistribution(Pi);
            }
            return M;
        }

        public override double[] GetQuants()
        {
            var M = normalQuant(Low, High, Loc, Scale, Number);
            Array.Sort(M);
            return M;
        }
    }

    [Serializable]
    public class NormalBiQuant : NormalQuant
    {
        public NormalBiQuant()
            : base()
        { }

        public NormalBiQuant(NormalBiQuant src)
        {
            this._low = src._low;
            this._high = src._high;
            this._number = src._number;
            this._loc = src._loc;
            this._scale = src._scale;
        }

        public override Quant Clone()
        {
            return new NormalBiQuant(this);
        }

        public override String GetName()
        {
            return "Двухполосный нормальный";
        }

        protected override double[] normalQuant(double low, double high, double loc, double scale, int number)
        {
            if (number % 2 != 0) throw new InterfaceException("Количество квантов должно быть четным");
            if (Math.Sign(low) != Math.Sign(high)) throw new InterfaceException("Знак Дн и Дв должен быть одинаковым");

            var M1 = base.normalQuant(low, high, loc, scale, number / 2);
            var M2 = base.normalQuant(-high, -low, loc, scale, number / 2);

            var M = new double[M1.Length + M2.Length];
            M1.CopyTo(M, 0);
            M2.CopyTo(M, M1.Length);
            return M;
        }
    }

    [Serializable]
    public class Part
    {
        public Quant quant;
        public string name;
        public double[] values;

        public Part(string name)
        {
            this.name = name;
        }

        public void Update()
        {
            values = quant != null ? quant.GetQuants() : null;
        }
    }

    [Serializable]
    public class Combined
    {
        public Part[] parts;
        public string name;
        public double[] data;
        public double[] quants;

        public Combined(string name="")
        {
            this.name = name;
            this.parts = new Part[3]
            {
                new Part("Производственная"),
                new Part("Температурная"),
                new Part("Старения")
            };
        }

        public void Update(int number)
        {
            double[][] values = new double[parts.Length][];
            for (int i = 0; i < values.Length; ++i)
            {
                values[i] = parts[i].values;
            }

            data = Utility.RowSum(Utility.Product(values));
            Array.Sort(data);
            quants = Utility.EvenStride(data, number);
        }
    }

    [Serializable]
    public class Final
    {
        public Combined[] parts;
        public Maket maket { get; set; }

        public Final(Maket maket)
        {
            var partsList = new List<Combined>();
            this.maket = maket;
            foreach(var partName in maket.TypeNames())
            {
                partsList.Add(new Combined(partName));
            }
            parts = partsList.ToArray();
        }

        public double[,] UpdateMatrix(int[] indices)
        {
            double[][] values = new double[indices.Length][];
            for (int i = 0; i < values.Length; ++i)
                values[i] = parts[indices[i]].quants;

            return Utility.Product(values);
        }
    }

    public struct NamedIndexList
    {
        public string[] names;
        public int[] indices;
    }
}
