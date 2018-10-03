﻿using NSFW.TimingEditor.Controls;
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
            if (cell.Value is double)
            {
                return (double)cell.Value;
            }
            if (cell.Value is string)
            {
                return double.Parse(cell.Value as string);
            }
            throw new FormatException("Can't parse " + cell.Value.ToString());
        }

        public static void LoadTable(string tableText, ITable table)
        {
            StringReader reader = new StringReader(tableText);
            string line = reader.ReadLine();
            
            if (line.StartsWith("[Table2D]"))
            {
                line = line.Replace("2", "3");
                table.Is2dTable = true;
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
            
            List<List<double>> tableData = new List<List<double>>();
            while (true)
            {
                line = reader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
            
                if (table.Is2dTable)
                {
                    line = line.Insert(0, "0.0\t");
                }
            
                double[] columnValues = GetValues(line);
                table.RowHeaders.Add(columnValues[0]);
                
                var tableRowData = columnValues.Skip(1).ToArray();
                tableData.Add(tableRowData.ToList());
            }
            
            //table.Reset();
            table.PopulateCells(tableData);
        }

        public static void PopulateCells(this ITable table, List<List<double>> tableData)
        {
            for (int rowNumber = 0; rowNumber < tableData.Count; rowNumber++)
            {
                for (int columnNumber = 0; columnNumber < tableData[rowNumber].Count; columnNumber++)
                {
                    List<double> row = tableData[rowNumber];
                    double value = row[columnNumber];

                    table.SetCell(columnNumber, rowNumber, value);
                }
            }

            table.Populated();
        }

        public static string CopyTable(ITable table)
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("[Table3D]");

            for (int i = 0; i < table.ColumnHeaders.Count; i++)
            {
                if (i != 0)
                {
                    writer.Write('\t');
                }

                writer.Write(table.ColumnHeaders[i].ToString(DoubleFormat));
            }
            writer.WriteLine();

            for (int row = 0; row < table.RowHeaders.Count; row++)
            {
                for (int column = 0; column < table.ColumnHeaders.Count; column++)
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
            Table result = new Table();
            result.Reset();
            int newColumnCount = desiredColumns - source.ColumnHeaders.Count;
            for (int i = 0; i < newColumnCount; i++)
            {
                result.ColumnHeaders.Add(0);
            }
            for (int i = newColumnCount; i < newColumnCount + source.ColumnHeaders.Count; i++)
            {
                result.ColumnHeaders.Add(source.ColumnHeaders[i - newColumnCount]);
            }

            for (int i = 0; i < source.RowHeaders.Count; i++)
            {
                result.RowHeaders.Add(source.RowHeaders[i]);
            }

            for (int x = 0; x < source.ColumnHeaders.Count; x++)
            {
                for (int y = 0; y < source.RowHeaders.Count; y++)
                {
                    double value = source.GetCell(x, y);
                    result.SetCell(x + newColumnCount, y, value);
                }
            }

            for (int x = 0; x < newColumnCount; x++)
            {
                for (int y = 0; y < source.RowHeaders.Count; y++)
                {
                    double value = source.GetCell(0, y);
                    result.SetCell(x, y, value);
                }
            }

            result.Populated();
            return result;
        }

        public static Table TrimLeft(ITable source, int columnsToRemove)
        {
            Table result = new Table();
            result.Reset();

            for (int i = columnsToRemove; i < source.ColumnHeaders.Count; i++)
            {
                result.ColumnHeaders.Add(source.ColumnHeaders[i]);
            }

            for (int i = 0; i < source.RowHeaders.Count; i++)
            {
                result.RowHeaders.Add(source.RowHeaders[i]);
            }

            for (int x = columnsToRemove; x < source.ColumnHeaders.Count; x++)
            {
                for (int y = 0; y < source.RowHeaders.Count; y++)
                {
                    double value = source.GetCell(x, y);
                    result.SetCell(x - columnsToRemove, y, value);
                }
            }

            result.Populated();
            return result;
        }

        private static DataGridViewCellStyle defaultStyle;
        //private static DataGridViewCellStyle selectedStyle;

        public static DataGridViewCellStyle DefaultStyle
        {
            get
            {
                if (defaultStyle == null)
                {
                    defaultStyle = new DataGridViewCellStyle();
                    defaultStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    defaultStyle.BackColor = System.Drawing.Color.White;
                    defaultStyle.SelectionBackColor = Color.Black;
                    defaultStyle.SelectionForeColor = Color.White;
                }
                return defaultStyle;
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

            for (int x = 0; x < table.ColumnHeaders.Count; x++)
            {
                for (int y = 0; y < table.RowHeaders.Count; y++)
                {
                    double cell = table.GetCell(x, y);
                    min = Math.Min(cell, min);
                    max = Math.Max(cell, max);
                }
            }
        }

        public static void ColorTable(DataGridView dataGridView, ITable table, int selectedX, int selectedY, string[,] cellHit)
        {
            double min, max, unbrightness;
            Color color;
            DataGridViewCellStyle style;

            table.GetMinMax(out min, out max);
            double middle = (max + min) / 2;

            for (int x = 0; x < dataGridView.Columns.Count; x++)
            {
                for (int y = 0; y < dataGridView.Rows.Count; y++)
                {
                    double value = table.GetCell(x, y);

                    if (cellHit != null)
                    {
                        if (cellHit[x, y] != null)
                        {
                            dataGridView.Rows[y].Cells[x] = new CustomDataGridViewCell();
                        }
                        else if (dataGridView.Rows[y].Cells[x] is CustomDataGridViewCell)
                        {
                            dataGridView.Rows[y].Cells[x] = new DataGridViewTextBoxCell();
                        }
                    }

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
                            if (value > middle)
                            {
                                unbrightness = 1 - (value - middle) / (max - middle);
                                color = Color.FromArgb(255, 255, (int)(255 * unbrightness));
                            }
                            else
                            {
                                unbrightness = ((1 - (middle - value) / (middle - min)) + 1) / 2;
                                color = Color.FromArgb((int)(255 * unbrightness), (int)(255 * unbrightness), 255);
                            }
                        }
                    }

                    style = DefaultStyle.Clone();
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
            for (int i = 0; i < table.ColumnHeaders.Count; i++)
            {
                DataGridViewColumn column = new DataGridViewColumn(template);
                column.HeaderCell.Value = table.ColumnHeaders[i];
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = ColumnWidth;
                column.HeaderText = table.ColumnHeaders[i].ToString(DoubleFormat);
                column.HeaderCell.Style = DefaultStyle;
                dataGridView.Columns.Add(column);
            }

            dataGridView.Rows.Clear();
            for (int i = 0; i < table.RowHeaders.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.HeaderCell.Value = table.RowHeaders[i].ToString();
                row.HeaderCell.Style = DefaultStyle;
                dataGridView.Rows.Add(row);
            }

            for (int x = 0; x < dataGridView.Columns.Count; x++)
            {
                for (int y = 0; y < dataGridView.Rows.Count; y++)
                {
                    double value = table.GetCell(x, y);
                    DataGridViewCellStyle style = DefaultStyle.Clone();

                    dataGridView.Rows[y].Cells[x].Value = value.ToString(DoubleFormat);
                    dataGridView.Rows[y].Cells[x].Style = DefaultStyle;
                }
            }

            int oldWidth = dataGridView.Width;
            int newWidth = dataGridView.RowHeadersWidth + 2;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                newWidth += dataGridView.Columns[i].Width;
            }

            int delta = newWidth - oldWidth;
            //dataGridView.Width += delta;
            form.Width += delta;

            int oldHeight = dataGridView.Height;
            int newHeight = dataGridView.ColumnHeadersHeight + 2;
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                newHeight += dataGridView.Rows[i].Height;
            }

            delta = newHeight - oldHeight;
            //dataGridView.Height += delta;
            form.Height += delta;
        }

        /*        public static void Highlight(DataGridView grid, int selectedColumn, int selectedRow)
                {
                    for (int row = 0; row < grid.Rows.Count; row++)
                    {
                        for (int column = 0; column < grid.Columns.Count; column++)
                        {
                            DataGridViewCellStyle style =
                                (row == selectedRow || column == selectedColumn) ?
                                SelectedStyle : DefaultStyle;
                            grid.Rows[row].Cells[column].Style = style;
                        }
                    }
                }
        */

        public static double[] GetValues(string line)
        {
            var splitArray = line.Split('\t');
            return GetValues(splitArray);
        }

        public static double[] GetValues(string[] valueStrings)
        {
            double[] result = new double[valueStrings.Length];
            for (int i = 0; i < result.Length; i++)
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

        public static double LinearInterpolation(double x, double x1, double x2, double y1, double y2)
        {
            return (x1 == x2) ? 0.0 : (y1 + (x - x1) * (y2 - y1) / (x2 - x1));
        }

        public static int ClosestValueIndex(this double[] list, string val)
        {
            return ClosestValueIndex(list.ToList(), double.Parse(val));
        }

        public static int ClosestValueIndex(this IList<double> list, double val)
        {
            int index = ((List<double>)list).BinarySearch(val);
            if (index < 0)
            {
                int idxPrev = Math.Max(0, -index - 2);
                int idxNext = Math.Min(list.Count - 1, -index - 1);
                return val - list[idxPrev] <= list[idxNext] - val ? idxPrev : idxNext;
            }
            return index;
        }
    }
}