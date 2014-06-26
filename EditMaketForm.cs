using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Antropod.MatrixMethod
{
    public partial class EditMaketForm : Form
    {
        Maket _data;

        private Maket Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;

                if (value == null)
                {
                    dataGridView1.DataSource = null;
                    dataGridView2.DataSource = null;
                }
                else
                {
                    dataGridView1.DataSource = _data.Types;
                    dataGridView2.DataSource = _data.Components;
                }
            }
        }


        public EditMaketForm()
        {
            InitializeComponent();

            Data = Maket.CreateDefault();

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public Maket EditSubject(Maket subject)
        {
            Data = (subject == null) ? Maket.CreateDefault() : subject;
            var dialogResult = ShowDialog();
            return Data;
        }

        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            Data = Maket.CreateDefault();
        }
    }

    [Serializable]
    public class Maket
    {
        public BindingList<StringValue> Types { get; set; }
        public BindingList<StringIntValue> Components { get; set; }

        public Maket()
        {
            Types = new BindingList<StringValue>();
            Components = new BindingList<StringIntValue>();
        }

        public Maket(Maket src)
        {
            this.Types = new BindingList<StringValue>(src.Types);
            this.Components = new BindingList<StringIntValue>(src.Components);
        }

        public Maket Clone()
        {
            throw new NotImplementedException();
            // TODO: make clone really work
            //return new Maket(this);
        }

        public static Maket CreateDefault()
        {
            var result = new Maket();

            result.Types.Add(new StringValue("Резистор"));
            result.Types.Add(new StringValue("Конденсатор"));

            result.Components.Add(new StringIntValue("R1", 0));
            result.Components.Add(new StringIntValue("R2", 0));
            result.Components.Add(new StringIntValue("R3", 0));
            result.Components.Add(new StringIntValue("C1", 1));

            return result;
        }

        public IEnumerable<string> TypeNames()
        {
            foreach (var entry in Types)
            {
                yield return entry.Name;
            }
        }

        public IEnumerable<string> ComponentNames()
        {
            foreach (var entry in Components)
            {
                yield return entry.Key;
            }
        }

        public IEnumerable<int> ComponentIndices()
        {
            foreach (var entry in Components)
            {
                yield return entry.Value;
            }
        }
    }

    [Serializable]
    public class StringValue
    {
        [DisplayName("Тип элемента")]
        public string Name { get; set; }

        public StringValue()
        {}

        public StringValue(string name)
        {
            Name = name;
        }
    }

    [Serializable]
    public class StringIntValue
    {
        [DisplayName("Название элемента")]
        public string Key { get; set; }

        [DisplayName("Номер в списке типов")]
        public int Value { get; set; }

        public StringIntValue()
        {}

        public StringIntValue(string key, int value)
        {
            Key = key;
            Value = value;
        }
    }
}
