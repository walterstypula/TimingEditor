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
        public class CellPopup : Form
        {
            public CellPopup()
            {
                textBox = new System.Windows.Forms.RichTextBox();
                SuspendLayout();

                textBox.BackColor = System.Drawing.Color.White;
                textBox.Padding = new Padding(3, 3, 3, 3);
                textBox.ReadOnly = true;
                textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                textBox.Dock = System.Windows.Forms.DockStyle.Fill;
                textBox.Name = "textBox";
                textBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
                textBox.Size = new System.Drawing.Size(175, 220);
                textBox.TabIndex = 0;
                textBox.Text = "";

                AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                AutoScroll = true;
                Padding = new Padding(3, 3, 3, 3);
                BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
                ClientSize = new System.Drawing.Size(175, 220);
                ControlBox = false;
                Controls.Add(textBox);
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "CellPopup";
                ShowIcon = false;
                ShowInTaskbar = false;
                SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                StartPosition = FormStartPosition.Manual;
                ResumeLayout(false);
                PerformLayout();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            protected override bool ShowWithoutActivation
            {
                get { return true; }
            }

            private System.ComponentModel.IContainer components = null;
            public System.Windows.Forms.RichTextBox textBox = null;
        }

        private TimingTables tables = new TimingTables();
        private bool changingTables;
        private bool inCellMouseEnter;
        private int selectedColumn;
        private int selectedRow;
        private int advancePadding;
        private bool editControlKeyDownSubscribed;
        private string[,] baseTimingCellHita;
        private string[,] overlay;

        private CellPopup cellPopup;

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

            tableListEntries.Add(new TableListEntry("Initial base timing", tables.InitialBaseTiming, true,
                "When you paste into this table, the modified base timing table will also be initialized with the same data.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Initial advance timing", tables.InitialAdvanceTiming, true,
                "When you paste into this table, the modified advance timing table will also be initialized with the same data.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Initial total timing", tables.InitialTotalTiming, false,
                "You cannot edit this table.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Modified base timing", tables.ModifiedBaseTiming, true,
                "", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Modified advance timing", tables.ModifiedAdvanceTiming, true,
                "The base timing will be adjusted when you change cells in this table, so that the total timing does not change.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Modified total timing", tables.ModifiedTotalTiming, false,
                "When you edit cells in this table, the changes are actually made to the base timing table.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Delta total timing", tables.DeltaTotalTiming, false,
                "This table shows the difference between the initial total timing and the modified total timing.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Target Fuel Map", tables.TargetFuel, true,
                "This table is the Target Fuel table used for MAF adjustments.", TuningMode.Timing));
            tableListEntries.Add(new TableListEntry("Maf", tables.InitialMaf, true,
                "This table is MAF.", TuningMode.MAF));
            tableListEntries.Add(new TableListEntry("Modified Maf", tables.ModifiedMaf, true,
                "This table is MAF adjustments.", TuningMode.MAF));
            tableListEntries.Add(new TableListEntry("Delta Maf", tables.DeltaMaf, true,
                "This table shows the difference between MAF and Modified MAF adjustments.", TuningMode.MAF));

            tableList.Items.AddRange(tableListEntries.Where(p => p.TuningMode == TuningMode.Timing).ToArray());

            if (Program.Debug)
            {
                try
                {
                    using (var file = new FileStream("..\\..\\tableTimingBase.txt", FileMode.Open))
                    {
                        var reader = new StreamReader(file);
                        var content = reader.ReadToEnd();
                        Util.LoadTable(content, tables.InitialBaseTiming);
                        tables.InitialBaseTiming.IsReadOnly = true;
                        Util.LoadTable(content, tables.ModifiedBaseTiming);
                    }
                    using (var file = new FileStream("..\\..\\tableTimingAdvance.txt", FileMode.Open))
                    {
                        var reader = new StreamReader(file);
                        var content = reader.ReadToEnd();
                        Util.LoadTable(content, tables.InitialAdvanceTiming);
                        tables.InitialAdvanceTiming.IsReadOnly = true;
                        Util.LoadTable(content, tables.ModifiedAdvanceTiming);
                    }
                    using (var file = new FileStream("..\\..\\tableFuelBase.txt", FileMode.Open))
                    {
                        var reader = new StreamReader(file);
                        var content = reader.ReadToEnd();
                        Util.LoadTable(content, tables.TargetFuel);
                    }
                    using (var file = new FileStream("..\\..\\tableMafBase.txt", FileMode.Open))
                    {
                        var reader = new StreamReader(file);
                        var content = reader.ReadToEnd();
                        Util.LoadTable(content, tables.InitialMaf);
                        tables.InitialMaf.IsReadOnly = true;
                        Util.LoadTable(content, tables.ModifiedMaf);
                    }
                    tableList_SelectedIndexChanged(null, null);
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

        private void tableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            disposeCellPopup();
            if (tableList.SelectedItem == null)
            {
                return;
            }

            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            string title;

            title = "Timing Editor {0}: " + entry.Description;
            pasteButton.Enabled = entry.AllowPaste;
            statusStrip1.Items[0].Text = entry.StatusText;

            Text = string.Format(title, "v15");

            if (entry.Table.IsPopulated)
            {
                try
                {
                    List<int[]> selectedIndices = new List<int[]>(dataGrid.SelectedCells.Count);
                    DataGridViewSelectedCellCollection selected = dataGrid.SelectedCells;
                    foreach (DataGridViewCell cell in selected)
                    {
                        selectedIndices.Add(new int[2] { cell.ColumnIndex, cell.RowIndex });
                    }

                    dataGrid.ReadOnly = entry.Table.IsReadOnly;
                    changingTables = true;
                    Util.ShowTable(this, entry.Table, dataGrid);
                    dataGrid.ClearSelection();
                    DrawSideViews(selectedColumn, selectedRow);
                    Util.ColorTable(dataGrid, entry.Table, selectedColumn, selectedRow, overlay);

                    changingTables = false;
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
                    statusStrip1.Items[0].Text = "This only works if the base and advance tables are the same size";
                }
            }
            else
            {
                changingTables = true;
                dataGrid.Columns.Clear();
                changingTables = false;
            }

            logOverlayButton.Enabled = true;

            if (entry.Table.IsReadOnly)
            {
                smoothButton.Enabled = false;
                logOverlayButton.Enabled = false;
            }
            disposeCellPopup();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            disposeCellPopup();
            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

            ITable copyFrom = entry.Table;
            if ((entry.Table == tables.InitialAdvanceTiming) || (entry.Table == tables.ModifiedAdvanceTiming))
            {
                if (advancePadding > 0)
                {
                    string message = string.Format("The advance table will have the leftmost {0} columns removed.", advancePadding);
                    MessageBox.Show(this, message, "Timing Editor", MessageBoxButtons.OK);
                    copyFrom = Util.TrimLeft(copyFrom, advancePadding);
                }
            }

            string text = Util.CopyTable(copyFrom);
            Clipboard.SetData(DataFormats.Text, text);
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            disposeCellPopup();
            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

            try
            {
                string tableText = Clipboard.GetData(System.Windows.Forms.DataFormats.Text) as string;
                if (string.IsNullOrEmpty(tableText))
                {
                    throw new ApplicationException("Doesn't contain text.");
                }

                changingTables = true;
                bool wasReadOnly = entry.Table.IsReadOnly;
                if (wasReadOnly)
                {
                    entry.Table.IsReadOnly = false;
                }

                Table temporaryTable = new Table();
                Util.LoadTable(tableText, temporaryTable);

                if ((entry.Table == tables.InitialAdvanceTiming) || (entry.Table == tables.ModifiedAdvanceTiming))
                {
                    if (temporaryTable.ColumnHeaders.Count < tables.InitialBaseTiming.ColumnHeaders.Count)
                    {
                        MessageBox.Show(this, "The advance table will have values added to the left side, to make it align with the base timing table.", "Timing Editor", MessageBoxButtons.OK);
                        advancePadding = tables.InitialBaseTiming.ColumnHeaders.Count - temporaryTable.ColumnHeaders.Count;
                        temporaryTable = Util.PadLeft(temporaryTable, tables.InitialBaseTiming.ColumnHeaders.Count);
                    }
                    else
                    {
                        advancePadding = 0;
                    }
                }

                temporaryTable.CopyTo(entry.Table);

                if (wasReadOnly)
                {
                    entry.Table.IsReadOnly = true;
                }
                Util.ShowTable(this, entry.Table, dataGrid);
                Util.ColorTable(dataGrid, entry.Table, selectedColumn, selectedRow, new string[entry.Table.ColumnHeaders.Count, entry.Table.RowHeaders.Count]);
                dataGrid.ClearSelection();
                changingTables = false;

                if (entry.Table == tables.InitialBaseTiming)
                {
                    overlay = null;
                    temporaryTable.CopyTo(tables.ModifiedBaseTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedBaseTiming);
                }

                if (entry.Table == tables.InitialAdvanceTiming)
                {
                    overlay = null;
                    temporaryTable.CopyTo(tables.ModifiedAdvanceTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedAdvanceTiming);
                }

                if (entry.Table == tables.InitialMaf)
                {
                    overlay = null;
                    temporaryTable.CopyTo(tables.ModifiedMaf);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("Clipboard does not contain valid table data.\r\n" + ex.Message);
            }
        }

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (changingTables || inCellMouseEnter)
            {
                return;
            }

            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

            ITable table = entry.Table;
            if (table == null)
            {
                return;
            }

            if (table.IsReadOnly)
            {
                return;
            }

            object cellValue = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            string newStringValue = cellValue as string;

            double value;
            if (double.TryParse(newStringValue, out value))
            {
                EditCell edit = new EditCell(table, e.ColumnIndex, e.RowIndex, value);
                CommandHistory.Instance.Execute(edit);

                // The "smooth" button stops working if this code is enabled...
                /*                foreach (DataGridViewCell cell in this.dataGrid.SelectedCells)
                                {
                                    // TODO: create an "EditSelectedCells" command, execute that instead, for better undo/redo
                                    EditCell edit = new EditCell(table, cell.ColumnIndex, cell.RowIndex, value);
                                    CommandHistory.Instance.Execute(edit);
                                    cell.Value = value;
                                }
                 */
            }

            DrawSideViews(e.ColumnIndex, e.RowIndex);
        }

        private void dataGrid_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            DataGridViewSelectedCellCollection selectedCells = dataGrid.SelectedCells;

            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            if (entry.Table.IsReadOnly)
            {
                smoothButton.Enabled = false;
                logOverlayButton.Enabled = false;
            }
            else
            {
                /*                DataGridViewCellStyle style = Util.DefaultStyle;
                                style.BackColor = Color.Black;
                                style.ForeColor = Color.White;

                                foreach (DataGridViewCell cell in this.dataGrid.SelectedCells)
                                {
                                    cell.Style = style;
                                }
                */
                if (Smooth(selectedCells, false))
                {
                    smoothButton.Enabled = true;
                }
                else
                {
                    smoothButton.Enabled = false;
                }

                logOverlayButton.Enabled = true;
            }
        }

        private void dataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            SuspendLayout();
            if (dataGrid.ColumnCount > 0 && dataGrid.RowCount > 0)
            {
                inCellMouseEnter = true;
                selectedColumn = e.ColumnIndex;
                selectedRow = e.RowIndex;
                TableListEntry entry = tableList.SelectedItem as TableListEntry;
                Util.ColorTable(dataGrid, entry.Table, selectedColumn, selectedRow, null);
                inCellMouseEnter = false;
            }
            DrawSideViews(selectedColumn, selectedRow);
            ResumeLayout(false);
            PerformLayout();
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    undoButton_Click(this, e);
                }

                if (e.KeyCode == Keys.Y)
                {
                    redoButton_Click(this, e);
                }
            }
        }

        private void dataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (!editControlKeyDownSubscribed)
            {
                e.Control.KeyDown += dataGridEditControl_KeyDown;
                editControlKeyDownSubscribed = true;
            }
        }

        private void dataGridEditControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 187)
            {
                Delta(+0.35);
                e.Handled = true;
                dataGrid.CancelEdit();
                dataGrid.EndEdit();
            }

            if (e.KeyValue == 189)
            {
                Delta(-0.35);
                e.Handled = true;
                dataGrid.CancelEdit();
                dataGrid.EndEdit();
            }
        }

        private void Delta(double delta)
        {
            foreach (DataGridViewCell cell in dataGrid.SelectedCells)
            {
                double value;
                if (double.TryParse(cell.Value.ToString(), out value))
                {
                    value += delta;
                    cell.Value = value.ToString();
                }
            }
        }

        private bool TryGetValue(int x, int y, out double value)
        {
            value = 0;

            object o = dataGrid.Rows[y].Cells[x].Value;
            if (o == null)
            {
                return false;
            }

            return double.TryParse(o.ToString(), out value);
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            CommandHistory.Instance.Redo();

            changingTables = true;
            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            Util.ShowTable(this, entry.Table, dataGrid);
            dataGrid.ClearSelection();
            if (entry.Table == tables.InitialAdvanceTiming || entry.Table == tables.ModifiedAdvanceTiming
                || entry.Table == tables.InitialBaseTiming || entry.Table == tables.ModifiedBaseTiming)
            {
                Util.ColorTable(dataGrid, entry.Table, selectedColumn, selectedRow, overlay);
            }
            else
            {
                Util.ColorTable(dataGrid, entry.Table, selectedColumn, selectedRow, null);
            }

            changingTables = false;
            disposeCellPopup();
            DrawSideViews(selectedColumn, selectedRow);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            Command command = CommandHistory.Instance.Undo();

            changingTables = true;
            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            Util.ShowTable(this, entry.Table, dataGrid);
            dataGrid.ClearSelection();
            Util.ColorTable(dataGrid, entry.Table, selectedColumn, selectedRow, overlay);

            changingTables = false;
            disposeCellPopup();
            DrawSideViews(selectedColumn, selectedRow);
        }

        private void dataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGrid.SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                dataGrid.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                dataGrid.Columns[e.ColumnIndex].Selected = true;
            }
        }

        private void dataGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGrid.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
            {
                dataGrid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                dataGrid.Rows[e.RowIndex].Selected = true;
            }
        }

        private void smoothComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection selectedCells = dataGrid.SelectedCells;
            TableListEntry entry = tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

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

        private void disposeCellPopup()
        {
            if (cellPopup != null)
            {
                cellPopup.Dispose();
                cellPopup = null;
            }
        }

        private void logOverlayButton_Click(object sender, EventArgs e)
        {
            var entry = tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

            disposeCellPopup();
            dataGrid.ClearSelection();
            if (entry.Table.IsReadOnly)
            {
                logOverlayButton.Enabled = false;
                return;
            }
            else if (!entry.Table.IsPopulated)
            {
                MessageBox.Show("Error: Please populate table first");
                return;
            }

            string line;
            var file = new OpenFileDialog();

            if (file.ShowDialog() != DialogResult.OK)
            { return; }

            var overlayStream = new StreamReader(file.FileName, Encoding.Default);
            try
            {
                line = overlayStream.ReadLine();

                if (line == null)
                {
                    return;
                }

                var header = line.Split(',')
                                 .Select(s => s.Trim())
                                 .ToArray();

                var logOverlay = new LogOverlayForm(header);

                if (DialogResult.OK != logOverlay.ShowDialog(this))
                {
                    return;
                }

                string[] selected = logOverlay.SelectedLogParameters;
                if (selected.Length == 0)
                {
                    MessageBox.Show("Error: No parameters were selected");
                }

                string xAxis = logOverlay.XAxis;
                string yAxis = logOverlay.YAxis;

                int xIdx = Array.IndexOf(header, xAxis);
                int yIdx = Array.IndexOf(header, yAxis);
                int[] indeces = new int[selected.Length];

                for (int i = 0; i < selected.Length; ++i)
                {
                    indeces[i] = Array.IndexOf(header, selected[i]);
                }

                Cursor cursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                double xAxisLogValue, yAxisLogValue, v;
                int xArrIdx, yArrIdx;
                changingTables = true;

                var columnHeaders = entry.Table.ColumnHeaders;
                var rowHeaders = entry.Table.RowHeaders;

                overlay = new string[columnHeaders.Count, rowHeaders.Count];
                string[,] cellHit = overlay;

                try
                {
                    var xDict = new Dictionary<int, Dictionary<int, Dictionary<string, string>>>();
                    Dictionary<int, Dictionary<string, string>> yDict;
                    Dictionary<string, string> paramDict;
                    string val;
                    while ((line = overlayStream.ReadLine()) != null)
                    {
                        string[] vals = line.Split(',');

                        if (!double.TryParse(vals[xIdx], out xAxisLogValue) || !double.TryParse(vals[yIdx], out yAxisLogValue))
                        {
                            continue;
                        }

                        xArrIdx = columnHeaders.ClosestValueIndex(xAxisLogValue);
                        yArrIdx = rowHeaders.ClosestValueIndex(yAxisLogValue);
                        for (int idx = 0; idx < indeces.Length; ++idx)
                        {
                            if (idx == xIdx || idx == yIdx)
                            {
                                continue;
                            }

                            if (!double.TryParse(vals[indeces[idx]], out v) || v == 0.0)
                            {
                                continue;
                            }

                            if (!xDict.TryGetValue(xArrIdx, out yDict))
                            {
                                yDict = new Dictionary<int, Dictionary<string, string>>();
                                xDict[xArrIdx] = yDict;
                            }
                            if (!yDict.TryGetValue(yArrIdx, out paramDict))
                            {
                                paramDict = new Dictionary<string, string>();
                                yDict[yArrIdx] = paramDict;
                            }
                            if (!paramDict.TryGetValue(selected[idx], out val))
                            {
                                paramDict[selected[idx]] = $"    [{xAxisLogValue}, {yAxisLogValue}, {v}]\r\n";
                            }
                            else
                            {
                                paramDict[selected[idx]] += ($"    [{xAxisLogValue}, {yAxisLogValue}, {v}]\r\n");
                            }
                        }
                    }

                    foreach (KeyValuePair<int, Dictionary<int, Dictionary<string, string>>> xPair in xDict)
                    {
                        foreach (KeyValuePair<int, Dictionary<string, string>> yPair in xPair.Value)
                        {
                            foreach (KeyValuePair<string, string> paramPair in yPair.Value)
                            {
                                if (cellHit[xPair.Key, yPair.Key] == null)
                                {
                                    cellHit[xPair.Key, yPair.Key] = "";
                                }

                                cellHit[xPair.Key, yPair.Key] += ($"{paramPair.Key}:\r\n{paramPair.Value}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex}");
                }
                Util.ColorTable(dataGrid, entry.Table, selectedColumn, selectedRow, cellHit);
                dataGrid.Refresh();
                changingTables = false;
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

        public class Overlay
        {
            private string xAxisHeader;
            private string yAxisHeader;
            private StringBuilder logData;

            public List<OverlayPoint> ProcessOverlay(int rows, int columns)
            {
            }

            public void AddLog(string content, params string[] dataHeaders)
            {
                var reader = new StringReader(content);
                var line = reader.ReadLine();
                int xAxisHeaderIndex = line.IndexOf(xAxisHeader);
                int yAxisHeaderIndex = line.IndexOf(yAxisHeader);

                //Get index of xAxisHeader
                //Get index of yAxisHeader
            }
        }

        public class OverlayPoint
        {
            public double xAxisLookupValue { get; }
            public double yAxisLookupBalue { get; }
            public readonly Dictionary<string, List<string>> Data = new Dictionary<string, List<string>>();

            public OverlayPoint(double xAxisLookupValue, double yAxisLookupBalue)
            {
                this.xAxisLookupValue = xAxisLookupValue;
                this.yAxisLookupBalue = yAxisLookupBalue;
            }

            public void AddData(string header, string value)
            {
                if (!Data.ContainsKey(header))
                {
                    Data.Add(header, new List<string>() { value });
                }
                else
                {
                    Data[header].Add(value);
                }
            }
        }

        private void dataGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                disposeCellPopup();
                TableListEntry entry = tableList.SelectedItem as TableListEntry;
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && entry != null && dataGrid.GetCellCount(DataGridViewElementStates.Selected) == 1 &&
                    dataGrid.SelectedCells[0].RowIndex == e.RowIndex && dataGrid.SelectedCells[0].ColumnIndex == e.ColumnIndex && dataGrid[e.ColumnIndex, e.RowIndex].IsInEditMode == false)
                {
                    string[,] cellHit = overlay;

                    if (cellHit != null)
                    {
                        if (cellHit[e.ColumnIndex, e.RowIndex] != null)
                        {
                            cellPopup = new CellPopup();
                            cellPopup.textBox.Text = cellHit[e.ColumnIndex, e.RowIndex];
                            Rectangle r = dataGrid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                            cellPopup.Location = dataGrid.PointToScreen(new Point(r.Location.X + r.Width, r.Location.Y - cellPopup.Height));
                            cellPopup.Show(dataGrid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
        }

        private void dataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            disposeCellPopup();
        }

        private void dataGrid_Leave(object sender, EventArgs e)
        {
            disposeCellPopup();
        }

        private void TimingForm_MouseDown(object sender, MouseEventArgs e)
        {
            disposeCellPopup();
        }

        private void TimingForm_ResizeBegin(object sender, EventArgs e)
        {
            disposeCellPopup();
        }

        private void TimingForm_LocationChanged(object sender, EventArgs e)
        {
            disposeCellPopup();
        }

        private void tableList_MouseDown(object sender, MouseEventArgs e)
        {
            disposeCellPopup();
        }

        private void horizontalPanel_MouseDown(object sender, MouseEventArgs e)
        {
            disposeCellPopup();
        }

        private void verticalPanel_MouseDown(object sender, MouseEventArgs e)
        {
            disposeCellPopup();
        }

        private void smoothComboBox_MouseDown(object sender, MouseEventArgs e)
        {
            disposeCellPopup();
        }

        private void dataGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            disposeCellPopup();
        }

        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void timingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableList.Items.Clear();
            tableList.Items.AddRange(tableListEntries.Where(p => p.TuningMode == TuningMode.Timing).ToArray());
            tableList.SelectedIndex = 0;
        }

        private void mAFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableList.Items.Clear();
            tableList.Items.AddRange(tableListEntries.Where(p => p.TuningMode == TuningMode.MAF).ToArray());
            tableList.SelectedIndex = 0;
        }
    }
}