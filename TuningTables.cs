﻿using NSFW.TimingEditor.Enums;
using NSFW.TimingEditor.Tables;

namespace NSFW.TimingEditor
{
    public class TuningTables
    {
        public ITable InitialBaseTiming { get; }
        public ITable InitialAdvanceTiming { get; }
        public ITable InitialTotalTiming { get; }
        public ITable ModifiedBaseTiming { get; }
        public ITable ModifiedAdvanceTiming { get; }
        public ITable ModifiedTotalTiming { get; }
        public ITable DeltaTotalTiming { get; }

        public ITable TargetFuel { get; }
        public ITable InitialMaf { get; }
        public ITable ModifiedMaf { get; }
        public ITable DeltaMaf { get; }

        public TuningTables()
        {
            InitialBaseTiming = new Table();
            InitialAdvanceTiming = new Table();
            InitialTotalTiming = new CombinedTable(InitialBaseTiming, InitialAdvanceTiming, Operation.Sum);
            ModifiedBaseTiming = new Table();
            ModifiedAdvanceTiming = new PassThroughTable(ModifiedBaseTiming);
            ModifiedTotalTiming = new CombinedTable(ModifiedBaseTiming, ModifiedAdvanceTiming, Operation.Sum);
            DeltaTotalTiming = new CombinedTable(InitialTotalTiming, ModifiedTotalTiming, Operation.Difference);
            TargetFuel = new Table();
            InitialMaf = new Table(true);
            ModifiedMaf = new Table(true);
            DeltaMaf = new CombinedTable(InitialMaf, ModifiedMaf, Operation.Difference);
        }
    }
}