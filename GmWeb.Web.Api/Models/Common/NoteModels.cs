using System;
using System.ComponentModel.DataAnnotations;
using GmWeb.Web.Common.Models.Carma;

namespace GmWeb.Web.Api.Models.Common
{
    public class NoteUpsertDTO
    {
        public bool? IsFlagged { get; set; }
        public int? SubjectID { get; set; }
    }
    public class NoteInsertDTO : NoteUpsertDTO
    {
        [Required]
        [StringLength(1024)]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
    }
    public class NoteUpdateDTO : NoteUpsertDTO
    {
        [Required]
        public Guid NoteID { get; set; }
        [StringLength(1024)]
        public string Title { get; set; }
        public string Message { get; set; }
    }
    public class NoteDetailsDTO : NoteDTO
    {
        public UserDTO Author { get; set; }
        public UserDTO Subject { get; set; }
        public UserDTO ModifiedBy { get; set; }
    }

    public class NoteDTO
    {
        public Guid NoteID { get; set; }
        public int AuthorID { get; set; }
        public int? SubjectID { get; set; }
        public int ModifiedByID { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsFlagged { get; set; }
    }
}
