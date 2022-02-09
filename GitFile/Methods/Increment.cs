using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitFile.Methods
{
    public class Increment : IGitMethod
    {
        private readonly List<string> _params;
        public string Result { get; set; }   

        public Increment(string param)
        {
            _params = param.Split(',').ToList();
            var varsion = _params.FirstOrDefault();
            var versionSplit = varsion.Split('.');
            var isWasAddition = false;
            var isStop = false;

            for (int i = versionSplit.Length - 1; i > 0; i--)
            {
                if (isStop) break;
                if (isWasAddition && int.Parse(versionSplit[i]) <= int.Parse(_params[i])) break;

                if (int.Parse(versionSplit[i]) < int.Parse(_params[i]))
                {
                    versionSplit[i] = (int.Parse(versionSplit[i]) + 1).ToString();
                    isStop = true;
                }
                else
                {
                    versionSplit[i] = "0";
                    versionSplit[i - 1] = (int.Parse(versionSplit[i - 1]) + 1).ToString();
                    isWasAddition = true;
                }
            }

            Result = string.Join(".", versionSplit);
        }
    }
}
