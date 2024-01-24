using GmWeb.Logic.Data.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Data.Models;

namespace GmWeb.Logic.Data.Models.Carma
{
    [Table("Notes", Schema = "carma")]
    public partial class Note : BaseDataModel, IGuidKeyModel
    {
        Guid IGuidKeyModel.PrimaryKey => this.NoteID;

        [Key]
        public Guid NoteID { get; set; }
        [ForeignKey("NoteAuthor")]
        public int AuthorID { get; set; }
        [ForeignKey("AuthorID")]
        public virtual User NoteAuthor { get; set; }
        [ForeignKey("NoteSubject")]
        public int? SubjectID { get; set; }
        [ForeignKey("SubjectID")]
        public virtual User NoteSubject { get; set; }
        [ForeignKey("ModifiedBy")]
        public int ModifiedByID { get; set; }
        [ForeignKey("ModifiedByID")]
        public virtual User ModifiedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string Title { get; set; }
        [EncryptedStringColumn]
        public string Message { get; set; }

    }
}