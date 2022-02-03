using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitFile
{
    public static class GitCompiler
    {
        private static string CommandTitle { get; set; }
        private static string CommandLine { get; set; }

        public static void StartGitFile(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            DTE2 dte2 = (DTE2)dte;
            var p = new System.Diagnostics.Process();
            string line;

            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().Trim();

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Contains("."))
                        {
                            CommandTitle = line.Replace(".", "");
                            CommandLine = CommandTitle + " ";
                        }
                        else
                        {
                            CommandLine += line;

                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.FileName = "cmd.exe";
                            p.StartInfo.Arguments = "/C " + CommandLine;
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.Start();

                            string output = p.StandardOutput.ReadToEnd();

                            dte2.ToolWindows.CommandWindow.OutputString(output);

                            CommandLine = CommandTitle + " ";
                        }
                    }
                }
            }
        }
    }
}
