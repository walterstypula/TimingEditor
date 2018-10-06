using System;
using System.Collections.Generic;
using NSFW.TimingEditor;

namespace NSFW.TimingEditor.Tables
{
    public class PassThroughTable : ITable
    {
        private ITable baseTable;
        private ITable advanceTable;
        private bool populated;

        public bool IsReadOnly { get { return false; } set { throw new InvalidOperationException(); } }
        public List<double> RowHeaders { get { return advanceTable.RowHeaders; } }
        public List<double> ColumnHeaders { get { return advanceTable.ColumnHeaders; } }
        public string XAxisHeader { get; set; }

        public PassThroughTable(ITable baseTable)
        {
            this.baseTable = baseTable;
            advanceTable = new Table();
        }

        public bool Is2DTable { get; set; }

        public ITable Clone()
        {
            PassThroughTable result = new PassThroughTable(baseTable);
            result.advanceTable = advanceTable.Clone();
            result.populated = populated;
            return result;
        }

        public void CopyTo(ITable other)
        {
            other.Reset();

            for (int i = 0; i < baseTable.RowHeaders.Count; i++)
            {
                other.RowHeaders.Add(baseTable.RowHeaders[i]);
            }

            for (int i = 0; i < baseTable.ColumnHeaders.Count; i++)
            {
                other.ColumnHeaders.Add(baseTable.ColumnHeaders[i]);
            }

            for (int x = 0; x < baseTable.ColumnHeaders.Count; x++)
            {
                for (int y = 0; y < baseTable.RowHeaders.Count; y++)
                {
                    other.SetCell(x, y, GetCell(x, y));
                }
            }
            other.Populated();
        }

        public bool IsPopulated
        {
            get
            {
                return baseTable.IsPopulated && advanceTable.IsPopulated;
            }
        }

        public void Reset()
        {
            populated = false;
            advanceTable.Reset();
        }

        public void Populated()
        {
            populated = true;
            advanceTable.Populated();
        }

        public double GetCell(int x, int y)
        {
            return advanceTable.GetCell(x, y);
        }

        public void SetCell(int columnNumber, int rowNumber, double value)
        {
            if (populated)
            {
                double oldTotalValue = advanceTable.GetCell(columnNumber, rowNumber);
                double delta = value - oldTotalValue;

                double oldBaseValue = baseTable.GetCell(columnNumber, rowNumber);
                baseTable.SetCell(columnNumber, rowNumber, oldBaseValue - delta);
                advanceTable.SetCell(columnNumber, rowNumber, oldTotalValue + delta);
            }
            else
            {
                advanceTable.SetCell(columnNumber, rowNumber, value);
            }
        }
    }
}