using System;
using System.Reflection;
using System.Windows.Forms;
using NSFW.TimingEditor.Utils;

namespace NSFW.TimingEditor
{
    internal static class Program
    {
        public static bool Debug;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "debug")
                {
                    Debug = true;
                }
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.ManifestModule.Name;

            if ((string.Compare("tableeditor.exe", name, StringComparison.OrdinalIgnoreCase) == 0) ||
                (args.Length == 1 && args[0] == "table"))
            {
                Util.DoubleFormat = "0.0000";
                Util.RowHeaderWidth = 60;
                Util.ColumnWidth = 60;
            }
            else
            {
                Util.DoubleFormat = "0.00";
                Util.RowHeaderWidth = 60;
                Util.ColumnWidth = 40;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TimingForm());
        }
    }
}