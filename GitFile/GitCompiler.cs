using EnvDTE;
using EnvDTE80;
using GitFile.Methods;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MiscUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private static Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

        public static void StartGitFile(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            DTE2 dte2 = (DTE2)dte;
            string line;
            string output;
            string outputErrors;
            System.Diagnostics.Process p = InitProcess();

            using (StreamReader sr = new StreamReader(filePath))
            {
                try
                {
                    var IsIfTrue = false;

                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine().Trim();

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            if (IsContainsComment(line))
                            {
                                line = DeleteComments(line);
                            }
                            if (IsContainsVariables(line))
                            {
                                line = ReplaceVariablesToValue(line);
                            }
                            if (line.FirstOrDefault() == '.')
                            {
                                CommandTitle = line.Replace(".", "");
                                CommandLine = CommandTitle + " ";
                            }
                            else if (line.FirstOrDefault() == ':')
                            {
                                if (line.Contains(":if"))
                                {
                                    var match = Regex.Match(line, @"\((.*?)\s*(==|!=|>|>=|<|<=)\s*(.*?)\)");

                                    string firstValue = GetVariableValue(match.Groups[1].Value);
                                    string operatorValue = match.Groups[2].Value;
                                    string secondValue = GetVariableValue(match.Groups[3].Value);
                                    var op = ConvertToBinaryConditionOperator<string>(operatorValue);

                                    if (op(firstValue, secondValue))
                                    {
                                        line = line.Replace(Regex.Match(line, @":if\s*\((.*?)\)").Value, "");
                                        ExecuteCommandAndOutputResult(line);
                                        IsIfTrue = true;
                                    }
                                }
                                else if (line.Contains(":else"))
                                {
                                    if (!IsIfTrue)
                                    {
                                        line = line.Replace(Regex.Match(line, @":else\s*").Value, "");
                                        ExecuteCommandAndOutputResult(line);
                                        IsIfTrue = false;
                                    }
                                }
                                else
                                {
                                    string variable = line.Split(' ').First().Replace(":", "");
                                    string value = string.Empty;

                                    GitMethod gitMethod = new GitMethod(line);

                                    if (!gitMethod.IsMethod())
                                    {
                                        (string command, int[] range) = GetCommandAndCountWordsFromLine(line);
                                        string commandOutput = ExecuteCommandAndGetOutput(command);
                                        value = GetValueFromCommandOutput(commandOutput, range);
                                    }
                                    else
                                    {
                                        value = gitMethod.Start();
                                    }

                                    SetVariable(variable, value);

                                    dte2.ToolWindows.CommandWindow.OutputString($"{variable} = {value} \n");
                                }
                            }
                            else ExecuteCommandAndOutputResult(line);
                        }
                    }
                }
                catch (Exception ex) { dte2.ToolWindows.CommandWindow.OutputString(ex.Message); }
            }

            #region ExecuteCommands

            string ExecuteCommand(string command)
            {
                CommandLine += command;
                p.StartInfo.Arguments = "/C " + CommandLine;
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                outputErrors = p.StandardError.ReadToEnd();
                CommandLine = CommandTitle + " ";

                if (!string.IsNullOrEmpty(outputErrors)) throw new Exception(outputErrors);

                return output;
            }

            void ExecuteCommandAndOutputResult(string command)
            {
                ExecuteCommand(command);
                dte2.ToolWindows.CommandWindow.OutputString(output);
            }

            string ExecuteCommandAndGetOutput(string command)
            {
                ExecuteCommand(command);
                return output;
            }

            #endregion
        }

        public static Func<T, T, bool> ConvertToBinaryConditionOperator<T>(string op)
        {
            switch (op)
            {
                case "<": return Operator.LessThan<T>;
                case ">": return Operator.GreaterThan<T>;
                case "==": return Operator.Equal<T>;
                case "!=": return Operator.NotEqual<T>;
                case "<=": return Operator.LessThanOrEqual<T>;
                case ">=": return Operator.GreaterThanOrEqual<T>;       
                default: throw new ArgumentException(nameof(op));
            }
        }

        private static string GetVariableValue(string variable)
        {
            if (Variables.Keys.Contains(variable))
                return Variables[variable];
            else
                return variable;
        }

        private static void SetVariable(string variable, string value)
        {
            if (Variables.Keys.Contains(variable))
                Variables[variable] = value;
            else
                Variables.Add(variable, value);
        }

        private static string GetValueFromCommandOutput(string commandOutput, int[] range)
        {
            string value;

            var split = commandOutput.Replace("\n", "\n ").Split(' ');

            if (range[1] > 0)
                value = string.Join(" ", split.Skip(range[0]).Take(range[1]));
            else
                value = string.Join(" ", split.Skip(range[0]));

            value = value.Trim('\n');

            // Меняем кодировку строки.
            byte[] bytes = Encoding.Default.GetBytes(value);
            return Encoding.UTF8.GetString(bytes);
        }

        private static (string command, int[] range) GetCommandAndCountWordsFromLine(string line)
        {
            string command = string.Empty;
            int[] range = new int[2] { 0, 0 };

            if (line.Contains("=>"))
            {
                var match = Regex.Match(line, "=(.*?)=>");
                var matchRange = Regex.Match(line, @"\((.*?)\)").Value;
                var split = matchRange.Replace("(", "").Replace(")", "").Split(',');

                command = match.Value;
                range[0] = int.Parse(split[0]);

                if (split.Length > 1)
                    range[1] = int.Parse(split[1]);
            }
            else
            {
                command = new string(line.SkipWhile(s => s != '=').ToArray());
            }

            command = command.Replace("=>", "").Replace("=", "");
            return (command, range);
        }

        private static bool IsContainsComment(string line)
        {
            return line.Contains("<") && line.Contains(">");
        }

        private static bool IsContainsVariables(string line)
        {
            return line.Contains("{") && line.Contains("}");
        }

        private static string ReplaceVariablesToValue(string line)
        {
            var variableName = string.Empty;

            foreach (Match match in Regex.Matches(line, "{(.*?)}"))
            {
                variableName = match.Value.Replace("{", "").Replace("}", "");
                line = line.Replace(match.Value, GetVariableValue(variableName));
            }

            return line;
        }

        private static string DeleteComments(string line)
        {
            foreach (Match match in Regex.Matches(line, "<(.*?)>"))
            {
                line = line.Replace(match.Value, "");
            }

            return line;
        }

        private static System.Diagnostics.Process InitProcess()
        {
            var p = new System.Diagnostics.Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            return p;
        }
    }
}
