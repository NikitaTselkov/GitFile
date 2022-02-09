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
        public List<string> Params { get; }
        public string Result { get; }

        // Increment(version, params)
        // Increment(1.0, 9)
        // Increment(1.0.0, 9, 9)
        public Increment(string param)
        {
            Params = param.Split(',').ToList();
            var varsion = Params.FirstOrDefault();
            var versionSplit = varsion.Split('.');
            var isWasAddition = false;
            var isStop = false;

            for (int i = versionSplit.Length - 1; i > 0; i--)
            {
                if (isStop) break;
                if (isWasAddition && int.Parse(versionSplit[i]) <= int.Parse(Params[i])) break;

                if (int.Parse(versionSplit[i]) < int.Parse(Params[i]))
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
