using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Importing.Clients;

[Flags]
public enum ImportSources
{
    None =              0b00,
    Imagesets =         0b01,
    Spreadsheets =       0b10
}
