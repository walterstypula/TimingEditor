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

        private static void SmoothVertically(DataGridViewSelectedCellCollection selectedCells)
        {
            var columnsGroup = selectedCells.ToList().GroupBy(p => p.ColumnIndex);
            SmoothCells(columnsGroup);
        }

        private static void SmoothHorizontally(DataGridViewSelectedCellCollection selectedCells)
        {
            var rowsGroup = selectedCells.ToList().GroupBy(p => p.RowIndex);
            SmoothCells(rowsGroup);
        }

        private static List<double> Smooth(List<double> arr)
        {
            List<double> result = new List<double>();
            int windowSize = 1;

            for (int i = 0; i < arr.Count; i++)
            {
                int leftOffeset = i - windowSize;
                int from = leftOffeset >= 0 ? leftOffeset : 0;
                int to = i + windowSize + 1;
                int count = 0;
                double sum = 0;

                for (int j = from; j < to && j < arr.Count; j++)
                {
                    sum += arr[j];

                    if (j == i)
                    {
                        sum += arr[j] * 2;
                        count += 2;
                    }

                    count++;
                }

                result.Add(sum / count);
            }

            return result;
        }

        private static void SmoothCells(IEnumerable<IGrouping<int, DataGridViewCell>> groupedCells)
        {
            foreach (var r in groupedCells)
            {
                var cells = r.OrderBy(p => p.ColumnIndex).ToArray();
                var values = cells.Select(p => p.ValueAsDouble()).ToList();
                var cleanData = Smooth(values);

                for (int i = 0; i < cleanData.Count; i++)
                {
                    cells[i].Value = cleanData[i].ToString();
                }
            }
        }

        private static void Smooth(DataGridViewSelectedCellCollection selectedCells)
        {
            SmoothHorizontally(selectedCells);
            SmoothVertically(selectedCells);
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