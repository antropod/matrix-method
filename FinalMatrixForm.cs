using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Antropod.DataGridViewVirtual;
using Antropod;
using System.IO;
using System.Globalization;

namespace Antropod.MatrixMethod
{
    public partial class FinalMatrixForm : Form
    {
        private HistogramForm histogramForm;

        private ApplicationData appData;

        //private Final subject;
        private double[,] matrix;

        private const string FILE_NAME = "program.dat";
        private const string DIALOG_FILTER = "Файлы MX|*.mx";
        private EditCombinedForm editCombinedForm;
        private TwoDimensionalArrayView dataView;

        public FinalMatrixForm()
        {
            InitializeComponent();
            dataView = new TwoDimensionalArrayView(dataGridView2, null);
            editCombinedForm = new EditCombinedForm();
            histogramForm = new HistogramForm();

            appData = ApplicationData.LoadFromFile(FILE_NAME);
            appData.PropertyChanged += appData_PropertyChanged;
            appData.NotifyChanged("");
        }

        void appData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var part in appData.final.parts)
                listBox1.Items.Add(part.name);

            dataView.Data = null;
            matrix = null;
            propertyGrid1.SelectedObject = appData.stat;
        }

        public void MakeMatrix()
        {
            dataView.Data = null;
            var indices = appData.final.maket.ComponentIndices().ToArray();
            matrix = appData.final.UpdateMatrix(indices);
            dataView.Data = matrix;
            var names = appData.final.maket.ComponentNames().ToArray();
            for (int i = 0; i < names.Length; ++i)
                dataGridView2.Columns[i].HeaderText = names[i];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MakeMatrix();
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void Save()
        {
            Utility.Serialize(appData, FILE_NAME);
        }

        private void FinalMatrixForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Save(); 
        }

        private void changeComponent_Click(object sender, EventArgs e)
        {
            try
            {
                int index = listBox1.SelectedIndex;
                if (index == -1) throw new InterfaceException("Выберите компонент");
                var part = appData.final.parts[index];
                editCombinedForm.ClearControls();
                editCombinedForm.Subject = part;
                var result = editCombinedForm.ShowDialog();
                //if (result == DialogResult.OK)
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void saveData(object sender, EventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = DIALOG_FILTER;
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                Utility.Serialize(appData, dialog.FileName);
            }
            catch(Exception exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void loadData(object sender, EventArgs e)
        {
            try
            {
                using(var dialog = new OpenFileDialog())
                {
                    dialog.Filter = DIALOG_FILTER;
                    dialog.FilterIndex = 1;
                    dialog.RestoreDirectory = true;
                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    appData = ApplicationData.LoadFromFile(dialog.FileName);
                    appData.PropertyChanged += appData_PropertyChanged;
                    appData.NotifyChanged("");
                }
            }
            catch (Exception exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void newData(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы уверены?", "Новый расчет",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            var editMaketForm = new EditMaketForm();
            var maket = Maket.CreateDefault();
            maket = editMaketForm.EditSubject(maket);
            appData.final = new Final(maket);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                MakeMatrix();
                if (matrix == null) return;
                appData.stat.Recalculate(matrix);
                propertyGrid1.Refresh();

                dataView.Data = appData.stat.newMatrix;
                //dataView.Data = newMatrix;
                //dataGridView2.Columns[dataGridView2.ColumnCount - 2].HeaderText = "Функция";
            }
            catch(Exception exc)
            {
                Utility.ExceptionMessageBox(exc);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                histogramForm.SetData(appData.stat.calculatedData, (int)numericUpDown1.Value);
                histogramForm.Show();
                histogramForm.Focus();
            }
            catch (InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
