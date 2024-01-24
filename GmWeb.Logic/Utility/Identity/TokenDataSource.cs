using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Identity;

[Flags]
public enum TokenDataSource
{
    Cookie = 1,
    Header = 2,
    Parameter = 4
}
