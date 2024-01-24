using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Utility.Identity.DTO;

public class LookupUserDTO
{
    public Guid? AccountID { get; set; }
    public Guid? LookupID { get; set; }
    public int? UserID { get; set; }
    public string Email { get; set; }
}
