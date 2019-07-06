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
            this.copyButton = new System.Windows.Forms.Button();
            this.pasteButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.smoothButton = new System.Windows.Forms.Button();
            this.logOverlayButton = new System.Windows.Forms.Button();
            this.redoButton = new System.Windows.Forms.Button();
            this.undoButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tuningModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AdditionalLogOverlay = new System.Windows.Forms.Button();
            this.AutoTune = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableList
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.tableList, 3);
            this.tableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList.FormattingEnabled = true;
            this.tableList.IntegralHeight = false;
            this.tableList.Location = new System.Drawing.Point(3, 3);
            this.tableList.Name = "tableList";
            this.tableList.Size = new System.Drawing.Size(168, 145);
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
            this.dataGrid.Location = new System.Drawing.Point(183, 253);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.Size = new System.Drawing.Size(700, 364);
            this.dataGrid.TabIndex = 1;
            this.dataGrid.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DataGrid_CellBeginEdit);
            this.dataGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellEnter);
            this.dataGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellLeave);
            this.dataGrid.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellMouseEnter);
            this.dataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellValueChanged);
            this.dataGrid.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGrid_ColumnHeaderMouseClick);
            this.dataGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DataGrid_EditingControlShowing);
            this.dataGrid.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGrid_RowHeaderMouseClick);
            this.dataGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGrid_KeyDown);
            this.dataGrid.Leave += new System.EventHandler(this.DataGrid_Leave);
            // 
            // horizontalPanel
            // 
            this.horizontalPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalPanel.BackColor = System.Drawing.SystemColors.Window;
            this.horizontalPanel.Location = new System.Drawing.Point(3, 3);
            this.horizontalPanel.Name = "horizontalPanel";
            this.horizontalPanel.Size = new System.Drawing.Size(163, 220);
            this.horizontalPanel.TabIndex = 2;
            this.horizontalPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HorizontalPanel_MouseDown);
            // 
            // verticalPanel
            // 
            this.verticalPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.verticalPanel.BackColor = System.Drawing.SystemColors.Window;
            this.verticalPanel.Location = new System.Drawing.Point(3, 253);
            this.verticalPanel.Name = "verticalPanel";
            this.verticalPanel.Size = new System.Drawing.Size(174, 171);
            this.verticalPanel.TabIndex = 3;
            this.verticalPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VerticalPanel_MouseDown);
            // 
            // copyButton
            // 
            this.copyButton.Location = new System.Drawing.Point(3, 154);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(44, 23);
            this.copyButton.TabIndex = 4;
            this.copyButton.Text = "&Copy";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.Location = new System.Drawing.Point(53, 154);
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.Size = new System.Drawing.Size(44, 23);
            this.pasteButton.TabIndex = 5;
            this.pasteButton.Text = "&Paste";
            this.pasteButton.UseVisualStyleBackColor = true;
            this.pasteButton.Click += new System.EventHandler(this.PasteButton_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 644);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(886, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(59, 17);
            this.toolStripStatusLabel1.Text = "statusText";
            // 
            // smoothButton
            // 
            this.smoothButton.Location = new System.Drawing.Point(3, 216);
            this.smoothButton.Name = "smoothButton";
            this.smoothButton.Size = new System.Drawing.Size(44, 23);
            this.smoothButton.TabIndex = 7;
            this.smoothButton.Text = "&Smooth";
            this.smoothButton.UseVisualStyleBackColor = true;
            this.smoothButton.Click += new System.EventHandler(this.SmoothButton_Click);
            // 
            // logOverlayButton
            // 
            this.logOverlayButton.Location = new System.Drawing.Point(103, 154);
            this.logOverlayButton.Name = "logOverlayButton";
            this.logOverlayButton.Size = new System.Drawing.Size(68, 23);
            this.logOverlayButton.TabIndex = 8;
            this.logOverlayButton.Text = "Log &Overlay";
            this.logOverlayButton.UseVisualStyleBackColor = true;
            this.logOverlayButton.Click += new System.EventHandler(this.LogOverlayButton_Click);
            // 
            // redoButton
            // 
            this.redoButton.Location = new System.Drawing.Point(3, 185);
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(44, 23);
            this.redoButton.TabIndex = 9;
            this.redoButton.Text = "&Redo";
            this.redoButton.UseVisualStyleBackColor = true;
            this.redoButton.Click += new System.EventHandler(this.RedoButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Location = new System.Drawing.Point(53, 185);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(44, 23);
            this.undoButton.TabIndex = 10;
            this.undoButton.Text = "&Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tuningModeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(886, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tuningModeToolStripMenuItem
            // 
            this.tuningModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timingToolStripMenuItem,
            this.mAFToolStripMenuItem});
            this.tuningModeToolStripMenuItem.Name = "tuningModeToolStripMenuItem";
            this.tuningModeToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.tuningModeToolStripMenuItem.Text = "Tuning Mode";
            // 
            // timingToolStripMenuItem
            // 
            this.timingToolStripMenuItem.Name = "timingToolStripMenuItem";
            this.timingToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.timingToolStripMenuItem.Text = "Timing";
            this.timingToolStripMenuItem.Click += new System.EventHandler(this.TimingToolStripMenuItem_Click);
            // 
            // mAFToolStripMenuItem
            // 
            this.mAFToolStripMenuItem.Name = "mAFToolStripMenuItem";
            this.mAFToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.mAFToolStripMenuItem.Text = "Maf";
            this.mAFToolStripMenuItem.Click += new System.EventHandler(this.MAFToolStripMenuItem_Click);
            // 
            // AdditionalLogOverlay
            // 
            this.AdditionalLogOverlay.Enabled = false;
            this.AdditionalLogOverlay.Location = new System.Drawing.Point(103, 185);
            this.AdditionalLogOverlay.Name = "AdditionalLogOverlay";
            this.AdditionalLogOverlay.Size = new System.Drawing.Size(68, 23);
            this.AdditionalLogOverlay.TabIndex = 13;
            this.AdditionalLogOverlay.Text = "Addt’l Log";
            this.AdditionalLogOverlay.UseVisualStyleBackColor = true;
            this.AdditionalLogOverlay.Click += new System.EventHandler(this.AdditionalLogOverlay_Click);
            // 
            // AutoTune
            // 
            this.AutoTune.Enabled = false;
            this.AutoTune.Location = new System.Drawing.Point(103, 216);
            this.AutoTune.Name = "AutoTune";
            this.AutoTune.Size = new System.Drawing.Size(68, 23);
            this.AutoTune.TabIndex = 14;
            this.AutoTune.Text = "AutoTune";
            this.AutoTune.UseVisualStyleBackColor = true;
            this.AutoTune.Visible = false;
            this.AutoTune.Click += new System.EventHandler(this.AutoTune_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.dataGrid, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.verticalPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(886, 620);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel2.Controls.Add(this.tableList, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.AutoTune, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.copyButton, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.AdditionalLogOverlay, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.redoButton, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.logOverlayButton, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.undoButton, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.smoothButton, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.pasteButton, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(174, 244);
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(183, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(700, 244);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.FlowLayoutPanel1_Scroll);
            // 
            // TimingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 666);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
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
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button smoothButton;
        private System.Windows.Forms.Button logOverlayButton;
        private System.Windows.Forms.Button redoButton;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tuningModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAFToolStripMenuItem;
        private System.Windows.Forms.Button AdditionalLogOverlay;
        private System.Windows.Forms.Button AutoTune;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}

