using NSFW.TimingEditor.Extensions;
using NSFW.TimingEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSFW.TimingEditor
{
    public class Overlay
    {
        private readonly StringBuilder _logData = new StringBuilder();
        private OverlayHeaderInfo _overlayHeaders;

        public Overlay(string file, Dictionary<string, double> filters)
        {
            ProcessFile(file, filters);
        }

        public List<string> Headers { get; } = new List<string>();
        public void AddHeaderInfo(params string[] displayDataHeaders)
        {
            _overlayHeaders.AddHeaderInfo(displayDataHeaders);
        }

        public void AddLog(string content)
        {
            _logData.AppendLine(content.Trim());
        }

        public List<OverlayPoint> ProcessOverlay(IEnumerable<double> columnHeaderValues, IEnumerable<double> rowHeaderValues)
        {
            return ProcessOverlay(columnHeaderValues.ToArray(), rowHeaderValues.ToArray());
        }

        public List<OverlayPoint> ProcessOverlay(double[] columnHeaderValues, double[] rowHeaderValues)
        {
            var list = new List<OverlayPoint>();
            var sr = new StringReader(_logData.ToString());

            while (true)
            {
                var line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                var lineArray = line.Split(',');
                var xAxisValue = lineArray[_overlayHeaders.RowHeaderIndex];
                var yAxisValue = lineArray[_overlayHeaders.ColumnHeaderIndex];

                var xIndex = columnHeaderValues.ClosestValueIndex(xAxisValue);
                var yIndex = rowHeaderValues.ClosestValueIndex(yAxisValue);
                var xAxisValueRef = columnHeaderValues[xIndex];
                var yAxisValueRef = rowHeaderValues[yIndex];

                var rpm = lineArray[_overlayHeaders.RpmIndex];
                var load = lineArray[_overlayHeaders.EngineLoadIndex];
                var mafv = lineArray[_overlayHeaders.MafvIndex];

                var point = list.FirstOrDefault(p => p.RowIndex == xIndex && p.ColumnIndex == yIndex)
                                ?? new OverlayPoint(xIndex, yIndex, xAxisValueRef, yAxisValueRef);

                if (!list.Contains(point))
                {
                    list.Add(point);
                }

                foreach (var header in _overlayHeaders.HeaderIndices)
                {
                    var value = lineArray[header.Value];
                    point.AddData(header.Key, double.Parse(rpm), double.Parse(load), double.Parse(mafv), double.Parse(value));
                }
            }

            return list;
        }

        public void SetHeaders(string xAxisHeader, string yAxisHeader)
        {
            if (xAxisHeader == null || yAxisHeader == null)
            {
                return;
            }

            _overlayHeaders.SetHeaders(xAxisHeader, yAxisHeader);
        }

        public bool SetRowHeader(string regEx)
        {
            return _overlayHeaders.SetRowHeader(regEx);
        }

        private string FilterLog(string[] headers, string content, Dictionary<string, double> filters)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<int, double> indexFilters = new Dictionary<int, double>();
            foreach (var filter in filters)
            {
                var index = headers.IndexOf(filter.Key);
                indexFilters.Add(index, filter.Value);
            }

            using (TextReader sr = new StringReader(content))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var splitLine = line.Split(",".ToCharArray());

                    var allFiltersMet = indexFilters.All(k => splitLine[k.Key].ToDouble() >= k.Value);

                    if (allFiltersMet == true)
                    {
                        sb.AppendLine(line);
                    }
                }
            }

            return sb.ToString().Trim();
        }

        private void ProcessFile(string file, Dictionary<string, double> filters)
        {
            using (var overlayStream = new StreamReader(file, Encoding.Default))
            {
                var line = overlayStream.ReadLine();

                if (line == null)
                {
                    return;
                }

                var headers = line.Split(',')
                                  .Select(s => s.Trim())
                                  .ToArray();

                if (Headers.Count == 0)
                {
                    Headers.AddRange(headers);
                }
                else if (headers.Length != Headers.Count || !headers.All(h => Headers.Exists(e => e == h)))
                {
                    return;
                }	  							 

                _overlayHeaders = new OverlayHeaderInfo(headers);

                var content = overlayStream.ReadToEnd();

                var filteredContent = FilterLog(headers, content, filters);
                AddLog(filteredContent);
            }
        }
    }

    public class OverlayPoint
    {
        public readonly Dictionary<string, List<string>> LogData = new Dictionary<string, List<string>>();
        public readonly Dictionary<string, List<TableData>> ValueData = new Dictionary<string, List<TableData>>();

        public OverlayPoint(int xAxisIndex, int yAxisIndex, double xAvisValue, double yAxisValue)
        {
            RowIndex = xAxisIndex;
            ColumnIndex = yAxisIndex;
            RowValue = xAvisValue;
            ColumnValue = yAxisValue;
        }

        public int ColumnIndex { get; }
        public double ColumnValue { get; }
        public int RowIndex { get; }
        public double RowValue { get; }

        public void AddData(string header, double rpm, double load, double mafv, double value)
        {
            var compositeData = $"{rpm.ToString().PadRight(4)} {load.ToString().PadRight(4)} {mafv.ToString().PadRight(4)} {value.ToString().PadRight(4)}";

            if (!LogData.ContainsKey(header))
            {
                LogData.Add(header, new List<string>() { compositeData });
                ValueData.Add(header, new List<TableData>() { new TableData() { Load = load, MafV = mafv, Rpm = rpm, Value = value } });
            }
            else
            {
                LogData[header].Add(compositeData);
                ValueData[header].Add(new TableData() { Load = load, MafV = mafv, Rpm = rpm, Value = value });
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var dataPoint in LogData)
            {
                sb.AppendLine(dataPoint.Key);
                var average = ValueData[dataPoint.Key].Average(p => p.Value);
                sb.AppendLine($"AVG: {average}");
                foreach (var v in dataPoint.Value)
                {
                    sb.AppendLine($"   {v}");
                }
            }

            return sb.ToString();
        }
    }

    public class TableData
    {
        public double Load { get; set; }
        public double MafV { get; set; }
        public double Rpm { get; set; }
        public double Value { get; set; }
    }

    internal class OverlayHeaderInfo
    {
        internal readonly Dictionary<string, int> HeaderIndices = new Dictionary<string, int>();
        internal string ColumnHeader;
        internal int ColumnHeaderIndex = -1;
        internal int EngineLoadIndex = -1;
        internal int MafvIndex = -1;
        internal string RowHeader;
        internal int RowHeaderIndex = -1;
        internal int RpmIndex = -1;
        private readonly string[] _headers;

        internal OverlayHeaderInfo(string[] logHeaderLine)
        {
            if (logHeaderLine.Length <= 0)
            {
                throw new ApplicationException($"First line in log file does not contains headers.");
            }

            _headers = logHeaderLine;

            for (var i = 0; i < _headers.Length; i++)
            {
                if (Regex.IsMatch(_headers[i], RequiredLogHeaders.EngineLoadRegEx, RegexOptions.IgnoreCase))
                {
                    EngineLoadIndex = i;
                }
                else if (Regex.IsMatch(_headers[i], RequiredLogHeaders.RpmRegEx, RegexOptions.IgnoreCase))
                {
                    RpmIndex = i;
                }
                else if (Regex.IsMatch(_headers[i], RequiredLogHeaders.MafvRegEx, RegexOptions.IgnoreCase))
                {
                    MafvIndex = i;
                }
            }
        }

        internal OverlayHeaderInfo(string[] logHeaderLine, string xAxisHeader, string yAxisHeader)
            : this(logHeaderLine)
        {
            SetHeaders(xAxisHeader, yAxisHeader);
        }

        public bool SetRowHeader(string regEx)
        {
            if (regEx == null)
                return false;

            var result = _headers.FirstOrDefault(header => Regex.IsMatch(header, regEx, RegexOptions.IgnoreCase));

            if (result == null)
                return false;

            SetHeaders(result, ColumnHeader);
            return true;
        }

        internal void AddHeaderInfo(params string[] displayDataHeaders)
        {
            HeaderIndices.Clear();

            foreach (var header in displayDataHeaders)
            {
                if (HeaderIndices.ContainsKey(header))
                {
                    throw new ApplicationException($"Duplicate header found.");
                }

                var headerIndex = Array.IndexOf(_headers, header);

                if (headerIndex == -1)
                {
                    throw new ApplicationException($"{header} header not found in log file.");
                }

                HeaderIndices.Add(header, headerIndex);
            }
        }

        internal void SetHeaders(string rowHeader, string columnHeader)
        {
            if (rowHeader == null || columnHeader == null)
            {
                return;
            }

            RowHeaderIndex = _headers.IndexOf(rowHeader);
            ColumnHeaderIndex = _headers.IndexOf(columnHeader);

            if (ColumnHeaderIndex == -1 || RowHeaderIndex == -1)
            {
                throw new ApplicationException($"Either {RowHeader} or {ColumnHeader} headers not found in log file.");
            }

            RowHeader = rowHeader;
            ColumnHeader = columnHeader;
        }
    }
}