using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitFile.Methods
{
    public class GitMethod
    {
        private string _currentMethod;
        private Methods _currentMethodName;

        public GitMethod(string code)
        {
            _currentMethod = GetMethodFromLine(code);
            _currentMethodName = GetMethodName();
        }

        public string Start()
        {
            var gitMethod = (IGitMethod)Activator.CreateInstance(Type.GetType("GitFile.Methods." + _currentMethodName), GetParams());
            return gitMethod.Result;
        }

        public bool IsMethod()
        {
            return _currentMethodName != Methods.Null;
        }

        private string GetParams()
        {
            return Regex.Match(_currentMethod, @"\((.*?)\)").Value.Replace("(", "").Replace(")", "");
        }

        private string GetMethodFromLine(string line)
        {
            return new string(line.SkipWhile(s => s != '=').ToArray()).Replace("=", "").Trim();
        }
        
        private Methods GetMethodName()
        {
            Methods result = Methods.Null;
            Enum.TryParse(new string(_currentMethod.TakeWhile(s => s != '(').ToArray()), out result);
            return result;
        }
    }
}
