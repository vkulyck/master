using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User = GmWeb.Logic.Data.Models.Carma.User;

namespace GmWeb.Logic.Services.QRCode;
using BasePayload = QRCoder.PayloadGenerator.Payload;
public class TotpPayload : BasePayload
{
    public string Payload => this.Config.AuthenticatorUri;
    public TotpConfig Config { get; private set; }
    public TotpPayload(TotpConfig config)
    {
        this.Config = config;
    }

    public override string ToString() => this.Payload;
}
