using System.Collections.Generic;

namespace GmWeb.Logic.Utility.Phone
{
    public class ServiceResponse
    {
        public bool IsValid { get; set; }
        public IList<string> Errors = new List<string>();
    }
}
