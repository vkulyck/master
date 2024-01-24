using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using GmWeb.Logic.Data.Context.Carma;
using GmWeb.Logic.Data.Models.Carma;

namespace GmWeb.Logic.Services.Printing;

public class UserZplGenerator : PrinterCommandGenerator<User>
{
    public UserZplGenerator(IOptions<PrinterOptions> options) : base(options) { }

    /// <summary>
    /// Generates a ZPL command corresponding to the provided user's identity.
    /// </summary>
    /// <param name="user">The input user which provides identity details to be printed by the output command.</param>
    /// <returns>A formatted ZPL command which will output identifying data from the provided <paramref name="user"/> parameter.</returns>
    /// <remarks>
    /// The Zebra Programming Language, or ZPL, is the domain-specific language
    /// developed by Zebra Technologies to simplify the specification and automation
    /// of high-throughput print jobs in commercial and industrial applications. ZPL
    /// is comprised of hundreds of individual commands, each of which vary in syntax, 
    /// scope, behavior, and parameterization. For a comprehensive guide of these 
    /// commands, see the
    /// <see href="https://www.zebra.com/content/dam/zebra_new_ia/en-us/manuals/printers/common/programming/zpl-zbi2-pm-en.pdf">
    /// ZPL II [et al] Programming Guide</see>, or any of the earlier (and more concise versions) 
    /// included below. Additionally, the <see cref="http://labelary.com">Labelary.com suite</see>
    /// provides freely-available tools for creating and executing print jobs. The suite includes
    /// a <see cref="http://labelary.com/viewer.html">ZPL preview engine</see> that can generate 
    /// graphical label output without the need to access or print on a physical Zebra printer.
    /// 
    /// <list type="bullet">
    /// <item><seealso href="https://support.zebra.com/cpws/docs/zpl/BQ_Command.pdf"/>^BQ Command Documentation</item>
    /// <item><seealso href="https://www.zebra.com/us/en/support-downloads/knowledge-articles/zpl-command-information-and-details.html">ZPL Command Information and Details</seealso></item>
    /// <item><seealso href="https://www.servopack.de/support/zebra/ZPLII-Prog.pdf"/>ZPL II Programming Guide, Firmware X.10 to X.13</item>
    /// <item><seealso href="https://www.zebra.com/content/dam/zebra_new_ia/en-us/manuals/printers/common/programming/zplii-pm-vol2-en.pdf"/>ZPL II Programming Guide Vol. 2</item>
    /// <item><seealso href="http://www.cretton.net/central/manuais/programar_zpl2.pdf"/>ZPL II Programming Guide Vol. 1, Firmware X.10</item>
    /// </list>
    /// </remarks>
    public override string CreateCommand(User user)
    {
        var interpolated = $@"
            ^XA^MMT^PW497^LL0239^LH55,40
            ^FO0,10^BQN,2,5^FDMM,A{user.LookupID}^FS
            ^FO124,35^A0N,23,23^FWR^FH\^FDUID: {user.UserID:D5}^FS
            ^FT252,50^A0N,30,30^FH^FD{user.LastName}^FS
            ^FT252,80^A0N,30,30^FH^FD{user.FirstName}^FS
            ^FT252,110^A0N,30,30^FH^FD{user.Profile.Residence.BuildingCode} # {user.Profile.Residence.UnitNumber}^FS
            ^FT252,140^A0N,30,30^FH^FDUID: {user.UserID:D5}^FS
            ^PQ1,0,1,Y^XZ
        ";
        var formatted = Regex.Replace(interpolated, @"^\s*", string.Empty, RegexOptions.Multiline);
        return formatted;
    }
}
