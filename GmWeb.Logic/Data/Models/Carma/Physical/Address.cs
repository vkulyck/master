using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Enums;

namespace GmWeb.Logic.Data.Models.Carma;

public partial class Address : BaseDataModel, IIntegerKeyModel
{
    /*
        country => Country (always required, 2 character ISO code)
        name_line => Full name (default name entry)
        first_name => First name
        last_name => Last name
        organisation_name => Company
        administrative_area => State / Province / Region (ISO code when available)
        sub_administrative_area => County / District (unused)
        locality => City / Town
        dependent_locality => Dependent locality (unused)
        postal_code => Postal code / ZIP Code
        thoroughfare => Street address
        premise => Apartment, Suite, Box number, etc.
        sub_premise => Sub premise (unused)
    */
    int IIntegerKeyModel.PrimaryKey => this.AddressID;
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AddressID { get; set; }
}
