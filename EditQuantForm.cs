using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Antropod.DataGridViewVirtual;

namespace Antropod.MatrixMethod
{
    public partial class EditQuantForm : Form
    {
        public Quant[] avaialibleQuants = new Quant[]
        {
            new UniformQuant(),
            new NormalQuant(),
            new UniformBiQuant(),
            new NormalBiQuant(),
            new UserTypedQuant()
        };

        public Quant editingQuant;
        double[] quantData;

        public EditQuantForm()
        {
            InitializeComponent();
            InitComboboxItems();
        }

        public void InitComboboxItems()
        {
            foreach (var quant in avaialibleQuants)
                comboBox1.Items.Add(quant.GetName());
        }

        public Quant EditQuant(Quant quant)
        {
            Utility.ClearDataGrid(dataGridView1);

            if (quant == null)
                editingQuant = new UniformQuant();
            else
                editingQuant = quant.Clone();

            propertyGrid1.SelectedObject = editingQuant;

            int index = -1;
            for (int i = 0; i < avaialibleQuants.Length; ++i)
            {
                if (editingQuant.GetType() == avaialibleQuants[i].GetType())
                {
                    index = i;
                    break;
                }
            }
            comboBox1.SelectedIndex = index;

            var result = ShowDialog();


            if (result == DialogResult.OK)
                return editingQuant;
            return quant;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = comboBox1.SelectedIndex;
                if (index == -1) return;
                var quant = avaialibleQuants[index];
                if (editingQuant.GetType() == quant.GetType()) return;
                editingQuant = avaialibleQuants[index].Clone();
                propertyGrid1.SelectedObject = editingQuant;
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                quantData = editingQuant.GetQuants();
                Utility.FeedDataToGrid(dataGridView1, quantData);
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                quantData = editingQuant.GetQuants();
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
                DialogResult = DialogResult.None;
            }
            
        }
    }
}
