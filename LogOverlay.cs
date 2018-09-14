using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public partial class LogOverlay : Form
    {
        public LogOverlay()
        {
            InitializeComponent();
        }

        public String[] LogParameters
        {
            get
            {
                if (headerListBox.SelectedItems.Count > 0)
                {
                    int i = 0;
                    String[] s = new String[headerListBox.CheckedItems.Count];
                    foreach (Object o in headerListBox.CheckedItems)
                        s[i++] = o.ToString();
                    return s;
                }
                else
                    return new String[0];
            }
            set
            {
                if (value.Length > 0)
                {
                    headerListBox.Items.Clear();
                    xAxisComboBox.Items.Clear();
                    yAxisComboBox.Items.Clear();
                    String engLoad = null;
                    String engSpeed = null;
                    foreach (String s in value)
                    {
                        headerListBox.Items.Add(s);
                        xAxisComboBox.Items.Add(s);
                        yAxisComboBox.Items.Add(s);
                        if (Regex.IsMatch(s, ".*\\bengine[_\\s]load\\b.*", RegexOptions.IgnoreCase))
                            engLoad = s;
                        else if (Regex.IsMatch(s, ".*\\b(engine[_\\s]speed|rpm)\\b.*", RegexOptions.IgnoreCase))
                            engSpeed = s;
                    }
                    if (engLoad != null)
                        xAxisComboBox.SelectedItem = engLoad;
                    else
                        xAxisComboBox.SelectedIndex = 0;
                    if (engSpeed != null)
                        yAxisComboBox.SelectedItem = engSpeed;
                    else
                        yAxisComboBox.SelectedIndex = 0;
                }
            }
        }

        public String XAxis
        {
            get { return xAxisComboBox.SelectedItem.ToString(); }
        }

        public String YAxis
        {
            get { return yAxisComboBox.SelectedItem.ToString(); }
        }
    }
}
