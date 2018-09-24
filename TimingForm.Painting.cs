using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public partial class TimingForm : Form
    {
        private void DrawSideViews(int activeColumn, int activeRow)
        {
            Bitmap horizontalPanelBitmap = new Bitmap(horizontalPanel.Width, horizontalPanel.Height);
            Graphics horizontalPanelBackBuffer = Graphics.FromImage(horizontalPanelBitmap);
            //Graphics horizontalPanelBackBuffer = horizontalPanel.CreateGraphics();

            Bitmap verticalPanelBitmap = new Bitmap(verticalPanel.Width, verticalPanel.Height);
            Graphics verticalPanelBackBuffer = Graphics.FromImage(verticalPanelBitmap);
            //Graphics verticalPanelBackBuffer = verticalPanel.CreateGraphics();

            horizontalPanelBackBuffer.FillRectangle(Brushes.White, horizontalPanel.ClientRectangle);
            verticalPanelBackBuffer.FillRectangle(Brushes.White, verticalPanel.ClientRectangle);

            double min;
            double max;
            GetMinMax(out min, out max);

            Pen pen = Pens.Gray;
            for (int row = 0; row < dataGrid.Rows.Count; row++)
            {
                DrawRow(horizontalPanelBackBuffer, pen, row, min, max);
            }

            for (int column = 0; column < dataGrid.Columns.Count; column++)
            {
                DrawColumn(verticalPanelBackBuffer, pen, column, min, max);
            }

            if ((activeColumn >= 0) && (activeColumn < dataGrid.Columns.Count) &&
                (activeRow >= 0) && (activeRow < dataGrid.Rows.Count))
            {
                using (Pen heavyPen = new Pen(Color.Black, 3))
                {
                    DrawRow(horizontalPanelBackBuffer, heavyPen, activeRow, min, max);
                    DrawColumn(verticalPanelBackBuffer, heavyPen, activeColumn, min, max);
                }

                using (Pen lightPen = new Pen(Color.Gray, 2))
                {
                    int x = GetRowX(activeColumn);
                    horizontalPanelBackBuffer.DrawLine(lightPen, x, 0, x, horizontalPanel.Height);

                    int y = GetColumnY(activeRow);
                    verticalPanelBackBuffer.DrawLine(lightPen, 0, y, verticalPanel.Width, y);
                }
            }

            SmoothInfo si = GetSmoothInfo(min, max);
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

            Graphics graphics = horizontalPanel.CreateGraphics();
            graphics.DrawImage(horizontalPanelBitmap, 0, 0);
            graphics = verticalPanel.CreateGraphics();
            graphics.DrawImage(verticalPanelBitmap, 0, 0);
        }

        private void GetMinMax(out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;
            double value;
            for (int row = 0; row < dataGrid.Rows.Count; row++)
            {
                for (int column = 0; column < dataGrid.Columns.Count; column++)
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
            DataGridViewSelectedCellCollection selected = dataGrid.SelectedCells;
            if (SelectedColumn(selected))
            {
                SmoothInfo result = new SmoothInfo();
                result.MinValue = min;
                result.MaxValue = max;
                IEnumerable<DataGridViewCell> cells = selected.Cast<DataGridViewCell>();
                int minY = cells.Min(cell => cell.RowIndex);
                int maxY = cells.Max(cell => cell.RowIndex);
                result.A = cells.Where(cell => cell.RowIndex == minY).First();
                result.B = cells.Where(cell => cell.RowIndex == maxY).First();
                return result;
            }

            if (SelectedRow(dataGrid.SelectedCells))
            {
                SmoothInfo result = new SmoothInfo();
                result.MinValue = min;
                result.MaxValue = max;
                IEnumerable<DataGridViewCell> cells = selected.Cast<DataGridViewCell>();
                int minX = cells.Min(cell => cell.ColumnIndex);
                int maxX = cells.Max(cell => cell.ColumnIndex);
                result.A = cells.Where(cell => cell.ColumnIndex == minX).First();
                result.B = cells.Where(cell => cell.ColumnIndex == maxX).First();
                return result;
            }

            return null;
        }

        private void DrawRowSmooth(Graphics graphics, SmoothInfo si)
        {
            double valueA, valueB;
            if (!TryGetValue(si.A.ColumnIndex, si.A.RowIndex, out valueA))
            {
                return;
            }

            if (!TryGetValue(si.B.ColumnIndex, si.B.RowIndex, out valueB))
            {
                return;
            }

            float x1 = GetRowX(si.A.ColumnIndex);
            float y1 = GetRowY(si.MinValue, si.MaxValue, valueA);
            float x2 = GetRowX(si.B.ColumnIndex);
            float y2 = GetRowY(si.MinValue, si.MaxValue, valueB);

            using (Pen pen = new Pen(Color.Blue, 3))
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
            double valueA, valueB;
            if (!TryGetValue(si.A.ColumnIndex, si.A.RowIndex, out valueA))
            {
                return;
            }

            if (!TryGetValue(si.B.ColumnIndex, si.B.RowIndex, out valueB))
            {
                return;
            }

            using (Pen pen = new Pen(Color.Blue, 3))
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
            double value;
            if (!TryGetValue(0, row, out value))
            {
                return;
            }

            int lastX = dataGrid.RowHeadersWidth;
            int lastY = GetRowY(min, max, value);

            int nextX;
            int nextY;
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (!TryGetValue(i, row, out value))
                {
                    return;
                }

                nextX = GetRowX(i);
                nextY = GetRowY(min, max, value);

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
            double value;
            if (!TryGetValue(column, 0, out value))
            {
                return;
            }

            int lastX = GetColumnX(min, max, value);
            int lastY = dataGrid.ColumnHeadersHeight;
            int nextX;
            int nextY;
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                if (!TryGetValue(column, i, out value))
                {
                    return;
                }

                nextX = GetColumnX(min, max, value);
                nextY = GetColumnY(i);

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
            int width = horizontalPanel.Width - dataGrid.RowHeadersWidth;
            int offset = (width / (dataGrid.Columns.Count * 2)) + dataGrid.RowHeadersWidth;

            return ((width * i) / (dataGrid.Columns.Count)) + offset;
        }

        private int GetRowY(double min, double max, double value)
        {
            double difference = max - min;
            double magnitude = difference == 0 ? 0.5 : (max - value) / difference;
            double result = magnitude * (horizontalPanel.Height * 0.8);
            return (int)result + (int)(horizontalPanel.Height * 0.1);
        }

        private int GetColumnX(double min, double max, double value)
        {
            double difference = max - min;
            double magnitude = difference == 0 ? 0.5 : (max - value) / difference;
            double result = magnitude * (verticalPanel.Width * 0.8);
            return (int)result + (int)(verticalPanel.Width * 0.1);
        }

        private int GetColumnY(int i)
        {
            int height = verticalPanel.Height - dataGrid.ColumnHeadersHeight;
            int offset = (height / (dataGrid.Columns.Count * 2)) + dataGrid.ColumnHeadersHeight;
            return ((height * i) / (dataGrid.Rows.Count)) + offset;
        }
    }
}