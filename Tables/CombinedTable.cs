using System;
using System.Collections.Generic;
using NSFW.TimingEditor;
using NSFW.TimingEditor.Enums;

namespace NSFW.TimingEditor.Tables
{
    public class CombinedTable : ITable
    {
        private ITable a;
        private ITable b;
        private Operation operation;

        public bool IsReadOnly { get { return a.IsReadOnly || b.IsReadOnly || operation != Operation.Sum; } set { throw new InvalidOperationException(); } }
        public IList<double> RowHeaders { get { return a.RowHeaders; } }
        public IList<double> ColumnHeaders { get { return a.ColumnHeaders; } }

        public CombinedTable(ITable a, ITable b, Operation operation)
        {
            this.a = a;
            this.b = b;
            this.operation = operation;
        }

        public ITable Clone()
        {
            throw new InvalidOperationException();
            /*CombinedTable result = new CombinedTable();
            result.a = this.a.Clone();
            result.b = this.b.Clone();
            result.operation = this.operation;
            return result;*/
        }

        public void CopyTo(ITable other)
        {
            throw new InvalidOperationException();
        }

        public bool IsPopulated
        {
            get
            {
                return a.IsPopulated && b.IsPopulated;
            }
        }

        public bool Is2dTable { get; set; }

        public void Reset()
        {
        }

        public void Populated()
        {
            throw new InvalidOperationException();
        }

        public double GetCell(int x, int y)
        {
            if (operation == Operation.Sum)
            {
                return a.GetCell(x, y) + b.GetCell(x, y);
            }
            else if (operation == Operation.Difference)
            {
                return b.GetCell(x, y) - a.GetCell(x, y);
            }
            else
            {
                throw new InvalidOperationException("Undefined CombinedTable Operation: " + operation.ToString());
            }
        }

        public void SetCell(int x, int y, double value)
        {
            double oldTotalValue = GetCell(x, y);
            double delta = value - oldTotalValue;

            if (operation == Operation.Sum)
            {
                double oldValue = a.GetCell(x, y);
                double newValue = oldValue + delta;
                a.SetCell(x, y, newValue);
                return;
            }

            throw new InvalidOperationException("Cannot set the value of a difference table.");
        }
    }
}