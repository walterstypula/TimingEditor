using NSFW.TimingEditor.Controls;
using NSFW.TimingEditor.Enums;
using NSFW.TimingEditor.Extensions;
using NSFW.TimingEditor.Tables;
using NSFW.TimingEditor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public partial class TimingForm : Form
    {
        private readonly TuningTables _tables = new TuningTables();
        private bool _changingTables;
        private bool _inCellMouseEnter;
        private int _selectedColumn;
        private int _selectedRow;
        private int _advancePadding;
        private bool _editControlKeyDownSubscribed;
        private Overlay _overlay;
        private bool _isMaf;
        private readonly List<TableListEntry> _tableListEntries = new List<TableListEntry>();

        public TimingForm()
        {
            InitializeComponent();
            smoothToolStripMenuItem.Enabled = true;
        }

        private void CommandHistory_UpdateButtons(object sender, EventArgs args)
        {
            undoToolStripMenuItem.Enabled = CommandHistory.Instance.CanUndo;
            redoToolStripMenuItem.Enabled = CommandHistory.Instance.CanRedo;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dataGrid.Scroll += DataGrid_Scroll;

            CommandHistory.Instance.UpdateCommandHistoryButtons += CommandHistory_UpdateButtons;
            CommandHistory_UpdateButtons(null, null);

            _tableListEntries.Add(new TableListEntry(_tables.InitialBaseTiming, "Initial base timing", true,
                "When you paste into this table, the modified base timing table will also be initialized with the same data.", TuningMode.Timing));
            _tableListEntries.Add(new TableListEntry(_tables.InitialAdvanceTiming, "Initial advance timing", true,
                "When you paste into this table, the modified advance timing table will also be initialized with the same data.", TuningMode.Timing));
            _tableListEntries.Add(new TableListEntry(_tables.InitialTotalTiming, "Initial total timing", false,
                "You cannot edit this table.", TuningMode.Timing));
            _tableListEntries.Add(new TableListEntry(_tables.ModifiedBaseTiming, "Modified base timing", true,
                "", TuningMode.Timing));
            _tableListEntries.Add(new TableListEntry(_tables.ModifiedAdvanceTiming, "Modified advance timing", true,
                "The base timing will be adjusted when you change cells in this table, so that the total timing does not change.", TuningMode.Timing));
            _tableListEntries.Add(new TableListEntry(_tables.ModifiedTotalTiming, "Modified total timing", false,
                "When you edit cells in this table, the changes are actually made to the base timing table.", TuningMode.Timing));
            _tableListEntries.Add(new TableListEntry(_tables.DeltaTotalTiming, "Delta total timing", false,
                "This table shows the difference between the initial total timing and the modified total timing.", TuningMode.Timing));
            _tableListEntries.Add(new TableListEntry(_tables.TargetFuel, "Target Fuel Map", true,
                "This table is the Target Fuel table used for Maf adjustments.", TuningMode.Both));
            _tableListEntries.Add(new TableListEntry(_tables.InitialMaf, "Maf", true,
                "This table is Maf.", TuningMode.Maf));
            _tableListEntries.Add(new TableListEntry(_tables.ModifiedMaf, "Modified Maf", true,
                "This table is Maf adjustments.", TuningMode.Maf));
            _tableListEntries.Add(new TableListEntry(_tables.DeltaMaf, "Delta Maf", true,
                "This table shows the difference between Maf and Modified Maf adjustments.", TuningMode.Maf));

            tableList.Items.AddRange(_tableListEntries.Where(p => p.TuningMode == TuningMode.Timing || p.TuningMode == TuningMode.Both).ToArray());

            try
            {
                using (var file = new FileStream("Files\\tableTimingBase.txt", FileMode.Open))
                {
                    var reader = new StreamReader(file);
                    var content = reader.ReadToEnd();
                    Util.LoadTable(content, _tables.InitialBaseTiming);
                    _tables.InitialBaseTiming.IsReadOnly = true;
                    Util.LoadTable(content, _tables.ModifiedBaseTiming);
                }
                using (var file = new FileStream("Files\\tableTimingAdvance.txt", FileMode.Open))
                {
                    var reader = new StreamReader(file);
                    var content = reader.ReadToEnd();
                    Util.LoadTable(content, _tables.InitialAdvanceTiming);
                    _tables.InitialAdvanceTiming.IsReadOnly = true;
                    Util.LoadTable(content, _tables.ModifiedAdvanceTiming);
                }
                using (var file = new FileStream("Files\\tableFuelBase.txt", FileMode.Open))
                {
                    var reader = new StreamReader(file);
                    var content = reader.ReadToEnd();
                    Util.LoadTable(content, _tables.TargetFuel);
                    _tables.TargetFuel.IsReadOnly = false;
                }
                using (var file = new FileStream("Files\\tableMafBase.txt", FileMode.Open))
                {
                    var reader = new StreamReader(file);
                    var content = reader.ReadToEnd();
                    Util.LoadTable(content, _tables.InitialMaf);
                    _tables.InitialMaf.IsReadOnly = true;
                    Util.LoadTable(content, _tables.ModifiedMaf);
                }
                TableList_SelectedIndexChanged(null, null);
            }
            catch (IOException)
            {
            }
            catch (ApplicationException)
            {
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            tableList.SelectedIndex = 0;
        }

        private void DataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            flowLayoutPanel1.HorizontalScroll.Value = e.NewValue;
        }

        private void SetXAxisOverlay(string xAxisHeader)
        {
            string engLoad = null;

            var headers = new List<string>();

            foreach (string s in xAxisComboBox.Items)
            {
                headers.Add(s);
            }

            foreach (var s in headers)
            {
                if (Regex.IsMatch(s, xAxisHeader, RegexOptions.IgnoreCase) || s == xAxisHeader)
                {
                    engLoad = s;
                    break;
                }
            }

            if (engLoad != null)
            {
                xAxisComboBox.SelectedItem = engLoad;
            }
        }

        private void TableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            smoothToolStripMenuItem.Enabled = !entry.Table.IsReadOnly;
            autoTuneToolStripMenuItem.Enabled = _isMaf && entry.Table.Is2DTable && !entry.Table.IsReadOnly;
            autoTuneTypeToolStripMenuItem.Enabled = autoTuneToolStripMenuItem.Enabled;

            var title = $"Timing Editor: {entry.Description}";
            pasteToolStripMenuItem.Enabled = entry.AllowPaste;
            statusStrip1.Items[0].Text = entry.StatusText;

            Text = title;

            if (entry.Table.IsPopulated)
            {
                try
                {
                    List<int[]> selectedIndices = new List<int[]>(dataGrid.SelectedCells.Count);
                    DataGridViewSelectedCellCollection selected = dataGrid.SelectedCells;
                    foreach (DataGridViewCell cell in selected)
                    {
                        selectedIndices.Add(new[] { cell.ColumnIndex, cell.RowIndex });
                    }

                    dataGrid.ReadOnly = entry.Table.IsReadOnly;
                    _changingTables = true;
                    Util.ShowTable(this, entry.Table, dataGrid);
                    dataGrid.ClearSelection();
                    DrawSideViews(_selectedColumn, _selectedRow);
                    Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);

                    _changingTables = false;
                    foreach (int[] pair in selectedIndices)
                    {
                        if (dataGrid.Rows.Count <= pair[1] || dataGrid.Rows[pair[1]].Cells.Count <= pair[0])
                        {
                            continue;
                        }

                        dataGrid.Rows[pair[1]].Cells[pair[0]].Selected = true;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    dataGrid.ClearSelection();
                    dataGrid.Rows.Clear();
                    statusStrip1.Items[0].Text = @"This only works if the base and advance tables are the same size";
                }
            }
            else
            {
                _changingTables = true;
                dataGrid.Columns.Clear();
                _changingTables = false;
            }

            SetXAxisOverlay(entry.Table.XAxisHeader);
        }

        private void CopyClick(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            var copyFrom = entry.Table;
            if ((entry.Table == _tables.InitialAdvanceTiming) || (entry.Table == _tables.ModifiedAdvanceTiming))
            {
                if (_advancePadding > 0)
                {
                    var message = $"The advance table will have the leftmost {_advancePadding} columns removed.";
                    MessageBox.Show(this, message, @"Timing Editor", MessageBoxButtons.OK);
                    copyFrom = Util.TrimLeft(copyFrom, _advancePadding);
                }
            }

            var text = Util.CopyTable(copyFrom);
            Clipboard.SetData(DataFormats.Text, text);
        }

        private void PasteClick(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            try
            {
                var tableText = Clipboard.GetData(DataFormats.Text) as string;
                if (string.IsNullOrEmpty(tableText))
                {
                    throw new ApplicationException("Doesn't contain text.");
                }

                _changingTables = true;
                var wasReadOnly = entry.Table.IsReadOnly;
                if (wasReadOnly)
                {
                    entry.Table.IsReadOnly = false;
                }

                var temporaryTable = new Table();
                Util.LoadTable(tableText, temporaryTable);

                if ((entry.Table == _tables.InitialAdvanceTiming) || (entry.Table == _tables.ModifiedAdvanceTiming))
                {
                    if (temporaryTable.ColumnHeaders.Count < _tables.InitialBaseTiming.ColumnHeaders.Count)
                    {
                        MessageBox.Show(this, @"The advance table will have values added to the left side, to make it align with the base timing table.", @"Timing Editor", MessageBoxButtons.OK);
                        _advancePadding = _tables.InitialBaseTiming.ColumnHeaders.Count - temporaryTable.ColumnHeaders.Count;
                        temporaryTable = Util.PadLeft(temporaryTable, _tables.InitialBaseTiming.ColumnHeaders.Count);
                    }
                    else
                    {
                        _advancePadding = 0;
                    }
                }

                temporaryTable.CopyTo(entry.Table);

                if (wasReadOnly)
                {
                    entry.Table.IsReadOnly = true;
                }
                Util.ShowTable(this, entry.Table, dataGrid);
                Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);
                dataGrid.ClearSelection();
                _changingTables = false;

                if (entry.Table == _tables.InitialBaseTiming)
                {
                    temporaryTable.CopyTo(_tables.ModifiedBaseTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedBaseTiming);
                }

                if (entry.Table == _tables.InitialAdvanceTiming)
                {
                    temporaryTable.CopyTo(_tables.ModifiedAdvanceTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedAdvanceTiming);
                }

                if (entry.Table == _tables.InitialMaf)
                {
                    temporaryTable.CopyTo(_tables.ModifiedMaf);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(@"Clipboard does not contain valid table data." + ex.Message);
            }
        }

        private void DataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_changingTables || _inCellMouseEnter || !(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            var table = entry.Table;
            if (table == null || table.IsReadOnly)
            {
                return;
            }

            var cellValue = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            var newStringValue = cellValue as string;

            if (double.TryParse(newStringValue, out var value))
            {
                var edit = new EditCell(table, e.ColumnIndex, e.RowIndex, value);
                CommandHistory.Instance.Execute(edit);
            }

            DrawSideViews(e.ColumnIndex, e.RowIndex);
        }

        private void DataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            SuspendLayout();
            if (dataGrid.ColumnCount > 0 && dataGrid.RowCount > 0)
            {
                _inCellMouseEnter = true;
                _selectedColumn = e.ColumnIndex;
                _selectedRow = e.RowIndex;

                if (!(tableList.SelectedItem is TableListEntry entry))
                {
                    return;
                }

                Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, null);
                _inCellMouseEnter = false;
            }
            DrawSideViews(_selectedColumn, _selectedRow);
            ResumeLayout(false);
            PerformLayout();
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Z:
                    UndoClick(this, e);
                    break;

                case Keys.Y:
                    RedoClick(this, e);
                    break;
            }
        }

        private void DataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (_editControlKeyDownSubscribed)
            {
                return;
            }

            e.Control.KeyDown += DataGridEditControl_KeyDown;
            _editControlKeyDownSubscribed = true;
        }

        private void DataGridEditControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.OemMinus:
                    Delta(-0.35);
                    e.Handled = true;
                    dataGrid.CancelEdit();
                    dataGrid.EndEdit();
                    break;

                case Keys.Oemplus:
                    Delta(+0.35);
                    e.Handled = true;
                    dataGrid.CancelEdit();
                    dataGrid.EndEdit();
                    break;
            }
        }

        private void Delta(double delta)
        {
            foreach (DataGridViewCell cell in dataGrid.SelectedCells)
            {
                if (!double.TryParse(cell.Value.ToString(), out var value))
                {
                    continue;
                }

                value += delta;
                cell.Value = value.ToString();
            }
        }

        private bool TryGetValue(int x, int y, out double value)
        {
            value = 0;
            var o = dataGrid.Rows[y].Cells[x].Value;
            return o != null && double.TryParse(o.ToString(), out value);
        }

        private void RedoClick(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            CommandHistory.Instance.Redo();

            _changingTables = true;

            Util.ShowTable(this, entry.Table, dataGrid);
            dataGrid.ClearSelection();
            if (entry.Table == _tables.InitialAdvanceTiming || entry.Table == _tables.ModifiedAdvanceTiming
                || entry.Table == _tables.InitialBaseTiming || entry.Table == _tables.ModifiedBaseTiming)
            {
                Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);
            }
            else
            {
                Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, null);
            }

            _changingTables = false;

            DrawSideViews(_selectedColumn, _selectedRow);
        }

        private void UndoClick(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            CommandHistory.Instance.Undo();

            _changingTables = true;
            Util.ShowTable(this, entry.Table, dataGrid);
            dataGrid.ClearSelection();
            Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);

            _changingTables = false;

            DrawSideViews(_selectedColumn, _selectedRow);
        }

        private void DataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGrid.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                return;
            }

            dataGrid.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
            dataGrid.Columns[e.ColumnIndex].Selected = true;
        }

        private void DataGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGrid.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
            {
                return;
            }

            dataGrid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            dataGrid.Rows[e.RowIndex].Selected = true;
        }

        private void InitializeForm(string[] headers)
        {
            if (headers.Length <= 0)
            {
                return;
            }

            lbOverlayHeaders.Items.Clear();
            xAxisComboBox.Items.Clear();
            yAxisComboBox.Items.Clear();

            string engLoad = null;
            string engSpeed = null;
            foreach (var s in headers)
            {
                lbOverlayHeaders.Items.Add(s);
                xAxisComboBox.Items.Add(s);
                yAxisComboBox.Items.Add(s);

                if (_isMaf && Regex.IsMatch(s, RequiredLogHeaders.MafvRegEx, RegexOptions.IgnoreCase))
                {
                    engLoad = s;
                }
                else if (Regex.IsMatch(s, RequiredLogHeaders.EngineLoadRegEx, RegexOptions.IgnoreCase))
                {
                    engLoad = s;
                }
                else if (Regex.IsMatch(s, RequiredLogHeaders.RpmRegEx, RegexOptions.IgnoreCase))
                {
                    engSpeed = s;
                }
            }

            _overlay.SetHeaders(engLoad, engSpeed);

            if (engLoad != null)
            {
                SetSelectedItem(xAxisComboBox, engLoad);
            }

            if (engSpeed != null)
            {
                SetSelectedItem(yAxisComboBox, engSpeed);
            }
        }

        private void SetSelectedItem(ComboBox comboBox, string selectedItem)
        {
            comboBox.Tag = false;
            comboBox.SelectedItem = selectedItem;
            comboBox.Tag = null;
        }

        public string[] SelectedLogParameters
        {
            get
            {
                if (lbOverlayHeaders.CheckedItems.Count <= 0)
                {
                    return new string[0];
                }

                int i = 0;
                string[] s = new string[lbOverlayHeaders.CheckedItems.Count];

                foreach (object o in lbOverlayHeaders.CheckedItems)
                {
                    s[i++] = o.ToString();
                }

                return s;
            }
        }

        public string XAxis
        {
            get { return xAxisComboBox.SelectedItem?.ToString(); }
        }

        public string YAxis
        {
            get { return yAxisComboBox.SelectedItem?.ToString(); }
        }

        private void LogOverlayButton_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog();
            file.Multiselect = true;

            if (file.ShowDialog() != DialogResult.OK)
            { return; }

            try
            {
                var files = file.FileNames;

                var filters = AppSettings.LogFilters;

                _overlay = new Overlay(filters, files);
                InitializeForm(_overlay.Headers.ToArray());

                _changingTables = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
        }

        private string FilterLog(string headers, string content, Dictionary<string, double> filters)
        {
            StringBuilder sb = new StringBuilder();

            var splitHeaders = headers.Split(",".ToCharArray());

            Dictionary<int, double> indexFilters = new Dictionary<int, double>();
            foreach (var filter in filters)
            {
                var index = splitHeaders.IndexOf(filter.Key);
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

        private void AutoTune_Click(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry) || entry.Table != _tables.ModifiedMaf)
            {
                return;
            }

            var afrCorrectionHeaders = _overlay.Headers.Where(h => Regex.IsMatch(h, RequiredLogHeaders.AfCorrectionRegEx, RegexOptions.IgnoreCase) ||
                                        Regex.IsMatch(h, RequiredLogHeaders.AfLearningRegEx, RegexOptions.IgnoreCase)).ToList();
            var existingHeaders = afrCorrectionHeaders.ToList();
            existingHeaders.Add(AppSettings.AutoTuneAfrSource);
            existingHeaders.AddRange(SelectedLogParameters);

            _overlay.AddHeaderInfo(existingHeaders.ToArray());
            var overlayPoints = _overlay.ProcessOverlay(_tables.ModifiedMaf.ColumnHeaders, _tables.ModifiedMaf.RowHeaders);

            foreach (var op in overlayPoints)
            {
                var allCurrentAfrData = op.ValueData[AppSettings.AutoTuneAfrSource];
                var validAutoTuneAfrData = allCurrentAfrData;// allCurrentAfrData.SkipOutliers(k: 3, selector: result => result.Value);

                if (validAutoTuneAfrData.Count() <= 1)
                {
                    continue;
                }

                var afrErrorList = new List<double>();

                for (int i = 0; i < validAutoTuneAfrData.Count; i++)
                {
                    if (autoTuneTypeToolStripMenuItem.SelectedIndex == 0)
                    {
                        var currentAfr = validAutoTuneAfrData[i];

                        var targetAfr = (_tables.TargetFuel as Table).InterpolateXY(currentAfr.Load, currentAfr.Rpm);

                        //var xTargetAfrValue = _tables.TargetFuel.ColumnHeaders.ClosestValueIndex(currentAfr.Load);
                        //var yTargetAfrValue = _tables.TargetFuel.RowHeaders.ClosestValueIndex(currentAfr.Rpm);
                        //var targetAfr = _tables.TargetFuel.GetCell(xTargetAfrValue, yTargetAfrValue);

                        afrErrorList.Add(CalcAfrError(currentAfr.Value, targetAfr));
                    }
                    else
                    {
                        var afr1 = op.ValueData[afrCorrectionHeaders[0]][i].Value;
                        var afr2 = op.ValueData[afrCorrectionHeaders[1]][i].Value;
                        afrErrorList.Add(afr1 + afr2);
                    }
                }

                var targetCell = dataGrid[op.ColumnIndex, op.RowIndex] as CustomDataGridViewCell;

                afrErrorList = afrErrorList.SkipOutliers(k: 3, selector: result => result).ToList();

                var avgAfrError = afrErrorList.Average();

                var currentValue = double.Parse(targetCell.Value.ToString());
                var correction = currentValue * 0.01;

                var newValue = currentValue + avgAfrError * correction;
                targetCell.Value = newValue.ToString();
            }

            _overlay.AddHeaderInfo(SelectedLogParameters);
        }

        private static double CalcAfrError(double currentAfr, double targetAfr)
        {
            return (currentAfr - targetAfr) / targetAfr * 100;
        }

        private void FlowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            dataGrid.HorizontalScrollingOffset = e.NewValue;
        }

        private void TuningModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableList.Items.Clear();
            _isMaf = !_isMaf;

            if (_isMaf)
            {
                tableList.Items.AddRange(_tableListEntries.Where(p => p.TuningMode == TuningMode.Maf || p.TuningMode == TuningMode.Both).ToArray());
                tuningModeToolStripMenuItem.Text = "Timing Tuning";
            }
            else
            {
                tableList.Items.AddRange(_tableListEntries.Where(p => p.TuningMode == TuningMode.Timing || p.TuningMode == TuningMode.Both).ToArray());
                tuningModeToolStripMenuItem.Text = "MAF Tuning";
            }

            tableList.SelectedIndex = 0;
            autoTuneToolStripMenuItem.Visible = _isMaf;
            autoTuneToolStripMenuItem.Enabled = _isMaf;
            autoTuneTypeToolStripMenuItem.Visible = _isMaf;
            autoTuneTypeToolStripMenuItem.Enabled = _isMaf;
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyClick(sender, e);
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClick(sender, e);
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoClick(sender, e);
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedoClick(sender, e);
        }

        private void LogOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogOverlayButton_Click(sender, e);
        }

        private void LbOverlayHeaders_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var clb = sender as CheckedListBox;

            var items = SelectedLogParameters.ToList();
            var newItem = clb.Items[e.Index] as string;

            if (e.NewValue == CheckState.Checked && !items.Contains(newItem))
            {
                items.Add(newItem);
            }
            else if (e.NewValue == CheckState.Unchecked && items.Contains(newItem))
            {
                items.Remove(newItem);
            }

            _overlay.AddHeaderInfo(items.ToArray());

            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            if (!entry.Table.IsPopulated)
            {
                MessageBox.Show(@"Error: Please populate table first");
                return;
            }

            Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);

            //dataGrid.Refresh();
            _changingTables = false;

            if (dataGrid.SelectedCells.Count != 1 || !(dataGrid.SelectedCells[0] is CustomDataGridViewCell item))
            {
                return;
            }

            rtbOverlayCellData.Text = item.PointData.ToString();
        }

        private void XAxisComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            entry.Table.XAxisHeader = XAxis;
            _overlay.SetHeaders(XAxis, YAxis);

            if (!entry.Table.IsPopulated)
            {
                MessageBox.Show(@"Error: Please populate table first");
                return;
            }
            Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);
        }

        private void YAxisComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            _overlay.SetHeaders(XAxis, YAxis);

            if (!entry.Table.IsPopulated)
            {
                MessageBox.Show(@"Error: Please populate table first");
                return;
            }
            Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);
        }

        private void DataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGrid.SelectedCells.Count != 1 || !(dataGrid.SelectedCells[0] is CustomDataGridViewCell item))
            {
                return;
            }

            rtbOverlayCellData.Text = item.PointData.ToString();
        }

        private void AutoTuneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("By continuing you (the application user) use the software at YOUR OWN RISK, and absolve the application author from any and all damages or harm the use of this application may cause.\r\n\r\nThe application user understands this feature is meant as a guide, and application generated values are not to be used.\r\n\r\nWould you like to continue?", "Continue?", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                AutoTune_Click(sender, e);
            }
        }

        private void SmoothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SmoothButton_Click(sender, e);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((tableList.SelectedItem as TableListEntry).Table == _tables.ModifiedBaseTiming)
            {
                System.IO.File.WriteAllText(@"Files\\tableTimingBase.txt", Util.CopyTable(_tables.ModifiedBaseTiming));
            }
            else if ((tableList.SelectedItem as TableListEntry).Table == _tables.ModifiedAdvanceTiming)
            {
                System.IO.File.WriteAllText(@"Files\\tableTimingAdvance.txt", Util.CopyTable(_tables.ModifiedAdvanceTiming));
            }
            else if ((tableList.SelectedItem as TableListEntry).Table == _tables.TargetFuel)
            {
                System.IO.File.WriteAllText(@"Files\\tableFuelBase.txt", Util.CopyTable(_tables.TargetFuel));
            }
            else if ((tableList.SelectedItem as TableListEntry).Table == _tables.ModifiedMaf)
            {
                System.IO.File.WriteAllText(@"Files\\tableMafBase.txt", Util.CopyTable(_tables.ModifiedMaf));
            }
        }

        private void HorizontalPanel_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}