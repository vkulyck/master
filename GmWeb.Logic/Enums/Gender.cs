using System;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums
{
    [Flags]
    public enum Gender : int
    {
        Unspecified            = 0b000000,

        // Gender Expression
        Agender                 = 0b000100,
        //Male                  = 0b000001,
        Male                    = 0b000101,
        //Female                = 0b000010,
        Female                  = 0b000110,
        NonBinary               = 0b000111,

        // Gender Dynamics
        Cisgender               = 0b101000,
        Transgender             = 0b110000,
        GenderFluid             = 0b100000,

        // Combinations
        TransgenderMale         = 0b1101010,
        TransgenderFemale       = 0b1101100,
        CisgenderMale           = 0b1011010,
        CisgenderFemale         = 0b1011100

    }
}
