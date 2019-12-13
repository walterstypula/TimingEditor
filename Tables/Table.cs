using System;
using System.Collections.Generic;

namespace NSFW.TimingEditor.Tables
{
    public class Table : ITable
    {
        private List<double[]> cells;

        public bool IsReadOnly { get; set; }
        public bool IsPopulated { get; private set; }
        public List<double> RowHeaders { get; private set; } = new List<double>();
        public List<double> ColumnHeaders { get; private set; } = new List<double>();
        public string XAxisHeader { get; set; }
        public bool Is2DTable { get; set; }

        public Table()
        {
        }

        public Table(bool is2DTable)
        {
            Is2DTable = is2DTable;
        }

        public ITable Clone()
        {
            var result = new Table();

            result.IsPopulated = IsPopulated;
            result.IsReadOnly = IsReadOnly;

            result.cells = new List<double[]>();

            for (var row = 0; row < cells.Count; row++)
            {
                result.cells[row] = new double[cells[0].Length];
                for (var column = 0; column < cells.Count; column++)
                {
                    result.cells[row][column] = cells[row][column];
                }
            }

            result.RowHeaders = new List<double>(RowHeaders.Count);
            for (int i = 0; i < RowHeaders.Count; i++)
            {
                result.RowHeaders[i] = RowHeaders[i];
            }

            result.ColumnHeaders = new List<double>(ColumnHeaders.Count);
            for (int i = 0; i < ColumnHeaders.Count; i++)
            {
                result.ColumnHeaders[i] = ColumnHeaders[i];
            }

            return result;
        }

        public void CopyTo(ITable other)
        {
            other.Reset();
            bool wasReadOnly = other.IsReadOnly;
            if (wasReadOnly)
            {
                other.IsReadOnly = false;
            }

            foreach (var header in RowHeaders)
            {
                other.RowHeaders.Add(header);
            }

            foreach (var header in ColumnHeaders)
            {
                other.ColumnHeaders.Add(header);
            }

            for (var row = 0; row < cells.Count; row++)
            {
                for (var column = 0; column < cells[0].Length; column++)
                {
                    other.SetCell(column, row, cells[row][column]);
                }
            }

            other.Populated();

            if (wasReadOnly)
            {
                other.IsReadOnly = true;
            }
        }

        public void Reset()
        {
            cells = null;
            if (RowHeaders == null)
            {
                RowHeaders = new List<double>();
            }
            else
            {
                RowHeaders.Clear();
            }

            if (ColumnHeaders == null)
            {
                ColumnHeaders = new List<double>();
            }
            else
            {
                ColumnHeaders.Clear();
            }
        }

        public void Populated()
        {
            IsPopulated = true;
        }

        public double GetCell(int columnNumber, int rowNumber)
        {
            return cells[rowNumber][columnNumber];
        }

        public void SetCell(int columnNumber, int rowNumber, double value)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("This table is read-only");
            }

            if (cells == null)
            {
                cells = new List<double[]>();

                for (int i = 0; i < RowHeaders.Count; i++)
                {
                    cells.Add(new double[ColumnHeaders.Count]);
                }
            }

            double[] row = cells[rowNumber];
            row[columnNumber] = value;
        }

        private static int FindIndex(double value, double[] headers)
        {
            var index = 0;
            while (value >= headers[index] && index < headers.Length)
            {
                index++;
            };

            return index;
        }

        private static double FindRatio(int index, double value, double[] headers)
        {
            var indexPrevious = index - 1;

            var divider = (headers[index] - headers[indexPrevious]);

            divider = divider == 0 ? 1 : divider;

            var ratio = (value - headers[indexPrevious]) / divider;
            return ratio;
        }

        public double InterpolateXY(double xValue, double yValue)
        {
            return InterpolateXY(xValue, yValue, ColumnHeaders.ToArray(), RowHeaders.ToArray(), cells.ToArray());
        }

        private static double InterpolateXY(double xValue, double yValue, double[] xHeaders, double[] yHeaders, double[][] values)
        {
            //Find Index of Target Columns
            var columnIndex = FindIndex(xValue, xHeaders);
            var rowIndex = FindIndex(yValue, yHeaders);

            //Get Interpolation ratios
            var columnRatio = FindRatio(columnIndex, xValue, xHeaders);
            var rowRatio = FindRatio(rowIndex, yValue, yHeaders);

            var previousRowIndex = rowIndex - 1;
            var previousColumnIndex = columnIndex - 1;

            //Get surrounding values
            var cell1 = values[previousRowIndex][previousColumnIndex];
            var cell2 = values[previousRowIndex][columnIndex];
            var cell3 = values[rowIndex][previousColumnIndex];
            var cell4 = values[rowIndex][columnIndex];

            //Interpolation on columns
            var xInterpolation1 = cell1 + columnRatio * (cell2 - cell1);
            var xInterpolation2 = cell3 + columnRatio * (cell4 - cell3);

            //Add interpolation on Y
            return xInterpolation1 + rowRatio * (xInterpolation2 - xInterpolation1);
        }
    }
}