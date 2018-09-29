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
        public bool Is2dTable { get; set; }

        public Table()
        {
        }

        public Table(bool is2dTable)
        {
            Is2dTable = Is2dTable;
        }

        public ITable Clone()
        {
            Table result = new Table();

            result.IsPopulated = IsPopulated;
            result.IsReadOnly = IsReadOnly;

            result.cells = new List<double[]>();
            for (int x = 0; x < cells.Count; x++)
            {
                result.cells[x] = new double[cells[0].Length];
                for (int y = 0; y < cells.Count; y++)
                {
                    result.cells[x][y] = cells[x][y];
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

            for (int i = 0; i < RowHeaders.Count; i++)
            {
                other.RowHeaders.Add(RowHeaders[i]);
            }

            for (int i = 0; i < ColumnHeaders.Count; i++)
            {
                other.ColumnHeaders.Add(ColumnHeaders[i]);
            }

            for (int x = 0; x < cells.Count; x++)
            {
                for (int y = 0; y < cells[0].Length; y++)
                {
                    other.SetCell(x, y, cells[x][y]);
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
    }
}