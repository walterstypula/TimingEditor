using NSFW.TimingEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NSFW.TimingEditor
{
    internal class OverlayHeaderInfo
    {
        internal string XAxisHeader;
        internal string YAxisHeader;
        internal int XAxisHeaderIndex = -1;
        internal int YAxisHeaderIndex = -1;
        internal readonly Dictionary<string, int> HeaderIndices = new Dictionary<string, int>();
        private readonly string[] _headers;

        internal OverlayHeaderInfo(string logHeaderLine)
        {
            if (logHeaderLine.Length <= 0)
            {
                throw new ApplicationException($"First line in log file does not contains headers.");
            }

            _headers = logHeaderLine.Split(',');
        }

        internal OverlayHeaderInfo(string logHeaderLine, string xAxisHeader, string yAxisHeader)
            : this(logHeaderLine)
        {
            SetAxisHeaders(xAxisHeader, yAxisHeader);
        }

        internal void SetAxisHeaders(string xAxisHeader, string yAxisHeader)
        {
            XAxisHeaderIndex = _headers.IndexOf(xAxisHeader);
            YAxisHeaderIndex = _headers.IndexOf(yAxisHeader);

            if (YAxisHeaderIndex == -1 || XAxisHeaderIndex == -1)
            {
                throw new ApplicationException($"Either {XAxisHeader} or {YAxisHeader} headers not found in log file.");
            }

            XAxisHeader = xAxisHeader;
            YAxisHeader = yAxisHeader;
        }

        internal void AddHeaderInfo(params string[] displayDataHeaders)
        {
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

        public bool SetXAvisHeader(string regEx)
        {
            if (regEx == null)
                return false;

            var result = _headers.FirstOrDefault(header => Regex.IsMatch(header, regEx, RegexOptions.IgnoreCase));

            if (result == null)
                return false;

            SetAxisHeaders(result, YAxisHeader);
            return true;
        }
    }

    public class Overlay
    {
        private readonly OverlayHeaderInfo _overlayHeaders;
        private readonly StringBuilder _logData = new StringBuilder();

        public Overlay(string logHeaderLine, string xAxisHeader, string yAxisHeader)
        {
            _overlayHeaders = new OverlayHeaderInfo(logHeaderLine, xAxisHeader, yAxisHeader);
        }

        public bool SetXAvisHeader(string regEx)
        {
            return _overlayHeaders.SetXAvisHeader(regEx);
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
                var xAxisValue = lineArray[_overlayHeaders.XAxisHeaderIndex];
                var yAxisValue = lineArray[_overlayHeaders.YAxisHeaderIndex];

                var xIndex = columnHeaderValues.ClosestValueIndex(xAxisValue);
                var yIndex = rowHeaderValues.ClosestValueIndex(yAxisValue);
                var xAxisValueRef = columnHeaderValues[xIndex];
                var yAxisValueRef = rowHeaderValues[yIndex];

                var point = list.FirstOrDefault(p => p.XAxisIndex == xIndex && p.YAxisIndex == yIndex)
                                ?? new OverlayPoint(xIndex, yIndex, xAxisValueRef, yAxisValueRef);

                if (!list.Contains(point))
                {
                    list.Add(point);
                }

                foreach (var header in _overlayHeaders.HeaderIndices)
                {
                    var value = lineArray[header.Value];
                    point.AddData(header.Key, $"{yAxisValue} {xAxisValue} {value}");
                }
            }

            return list;
        }

        public void AddHeaderInfo(params string[] displayDataHeaders)
        {
            _overlayHeaders.AddHeaderInfo(displayDataHeaders);
        }

        public void AddLog(string content)
        {
            _logData.AppendLine(content.Trim());
        }
    }

    public class OverlayPoint
    {
        public int XAxisIndex { get; }
        public int YAxisIndex { get; }

        public double XAxisValue { get; }
        public double YAxisValue { get; }

        public readonly Dictionary<string, List<string>> LogData = new Dictionary<string, List<string>>();

        public OverlayPoint(int xAxisIndex, int yAxisIndex, double xAvisValue, double yAxisValue)
        {
            XAxisIndex = xAxisIndex;
            YAxisIndex = yAxisIndex;
            XAxisValue = xAvisValue;
            YAxisValue = yAxisValue;
        }

        public void AddData(string header, string value)
        {
            if (!LogData.ContainsKey(header))
            {
                LogData.Add(header, new List<string>() { value });
            }
            else
            {
                LogData[header].Add(value);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var dataPoint in LogData)
            {
                sb.AppendLine(dataPoint.Key);
                foreach (var v in dataPoint.Value)
                {
                    sb.AppendLine($"   {v}");
                }
            }

            return sb.ToString();
        }
    }
}