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
        public static bool IsContainsComment(string line)
        {
            return line.Contains("<!") && line.Contains(">");
        }

        public static bool IsContainsVariable(string line)
        {
            return Regex.Match(line, @":(.*?)\s*=").Success;
        }
        
        public static bool IsContainsVariablesInCommand(string line)
        {
            return line.Contains("{") && line.Contains("}");
        }

        public static bool IsNeedIgnorOutput(string line)
        {
            return Regex.Match(line, @"-->\s*Ignor").Success;
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
