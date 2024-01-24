using System;
using System.ComponentModel.DataAnnotations;
using GmWeb.Web.Common.Models.Carma;

namespace GmWeb.Web.Api.Models.Common
{
    public class ThreadUpsertDTO
    {
        public bool? IsFlagged { get; set; }
        public int? SubjectID { get; set; }
    }
    public class ThreadInsertDTO : ThreadUpsertDTO
    {
        [Required]
        [StringLength(1024)]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
    }
    public class ThreadUpdateDTO : ThreadUpsertDTO
    {
        public Guid NoteID { get => this.ThreadID; set => this.ThreadID = value; }
        [Required]
        public Guid ThreadID { get; set; }
        [StringLength(1024)]
        public string Title { get; set; }
        public string Message { get; set; }
    }
    public class ThreadDetailsDTO : ThreadDTO
    {
        public UserDTO Author { get; set; }
        public UserDTO Subject { get; set; }
        public UserDTO ModifiedBy { get; set; }
    }

    public class ThreadDTO
    {
        public Guid NoteID { get => this.ThreadID; set => this.ThreadID = value; }
        public Guid ThreadID { get; set; }
        public int AuthorID { get; set; }
        public int? SubjectID { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsFlagged { get; set; }
    }
}
