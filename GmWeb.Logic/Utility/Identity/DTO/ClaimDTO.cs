using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Utility.Identity.DTO;
public class ClaimDTO
{
    [Required]
    [DataType(DataType.Text)]
    public string Type { get; set; }
    [Required]
    [DataType(DataType.Text)]
    public string Value { get; set; }
    [DataType(DataType.Text)]
    public string ValueType { get; set; }
}