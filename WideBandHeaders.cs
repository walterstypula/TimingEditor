namespace NSFW.TimingEditor
{
    public class WideBandHeaders
    {
        public const string AEM_UEGO_9600 = "AEM UEGO Wideband [9600 baud] (AFR Gasoline)";
    }

    public class RequiredLogHeaders
    {
        public const string RpmRegEx = ".*\\b(engine[_\\s]speed|rpm)\\b.*";
        public const string EngineLoadRegEx = ".*\\bengine[_\\s]load\\b.*";
        public const string MafvRegEx = ".*\\bmass[_\\s]airflow\\b.*";
    }
}