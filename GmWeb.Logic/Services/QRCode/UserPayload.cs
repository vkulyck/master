using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User = GmWeb.Logic.Data.Models.Carma.User;

namespace GmWeb.Logic.Services.QRCode;

public class UserPayload : GuidPayload
{
    public UserPayload(User user) : base(user.LookupID) { }
}
