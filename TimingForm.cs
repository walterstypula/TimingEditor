using NSFW.TimingEditor.Controls;
using NSFW.TimingEditor.Enums;
using NSFW.TimingEditor.Extensions;
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
        private Overlay _overlay;
        private bool _isMaf;
        private CellPopup _cellPopup;
        private readonly List<TableListEntry> _tableListEntries = new List<TableListEntry>();

        public TimingForm()
        {
            InitializeComponent();
            smoothButton.Enabled = true;
        }

        private void CommandHistory_UpdateButtons(object sender, EventArgs args)
        {
            undoButton.Enabled = CommandHistory.Instance.CanUndo;
            redoButton.Enabled = CommandHistory.Instance.CanRedo;
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

        private void DataGrid_Scroll(object sender, ScrollEventArgs e)
        {
            flowLayoutPanel1.HorizontalScroll.Value = e.NewValue;
        }

        private void TableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisposeCellPopup();

            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            smoothButton.Enabled = !entry.Table.IsReadOnly;

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

            DisposeCellPopup();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            DisposeCellPopup();
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

        private void PasteButton_Click(object sender, EventArgs e)
        {
            DisposeCellPopup();
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

        private void RedoButton_Click(object sender, EventArgs e)
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
            DisposeCellPopup();
            DrawSideViews(_selectedColumn, _selectedRow);
        }

        private void UndoButton_Click(object sender, EventArgs e)
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
            DisposeCellPopup();
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

        private void DisposeCellPopup()
        {
            if (_cellPopup == null)
            {
                return;
            }

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

                var selected = logOverlay.SelectedLogParameters;
                if (selected.Length == 0)
                {
                    MessageBox.Show(@"Error: No parameters were selected");
                }

                var cursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                _changingTables = true;

                var content = overlayStream.ReadToEnd();
                _overlay = new Overlay(line, logOverlay.XAxis, logOverlay.YAxis);
                _overlay.AddHeaderInfo(logOverlay.SelectedLogParameters);
                _overlay.AddLog(content);

                Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);
                dataGrid.Refresh();
                _changingTables = false;
                Cursor.Current = cursor;
                AdditionalLogOverlay.Enabled = true;
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
            if (!(tableList.SelectedItem is TableListEntry))
            {
                return;
            }

            try
            {
                DisposeCellPopup();

                var result = e.ColumnIndex >= 0 && e.RowIndex >= 0 &&
                             dataGrid.GetCellCount(DataGridViewElementStates.Selected) == 1 &&
                             dataGrid.SelectedCells[0].RowIndex == e.RowIndex &&
                             dataGrid.SelectedCells[0].ColumnIndex == e.ColumnIndex &&
                             dataGrid[e.ColumnIndex, e.RowIndex].IsInEditMode == false;

                if (!result)
                {
                    return;
                }

                if (!(dataGrid[e.ColumnIndex, e.RowIndex] is CustomDataGridViewCell item))
                {
                    return;
                }

                _cellPopup = new CellPopup();
                _cellPopup.TextBox.Text = item.PointData.ToString();

                var r = dataGrid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                _cellPopup.Location = dataGrid.PointToScreen(new Point(r.Location.X + r.Width, r.Location.Y - _cellPopup.Height));
                _cellPopup.Show(dataGrid);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
        }

        private void DataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DisposeCellPopup();
        }

        private void DataGrid_Leave(object sender, EventArgs e)
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
            tableList.Items.AddRange(_tableListEntries.Where(p => p.TuningMode == TuningMode.Timing || p.TuningMode == TuningMode.Both).ToArray());
            tableList.SelectedIndex = 0;
            _isMaf = false;
            AutoTune.Visible = _isMaf;
            AutoTune.Enabled = _isMaf;
        }

        private void MAFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableList.Items.Clear();
            tableList.Items.AddRange(_tableListEntries.Where(p => p.TuningMode == TuningMode.Maf || p.TuningMode == TuningMode.Both).ToArray());
            tableList.SelectedIndex = 0;
            _isMaf = true;
            AutoTune.Visible = _isMaf;
            AutoTune.Enabled = _isMaf;
        }

        private void AdditionalLogOverlay_Click(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry))
            {
                return;
            }

            DisposeCellPopup();
            dataGrid.ClearSelection();

            var file = new OpenFileDialog();

            if (file.ShowDialog() != DialogResult.OK)
            { return; }

            var overlayStream = new StreamReader(file.FileName, Encoding.Default);

            var line = overlayStream.ReadLine();

            if (line == null)
            {
                return;
            }

            var content = overlayStream.ReadToEnd();
            _overlay.AddLog(content);

            Util.ColorTable(dataGrid, entry.Table, _selectedColumn, _selectedRow, _overlay);
            dataGrid.Refresh();
        }

        private void AutoTune_Click(object sender, EventArgs e)
        {
            if (!(tableList.SelectedItem is TableListEntry entry) || entry.Table != _tables.ModifiedMaf)
            {
                return;
            }

            var overlayPoints = _overlay.ProcessOverlay(_tables.ModifiedMaf.ColumnHeaders, _tables.ModifiedMaf.RowHeaders);

            foreach (var op in overlayPoints)
            {
                var allCurrentAfrData = op.ValueData[WideBandHeaders.AEM_UEGO_9600];
                var validAutoTuneAfrData = allCurrentAfrData;// allCurrentAfrData.SkipOutliers(k: 3, selector: result => result.Value);

                if (validAutoTuneAfrData.Count() <= 1)
                {
                    continue;
                }

                var afrErrorList = new List<double>();

                foreach (var currentAfr in validAutoTuneAfrData)
                {
                    var xTargetAfrValue = _tables.TargetFuel.ColumnHeaders.ClosestValueIndex(currentAfr.Load);
                    var yTargetAfrValue = _tables.TargetFuel.RowHeaders.ClosestValueIndex(currentAfr.Rpm);
                    var targetAfr = _tables.TargetFuel.GetCell(xTargetAfrValue, yTargetAfrValue);

                    afrErrorList.Add(CalcAfrError(currentAfr.Value, targetAfr));
                }

                var targetCell = dataGrid[op.RowIndex, op.ColumnIndex] as CustomDataGridViewCell;

                afrErrorList = afrErrorList.SkipOutliers(k: 1, selector: result => result).ToList();
                
                var avgAfrError = afrErrorList.Average();

                var currentValue = double.Parse(targetCell.Value.ToString());
                var correction = currentValue * 0.01;

                var newValue = currentValue + avgAfrError * correction;
                targetCell.Value = newValue.ToString();
            }
        }

        private static double CalcAfrError(double currentAfr, double targetAfr)
        {
            return (currentAfr - targetAfr) / targetAfr * 100;
        }

        private void FlowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            dataGrid.HorizontalScrollingOffset = e.NewValue;
        }
    }
}