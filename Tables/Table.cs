using System;
using System.Collections.Generic;
using NSFW.TimingEditor;

namespace NSFW.TimingEditor.Tables
{
    public class Table : ITable
    {
        private double[][] cells;

        public bool IsReadOnly { get; set; }
        public bool IsPopulated { get; private set; }
        public IList<double> RowHeaders { get; private set; }
        public IList<double> ColumnHeaders { get; private set; }
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

            result.cells = new double[cells.Length][];
            for (int x = 0; x < cells.Length; x++)
            {
                result.cells[x] = new double[cells[0].Length];
                for (int y = 0; y < cells.Length; y++)
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

            for (int x = 0; x < cells.Length; x++)
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

        public double GetCell(int x, int y)
        {
            return cells[x][y];
        }

        public void SetCell(int x, int y, double value)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException("This table is read-only");
            }

            if (cells == null)
            {
                cells = new double[ColumnHeaders.Count][];
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = new double[RowHeaders.Count];
                }
            }
            double[] column = cells[x];
            column[y] = value;
        }
    }
}