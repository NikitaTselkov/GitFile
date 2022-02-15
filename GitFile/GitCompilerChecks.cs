using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitFile
{
    public static class GitCompilerChecks
    {
        private static HashSet<string> _variables { get; set; } = new HashSet<string>() { "Empty" };

        public static void SetVariable(string variable)
        {
            _variables.Add(variable);
        }

        public static bool IsContainsComment(string line)
        {
            return line.Contains("<!") && line.Contains(">");
        }

        public static bool IsContainsVariable(string line)
        {
            var match = Regex.Match(line, @":(.*?)\s*=");
            return match.Success && !match.Value.Contains("Empty");
        }
        
        public static bool IsContainsVariablesInCommand(string line)
        {
            return line.Contains("{") && line.Contains("}");
        }

        public static bool IsNeedIgnoreOutput(string line)
        {
            return Regex.Match(line, @"-->\s*Ignore").Success;
        }

        public static bool IsContainsRange(string line)
        {
            return Regex.Match(line, @"=>\s*\((.*?)\)").Success;
        }

        public static bool IsExistsVariables(string line, out (string first, string second) variables)
        {
            variables = (string.Empty, string.Empty);
            var match = Regex.Match(line, @"\((.*?)\s*(==|!=|>=|>|<=|<)\s*(.*?)\)");

            string firstValue = match.Groups[1].Value;
            string secondValue = match.Groups[3].Value;

            if (_variables.Contains(firstValue))
                variables.first = firstValue;

            if (_variables.Contains(secondValue))
                variables.second = secondValue;

            return variables != (string.Empty, string.Empty);
        }

        public static bool IsContainsMethod(string line, out string methodName)
        {
            if (line.Contains(":if"))
                line = new string(line.SkipWhile(s => s != ')').ToArray());

            var code = new string(line.SkipWhile(s => s != '=').ToArray()).Replace("=", "").Trim();
            var result = Methods.Methods.Null;
            methodName = string.Empty;

            Enum.TryParse(new string(code.TakeWhile(s => s != '(').ToArray()), out result);

            if (result != Methods.Methods.Null)
            {
                methodName = result.ToString();
                return true;
            }

            return false;
        }
    }
}
