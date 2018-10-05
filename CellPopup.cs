using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NSFW.TimingEditor
{
        public class CellPopup : Form
        {
            public CellPopup()
            {
                textBox = new RichTextBox();
                SuspendLayout();

                textBox.BackColor = Color.White;
                textBox.Padding = new Padding(3, 3, 3, 3);
                textBox.ReadOnly = true;
                textBox.BorderStyle = BorderStyle.None;
                textBox.Dock = DockStyle.Fill;
                textBox.Name = "textBox";
                textBox.ScrollBars = RichTextBoxScrollBars.Vertical;
                textBox.Size = new Size(175, 220);
                textBox.TabIndex = 0;
                textBox.Text = "";

                AutoScaleDimensions = new SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                AutoScroll = true;
                Padding = new Padding(3, 3, 3, 3);
                BackgroundImageLayout = ImageLayout.None;
                ClientSize = new Size(175, 220);
                ControlBox = false;
                Controls.Add(textBox);
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

            private IContainer components = null;
            public RichTextBox textBox;
        }
}