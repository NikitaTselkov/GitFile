using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitFile.Methods
{
    public class String : IGitMethod
    {
        public List<string> Params { get; }
        public string Result { get; }

        // String(value)
        public String(string param)
        {
            Params = new List<string>() { param };
            Result = param;
        }
    }
}
