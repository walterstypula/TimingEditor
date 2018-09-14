using System;
using System.Collections.Generic;

namespace NSFW.TimingEditor
{
    public class PassThroughTable : ITable
    {
        private ITable baseTable;
        private ITable advanceTable;
        private bool populated;

        public bool IsReadOnly { get { return false; } set { throw new InvalidOperationException(); } }
        public IList<double> RowHeaders { get { return advanceTable.RowHeaders; } }
        public IList<double> ColumnHeaders { get { return advanceTable.ColumnHeaders; } }

        public PassThroughTable(ITable baseTable)
        {
            this.baseTable = baseTable;
            advanceTable = new Table();
        }

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

        public void SetCell(int x, int y, double value)
        {
            if (populated)
            {
                double oldTotalValue = advanceTable.GetCell(x, y);
                double delta = value - oldTotalValue;

                double oldBaseValue = baseTable.GetCell(x, y);
                baseTable.SetCell(x, y, oldBaseValue - delta);
                advanceTable.SetCell(x, y, oldTotalValue + delta);
            }
            else
            {
                advanceTable.SetCell(x, y, value);
            }
        }
    }
}