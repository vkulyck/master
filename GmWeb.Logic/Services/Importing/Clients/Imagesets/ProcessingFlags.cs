using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Importing.Clients.Imagesets;
[Flags]
public enum ProcessingFlags
{
    None = 0b00,
    Resize = 0b01,
    Compress = 0b10,
    Minimized = 0b11
}