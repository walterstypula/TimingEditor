using NSFW.TimingEditor.Enums;

namespace NSFW.TimingEditor
{
    public class TableListEntry
    {
        public string Description { get; }
        public ITable Table { get; }
        public bool AllowPaste { get; }
        public string StatusText { get; }
        public TuningMode TuningMode { get; }

        public TableListEntry(string description, ITable table, bool allowPaste, string statusText, TuningMode tuningMode)
        {
            Description = description;
            Table = table;
            AllowPaste = allowPaste;
            StatusText = statusText;
            TuningMode = tuningMode;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}