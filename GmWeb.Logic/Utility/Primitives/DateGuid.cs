using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Extensions.Collections;

namespace GmWeb.Logic.Utility.Primitives;

public record class DateGuid
{
    public DateTime DateTime { get; }
    public HalfGuid HalfGuid { get; }
    public Guid Guid { get; }

    public DateGuid(string guid, DateTime dt) : this(new HalfGuid(guid), dt) { }
    public DateGuid(Guid guid, DateTime dt) : this(new HalfGuid(guid), dt) { }
    public DateGuid(HalfGuid hg, DateTime dt)
    {
        this.DateTime = dt;
        this.HalfGuid = new HalfGuid(hg);
        this.Guid = new Guid(DateGuid.ToByteArray(this.HalfGuid, this.DateTime));
    }
    public DateGuid(string s) : this(new Guid(s)) { }
    public DateGuid(Guid guid)
    {
        this.Guid = guid;
        var bytes = this.Guid.ToByteArray();

        var hgBytes = bytes.Take(8).ToArray();
        this.HalfGuid = new HalfGuid(hgBytes);

        var dtBytes = bytes.Skip(8).ToArray();
        this.DateTime = DateTime.FromBinary(BitConverter.ToInt64(dtBytes)).ToLocalTime();
    }
    public byte[] ToByteArray()
        => DateGuid.ToByteArray(this.HalfGuid, this.DateTime);
    private static byte[] ToByteArray(HalfGuid hg, DateTime dt)
        => hg.ToByteArray().Concat(BitConverter.GetBytes(dt.ToUniversalTime().Ticks)).ToArray();

    public override String ToString()
        => this.ToString("D");

    public String ToString(string format)
        => this.Guid.ToString(format);
}
