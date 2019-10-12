namespace NSFW.TimingEditor
{
    public class RequiredLogHeaders
    {
        public const string RpmRegEx = ".*\\b(engine[_\\s]speed|rpm)\\b.*";
        public const string EngineLoadRegEx = ".*\\bengine[_\\s]load\\b.*";
        public const string MafvRegEx = ".*\\bmass.*voltage\\b.*";
        public const string FbkcRegEx = ".*\\bfeedback.*knock\\b.*";
        public const string FlkcRegEx = ".*\\bfine.*learning.*knock\\b.*";
        public const string AfCorrectionRegEx = ".*\\ba/f.*correction.*#1\\b.*";
        public const string AfLearningRegEx = ".*\\ba/f.*learning.*#1\\b.*";
    }
}