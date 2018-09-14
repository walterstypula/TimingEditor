using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
                this.textBox = new System.Windows.Forms.RichTextBox();
                this.SuspendLayout();

                this.textBox.BackColor = System.Drawing.Color.White;
                this.textBox.Padding = new Padding(3, 3, 3, 3);
                this.textBox.ReadOnly = true;
                this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
                this.textBox.Name = "textBox";
                this.textBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
                this.textBox.Size = new System.Drawing.Size(175, 220);
                this.textBox.TabIndex = 0;
                this.textBox.Text = "";

                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.AutoScroll = true;
                this.Padding = new Padding(3, 3, 3, 3);
                this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
                this.ClientSize = new System.Drawing.Size(175, 220);
                this.ControlBox = false;
                this.Controls.Add(this.textBox);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "CellPopup";
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
                this.StartPosition = FormStartPosition.Manual;
                this.ResumeLayout(false);
                this.PerformLayout();
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
            this.smoothComboBox.SelectedIndex = 0;
            this.smoothButton.Enabled = false;
            this.logOverlayButton.Enabled = false;
        }

        private void CommandHistory_UpdateButtons(object sender, EventArgs args)
        {
            this.undoButton.Enabled = CommandHistory.Instance.CanUndo;
            this.redoButton.Enabled = CommandHistory.Instance.CanRedo;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CommandHistory.Instance.UpdateCommandHistoryButtons += this.CommandHistory_UpdateButtons;
            this.CommandHistory_UpdateButtons(null, null);

            this.tableList.Items.Add(new TableListEntry("Initial base timing", this.tables.InitialBaseTiming, true,
                "When you paste into this table, the modified base timing table will also be initialized with the same data."));
            this.tableList.Items.Add(new TableListEntry("Initial advance timing", this.tables.InitialAdvanceTiming, true,
                "When you paste into this table, the modified advance timing table will also be initialized with the same data."));
            this.tableList.Items.Add(new TableListEntry("Initial total timing", this.tables.InitialTotalTiming, false,
                "You cannot edit this table."));
            this.tableList.Items.Add(new TableListEntry("Modified base timing", this.tables.ModifiedBaseTiming, true,
                ""));
            this.tableList.Items.Add(new TableListEntry("Modified advance timing", this.tables.ModifiedAdvanceTiming, true,
                "The base timing will be adjusted when you change cells in this table, so that the total timing does not change."));
            this.tableList.Items.Add(new TableListEntry("Modified total timing", this.tables.ModifiedTotalTiming, false,
                "When you edit cells in this table, the changes are actually made to the base timing table."));
            this.tableList.Items.Add(new TableListEntry("Delta total timing", this.tables.DeltaTotalTiming, false,
                "This table shows the difference between the initial total timing and the modified total timing."));
            this.tableList.Items.Add(new TableListEntry("Target Fuel Map", this.tables.TargetFuel, true,
                "This table is the Target Fuel table used for MAF adjustments."));

            if (Program.Debug)
            {
                try
                {
                    using (FileStream file = new FileStream("..\\..\\TimingBase.txt", FileMode.Open))
                    {
                        StreamReader reader = new StreamReader(file);
                        string content = reader.ReadToEnd();
                        Util.LoadTable(content, this.tables.InitialBaseTiming);
                        this.tables.InitialBaseTiming.IsReadOnly = true;
                        Util.LoadTable(content, this.tables.ModifiedBaseTiming);
                    }
                    using (FileStream file = new FileStream("..\\..\\TimingAdvance.txt", FileMode.Open))
                    {
                        StreamReader reader = new StreamReader(file);
                        string content = reader.ReadToEnd();
                        Util.LoadTable(content, this.tables.InitialAdvanceTiming);
                        this.tables.InitialAdvanceTiming.IsReadOnly = true;
                        Util.LoadTable(content, this.tables.ModifiedAdvanceTiming);
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

            this.tableList.SelectedIndex = 0;
        }

        private void tableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            disposeCellPopup();
            if (this.tableList.SelectedItem == null)
            {
                return;
            }

            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
            string title;

            title = "Timing Editor {0}: " + entry.Description;
            this.pasteButton.Enabled = entry.AllowPaste;
            this.statusStrip1.Items[0].Text = entry.StatusText;

            this.Text = string.Format(title, "v15");

            if (entry.Table.IsPopulated)
            {
                try
                {
                    List<int[]> selectedIndices = new List<int[]>(this.dataGrid.SelectedCells.Count);
                    DataGridViewSelectedCellCollection selected = this.dataGrid.SelectedCells;
                    foreach (DataGridViewCell cell in selected)
                    {
                        selectedIndices.Add(new int[2] { cell.ColumnIndex, cell.RowIndex });
                    }

                    this.dataGrid.ReadOnly = entry.Table.IsReadOnly;
                    this.changingTables = true;
                    Util.ShowTable(this, entry.Table, this.dataGrid);
                    this.dataGrid.ClearSelection();
                    this.DrawSideViews(this.selectedColumn, this.selectedRow);
                    Util.ColorTable(this.dataGrid, entry.Table, this.selectedColumn, this.selectedRow, overlay);

                    this.changingTables = false;
                    foreach (int[] pair in selectedIndices)
                    {
                        dataGrid.Rows[pair[1]].Cells[pair[0]].Selected = true;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    this.dataGrid.ClearSelection();
                    dataGrid.Rows.Clear();
                    this.statusStrip1.Items[0].Text = "This only works if the base and advance tables are the same size";
                }
            }
            else
            {
                this.changingTables = true;
                this.dataGrid.Columns.Clear();
                this.changingTables = false;
            }

            this.logOverlayButton.Enabled = true;

            if (entry.Table.IsReadOnly)
            {
                this.smoothButton.Enabled = false;
                this.logOverlayButton.Enabled = false;
            }
            disposeCellPopup();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            disposeCellPopup();
            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

            ITable copyFrom = entry.Table;
            if ((entry.Table == this.tables.InitialAdvanceTiming) || (entry.Table == this.tables.ModifiedAdvanceTiming))
            {
                if (this.advancePadding > 0)
                {
                    string message = string.Format("The advance table will have the leftmost {0} columns removed.", this.advancePadding);
                    MessageBox.Show(this, message, "Timing Editor", MessageBoxButtons.OK);
                    copyFrom = Util.TrimLeft(copyFrom, this.advancePadding);
                }
            }

            string text = Util.CopyTable(copyFrom);
            Clipboard.SetData(DataFormats.Text, text);
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            disposeCellPopup();
            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
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

                this.changingTables = true;
                bool wasReadOnly = entry.Table.IsReadOnly;
                if (wasReadOnly)
                {
                    entry.Table.IsReadOnly = false;
                }

                Table temporaryTable = new Table();
                Util.LoadTable(tableText, temporaryTable);

                if ((entry.Table == this.tables.InitialAdvanceTiming) || (entry.Table == this.tables.ModifiedAdvanceTiming))
                {
                    if (temporaryTable.ColumnHeaders.Count < this.tables.InitialBaseTiming.ColumnHeaders.Count)
                    {
                        MessageBox.Show(this, "The advance table will have values added to the left side, to make it align with the base timing table.", "Timing Editor", MessageBoxButtons.OK);
                        this.advancePadding = this.tables.InitialBaseTiming.ColumnHeaders.Count - temporaryTable.ColumnHeaders.Count;
                        temporaryTable = Util.PadLeft(temporaryTable, this.tables.InitialBaseTiming.ColumnHeaders.Count);
                    }
                    else
                    {
                        this.advancePadding = 0;
                    }
                }

                temporaryTable.CopyTo(entry.Table);

                if (wasReadOnly)
                {
                    entry.Table.IsReadOnly = true;
                }
                Util.ShowTable(this, entry.Table, this.dataGrid);
                Util.ColorTable(this.dataGrid, entry.Table, this.selectedColumn, this.selectedRow, new string[entry.Table.ColumnHeaders.Count, entry.Table.RowHeaders.Count]);
                this.dataGrid.ClearSelection();
                this.changingTables = false;

                if (entry.Table == this.tables.InitialBaseTiming)
                {
                    this.overlay = null;
                    temporaryTable.CopyTo(this.tables.ModifiedBaseTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedBaseTiming);
                }

                if (entry.Table == this.tables.InitialAdvanceTiming)
                {
                    this.overlay = null;
                    temporaryTable.CopyTo(this.tables.ModifiedAdvanceTiming);
                    //Util.LoadTable(tableText, this.tables.ModifiedAdvanceTiming);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("Clipboard does not contain valid table data.\r\n" + ex.Message);
            }
        }

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.changingTables || this.inCellMouseEnter)
            {
                return;
            }

            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
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

            object cellValue = this.dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
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

            this.DrawSideViews(e.ColumnIndex, e.RowIndex);
        }

        private void dataGrid_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            DataGridViewSelectedCellCollection selectedCells = this.dataGrid.SelectedCells;

            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
            if (entry.Table.IsReadOnly)
            {
                this.smoothButton.Enabled = false;
                this.logOverlayButton.Enabled = false;
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
                if (this.Smooth(selectedCells, false))
                {
                    this.smoothButton.Enabled = true;
                }
                else
                {
                    this.smoothButton.Enabled = false;
                }

                this.logOverlayButton.Enabled = true;
            }
        }

        private void dataGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            this.SuspendLayout();
            if (this.dataGrid.ColumnCount > 0 && this.dataGrid.RowCount > 0)
            {
                this.inCellMouseEnter = true;
                this.selectedColumn = e.ColumnIndex;
                this.selectedRow = e.RowIndex;
                TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
                Util.ColorTable(this.dataGrid, entry.Table, this.selectedColumn, this.selectedRow, null);
                this.inCellMouseEnter = false;
            }
            this.DrawSideViews(this.selectedColumn, this.selectedRow);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void dataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Z)
                {
                    this.undoButton_Click(this, e);
                }

                if (e.KeyCode == Keys.Y)
                {
                    this.redoButton_Click(this, e);
                }
            }
        }

        private void dataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (!this.editControlKeyDownSubscribed)
            {
                e.Control.KeyDown += dataGridEditControl_KeyDown;
                this.editControlKeyDownSubscribed = true;
            }
        }

        private void dataGridEditControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 187)
            {
                this.Delta(+0.35);
                e.Handled = true;
                this.dataGrid.CancelEdit();
                this.dataGrid.EndEdit();
            }

            if (e.KeyValue == 189)
            {
                this.Delta(-0.35);
                e.Handled = true;
                this.dataGrid.CancelEdit();
                this.dataGrid.EndEdit();
            }
        }

        private void Delta(double delta)
        {
            foreach (DataGridViewCell cell in this.dataGrid.SelectedCells)
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

            object o = this.dataGrid.Rows[y].Cells[x].Value;
            if (o == null)
            {
                return false;
            }

            return double.TryParse(o.ToString(), out value);
        }

        private void redoButton_Click(object sender, EventArgs e)
        {
            CommandHistory.Instance.Redo();

            this.changingTables = true;
            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
            Util.ShowTable(this, entry.Table, this.dataGrid);
            this.dataGrid.ClearSelection();
            if (entry.Table == this.tables.InitialAdvanceTiming || entry.Table == this.tables.ModifiedAdvanceTiming
                || entry.Table == this.tables.InitialBaseTiming || entry.Table == this.tables.ModifiedBaseTiming)
            {
                Util.ColorTable(this.dataGrid, entry.Table, this.selectedColumn, this.selectedRow, overlay);
            }
            else
            {
                Util.ColorTable(this.dataGrid, entry.Table, this.selectedColumn, this.selectedRow, null);
            }

            this.changingTables = false;
            disposeCellPopup();
            this.DrawSideViews(this.selectedColumn, this.selectedRow);
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            Command command = CommandHistory.Instance.Undo();

            this.changingTables = true;
            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
            Util.ShowTable(this, entry.Table, this.dataGrid);
            this.dataGrid.ClearSelection();
            Util.ColorTable(this.dataGrid, entry.Table, this.selectedColumn, this.selectedRow, overlay);

            this.changingTables = false;
            disposeCellPopup();
            this.DrawSideViews(this.selectedColumn, this.selectedRow);
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
            DataGridViewSelectedCellCollection selectedCells = this.dataGrid.SelectedCells;
            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

            if (entry.Table.IsReadOnly)
            {
                this.smoothButton.Enabled = false;
                return;
            }

            if (this.Smooth(selectedCells, false))
            {
                this.smoothButton.Enabled = true;
                return;
            }
            this.smoothButton.Enabled = false;
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
            TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
            if (entry == null)
            {
                return;
            }

            disposeCellPopup();
            this.dataGrid.ClearSelection();
            if (entry.Table.IsReadOnly)
            {
                this.logOverlayButton.Enabled = false;
                return;
            }
            else if (!entry.Table.IsPopulated)
            {
                MessageBox.Show("Error: Please populate table first");
                return;
            }

            string line;
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(file.FileName, Encoding.Default);
                try
                {
                    line = sr.ReadLine();

                    if (line == null)
                    {
                        return;
                    }

                    string[] header = line.Split(',');
                    int i = 0;
                    foreach (string h in header)
                    {
                        header[i++] = h.Trim();
                    }

                    LogOverlay logOverlay = new LogOverlay();
                    logOverlay.LogParameters = header;

                    if (DialogResult.OK != logOverlay.ShowDialog(this))
                    {
                        return;
                    }

                    string[] selected = logOverlay.LogParameters;
                    if (selected.Length == 0)
                    {
                        MessageBox.Show("Error: No parameters were selected");
                    }

                    string xAxis = logOverlay.XAxis;
                    string yAxis = logOverlay.YAxis;
                    string[,] cellHit = null;

                    overlay = new string[entry.Table.ColumnHeaders.Count, entry.Table.RowHeaders.Count];
                    cellHit = overlay;

                    int xIdx = Array.IndexOf(header, xAxis);
                    int yIdx = Array.IndexOf(header, yAxis);
                    int[] indeces = new int[selected.Length];
                    for (i = 0; i < selected.Length; ++i)
                    {
                        indeces[i] = Array.IndexOf(header, selected[i]);
                    }

                    Cursor cursor = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    double X, Y, x, y, v;
                    int xArrIdx, yArrIdx;
                    this.changingTables = true;
                    try
                    {
                        List<double> xAxisArray = (List<double>)entry.Table.ColumnHeaders;
                        List<double> yAxisArray = (List<double>)entry.Table.RowHeaders;
                        Dictionary<int, Dictionary<int, Dictionary<string, string>>> xDict = new Dictionary<int, Dictionary<int, Dictionary<string, string>>>();
                        Dictionary<int, Dictionary<string, string>> yDict;
                        Dictionary<string, string> paramDict;
                        string val;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] vals = line.Split(',');

                            if (!double.TryParse(vals[xIdx], out x) || !double.TryParse(vals[yIdx], out y))
                            {
                                continue;
                            }

                            X = xAxisArray[Util.ClosestValueIndex(x, xAxisArray)];
                            Y = yAxisArray[Util.ClosestValueIndex(y, yAxisArray)];
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

                                xArrIdx = xAxisArray.IndexOf(X);
                                yArrIdx = yAxisArray.IndexOf(Y);

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
                                    paramDict[selected[idx]] = "    [" + x + ", " + y + ", " + v + "]\r\n";
                                }
                                else
                                {
                                    paramDict[selected[idx]] += ("    [" + x + ", " + y + ", " + v + "]\r\n");
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

                                    cellHit[xPair.Key, yPair.Key] += (paramPair.Key + ":\r\n" + paramPair.Value);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                    Util.ColorTable(this.dataGrid, entry.Table, this.selectedColumn, this.selectedRow, cellHit);
                    this.dataGrid.Refresh();
                    this.changingTables = false;
                    Cursor.Current = cursor;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    sr.Close();
                }
            }
        }

        private void dataGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                disposeCellPopup();
                TableListEntry entry = this.tableList.SelectedItem as TableListEntry;
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
                MessageBox.Show("Error: " + ex.Message);
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
    }
}