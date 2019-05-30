using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
    public class CellPopup : Form
    {
        public CellPopup()
        {
            TextBox = new RichTextBox();
            SuspendLayout();

            TextBox.BackColor = Color.White;
            TextBox.Padding = new Padding(3, 3, 3, 3);
            TextBox.ReadOnly = true;
            TextBox.BorderStyle = BorderStyle.None;
            TextBox.Dock = DockStyle.Fill;
            TextBox.Name = "textBox";
            TextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            TextBox.Size = new Size(175, 220);
            TextBox.TabIndex = 0;
            TextBox.Text = "";

            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Padding = new Padding(3, 3, 3, 3);
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(175, 220);
            ControlBox = false;
            Controls.Add(TextBox);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CellPopup";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override bool ShowWithoutActivation => true;

        private readonly IContainer components = null;
        public readonly RichTextBox TextBox;
    }
}