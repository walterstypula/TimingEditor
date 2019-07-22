using NSFW.TimingEditor.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public partial class TimingForm
    {
        private void SmoothButton_Click(object sender, EventArgs e)
        {
            Smooth(dataGrid.SelectedCells);
        }

        private static void SmoothVertically(DataGridViewSelectedCellCollection selectedCells, int range, double decay)
        {
            var columnsGroup = selectedCells.ToList().GroupBy(p => p.ColumnIndex);
            SmoothCells(columnsGroup, range, decay);
        }

        private static void SmoothHorizontally(DataGridViewSelectedCellCollection selectedCells, int range, double decay)
        {
            var rowsGroup = selectedCells.ToList().GroupBy(p => p.RowIndex);
            SmoothCells(rowsGroup, range, decay);
        }

        private static void SmoothCells(IEnumerable<IGrouping<int, DataGridViewCell>> groupedCells, int range, double decay)
        {
            foreach (var r in groupedCells)
            {
                if (r.Count() < 3)
                {
                    continue;
                }

                var cells = r.OrderBy(p => p.ColumnIndex).ToArray();
                var values = cells.Select(p => p.ValueAsDouble()).ToList();
                var cleanData = Util.CleanData(values, range, decay);

                for (int i = 0; i < cleanData.Length; i++)
                {
                    cells[i].Value = cleanData[i].ToString();
                }
            }
        }

        private static void Smooth(DataGridViewSelectedCellCollection selectedCells)
        {
            var decay = 0.15;
            var range = 100;

            SmoothHorizontally(selectedCells, range, decay);
            SmoothVertically(selectedCells, range, decay);
        }

        private static bool SelectedColumn(System.Collections.ICollection selectedCells)
        {
            var column = -1;
            var total = 0;
            foreach (DataGridViewCell cell in selectedCells)
            {
                total++;
                if (column == -1)
                {
                    column = cell.ColumnIndex;
                }
                else
                {
                    if (column != cell.ColumnIndex)
                    {
                        total--;
                    }
                }
            }

            return (column != -1) && (total > 2);
        }

        private static bool SelectedRow(IEnumerable selectedCells)
        {
            var row = -1;
            var total = 0;
            foreach (DataGridViewCell cell in selectedCells)
            {
                total++;
                if (row == -1)
                {
                    row = cell.RowIndex;
                }
                else
                {
                    if (row != cell.RowIndex)
                    {
                        total--;
                    }
                }
            }

            return (row != -1) && (total > 2);
        }
    }
}