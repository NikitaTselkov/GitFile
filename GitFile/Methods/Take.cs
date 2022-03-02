using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitFile.Methods
{
    public class Take : IGitMethod
    {
        public List<string> Params { get; }
        public string Result { get; }

        // Take(value, count)
        public Take(string param)
        {
            Params = param.Split(',').ToList();

            if (Params.FirstOrDefault() == "")
            {
                Result = "";
                return;
            }

            if (Params.Count <= 1 || Params.Count > 2) throw new FormatException("Take parameters had the wrong format");
            if (Params.Last().Any(a => char.IsLetter(a))) throw new FormatException("Take parameters had the wrong format");

            Result = string.Join("", Params.FirstOrDefault()?.Take(Convert.ToInt32(Params.LastOrDefault())));
        }
    }
}
