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
using System.IO;
using System.Globalization;
using Mathos.Parser;

namespace matrix_method2
{
    public partial class FinalMatrixForm : Form
    {
        private HistogramForm histogramForm;

        private Final subject;
        private double[,] matrix;
        private double[] calculatedData;

        private const string FILE_NAME = "program.dat";
        private const string DIALOG_FILTER = "Файлы MX|*.mx";
        private EditCombinedForm editCombinedForm;
        private TwoDimensionalArrayView dataView;
        private NamedIndexList indexList;

        public Final Subject
        {
            get { return subject; }
            set
            {
                subject = value;

                listBox1.Items.Clear();
                foreach (var part in Subject.parts)
                    listBox1.Items.Add(part.name);

                dataView.Data = null;
                matrix = null;
            }
        }

        public FinalMatrixForm()
        {
            InitializeComponent();
            dataView = new TwoDimensionalArrayView(dataGridView2, null);

            Restore();
            editCombinedForm = new EditCombinedForm();

            indexList = new NamedIndexList();
            indexList.names = new string[]{"R1", "R2", "R3", "C1"};
            indexList.indices = new int[] { 0, 0, 0, 1 };

            histogramForm = new HistogramForm();
        }

        public void MakeMatrix()
        {
            dataView.Data = null;
            matrix = Subject.UpdateMatrix(indexList.indices);
            dataView.Data = matrix;
            for (int i = 0; i < indexList.names.Length; ++i)
                dataGridView2.Columns[i].HeaderText = indexList.names[i];
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
            Utility.Serialize(Subject, FILE_NAME);
        }

        private bool Restore()
        {
            try
            {
                Subject = (Final)Utility.Deserialize(FILE_NAME);
                return true;
            }
            catch(FileNotFoundException)
            {
                Subject = new Final();
                return false;
            }
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
                var part = Subject.parts[index];
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
                Utility.Serialize(Subject, dialog.FileName);
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
                var dialog = new OpenFileDialog();
                dialog.Filter = DIALOG_FILTER;
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                var data = (Final)Utility.Deserialize(dialog.FileName);
                if (data == null) throw new InterfaceException("Не удалось загрузить файл");
                Subject = data;
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
            Subject = new Final();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                MakeMatrix();

                if (matrix == null) return;
                var newMatrix = new double[matrix.GetLength(0), matrix.GetLength(1) + 1];
                calculatedData = new double[matrix.GetLength(0)];

                for (int i = 0; i < matrix.GetLength(0); ++i)
                    for (int j = 0; j < matrix.GetLength(1); ++j)
                        newMatrix[i, j] = matrix[i, j];

                var parser = new MathParser();
                parser.CULTURE_INFO = CultureInfo.CurrentCulture;
                var expression = textBox1.Text;
                for (int i = 0; i < matrix.GetLength(0); ++i)
                {
                    for (int j = 0; j < indexList.names.Length; ++j )
                    {
                        parser.LocalVariables[indexList.names[j]] = (decimal)matrix[i, j];
                    }
                    var result = (double)parser.Parse(expression);
                    newMatrix[i, newMatrix.GetLength(1) - 1] = result;
                    calculatedData[i] = result;
                }

                dataView.Data = newMatrix;
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
                histogramForm.SetData(calculatedData, (int)numericUpDown1.Value);
                histogramForm.Show();
                histogramForm.Focus();
            }
            catch(InterfaceException exc)
            {
                Utility.ExceptionMessageBox(exc);
            }

        }

    }
}
