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
            Result = string.Join("", Params.FirstOrDefault()?.Take(Convert.ToInt32(Params.LastOrDefault())));
        }
    }
}
