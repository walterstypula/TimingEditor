using System;
using System.Collections.Generic;
using System.Configuration;

namespace NSFW.TimingEditor
{
    public static class AppSettings
    {
        private static List<KeyValuePair<string, KeyValuePair<string, double>>> _logFilters;

        public static List<KeyValuePair<string, KeyValuePair<string, double>>> LogFilters
        {
            get
            {
                return _logFilters ?? ProcessLogFilters();
            }
        }

        private static List<KeyValuePair<string, KeyValuePair<string, double>>> ProcessLogFilters()
        {
            _logFilters = new List<KeyValuePair<string, KeyValuePair<string, double>>>();
            var rawFilters = ConfigurationManager.AppSettings["LogFilters"];
            var filters = rawFilters.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var fitler in filters)
            {
                var item = fitler.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var compare = item[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (double.TryParse(compare[1], out var value))
                {
                    var field = item[0];

                    var pairValue = new KeyValuePair<string, double>(compare[0], value);

                    _logFilters.Add(new KeyValuePair<string, KeyValuePair<string, double>>(field, pairValue));
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