using System;

namespace GmWeb.Logic.Enums
{
    [Flags]
    public enum PermitType : ulong
    {
        Nothing = 0b0000,

        ReadSelf = 0b0001,
        ReadAgencyPeers = 0b0010,
        ReadAgencyUsers = 0b0100,
        ReadAgencyData = 0b1000,

        WriteSelf = 0b0001 * 0x10,
        WriteAgencyPeers = 0b0010 * 0x10,
        WriteAgencyUsers = 0b0100 * 0x10,
        WriteAgencyData = 0b1000 * 0x10,

        Administration = 0xFF
    }
}
