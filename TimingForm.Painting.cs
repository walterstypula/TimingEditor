using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public partial class TimingForm
    {
        private void DrawSideViews(int activeColumn, int activeRow)
        {
            var horizontalPanelBitmap = new Bitmap(horizontalPanel.Width, horizontalPanel.Height);
            var horizontalPanelBackBuffer = Graphics.FromImage(horizontalPanelBitmap);
            //Graphics horizontalPanelBackBuffer = horizontalPanel.CreateGraphics();

            var verticalPanelBitmap = new Bitmap(verticalPanel.Width, verticalPanel.Height);
            var verticalPanelBackBuffer = Graphics.FromImage(verticalPanelBitmap);
            //Graphics verticalPanelBackBuffer = verticalPanel.CreateGraphics();

            horizontalPanelBackBuffer.FillRectangle(Brushes.White, horizontalPanel.ClientRectangle);
            verticalPanelBackBuffer.FillRectangle(Brushes.White, verticalPanel.ClientRectangle);

            GetMinMax(out var min, out var max);

            var pen = Pens.Gray;
            for (var row = 0; row < dataGrid.Rows.Count; row++)
            {
                DrawRow(horizontalPanelBackBuffer, pen, row, min, max);
            }

            for (var column = 0; column < dataGrid.Columns.Count; column++)
            {
                DrawColumn(verticalPanelBackBuffer, pen, column, min, max);
            }

            if ((activeColumn >= 0) && (activeColumn < dataGrid.Columns.Count) &&
                (activeRow >= 0) && (activeRow < dataGrid.Rows.Count))
            {
                using (var heavyPen = new Pen(Color.Black, 3))
                {
                    DrawRow(horizontalPanelBackBuffer, heavyPen, activeRow, min, max);
                    DrawColumn(verticalPanelBackBuffer, heavyPen, activeColumn, min, max);
                }

                using (var lightPen = new Pen(Color.Gray, 2))
                {
                    var x = GetRowX(activeColumn);
                    horizontalPanelBackBuffer.DrawLine(lightPen, x, 0, x, horizontalPanel.Height);

                    var y = GetColumnY(activeRow);
                    verticalPanelBackBuffer.DrawLine(lightPen, 0, y, verticalPanel.Width, y);
                }
            }

            var si = GetSmoothInfo(min, max);
            if (si != null)
            {
                if (si.A.RowIndex == si.B.RowIndex)
                {
                    DrawRowSmooth(horizontalPanelBackBuffer, si);
                }
                else
                {
                    DrawColumnSmooth(verticalPanelBackBuffer, si);
                }
            }

            var graphics = horizontalPanel.CreateGraphics();
            graphics.DrawImage(horizontalPanelBitmap, 0, 0);
            graphics = verticalPanel.CreateGraphics();
            graphics.DrawImage(verticalPanelBitmap, 0, 0);
        }

        private void GetMinMax(out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;
            double value;
            for (var row = 0; row < dataGrid.Rows.Count; row++)
            {
                for (var column = 0; column < dataGrid.Columns.Count; column++)
                {
                    if (!TryGetValue(column, row, out value))
                    {
                        return;
                    }

                    min = Math.Min(min, value);
                    max = Math.Max(max, value);
                }
            }
        }

        private class SmoothInfo
        {
            public DataGridViewCell A;
            public DataGridViewCell B;
            public double MinValue;
            public double MaxValue;
        }

        private SmoothInfo GetSmoothInfo(double min, double max)
        {
            var selected = dataGrid.SelectedCells;
            if (SelectedColumn(selected))
            {
                var result = new SmoothInfo();
                result.MinValue = min;
                result.MaxValue = max;
                var cells = selected.Cast<DataGridViewCell>().ToList();

                var minY = cells.Min(cell => cell.RowIndex);
                var maxY = cells.Max(cell => cell.RowIndex);
                result.A = cells.First(cell => cell.RowIndex == minY);
                result.B = cells.First(cell => cell.RowIndex == maxY);
                return result;
            }

            if (SelectedRow(dataGrid.SelectedCells))
            {
                var result = new SmoothInfo();
                result.MinValue = min;
                result.MaxValue = max;
                var cells = selected.Cast<DataGridViewCell>().ToList();

                var minX = cells.Min(cell => cell.ColumnIndex);
                var maxX = cells.Max(cell => cell.ColumnIndex);
                result.A = cells.First(cell => cell.ColumnIndex == minX);
                result.B = cells.First(cell => cell.ColumnIndex == maxX);
                return result;
            }

            return null;
        }

        private void DrawRowSmooth(Graphics graphics, SmoothInfo si)
        {
            if (!TryGetValue(si.A.ColumnIndex, si.A.RowIndex, out var valueA))
                return;

            if (!TryGetValue(si.B.ColumnIndex, si.B.RowIndex, out var valueB))
                return;

            float x1 = GetRowX(si.A.ColumnIndex);
            float y1 = GetRowY(si.MinValue, si.MaxValue, valueA);
            float x2 = GetRowX(si.B.ColumnIndex);
            float y2 = GetRowY(si.MinValue, si.MaxValue, valueB);

            using (var pen = new Pen(Color.Blue, 3))
            {
                graphics.DrawLine(
                    pen,
                    x1,
                    y1,
                    x2,
                    y2);
            }
        }

        private void DrawColumnSmooth(Graphics graphics, SmoothInfo si)
        {
            if (!TryGetValue(si.A.ColumnIndex, si.A.RowIndex, out var valueA))
                return;

            if (!TryGetValue(si.B.ColumnIndex, si.B.RowIndex, out var valueB))
                return;

            using (var pen = new Pen(Color.Blue, 3))
            {
                graphics.DrawLine(
                    pen,
                    GetColumnX(si.MinValue, si.MaxValue, valueA),
                    GetColumnY(si.A.RowIndex),
                    GetColumnX(si.MinValue, si.MaxValue, valueB),
                    GetColumnY(si.B.RowIndex));
            }
        }

        private void DrawRow(Graphics graphics, Pen pen, int row, double min, double max)
        {
            if (!TryGetValue(0, row, out var value))
                return;

            var lastX = dataGrid.RowHeadersWidth;
            var lastY = GetRowY(min, max, value);

            for (var i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (!TryGetValue(i, row, out value))
                    return;

                var nextX = GetRowX(i);
                var nextY = GetRowY(min, max, value);

                if (i != 0)
                {
                    graphics.DrawLine(pen, lastX, lastY, nextX, nextY);
                }

                lastX = nextX;
                lastY = nextY;
            }

            //nextX = this.horizontalPanel.Width;
            //nextY = lastY;
            //graphics.DrawLine(pen, lastX, lastY, nextX, nextY);
        }

        private void DrawColumn(Graphics graphics, Pen pen, int column, double min, double max)
        {
            if (!TryGetValue(column, 0, out var value))
                return;

            var lastX = GetColumnX(min, max, value);
            var lastY = dataGrid.ColumnHeadersHeight;
            for (var i = 0; i < dataGrid.Rows.Count; i++)
            {
                if (!TryGetValue(column, i, out value))
                    return;

                var nextX = GetColumnX(min, max, value);
                var nextY = GetColumnY(i);

                if (i != 0)
                {
                    graphics.DrawLine(pen, lastX, lastY, nextX, nextY);
                }

                lastX = nextX;
                lastY = nextY;
            }

            //nextX = lastX;
            //nextY = this.verticalPanel.Height;
            //graphics.DrawLine(pen, lastX, lastY, nextX, nextY);
        }

        private int GetRowX(int i)
        {
            var width = horizontalPanel.Width - dataGrid.RowHeadersWidth;
            var offset = (width / (dataGrid.Columns.Count * 2)) + dataGrid.RowHeadersWidth;

            return ((width * i) / (dataGrid.Columns.Count)) + offset;
        }

        private int GetRowY(double min, double max, double value)
        {
            var difference = max - min;
            var magnitude = difference == 0 ? 0.5 : (max - value) / difference;
            var result = magnitude * (horizontalPanel.Height * 0.8);
            return (int)result + (int)(horizontalPanel.Height * 0.1);
        }

        private int GetColumnX(double min, double max, double value)
        {
            var difference = max - min;
            var magnitude = difference == 0 ? 0.5 : (max - value) / difference;
            var result = magnitude * (verticalPanel.Width * 0.8);
            return (int)result + (int)(verticalPanel.Width * 0.1);
        }

        private int GetColumnY(int i)
        {
            var height = verticalPanel.Height - dataGrid.ColumnHeadersHeight;
            var offset = (height / (dataGrid.Columns.Count * 2)) + dataGrid.ColumnHeadersHeight;
            return ((height * i) / (dataGrid.Rows.Count)) + offset;
        }
    }
}