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
    public partial class EditCombinedForm : Form
    {
        private Combined subject;
        private EditQuantForm editQuantForm;

        public EditCombinedForm()
        {
            InitializeComponent();
            Subject = new Combined();
            editQuantForm = new EditQuantForm();
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:G3}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index == -1) return;

            var part = subject.parts[index];

            part.quant = editQuantForm.EditQuant(part.quant);
            part.Update();
            Utility.FeedDataToGrid(dataGridView1, part.values);
        }

        public void ClearControls()
        {
            listBox1.SelectedIndex = -1;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            chart1.Series[0].Points.Clear();
        }

        public void UpdateListBox()
        {
            listBox1.Items.Clear();
            foreach (var x in subject.parts)
                listBox1.Items.Add(x.name);
        }

        public void UpdateSubject(bool silent=false)
        {
            try
            {
                listBox1.SelectedIndex = listBox1.SelectedIndex;
                numericUpDown1.Value = subject.quants != null ? subject.quants.Length : 0;
                UpdateListBox();
                Combine();
                Histogram();
            }
            catch (InterfaceException)
            {
                if (!silent) throw;
            }
        }

        public Combined Subject
        {
            get { return subject; }
            set
            {
                subject = value;
                UpdateSubject(true);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index == -1) return;

            var part = subject.parts[index];
            Utility.FeedDataToGrid(dataGridView1, part.values);
        }

        public void Combine()
        {
            subject.Update((int)numericUpDown1.Value);
            Utility.FeedDataToGrid(dataGridView2, subject.quants);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Combine();
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void Histogram()
        {
            var hist = Utility.Histogram(subject.data, (int)numericUpDown2.Value, true);
            chart1.Series[0].Points.Clear();
            foreach (var h in hist)
                chart1.Series[0].Points.AddXY(h.x, h.y);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Histogram();
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearControls();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Combine();
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }
    }
}
