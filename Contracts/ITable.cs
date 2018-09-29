using System.Collections.Generic;

namespace NSFW.TimingEditor
{
    public interface ITable
    {
        bool IsReadOnly { get; set; }
        bool IsPopulated { get; }
        List<double> RowHeaders { get; }
        List<double> ColumnHeaders { get; }

        ITable Clone();

        void CopyTo(ITable destination);

        double GetCell(int column, int row);

        void SetCell(int column, int row, double value);

        void Reset();

        void Populated();

        bool Is2dTable { get; set; }
    }

    public interface I2DTable
    {
        IList<double> ColumnHeaders { get; }
    }
}