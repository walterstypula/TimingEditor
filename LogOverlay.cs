﻿using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public partial class LogOverlayForm : Form
    {
        private LogOverlayForm()
        {
            InitializeComponent();
        }

        public LogOverlayForm(string[] headers)
            : this()
        {
            InitializeForm(headers);
        }

        private void InitializeForm(string[] headers)
        {
            if (headers.Length <= 0)
            {
                return;
            }

            headerListBox.Items.Clear();
            xAxisComboBox.Items.Clear();
            yAxisComboBox.Items.Clear();

            string engLoad = null;
            string engSpeed = null;
            foreach (string s in headers)
            {
                headerListBox.Items.Add(s);
                xAxisComboBox.Items.Add(s);
                yAxisComboBox.Items.Add(s);
                if (Regex.IsMatch(s, ".*\\bengine[_\\s]load\\b.*", RegexOptions.IgnoreCase))
                {
                    engLoad = s;
                }
                else if (Regex.IsMatch(s, ".*\\b(engine[_\\s]speed|rpm)\\b.*", RegexOptions.IgnoreCase))
                {
                    engSpeed = s;
                }
            }

            xAxisComboBox.SelectedIndex = 0;
            yAxisComboBox.SelectedIndex = 0;

            if (engLoad != null)
            {
                xAxisComboBox.SelectedItem = engLoad;
            }

            if (engSpeed != null)
            {
                yAxisComboBox.SelectedItem = engSpeed;
            }
        }

        public string[] SelectedLogParameters
        {
            get
            {
                if (headerListBox.CheckedItems.Count <= 0)
                {
                    return new string[0];
                }

                int i = 0;
                string[] s = new string[headerListBox.CheckedItems.Count];

                foreach (object o in headerListBox.CheckedItems)
                {
                    s[i++] = o.ToString();
                }

                return s;
            }
        }

        public string XAxis
        {
            get { return xAxisComboBox.SelectedItem.ToString(); }
        }

        public string YAxis
        {
            get { return yAxisComboBox.SelectedItem.ToString(); }
        }
    }
}