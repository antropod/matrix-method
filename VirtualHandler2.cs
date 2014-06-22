// Version 2

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Antropod.DataGridViewVirtual
{
    public class TwoDimensionalArrayView
    {
        private DataGridView dataGridView;
        public double[,] data;

        public TwoDimensionalArrayView(DataGridView dataGridView,
            double[,] data = null)
        {
            this.dataGridView = dataGridView;
            SetDataGridParameters();
            Data = data;
        }

        public double[,] Data
        {
            set
            {
                this.data = value;
                dataGridView.ColumnCount = data != null ? data.GetLength(1) : 0;
                dataGridView.RowCount = data != null ? data.GetLength(0) : 0;
                dataGridView.Invalidate();
            }
        }

        private void SetDataGridParameters()
        {
            dataGridView.ReadOnly = true;
            dataGridView.VirtualMode = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToResizeRows = false;

            dataGridView.Columns.Clear();

            dataGridView.CellValueNeeded += new
                DataGridViewCellValueEventHandler(CellValueNeeded);
        }

        private void CellValueNeeded(object sender, DataGridViewCellValueEventArgs args)
        {
            args.Value = 0;
            if (data == null) return;
            if (args.ColumnIndex >= data.GetLength(1)) return;
            if (args.RowIndex >= data.GetLength(0)) return;
            if (dataGridView.Rows[args.RowIndex].IsNewRow) return;

            args.Value = data[args.RowIndex, args.ColumnIndex];
        }
    }
}