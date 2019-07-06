using NSFW.TimingEditor.Controls;
using NSFW.TimingEditor.Tables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NSFW.TimingEditor.Utils
{
    public static class Util
    {
        public static string DoubleFormat = "0.00";
        public static int RowHeaderWidth = 60;
        public static int ColumnWidth = 40;

        public static string Print(this IEnumerable<OverlayPoint> list)
        {
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendLine(item.ToString());
            }

            return sb.ToString();
        }

        public static int IndexOf(this string[] array, string columnName)
        {
            return Array.IndexOf(array, columnName);
        }

        public static double ValueAsDouble(this DataGridViewCell cell)
        {
            switch (cell.Value)
            {
                case double d:
                    return d;

                case string s:
                    return double.Parse(s);

                default:
                    throw new FormatException($"Can't parse {cell.Value}");
            }
        }

        public static void LoadTable(string tableText, ITable table)
        {
            var reader = new StringReader(tableText);
            var line = reader.ReadLine();

            if (line == null)
            {
                return;
            }

            if (line.StartsWith("[Table2D]"))
            {
                table.Is2DTable = true;
            }
            else if (!line.StartsWith("[Table3D]"))
            {
                throw new ApplicationException("Doesn't start with [Table3D].");
            }

            line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                throw new ApplicationException("Doesn't contain column headers.");
            }

            table.ColumnHeaders.AddRange(GetValues(line));

            var tableData = new List<List<double>>();
            while (true)
            {
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                if (table.Is2DTable)
                {
                    line = line.Insert(0, "0.0\t");
                }

                var columnValues = GetValues(line);
                table.RowHeaders.Add(columnValues[0]);

                var tableRowData = columnValues.Skip(1).ToArray();
                tableData.Add(tableRowData.ToList());
            }

            //table.Reset();
            table.PopulateCells(tableData);
        }

        private static void PopulateCells(this ITable table, IReadOnlyList<List<double>> tableData)
        {
            for (var rowNumber = 0; rowNumber < tableData.Count; rowNumber++)
            {
                for (var columnNumber = 0; columnNumber < tableData[rowNumber].Count; columnNumber++)
                {
                    var row = tableData[rowNumber];
                    var value = row[columnNumber];

                    table.SetCell(columnNumber, rowNumber, value);
                }
            }

            table.Populated();
        }

        public static string CopyTable(ITable table)
        {
            var writer = new StringWriter();
            writer.WriteLine("[Table3D]");

            for (var i = 0; i < table.ColumnHeaders.Count; i++)
            {
                if (i != 0)
                {
                    writer.Write('\t');
                }

                writer.Write(table.ColumnHeaders[i].ToString(DoubleFormat));
            }
            writer.WriteLine();

            for (var row = 0; row < table.RowHeaders.Count; row++)
            {
                for (var column = 0; column < table.ColumnHeaders.Count; column++)
                {
                    if (column == 0)
                    {
                        writer.Write((int)table.RowHeaders[row]);
                    }

                    writer.Write('\t');
                    writer.Write(table.GetCell(column, row).ToString(DoubleFormat));
                }
                writer.WriteLine();
            }
            writer.WriteLine();
            writer.Flush();
            return writer.ToString();
        }

        public static Table PadLeft(Table source, int desiredColumns)
        {
            var result = new Table();
            result.Reset();
            var newColumnCount = desiredColumns - source.ColumnHeaders.Count;
            for (var i = 0; i < newColumnCount; i++)
            {
                result.ColumnHeaders.Add(0);
            }
            for (var i = newColumnCount; i < newColumnCount + source.ColumnHeaders.Count; i++)
            {
                result.ColumnHeaders.Add(source.ColumnHeaders[i - newColumnCount]);
            }

            for (var i = 0; i < source.RowHeaders.Count; i++)
            {
                result.RowHeaders.Add(source.RowHeaders[i]);
            }

            for (var x = 0; x < source.ColumnHeaders.Count; x++)
            {
                for (var y = 0; y < source.RowHeaders.Count; y++)
                {
                    var value = source.GetCell(x, y);
                    result.SetCell(x + newColumnCount, y, value);
                }
            }

            for (var x = 0; x < newColumnCount; x++)
            {
                for (var y = 0; y < source.RowHeaders.Count; y++)
                {
                    var value = source.GetCell(0, y);
                    result.SetCell(x, y, value);
                }
            }

            result.Populated();
            return result;
        }

        public static Table TrimLeft(ITable source, int columnsToRemove)
        {
            var result = new Table();
            result.Reset();

            for (var i = columnsToRemove; i < source.ColumnHeaders.Count; i++)
            {
                result.ColumnHeaders.Add(source.ColumnHeaders[i]);
            }

            for (var i = 0; i < source.RowHeaders.Count; i++)
            {
                result.RowHeaders.Add(source.RowHeaders[i]);
            }

            for (var x = columnsToRemove; x < source.ColumnHeaders.Count; x++)
            {
                for (var y = 0; y < source.RowHeaders.Count; y++)
                {
                    var value = source.GetCell(x, y);
                    result.SetCell(x - columnsToRemove, y, value);
                }
            }

            result.Populated();
            return result;
        }

        private static DataGridViewCellStyle _defaultStyle;
        //private static DataGridViewCellStyle selectedStyle;

        private static DataGridViewCellStyle DefaultStyle
        {
            get
            {
                if (_defaultStyle != null)
                {
                    return _defaultStyle;
                }

                _defaultStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    BackColor = Color.White,
                    SelectionBackColor = Color.Black,
                    SelectionForeColor = Color.White
                };
                return _defaultStyle;
            }
        }

        /*        public static DataGridViewCellStyle SelectedStyle
                {
                    get
                    {
                        if (selectedStyle == null)
                        {
                            selectedStyle = new DataGridViewCellStyle();
                            selectedStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            selectedStyle.BackColor = System.Drawing.Color.LightGray;
                        }
                        return selectedStyle;
                    }
                }*/

        private static void GetMinMax(this ITable table, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;

            if ((table.ColumnHeaders == null) || (table.RowHeaders == null))
            {
                return;
            }

            for (var x = 0; x < table.ColumnHeaders.Count; x++)
            {
                for (var y = 0; y < table.RowHeaders.Count; y++)
                {
                    var cell = table.GetCell(x, y);
                    min = Math.Min(cell, min);
                    max = Math.Max(cell, max);
                }
            }
        }

        public static void ColorTable(DataGridView dataGridView, ITable table, int selectedX, int selectedY, Overlay overlay)
        {
            table.GetMinMax(out var min, out var max);
            var middle = (max + min) / 2;

            overlay?.SetRowHeader(table.XAxisHeader);

            var cellHit = overlay?.ProcessOverlay(table.ColumnHeaders, table.RowHeaders);

            for (var x = 0; x < dataGridView.Columns.Count; x++)
            {
                for (var y = 0; y < dataGridView.Rows.Count; y++)
                {
                    var value = table.GetCell(x, y);

                    if (cellHit != null)
                    {
                        var highlight = cellHit.FirstOrDefault(p => p.RowIndex == x && p.ColumnIndex == y);

                        if (highlight != null)
                        {
                            dataGridView.Rows[y].Cells[x] = new CustomDataGridViewCell(highlight);
                        }
                        else if (dataGridView.Rows[y].Cells[x] is CustomDataGridViewCell)
                        {
                            dataGridView.Rows[y].Cells[x] = new DataGridViewTextBoxCell();
                        }
                    }

                    Color color;
                    if ((x == selectedX) || (y == selectedY))
                    {
                        color = Color.Gray;
                    }
                    else
                    {
                        if (max - middle == 0 || middle - min == 0)
                        {
                            color = Color.White;
                        }
                        else
                        {
                            double brightness;
                            if (value > middle)
                            {
                                brightness = 1 - (value - middle) / (max - middle);
                                color = Color.FromArgb(255, 255, (int)(255 * brightness));
                            }
                            else
                            {
                                brightness = ((1 - (middle - value) / (middle - min)) + 1) / 2;
                                color = Color.FromArgb((int)(255 * brightness), (int)(255 * brightness), 255);
                            }
                        }
                    }

                    var style = DefaultStyle.Clone();
                    style.BackColor = color;
                    dataGridView.Rows[y].Cells[x].Value = value.ToString(DoubleFormat);
                    dataGridView.Rows[y].Cells[x].Style = style;
                }
            }
        }

        public static void ShowTable(Form form, ITable table, DataGridView dataGridView)
        {
            DataGridViewCell template = new DataGridViewTextBoxCell();
            //template.Style.BackColor = Color.Wheat;

            dataGridView.Columns.Clear();
            dataGridView.RowHeadersWidth = RowHeaderWidth;
            for (var i = 0; i < table.ColumnHeaders.Count; i++)
            {
                var column = new DataGridViewColumn(template);
                column.HeaderCell.Value = table.ColumnHeaders[i];
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = ColumnWidth;
                column.HeaderText = table.ColumnHeaders[i].ToString(DoubleFormat);
                column.HeaderCell.Style = DefaultStyle;
                dataGridView.Columns.Add(column);
            }

            dataGridView.Rows.Clear();
            foreach (var t in table.RowHeaders)
            {
                var row = new DataGridViewRow();
                row.HeaderCell.Value = t.ToString();
                row.HeaderCell.Style = DefaultStyle;
                dataGridView.Rows.Add(row);
            }

            for (var x = 0; x < dataGridView.Columns.Count; x++)
            {
                for (var y = 0; y < dataGridView.Rows.Count; y++)
                {
                    var value = table.GetCell(x, y);

                    dataGridView.Rows[y].Cells[x].Value = value.ToString(DoubleFormat);
                    dataGridView.Rows[y].Cells[x].Style = DefaultStyle;
                }
            }

            var oldWidth = dataGridView.Width;
            var newWidth = dataGridView.RowHeadersWidth + 2;
            for (var i = 0; i < dataGridView.Columns.Count; i++)
            {
                newWidth += dataGridView.Columns[i].Width;
            }

            var delta = newWidth - oldWidth;
            dataGridView.Width += delta;

            var oldHeight = dataGridView.Height;

            var scrollBarHeight = 0;

            foreach (var scroll in dataGridView.Controls.OfType<HScrollBar>())
            {
                scrollBarHeight = scroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;
            }

            var newHeight = dataGridView.ColumnHeadersHeight + 2 + scrollBarHeight;

            for (var i = 0; i < dataGridView.Rows.Count; i++)
            {
                newHeight += dataGridView.Rows[i].Height;
            }

            delta = newHeight - oldHeight;
            dataGridView.Height += delta;
            //form.Height += delta;
        }

        internal static List<DataGridViewCell> ToList(this DataGridViewSelectedCellCollection collection)
        {
            var list = new List<DataGridViewCell>();

            foreach (DataGridViewCell i in collection)
            {
                list.Add(i);
            }

            return list;
        }

        public static double[] GetValues(string line)
        {
            var splitArray = line.Split('\t');
            return GetValues(splitArray);
        }

        public static double[] GetValues(string[] valueStrings)
        {
            var result = new double[valueStrings.Length];
            for (var i = 0; i < result.Length; i++)
            {
                Exception exception = null;
                try
                {
                    result[i] = double.Parse(valueStrings[i]);
                }
                catch (ArgumentNullException ex)
                {
                    exception = ex;
                }
                catch (FormatException ex)
                {
                    exception = ex;
                }
                catch (OverflowException ex)
                {
                    exception = ex;
                }
                if (exception != null)
                {
                    throw new ApplicationException(exception.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// Originally written by John Wakefield of http://www.robosoup.com/2014/01/cleaning-noisy-time-series-data-low-pass-filter-c.html.
        /// Minor modifications made to prevent skewing of start and end.
        /// </summary>
        /// <param name="noisy"></param>
        /// <param name="range"></param>
        /// <param name="decay"></param>
        /// <returns></returns>
        internal static double[] CleanData(List<double> noisy, int range, double decay)
        {
            var originalSize = noisy.Count;

            for (int i = 0; i < range; i++)
            {
                noisy.Insert(0, noisy.First());
                noisy.Add(noisy.Last());
            }

            double[] clean = new double[noisy.Count];
            double[] coefficients = Coefficients(range, decay);
            // Calculate divisor value.
            double divisor = 0;
            for (int i = -range; i <= range; i++)
            {
                divisor += coefficients[Math.Abs(i)];
            }
            // Clean main data.
            for (int i = range; i < clean.Length - range; i++)
            {
                double temp = 0;
                for (int j = -range; j <= range; j++)
                {
                    temp += noisy[i + j] * coefficients[Math.Abs(j)];
                }

                clean[i] = temp / divisor;
            }
            // Calculate leading and trailing slopes.
            double leadSum = 0;
            double trailSum = 0;
            int leadRef = range;
            int trailRef = clean.Length - range - 1;
            for (int i = 1; i <= range; i++)
            {
                leadSum += (clean[leadRef] - clean[leadRef + i]) / i;
                trailSum += (clean[trailRef] - clean[trailRef - i]) / i;
            }
            double leadSlope = leadSum / range;
            double trailSlope = trailSum / range;
            // Clean edges.
            for (int i = 1; i <= range; i++)
            {
                clean[leadRef - i] = clean[leadRef] + leadSlope * i;
                clean[trailRef + i] = clean[trailRef] + trailSlope * i;
            }

            var skip = (clean.Length - originalSize) / 2;
            return clean.Skip(skip).Take(originalSize).ToArray();
        }

        private static double[] Coefficients(int range, double decay)
        {
            // Precalculate coefficients.
            double[] coefficients = new double[range + 1];
            for (int i = 0; i <= range; i++)
            {
                coefficients[i] = Math.Pow(decay, i);
            }

            return coefficients;
        }

        public static double LinearInterpolation(double x, double x1, double x2, double y1, double y2)
        {
            return (x1 == x2) ? 0.0 : (y1 + (x - x1) * (y2 - y1) / (x2 - x1));
        }

        public static int ClosestValueIndex(this IEnumerable<double> list, string val)
        {
            return ClosestValueIndex(list.ToList(), double.Parse(val));
        }

        public static int ClosestValueIndex(this IList<double> list, double val)
        {
            var index = ((List<double>)list).BinarySearch(val);
            if (index >= 0)
            {
                return index;
            }

            var idxPrev = Math.Max(0, -index - 2);
            var idxNext = Math.Min(list.Count - 1, -index - 1);
            return val - list[idxPrev] <= list[idxNext] - val ? idxPrev : idxNext;
        }
    }
}