using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSFW.TimingEditor.Extensions
{
    public static class Extensions
    {
        public static int FindIndexOf(this String[] splitLine, String columnName)
        {
            int itemIndex = -1;

            var foundItem = splitLine.Select((item, index) => new { ItemName = item, Position = index }).Where(i => i.ItemName == columnName).FirstOrDefault();

            if (foundItem != null)
            {
                itemIndex = foundItem.Position;
            }

            return itemIndex;
        }

        public static double ToDouble(this String value)
        {
            double outputValue = 0.0;

            double.TryParse(value, out outputValue);

            return outputValue;
        }

        public static Int32 ToInt(this String value)
        {
            int outputValue = 0;

            int.TryParse(value, out outputValue);

            return outputValue;
        }
    }

}
