using System;
using System.Collections.Generic;

namespace NSFW.TimingEditor
{
    public abstract class Command
    {
        public abstract void Execute();

        public abstract void Undo();
    }

    public class EditCell : Command
    {
        private ITable table;
        private double oldValue;
        private double newValue;
        private int x;
        private int y;

        public ITable Table { get { return table; } }
        public int Y { get { return y; } }
        public int X { get { return x; } }
        public double OldValue { get { return oldValue; } }
        public double NewValue { get { return newValue; } }

        public EditCell(ITable table, int x, int y, double newValue)
        {
            this.table = table;
            this.x = x;
            this.y = y;
            this.newValue = newValue;
            oldValue = this.table.GetCell(this.x, this.y);
        }

        public override void Execute()
        {
            table.SetCell(x, y, newValue);
        }

        public override void Undo()
        {
            table.SetCell(x, y, oldValue);
        }
    }

    /*    public class EditMultipleCells : Command
        {
            private IList<EditCell> cells;
            public EditMultipleCells(IList<EditCell> cells)
            {
                this.cells = cells;
            }

            public override void Execute()
            {
                foreach (EditCell cell in this.cells)
                {
                    cell.Execute();
                }
            }

            public override void Undo()
            {
                foreach (EditCell cell in this.cells)
                {
                    cell.Undo();
                }
            }
        }

        public class Paste : Command
        {
            private ITable source;
            private ITable destination;
            private ITable backup;

            public Paste(ITable source, ITable destination)
            {
                this.source = source;
                this.destination = destination;
                this.backup = destination.Clone();
            }

            public override void Execute()
            {
                this.source.CopyTo(destination);
            }

            public override void Undo()
            {
                this.backup.CopyTo(this.destination);
            }
        }

        public class DoublePaste : Command
        {
            private Paste initial;
            private Paste modified;

            public DoublePaste(Paste initial, Paste modified)
            {
                this.initial = initial;
                this.modified = modified;
            }

            public override void Execute()
            {
                this.initial.Execute();
                this.modified.Execute();
            }

            public override void Undo()
            {
                this.initial.Undo();
                this.modified.Undo();
            }
        }
    */

    public delegate void UpdateCommandHistoryButtons(object sender, EventArgs args);

    public class CommandHistory
    {
        private static CommandHistory instance = new CommandHistory();
        private List<Command> commands;
        private List<Command> undone;

        public event UpdateCommandHistoryButtons UpdateCommandHistoryButtons;

        private CommandHistory()
        {
            commands = new List<Command>();
            undone = new List<Command>();
        }

        public static CommandHistory Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return instance;
            }
        }

        public bool CanUndo { get { return commands.Count > 0; } }
        public bool CanRedo { get { return undone.Count > 0; } }

        public void Execute(Command command)
        {
            command.Execute();
            commands.Add(command);
            undone.Clear();
            UpdateButtons();
        }

        public Command Undo()
        {
            if (commands.Count == 0)
            {
                return null;
            }

            int lastIndex = commands.Count - 1;
            Command command = commands[lastIndex];
            commands.RemoveAt(lastIndex);

            command.Undo();

            undone.Add(command);
            UpdateButtons();

            return command;
        }

        public Command Redo()
        {
            if (undone.Count == 0)
            {
                return null;
            }

            int lastIndex = undone.Count - 1;
            Command command = undone[lastIndex];
            undone.RemoveAt(lastIndex);

            command.Execute();

            commands.Add(command);
            UpdateButtons();

            return command;
        }

        private void UpdateButtons()
        {
            if (UpdateCommandHistoryButtons != null)
            {
                UpdateCommandHistoryButtons(this, new EventArgs());
            }
        }
    }
}