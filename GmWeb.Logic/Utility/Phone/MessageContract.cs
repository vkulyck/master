using System.Collections.Generic;

namespace GmWeb.Logic.Utility.Phone
{
    public abstract class MessageContract
    {

        public virtual bool IsValid() => true;
        public IList<string> Errors = new List<string>();
    }
}
