using NSFW.TimingEditor.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public partial class TimingForm
    {
        private void SmoothButton_Click(object sender, EventArgs e)
        {
            DisposeCellPopup();
            Smooth(dataGrid.SelectedCells, true);
        }

        private bool Smooth(DataGridViewSelectedCellCollection selectedCells, bool forReal)
        {
            var ret = false;
            IList<DataGridViewCell> cells;
            if (smoothComboBox.SelectedIndex == 0 || smoothComboBox.SelectedIndex == 1)
            {
                if (SelectedRow(selectedCells))
                {
                    if (forReal)
                    {
                        cells = SortCellsByRow(selectedCells);
                        SmoothHorizontal(cells);
                    }
                    ret = true;
                }
            }

            if (smoothComboBox.SelectedIndex != 0 && smoothComboBox.SelectedIndex != 2)
                return ret;

            if (!SelectedColumn(selectedCells))
                return ret;

            if (!forReal)
                return true;

            cells = SortCellsByColumn(selectedCells);
            SmoothVertical(cells);

            return true;
        }

        private static bool SelectedColumn(System.Collections.ICollection selectedCells)
        {
            var column = -1;
            var total = 0;
            foreach (DataGridViewCell cell in selectedCells)
            {
                total++;
                if (column == -1)
                    column = cell.ColumnIndex;
                else
                {
                    if (column != cell.ColumnIndex)
                        total--;
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
                    row = cell.RowIndex;
                else
                {
                    if (row != cell.RowIndex)
                        total--;
                }
            }

            return (row != -1) && (total > 2);
        }

        private void SmoothHorizontal(IList<DataGridViewCell> cells)
        {
            try
            {
                double x, x1, x2, y1, y2;
                for (int start = 0, end = 0; end < cells.Count;)
                {
                    x1 = dataGrid.Columns[cells[start].ColumnIndex].HeaderCell.ValueAsDouble();
                    y1 = cells[start].ValueAsDouble();
                    for (end = start; end < cells.Count && cells[start].RowIndex == cells[end].RowIndex; ++end)
                        continue;
                    x2 = dataGrid.Columns[cells[end - 1].ColumnIndex].HeaderCell.ValueAsDouble();
                    y2 = cells[end - 1].ValueAsDouble();
                    for (; start < end; ++start)
                    {
                        x = dataGrid.Columns[cells[start].ColumnIndex].HeaderCell.ValueAsDouble();
                        var value = Util.LinearInterpolation(x, x1, x2, y1, y2);
                        cells[start].Value = value.ToString(Util.DoubleFormat);
                    }
                }
            }
            catch (FormatException e)
            {
                statusStrip1.Items[0].Text = e.Message;
            }
            catch (ArgumentNullException)
            {
            }
        }

        private void SmoothVertical(IList<DataGridViewCell> cells)
        {
            try
            {
                double x, x1, x2, y1, y2;
                for (int start = 0, end = 0; end < cells.Count;)
                {
                    x1 = dataGrid.Rows[cells[start].RowIndex].HeaderCell.ValueAsDouble();
                    y1 = cells[start].ValueAsDouble();
                    for (end = start; end < cells.Count && cells[start].ColumnIndex == cells[end].ColumnIndex; ++end)
                        continue;
                    x2 = dataGrid.Rows[cells[end - 1].RowIndex].HeaderCell.ValueAsDouble();
                    y2 = cells[end - 1].ValueAsDouble();
                    for (; start < end; ++start)
                    {
                        x = dataGrid.Rows[cells[start].RowIndex].HeaderCell.ValueAsDouble();
                        var value = Util.LinearInterpolation(x, x1, x2, y1, y2);
                        cells[start].Value = value.ToString(Util.DoubleFormat);
                    }
                }
            }
            catch (FormatException e)
            {
                statusStrip1.Items[0].Text = e.Message;
            }
            catch (ArgumentNullException)
            {
            }
        }

        private List<DataGridViewCell> SortCellsByRow(DataGridViewSelectedCellCollection input)
        {
            var result = new List<DataGridViewCell>();
            foreach (DataGridViewCell cell in input)
            {
                if (cell == null)
                {
                    continue;
                }
                result.Add(cell);
            }
            result.Sort(delegate (DataGridViewCell a, DataGridViewCell b)
            {
                if (a.RowIndex < b.RowIndex)
                    return -1;
                else if (a.RowIndex == b.RowIndex)
                {
                    if (a.ColumnIndex < b.ColumnIndex)
                        return -1;
                    else if (a.ColumnIndex == b.ColumnIndex)
                        return 0;
                }
                return 1;
            });
            return result;
        }

        private static List<DataGridViewCell> SortCellsByColumn(DataGridViewSelectedCellCollection input)
        {
            var result = new List<DataGridViewCell>();
            foreach (DataGridViewCell cell in input)
            {
                if (cell == null)
                {
                    continue;
                }
                result.Add(cell);
            }
            result.Sort(delegate (DataGridViewCell a, DataGridViewCell b)
            {
                if (a.ColumnIndex < b.ColumnIndex)
                    return -1;
                else if (a.ColumnIndex == b.ColumnIndex)
                {
                    if (a.RowIndex < b.RowIndex)
                        return -1;
                    else if (a.RowIndex == b.RowIndex)
                        return 0;
                }
                return 1;
            });
            return result;
        }
    }
}