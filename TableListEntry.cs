﻿namespace NSFW.TimingEditor
{
    public class TableListEntry
    {
        public string Description { get; }
        public ITable Table { get; }
        public bool AllowPaste { get; }
        public bool HasData { get; }
        public string StatusText { get; }

        public TableListEntry(string description, ITable table, bool allowPaste, string statusText)
        {
            Description = description;
            Table = table;
            AllowPaste = allowPaste;
            HasData = false;
            StatusText = statusText;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}