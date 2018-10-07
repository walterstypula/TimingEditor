using System;
using System.Collections.Generic;
using System.Linq;

namespace NSFW.TimingEditor.Extensions
{
    /// <summary>
    /// Sourced from https://www.codeproject.com/Tips/602081/Standard-Deviation-Extension-for-Enumerable
    /// Authored by: Jacek Gajek
    /// </summary>
    public static class StandardDeviationEnumerableExtensions
    {
        /// <summary>
        /// Calculates a standard deviation of elements, using a specified selector.
        /// </summary>
        public static double StandardDeviation<T>(this IEnumerable<T> enumerable, Func<T, double> selector)
        {
            double sum = 0;
            var average = enumerable.Average(selector);
            var n = 0;
            foreach (var item in enumerable)
            {
                var diff = selector(item) - average;
                sum += diff * diff;
                n++;
            }
            return n == 0 ? 0 : Math.Sqrt(sum / n);
        }

        /// <summary>
        /// Filters elements to remove outliers. The enumeration will be
        /// selected three times, first to calculate an average, second
        /// for a standard deviation, and third to yield remaining elements. The outliers are these
        /// elements which are further from an average than k*(standard deviation). Set k=3 for
        /// standard three-sigma rule.
        /// </summary>
        public static IEnumerable<T> SkipOutliers<T>(this IEnumerable<T> enumerable, double k, Func<T, double> selector)
        {
            // Duplicating a SD code to avoid calculating an average twice.
            double sum = 0;
            var average = enumerable.Average(selector);
            var n = 0;
            foreach (var item in enumerable)
            {
                var diff = selector(item) - average;
                sum += diff * diff;
                n++;
            }
            var standardDeviation = n == 0 ? 0 : Math.Sqrt(sum / n);
            var delta = k * standardDeviation;
            foreach (var item in enumerable)
            {
                if (Math.Abs(selector(item) - average) <= delta)
                    yield return item;
            }
        }
    }
}