using GmWeb.Logic.Data.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Services.Deltas;

namespace GmWeb.Logic.Data.Models.Carma;

public abstract class Post : BaseDataModel, IGuidKeyModel
{
    Guid IGuidKeyModel.PrimaryKey => this.PostID;

    public abstract Guid PostID { get; }

    [ForeignKey("Author")]
    public int AuthorID { get; set; }
    [ForeignKey("AuthorID")]
    public virtual User Author { get; set; }
    [ForeignKey("Agency")]
    public int AgencyID { get; set; }
    [ForeignKey("AgencyID")]
    public virtual Agency Agency { get; set; }
    public DateTimeOffset Created { get; set; }

    [JsonColumn]
    public DeltaField ContentHistory { get; set; } = new DeltaField();
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset? ContentModified { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string Content { get; set; }
}
