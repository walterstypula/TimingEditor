using System.Drawing;
using System.Windows.Forms;

namespace NSFW.TimingEditor.Controls
{
    internal class CustomDataGridViewCell : DataGridViewTextBoxCell
    {
        private readonly DataGridViewAdvancedBorderStyle _style;

        public OverlayPoint PointData { get; }

        public CustomDataGridViewCell(OverlayPoint pointData)
        : this()
        {
            PointData = pointData;
        }

        private CustomDataGridViewCell()
        {
            _style = new DataGridViewAdvancedBorderStyle
            {
                Bottom = DataGridViewAdvancedCellBorderStyle.None,
                Top = DataGridViewAdvancedCellBorderStyle.None,
                Left = DataGridViewAdvancedCellBorderStyle.None,
                Right = DataGridViewAdvancedCellBorderStyle.None
            };
        }

        public DataGridViewAdvancedBorderStyle AdvancedBorderStyle
        {
            get { return _style; }
            set
            {
                _style.Bottom = value.Bottom;
                _style.Top = value.Top;
                _style.Left = value.Left;
                _style.Right = value.Right;
            }
        }

        protected override void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle bounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            base.PaintBorder(graphics, clipBounds, bounds, cellStyle, _style);
            using (var p = new Pen(Color.Navy, 5))
            {
                var rect = bounds;
                rect.X = rect.X + 1;
                rect.Y = rect.Y + 1;
                rect.Width -= 4;
                rect.Height -= 4;
                graphics.DrawRectangle(p, rect);
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, _style, paintParts);
            var rect = cellBounds;
            using (var p = new Pen(Color.Navy, 2))
            {
                rect.Width -= 1;
                rect.Height -= 1;
                graphics.DrawRectangle(p, rect);
            }
        }
    }
}