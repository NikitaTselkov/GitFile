using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitFileShared.Methods
{
    public interface IGitMethod
    {
        List<string> Params { get; }
        string Result { get; }
    }
}
