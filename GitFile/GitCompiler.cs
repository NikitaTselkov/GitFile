using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            string line;
            string output;
            string outputErrors;
            var p = new System.Diagnostics.Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

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
                        else if (IsContainsComment(line))
                        {
                            line = DeleteComments(line);

                            if (line.Any(a => char.IsLetter(a)))
                                ExecuteCommand();
                        }
                        else ExecuteCommand();
                    }
                }
            }

            void ExecuteCommand()
            {
                CommandLine += line;
                p.StartInfo.Arguments = "/C " + CommandLine;
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                outputErrors = p.StandardError.ReadToEnd();

                dte2.ToolWindows.CommandWindow.OutputString(string.IsNullOrWhiteSpace(output) ? outputErrors : output);
                CommandLine = CommandTitle + " ";
            }
        }

        private static bool IsContainsComment(string line)
        {
            return line.Contains("<") && line.Contains(">");
        }

        private static string DeleteComments(string line)
        {
            foreach (Match match in Regex.Matches(line, "<(.*?)>"))
            {
                line = line.Replace(match.Value, "");
            }

            return line;
        }
    }
}
