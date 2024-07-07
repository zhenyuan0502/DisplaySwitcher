using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplaySwitcher
{
    public static class DisplaySwitcherFunction
    {
        public static string WINDOWS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static string DISPLAY_SWITCH__PATH = Path.Combine(WINDOWS_PATH, "System32", "DisplaySwitch");
        public static Dictionary<string, string> SCREEN_OPTION = new Dictionary<string, string>()
        {
            { "First", "1" },
            { "Second", "4" },
            { "Duplicate", "2" },
            { "Extend", "3" }
        };

        public static void StartScreen(string screenNumberOption)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {DISPLAY_SWITCH__PATH} {screenNumberOption}";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
