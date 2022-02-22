using EnvDTE;
using EnvDTE80;
using GitFile.Methods;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
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
        private static string _commandTitle { get; set; }
        private static string _commandLine { get; set; }

        private static Dictionary<string, string> _variables { get; set; } = new Dictionary<string, string>() { { "Empty", string.Empty } };

        private static DTE _dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
        private static DTE2 _dte2 = (DTE2)_dte;
        private static string _output;
        private static string _outputErrors;
        private static System.Diagnostics.Process _process = InitProcess();
        private static bool _isIfTrue = false;
        private static bool _isNeedIgnoreOutput = false;

        public static void StartGitFile(string filePath)
        {
            string line;

            using (StreamReader sr = new StreamReader(filePath, Encoding.Default))
            {
                try
                {
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine().Trim();
                        GitCompilFile(line);
                        _isNeedIgnoreOutput = false;
                    }
                }
                catch (Exception ex) 
                {
                    _isNeedIgnoreOutput = false;
                    DisplayText(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        public static void SaveGitFile(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var document = _dte2.Documents.Cast<Document>().FirstOrDefault(doc => doc.FullName.ToLower() == filePath.ToLower());
            document.Save();  
        }

        private static void GitCompilFile(string line)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                if (GitCompilerChecks.IsContainsComment(line))
                {
                    line = DeleteComments(line);
                }
                if (GitCompilerChecks.IsContainsVariablesInCommand(line))
                {
                    line = ReplaceVariablesToValue(line);
                }
                if (GitCompilerChecks.IsNeedIgnoreOutput(line))
                {
                    line = line.Replace(Regex.Match(line, @"-->\s*Ignore").Value, "");
                    _isNeedIgnoreOutput = true;
                }
                if (line.FirstOrDefault() == '.')
                {
                    _commandTitle = line.Replace(".", "");
                    _commandLine = _commandTitle + " ";
                }
                else if (line.FirstOrDefault() == ':')
                {
                    if (line.Contains(":if"))
                    {
                        var match = Regex.Match(line, @"\((.*?)\s*(==|!=|>=|>|<=|<)\s*(.*?)\)");

                        string firstValue = GetVariableValue(match.Groups[1].Value);
                        string operatorValue = match.Groups[2].Value;
                        string secondValue = GetVariableValue(match.Groups[3].Value);

                        if (operatorValue == "==" || operatorValue == "!=")
                        {
                            var op = ConvertToBinaryConditionOperator<string>(operatorValue);

                            if (op(firstValue, secondValue))
                                _isIfTrue = true;
                        }
                        else
                        {
                            var op = ConvertToBinaryConditionOperator<int>(operatorValue);

                            if (op(ConvertStringToInt32(firstValue), ConvertStringToInt32(secondValue)))
                                _isIfTrue = true;
                        }

                        if (_isIfTrue)
                        {
                            line = line.Replace(Regex.Match(line, @":if\s*\((.*?)\)").Value, "");

                            if (line.Contains(":")) 
                                GitCompilFile(line.Trim());
                            else
                                ExecuteCommandAndOutputResult(line);
                        }
                    }
                    else if (line.Contains(":else"))
                    {
                        if (!_isIfTrue)
                        {
                            line = line.Replace(Regex.Match(line, @":else\s*").Value, "");

                            if (line.Contains(":"))
                                GitCompilFile(line.Trim());
                            else
                                ExecuteCommandAndOutputResult(line);
                        }

                        _isIfTrue = false;
                    }
                    else
                    {
                        string variable = line.Split(' ').First().Replace(":", "");

                        if (variable != "Empty")
                        {
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
                            DisplayText($"{variable} = {value} \n");
                        }
                    }
                }
                else ExecuteCommandAndOutputResult(line);
            }
        }

        #region ExecuteCommands

        private static string ExecuteCommand(string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            { 
                _commandLine += command;
                _process.StartInfo.Arguments = "/C " + _commandLine;
                _process.Start();

                _output = _process.StandardOutput.ReadToEnd();
                _outputErrors = _process.StandardError.ReadToEnd();
                _commandLine = _commandTitle + " ";

                if (!string.IsNullOrEmpty(_outputErrors))
                {
                    DisplayText(_outputErrors);

                    if (_outputErrors.StartsWith("git:"))
                    {
                        throw new Exception("Execution is suspended!");
                    }
                }
            }

            return _output;
        }

        private static void ExecuteCommandAndOutputResult(string command)
        {
            ExecuteCommand(command);
            DisplayText(_output);
        }

        private static string ExecuteCommandAndGetOutput(string command)
        {
            ExecuteCommand(command);
            return _output;
        }

        #endregion

        private static Func<T, T, bool> ConvertToBinaryConditionOperator<T>(string op)
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

        private static void DisplayText(string text)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!_isNeedIgnoreOutput)
            {
                var pane = GetPane("GitCompiler");

                _dte2.ExecuteCommand("View.Output");
                pane.OutputString(text);
                pane.Activate();
            }
        }

        private static OutputWindowPane GetPane(string title)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OutputWindowPanes panes = _dte2.ToolWindows.OutputWindow.OutputWindowPanes;

            try
            {
               return panes.Item(title);
            }
            catch (ArgumentException)
            {
               return panes.Add(title);
            }
        }

        private static int ConvertStringToInt32(string value)
        {
            var result = string.Empty;

            foreach (var item in value)
            {
                if (char.IsDigit(item))
                    result += item;
            }

            if (string.IsNullOrEmpty(result)) result = "0";

            return Convert.ToInt32(result);
        }

        private static string GetVariableValue(string variable)
        {
            if (_variables.Keys.Contains(variable))
                return _variables[variable];
            else
                return variable;
        }

        private static void SetVariable(string variable, string value)
        {
            if (_variables.Keys.Contains(variable))
                _variables[variable] = value;
            else
                _variables.Add(variable, value);
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
            Match match = Regex.Match(line, "<!(.*?)>$");          
            line = line.Replace(match.Value, "");

            return line;
        }

        private static System.Diagnostics.Process InitProcess()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var p = new System.Diagnostics.Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(_dte.Solution.FullName);
            return p;
        }
    }
}
