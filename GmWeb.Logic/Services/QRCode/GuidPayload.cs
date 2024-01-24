using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User = GmWeb.Logic.Data.Models.Carma.User;
using QRCoder;

namespace GmWeb.Logic.Services.QRCode;

using BasePayload = QRCoder.PayloadGenerator.Payload;

public class GuidPayload : PayloadGenerator.Payload
{
    public Guid Payload { get; set; }
    public GuidPayload(Guid payload)
    {
        this.Payload = payload;
    }
    public override string ToString() => this.Payload.ToString();
}
