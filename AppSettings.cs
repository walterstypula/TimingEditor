using System;
using System.Collections.Generic;
using System.Configuration;

namespace NSFW.TimingEditor
{
    public static class AppSettings
    {
        private static Dictionary<string, double> _logFilters;

        public static Dictionary<string, double> LogFilters
        {
            get
            {
                return _logFilters ?? ProcessLogFilters();
            }
        }

        private static Dictionary<string, double> ProcessLogFilters()
        {
            _logFilters = new Dictionary<string, double>();
            var rawFilters = ConfigurationManager.AppSettings["LogFilters"];
            var filters = rawFilters.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var fitler in filters)
            {
                var item = fitler.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (double.TryParse(item[1], out var value))
                {
                    var field = item[0];
                    _logFilters.Add(field, value);
                }
            }

            return _logFilters;
        }

        public static string AutoTuneAfrSource
        {
            get
            {
                return ConfigurationManager.AppSettings["AutoTuneAfrSource"];
            }
        }
    }
}