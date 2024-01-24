using System.Collections.Generic;

namespace GmWeb.Logic.Utility.Identity.DTO;

public class RecoveryCodesDTO
{
    public string AccountID { get; set; }
    public int Number { get; set; }
    public List<string> RecoveryCodes { get; set; }
}
