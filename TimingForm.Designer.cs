namespace NSFW.TimingEditor
{
    partial class TimingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableList = new System.Windows.Forms.ListBox();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.horizontalPanel = new System.Windows.Forms.Panel();
            this.verticalPanel = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.smoothButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tuningModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AdditionalLogOverlay = new System.Windows.Forms.Button();
            this.AutoTune = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpOverlay = new System.Windows.Forms.TableLayoutPanel();
            this.xAxisLabel = new System.Windows.Forms.Label();
            this.yAxisComboBox = new System.Windows.Forms.ComboBox();
            this.yAxisLabel = new System.Windows.Forms.Label();
            this.lbOverlayHeaders = new System.Windows.Forms.CheckedListBox();
            this.xAxisComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rtbOverlayCellData = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpOverlay.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableList
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.tableList, 2);
            this.tableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList.FormattingEnabled = true;
            this.tableList.IntegralHeight = false;
            this.tableList.ItemHeight = 16;
            this.tableList.Location = new System.Drawing.Point(4, 4);
            this.tableList.Margin = new System.Windows.Forms.Padding(4);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(224, 216);
            this.tableList.TabIndex = 0;
            this.tableList.SelectedIndexChanged += new System.EventHandler(this.TableList_SelectedIndexChanged);
            this.tableList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TableList_MouseDown);
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToResizeColumns = false;
            this.dataGrid.AllowUserToResizeRows = false;
            this.dataGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.Location = new System.Drawing.Point(244, 312);
            this.dataGrid.Margin = new System.Windows.Forms.Padding(4);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowHeadersWidth = 51;
            this.dataGrid.Size = new System.Drawing.Size(653, 450);
            this.dataGrid.TabIndex = 1;
            this.dataGrid.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DataGrid_CellBeginEdit);
            this.dataGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellEnter);
            this.dataGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellLeave);
            this.dataGrid.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellMouseEnter);
            this.dataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellValueChanged);
            this.dataGrid.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGrid_ColumnHeaderMouseClick);
            this.dataGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DataGrid_EditingControlShowing);
            this.dataGrid.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGrid_RowHeaderMouseClick);
            this.dataGrid.SelectionChanged += new System.EventHandler(this.DataGrid_SelectionChanged);
            this.dataGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGrid_KeyDown);
            this.dataGrid.Leave += new System.EventHandler(this.DataGrid_Leave);
            // 
            // horizontalPanel
            // 
            this.horizontalPanel.BackColor = System.Drawing.SystemColors.Window;
            this.horizontalPanel.Location = new System.Drawing.Point(4, 4);
            this.horizontalPanel.Margin = new System.Windows.Forms.Padding(4);
            this.horizontalPanel.Name = "horizontalPanel";
            this.horizontalPanel.Size = new System.Drawing.Size(217, 271);
            this.horizontalPanel.TabIndex = 2;
            this.horizontalPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HorizontalPanel_MouseDown);
            // 
            // verticalPanel
            // 
            this.verticalPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.verticalPanel.BackColor = System.Drawing.SystemColors.Window;
            this.verticalPanel.Location = new System.Drawing.Point(4, 312);
            this.verticalPanel.Margin = new System.Windows.Forms.Padding(4);
            this.verticalPanel.Name = "verticalPanel";
            this.verticalPanel.Size = new System.Drawing.Size(232, 210);
            this.verticalPanel.TabIndex = 3;
            this.verticalPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VerticalPanel_MouseDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 794);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1181, 26);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(74, 20);
            this.toolStripStatusLabel1.Text = "statusText";
            // 
            // smoothButton
            // 
            this.smoothButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smoothButton.Location = new System.Drawing.Point(4, 228);
            this.smoothButton.Margin = new System.Windows.Forms.Padding(4);
            this.smoothButton.Name = "smoothButton";
            this.smoothButton.Size = new System.Drawing.Size(108, 30);
            this.smoothButton.TabIndex = 7;
            this.smoothButton.Text = "&Smooth";
            this.smoothButton.UseVisualStyleBackColor = true;
            this.smoothButton.Click += new System.EventHandler(this.SmoothButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tuningModeToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.logOverlayToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1181, 28);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tuningModeToolStripMenuItem
            // 
            this.tuningModeToolStripMenuItem.Name = "tuningModeToolStripMenuItem";
            this.tuningModeToolStripMenuItem.Size = new System.Drawing.Size(102, 24);
            this.tuningModeToolStripMenuItem.Text = "MAF Tuning";
            this.tuningModeToolStripMenuItem.Click += new System.EventHandler(this.TuningModeToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(59, 24);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(58, 24);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItem_Click);
            // 
            // logOverlayToolStripMenuItem
            // 
            this.logOverlayToolStripMenuItem.Name = "logOverlayToolStripMenuItem";
            this.logOverlayToolStripMenuItem.Size = new System.Drawing.Size(102, 24);
            this.logOverlayToolStripMenuItem.Text = "Log Overlay";
            this.logOverlayToolStripMenuItem.Click += new System.EventHandler(this.LogOverlayToolStripMenuItem_Click);
            // 
            // AdditionalLogOverlay
            // 
            this.AdditionalLogOverlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AdditionalLogOverlay.Enabled = false;
            this.AdditionalLogOverlay.Location = new System.Drawing.Point(120, 266);
            this.AdditionalLogOverlay.Margin = new System.Windows.Forms.Padding(4);
            this.AdditionalLogOverlay.Name = "AdditionalLogOverlay";
            this.AdditionalLogOverlay.Size = new System.Drawing.Size(108, 30);
            this.AdditionalLogOverlay.TabIndex = 13;
            this.AdditionalLogOverlay.Text = "Addt’l Log";
            this.AdditionalLogOverlay.UseVisualStyleBackColor = true;
            this.AdditionalLogOverlay.Click += new System.EventHandler(this.AdditionalLogOverlay_Click);
            // 
            // AutoTune
            // 
            this.AutoTune.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AutoTune.Enabled = false;
            this.AutoTune.Location = new System.Drawing.Point(120, 228);
            this.AutoTune.Margin = new System.Windows.Forms.Padding(4);
            this.AutoTune.Name = "AutoTune";
            this.AutoTune.Size = new System.Drawing.Size(108, 30);
            this.AutoTune.TabIndex = 14;
            this.AutoTune.Text = "AutoTune";
            this.AutoTune.UseVisualStyleBackColor = true;
            this.AutoTune.Visible = false;
            this.AutoTune.Click += new System.EventHandler(this.AutoTune_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.24442F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.75558F));
            this.tableLayoutPanel1.Controls.Add(this.tlpOverlay, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.dataGrid, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.verticalPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.rtbOverlayCellData, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 28);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 308F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1181, 766);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // tlpOverlay
            // 
            this.tlpOverlay.ColumnCount = 2;
            this.tlpOverlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tlpOverlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpOverlay.Controls.Add(this.xAxisLabel, 0, 0);
            this.tlpOverlay.Controls.Add(this.yAxisComboBox, 1, 1);
            this.tlpOverlay.Controls.Add(this.yAxisLabel, 0, 1);
            this.tlpOverlay.Controls.Add(this.lbOverlayHeaders, 0, 2);
            this.tlpOverlay.Controls.Add(this.xAxisComboBox, 1, 0);
            this.tlpOverlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOverlay.Location = new System.Drawing.Point(904, 3);
            this.tlpOverlay.Name = "tlpOverlay";
            this.tlpOverlay.RowCount = 3;
            this.tlpOverlay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tlpOverlay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tlpOverlay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlpOverlay.Size = new System.Drawing.Size(274, 302);
            this.tlpOverlay.TabIndex = 17;
            // 
            // xAxisLabel
            // 
            this.xAxisLabel.AutoSize = true;
            this.xAxisLabel.Location = new System.Drawing.Point(4, 0);
            this.xAxisLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.xAxisLabel.Name = "xAxisLabel";
            this.xAxisLabel.Size = new System.Drawing.Size(51, 17);
            this.xAxisLabel.TabIndex = 3;
            this.xAxisLabel.Text = "X-Axis:";
            // 
            // yAxisComboBox
            // 
            this.yAxisComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.yAxisComboBox.FormattingEnabled = true;
            this.yAxisComboBox.Location = new System.Drawing.Point(64, 36);
            this.yAxisComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.yAxisComboBox.Name = "yAxisComboBox";
            this.yAxisComboBox.Size = new System.Drawing.Size(206, 24);
            this.yAxisComboBox.TabIndex = 6;
            this.yAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.YAxisComboBox_SelectedIndexChanged);
            // 
            // yAxisLabel
            // 
            this.yAxisLabel.AutoSize = true;
            this.yAxisLabel.Location = new System.Drawing.Point(4, 32);
            this.yAxisLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.yAxisLabel.Name = "yAxisLabel";
            this.yAxisLabel.Size = new System.Drawing.Size(51, 17);
            this.yAxisLabel.TabIndex = 4;
            this.yAxisLabel.Text = "Y-Axis:";
            // 
            // lbOverlayHeaders
            // 
            this.lbOverlayHeaders.CheckOnClick = true;
            this.tlpOverlay.SetColumnSpan(this.lbOverlayHeaders, 2);
            this.lbOverlayHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOverlayHeaders.FormattingEnabled = true;
            this.lbOverlayHeaders.Location = new System.Drawing.Point(4, 68);
            this.lbOverlayHeaders.Margin = new System.Windows.Forms.Padding(4);
            this.lbOverlayHeaders.Name = "lbOverlayHeaders";
            this.lbOverlayHeaders.Size = new System.Drawing.Size(266, 230);
            this.lbOverlayHeaders.Sorted = true;
            this.lbOverlayHeaders.TabIndex = 0;
            this.lbOverlayHeaders.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LbOverlayHeaders_ItemCheck);
            // 
            // xAxisComboBox
            // 
            this.xAxisComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xAxisComboBox.FormattingEnabled = true;
            this.xAxisComboBox.Location = new System.Drawing.Point(64, 4);
            this.xAxisComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.xAxisComboBox.Name = "xAxisComboBox";
            this.xAxisComboBox.Size = new System.Drawing.Size(206, 24);
            this.xAxisComboBox.TabIndex = 5;
            this.xAxisComboBox.SelectedIndexChanged += new System.EventHandler(this.XAxisComboBox_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tableList, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.AutoTune, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.AdditionalLogOverlay, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.smoothButton, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(232, 300);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.AutoScrollMinSize = new System.Drawing.Size(10, 10);
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.horizontalPanel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(244, 4);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(653, 300);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FlowLayoutPanel1_Scroll);
            // 
            // rtbOverlayCellData
            // 
            this.rtbOverlayCellData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbOverlayCellData.EnableAutoDragDrop = true;
            this.rtbOverlayCellData.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOverlayCellData.Location = new System.Drawing.Point(904, 311);
            this.rtbOverlayCellData.Name = "rtbOverlayCellData";
            this.rtbOverlayCellData.Size = new System.Drawing.Size(274, 452);
            this.rtbOverlayCellData.TabIndex = 7;
            this.rtbOverlayCellData.Text = "";
            // 
            // TimingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 820);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TimingForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResizeBegin += new System.EventHandler(this.TimingForm_ResizeBegin);
            this.LocationChanged += new System.EventHandler(this.TimingForm_LocationChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TimingForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpOverlay.ResumeLayout(false);
            this.tlpOverlay.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox tableList;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.Panel horizontalPanel;
        private System.Windows.Forms.Panel verticalPanel;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button smoothButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tuningModeToolStripMenuItem;
        private System.Windows.Forms.Button AdditionalLogOverlay;
        private System.Windows.Forms.Button AutoTune;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logOverlayToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tlpOverlay;
        private System.Windows.Forms.Label xAxisLabel;
        private System.Windows.Forms.ComboBox yAxisComboBox;
        private System.Windows.Forms.Label yAxisLabel;
        private System.Windows.Forms.CheckedListBox lbOverlayHeaders;
        private System.Windows.Forms.ComboBox xAxisComboBox;
        private System.Windows.Forms.RichTextBox rtbOverlayCellData;
    }
}

