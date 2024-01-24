using GmWeb.Logic.Data.Annotations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Carma;

[Table("UserConfigs", Schema = "carma")]
public partial class UserConfig : BaseDataModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserConfigID { get; set; }

    [ForeignKey("ConfigOwner")]
    public int OwnerID { get; set; }
    [ForeignKey("OwnerID")]
    public virtual User ConfigOwner { get; set; }

    [ForeignKey("ConfigSubject")]
    public int SubjectID { get; set; }
    [ForeignKey("SubjectID")]
    public virtual User ConfigSubject { get; set; }
}