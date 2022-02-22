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
            var version = Params.FirstOrDefault();
            var versionSplit = version.Split('.');
            var isWasAddition = false;
            var isStop = false;

            if (Params.FirstOrDefault() == "") throw new ArgumentNullException("Increment parameters cannot be empty");
            if (Params.Any(a => a.Any(c => char.IsLetter(c)))) throw new FormatException("Increment parameters had the wrong format");
            if (Params.Count != versionSplit.Length) throw new FormatException("Increment parameters had the wrong format");
            if (versionSplit.Any(a => a == "")) throw new FormatException("Increment parameters had the wrong format");
            if (versionSplit.Concat(Params.Skip(1)).Any(a => int.TryParse(a, out int result) == false)) throw new ArgumentOutOfRangeException(nameof(Params), "Increment parameters exceeded the maximum size");

            if (Params.Count == 1) versionSplit[0] = (int.Parse(version) + 1).ToString();
            else
            {
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
            }

            Result = string.Join(".", versionSplit);
        }
    }
}
