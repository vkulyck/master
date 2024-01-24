using GmWeb.Logic.Services.Deltas;
using GmWeb.Logic.Data.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GmWeb.Logic.Data.Models.Carma;

[Table("Threads", Schema = "carma")]
public partial class Thread : Post
{
    public override Guid PostID => this.ThreadID;
    [NotMapped]
    public Guid NoteID { get => this.ThreadID; set => this.ThreadID = value; }
    [Key]
    public Guid ThreadID { get; set; }
    [ForeignKey("Subject")]
    public int? SubjectID { get; set; }
    [ForeignKey("SubjectID")]
    public virtual User Subject { get; set; }
    [InverseProperty("Thread")]
    public virtual ICollection<Comment> Comments { get; set; }

    [JsonColumn]
    public DeltaField TitleHistory { get; set; } = new DeltaField();
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset? TitleModified { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string Title { get; set; }

}
