using NSFW.TimingEditor.Enums;
using NSFW.TimingEditor.Tables;
using NSFW.TimingEditor.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        private List<OverlayPoint> _overlay;
        private bool _isMaf;
        private CellPopup _cellPopup;

        public TimingForm()
        {
            InitializeComponent();
            smoothComboBox.SelectedIndex = 0;
            smoothButton.Enabled = false;
            logOverlayButton.Enabled = false;
        }

        private void CommandHistory_UpdateButtons(object sender, EventArgs args)
        {
            undoButton.Enabled = CommandHistory.Instance.CanUndo;
            redoButton.Enabled = CommandHistory.Instance.CanRedo;
        }

        private List<TableListEntry> tableListEntries = new List<TableListEntry>();

        private void MainForm_Load(object sender, EventArgs e)
        {
            CommandHistory.Instance.UpdateCommandHistoryButtons += CommandHistory_UpdateButtons;
            CommandHistory_UpdateButtons(null, null);

            tableListEntries.Add(new TableListEntry("Initial base timing", _tables.InitialBaseTiming, true,
                "When you paste into this table, the modified base timing table will also be initialized with the same data.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Initial advance timing", _tables.InitialAdvanceTiming, true,
                "When you paste into this table, the modified advance timing table will also be initialized with the same data.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Initial total timing", _tables.InitialTotalTiming, false,
                "You cannot edit this table.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Modified base timing", _tables.ModifiedBaseTiming, true,
                "", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Modified advance timing", _tables.ModifiedAdvanceTiming, true,
                "The base timing will be adjusted when you change cells in this table, so that the total timing does not change.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Modified total timing", _tables.ModifiedTotalTiming, false,
                "When you edit cells in this table, the changes are actually made to the base timing table.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Delta total timing", _tables.DeltaTotalTiming, false,
                "This table shows the difference between the initial total timing and the modified total timing.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Target Fuel Map", _tables.TargetFuel, true,
                "This table is the Target Fuel table used for Maf adjustments.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Maf", _tables.InitialMaf, true,
                "This table is Maf.", TuningMode.Maf));
            tableListEntries.Add(new TableListEntry("Modified Maf", _tables.ModifiedMaf, true,
                "This table is Maf adjustments.", TuningMode.Maf));
            tableListEntries.Add(new TableListEntry("Delta Maf", _tables.DeltaMaf, true,
                "This table shows the difference between Maf and Modified Maf adjustments.", TuningMode.Maf));

            tableList.Items.AddRange(tableListEntries.Where(p => p.TuningMode == TuningMode.Timing).ToArray());

            if (Program.Debug)
            {
                try
                {
                    using (var file = new FileStream("..\\..\\tableTimingBase.txt", FileMode.Open))
                    {
                        var reader = new StreamReader(file);
                        var content = reader.ReadToEnd();
                        Util.LoadTable(content, _tables.InitialBaseTiming);
                        _tables.InitialBaseTiming.IsReadOnly = true;
                        Util.LoadTable(content, _tables.ModifiedBaseTiming);
                    }
                    using (var file = new FileStream("..\\..\\tableTimingAdvance.txt", FileMode.Open))
                    {
                        var reader = new StreamReader(file);
                        var content = reader.ReadToEnd();
                        Util.LoadTable(content, _tables.InitialAdvanceTiming);
                        _tables.InitialAdvanceTiming.IsReadOnly = true;
                        Util.LoadTable(content, _tables.ModifiedAdvanceTiming);
                    }
                    using (var file = new FileStream("..\\..\\tableFuelBase.txt", FileMode.Open))
                    {
                        var reader = new StreamReader(file);
                        var content = reader.ReadToEnd();
                        Util.LoadTable(content, _tables.TargetFuel);
                    }
                    using (var file = new FileStream("..\\..\\tableMafBase.txt", FileMode.Open))
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
            }

            tableList.SelectedIndex = 0;
        }

        private void TableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisposeCellPopup();

            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            var title = $"Timing Editor: {entry.Description}";
            pasteButton.Enabled = entry.AllowPaste;
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
                        if (dataGrid.Rows.Count < pair[1] || dataGrid.Rows[pair[1]].Cells.Count < pair[0])
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

            logOverlayButton.Enabled = true;

            if (entry.Table.IsReadOnly)
            {
                smoothButton.Enabled = false;
                logOverlayButton.Enabled = false;
            }
            DisposeCellPopup();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            DisposeCellPopup();
            var entry = tableList.SelectedItem as TableListEntry;
            if (entry == null)
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

        private void pasteButton_Click(object sender, EventArgs e)
        {
            DisposeCellPopup();
            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            if (entry == null)
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
                    _overlay = null;
                    temporaryTable.CopyTo(_tables.ModifiedBaseTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedBaseTiming);
                }

                if (entry.Table == _tables.InitialAdvanceTiming)
                {
                    _overlay = null;
                    temporaryTable.CopyTo(_tables.ModifiedAdvanceTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedAdvanceTiming);
                }

                if (entry.Table == _tables.InitialMaf)
                {
                    _overlay = null;
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

        private void DataGrid_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            var selectedCells = dataGrid.SelectedCells;

            if (entry.Table.IsReadOnly)
            {
                smoothButton.Enabled = false;
                logOverlayButton.Enabled = false;
            }
            else
            {
                smoothButton.Enabled = Smooth(selectedCells, false);
                logOverlayButton.Enabled = true;
            }
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
                    return;

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
                return;

            switch (e.KeyCode)
            {
                case Keys.Z:
                    UndoButton_Click(this, e);
                    break;

                case Keys.Y:
                    RedoButton_Click(this, e);
                    break;
            }
        }

        private void DataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (_editControlKeyDownSubscribed)
                return;

            e.Control.KeyDown += DataGridEditControl_KeyDown;
            _editControlKeyDownSubscribed = true;
        }

        private void DataGridEditControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.OemMinus:
                    Delta(-0.35);
                    break;

                case Keys.Oemplus:
                    Delta(+0.35);
                    break;
            }

            e.Handled = true;
            dataGrid.CancelEdit();
            dataGrid.EndEdit();
        }

        private void Delta(double delta)
        {
            foreach (DataGridViewCell cell in dataGrid.SelectedCells)
            {
                if (!double.TryParse(cell.Value.ToString(), out var value))
                    continue;

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

        private void RedoButton_Click(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
                return;

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
            DisposeCellPopup();
            DrawSideViews(_selectedColumn, _selectedRow);
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
                return;

            CommandHistory.Instance.Undo();

            _changingTables = true;
            Util.ShowTable(this, entry.Table, dataGrid);
            dataGrid.ClearSelection();
            Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);

            _changingTables = false;
            DisposeCellPopup();
            DrawSideViews(_selectedColumn, _selectedRow);
        }

        private void DataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGrid.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect)
                return;

            dataGrid.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
            dataGrid.Columns[e.ColumnIndex].Selected = true;
        }

        private void DataGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGrid.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
                return;

            dataGrid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            dataGrid.Rows[e.RowIndex].Selected = true;
        }

        private void SmoothComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
                return;

            var selectedCells = dataGrid.SelectedCells;

            if (entry.Table.IsReadOnly)
            {
                smoothButton.Enabled = false;
                return;
            }

            if (Smooth(selectedCells, false))
            {
                smoothButton.Enabled = true;
                return;
            }
            smoothButton.Enabled = false;
        }

        private void DisposeCellPopup()
        {
            if (_cellPopup == null)
                return;

            _cellPopup.Dispose();
            _cellPopup = null;
        }

        private void LogOverlayButton_Click(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            DisposeCellPopup();
            dataGrid.ClearSelection();
            if (entry.Table.IsReadOnly)
            {
                logOverlayButton.Enabled = false;
                return;
            }

            if (!entry.Table.IsPopulated)
            {
                MessageBox.Show(@"Error: Please populate table first");
                return;
            }

            var file = new OpenFileDialog();

            if (file.ShowDialog() != DialogResult.OK)
            { return; }

            var overlayStream = new StreamReader(file.FileName, Encoding.Default);
            try
            {
                var line = overlayStream.ReadLine();

                if (line == null)
                {
                    return;
                }

                var header = line.Split(',')
                                 .Select(s => s.Trim())
                                 .ToArray();

                var logOverlay = new LogOverlayForm(header, _isMaf);

                if (DialogResult.OK != logOverlay.ShowDialog(this))
                {
                    return;
                }

                string[] selected = logOverlay.SelectedLogParameters;
                if (selected.Length == 0)
                {
                    MessageBox.Show(@"Error: No parameters were selected");
                }

                var cursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                _changingTables = true;

                var content = overlayStream.ReadToEnd();
                var o = new Overlay(line, logOverlay.XAxis, logOverlay.YAxis);
                o.AddHeaderInfo(logOverlay.SelectedLogParameters);
                o.AddLog(content);
                var cellHit = o.ProcessOverlay(entry.Table.ColumnHeaders.ToArray(), entry.Table.RowHeaders.ToArray());
                _overlay = cellHit;

                Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, cellHit);
                dataGrid.Refresh();
                _changingTables = false;
                Cursor.Current = cursor;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
            finally
            {
                overlayStream.Close();
            }
        }

        private void DataGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
                return;

            try
            {
                DisposeCellPopup();

                var result = e.ColumnIndex >= 0 && e.RowIndex >= 0 &&
                             dataGrid.GetCellCount(DataGridViewElementStates.Selected) == 1 &&
                             dataGrid.SelectedCells[0].RowIndex == e.RowIndex &&
                             dataGrid.SelectedCells[0].ColumnIndex == e.ColumnIndex &&
                             dataGrid[e.ColumnIndex, e.RowIndex].IsInEditMode == false;

                if (!result) return;

                var selectedDataPoint = _overlay?.FirstOrDefault(p =>
                    p.XAxisIndex == e.ColumnIndex && p.YAxisIndex == e.RowIndex);

                if (selectedDataPoint == null)
                    return;

                _cellPopup = new CellPopup();
                _cellPopup.textBox.Text = selectedDataPoint.ToString();

                var r = dataGrid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                _cellPopup.Location = dataGrid.PointToScreen(new Point(r.Location.X + r.Width, r.Location.Y - _cellPopup.Height));
                _cellPopup.Show(dataGrid);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
        }

        private void dataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DisposeCellPopup();
        }

        private void dataGrid_Leave(object sender, EventArgs e)
        {
            DisposeCellPopup();
        }

        private void TimingForm_MouseDown(object sender, MouseEventArgs e)
        {
            DisposeCellPopup();
        }

        private void TimingForm_ResizeBegin(object sender, EventArgs e)
        {
            DisposeCellPopup();
        }

        private void TimingForm_LocationChanged(object sender, EventArgs e)
        {
            DisposeCellPopup();
        }

        private void TableList_MouseDown(object sender, MouseEventArgs e)
        {
            DisposeCellPopup();
        }

        private void HorizontalPanel_MouseDown(object sender, MouseEventArgs e)
        {
            DisposeCellPopup();
        }

        private void VerticalPanel_MouseDown(object sender, MouseEventArgs e)
        {
            DisposeCellPopup();
        }

        private void SmoothComboBox_MouseDown(object sender, MouseEventArgs e)
        {
            DisposeCellPopup();
        }

        private void DataGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            DisposeCellPopup();
        }

        private void TimingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableList.Items.Clear();
            tableList.Items.AddRange(tableListEntries.Where(p => p.TuningMode == TuningMode.Timing).ToArray());
            tableList.SelectedIndex = 0;
            _isMaf = false;
        }

        private void MAFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableList.Items.Clear();
            tableList.Items.AddRange(tableListEntries.Where(p => p.TuningMode == TuningMode.Maf).ToArray());
            tableList.SelectedIndex = 0;
            _isMaf = true;
        }
    }
}