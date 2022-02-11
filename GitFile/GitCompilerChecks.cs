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
            return line.Contains("<") && line.Contains(">");
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
    }
}
