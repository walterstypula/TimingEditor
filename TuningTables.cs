using NSFW.TimingEditor.Enums;
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

        private const string Maf = ".*\\bmass[_\\s]airflow\\b.*";
        private const string EngineLoad = ".*\\bengine[_\\s]load\\b.*";

        public TuningTables()
        {
            InitialBaseTiming = new Table(){XAxisHeader = EngineLoad };
            InitialAdvanceTiming = new Table() { XAxisHeader = EngineLoad };
            InitialTotalTiming = new CombinedTable(InitialBaseTiming, InitialAdvanceTiming, Operation.Sum) { XAxisHeader = EngineLoad };
            ModifiedBaseTiming = new Table() { XAxisHeader = EngineLoad };
            ModifiedAdvanceTiming = new PassThroughTable(ModifiedBaseTiming) { XAxisHeader = EngineLoad };
            ModifiedTotalTiming = new CombinedTable(ModifiedBaseTiming, ModifiedAdvanceTiming, Operation.Sum) { XAxisHeader = EngineLoad };
            DeltaTotalTiming = new CombinedTable(InitialTotalTiming, ModifiedTotalTiming, Operation.Difference) { XAxisHeader = EngineLoad };
            TargetFuel = new Table() { XAxisHeader = EngineLoad };
            InitialMaf = new Table(true) { XAxisHeader = Maf }; ;
            ModifiedMaf = new Table(true) { XAxisHeader = Maf }; ;
            DeltaMaf = new CombinedTable(InitialMaf, ModifiedMaf, Operation.Difference) { XAxisHeader = Maf }; ;
        }
    }
}