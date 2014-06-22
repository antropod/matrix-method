using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace matrix_method2
{
    public partial class HistogramForm : Form
    {
        public void SetData(double[] data, int numIntervals)
        {
            chart1.Series[0].Points.Clear();
            if (data == null) return;
            var hist = Utility.Histogram(data, numIntervals, true);
                
            foreach (var h in hist)
                chart1.Series[0].Points.AddXY(h.x, h.y);
        }

        public HistogramForm()
        {
            InitializeComponent();

            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:G3}";
        }

        private void HistogramForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }
    }
}
