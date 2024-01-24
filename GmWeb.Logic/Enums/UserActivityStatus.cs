using System;

namespace GmWeb.Logic.Enums
{
    [Flags]
    public enum UserActivityStatus : byte
    {
        Unregistered =                  0b00000000,
        Registered =                    0b00000001,
        Unavailable =                   0b00000010,
        Canceled =                      0b00000100,
        AttendanceConfirmed =           0b00001000,
        IsAttending =                   0b00001001
    }
}
