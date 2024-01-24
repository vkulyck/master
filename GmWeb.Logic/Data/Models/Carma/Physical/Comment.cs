using GmWeb.Logic.Data.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GmWeb.Logic.Data.Models;

namespace GmWeb.Logic.Data.Models.Carma
{
    [Table("Comments", Schema = "carma")]
    public partial class Comment : Post
    {
        [Key]
        public Guid CommentID { get; set; }
        public override Guid PostID => this.CommentID;
        [ForeignKey("Thread")]
        public Guid ThreadID { get; set; }
        [ForeignKey("ThreadID")]
        public virtual Thread Thread { get; set; }
        [ForeignKey("ParentComment")]
        public Guid? ParentCommentID { get; set; }
        [ForeignKey("ParentCommentID")]
        public virtual Comment ParentComment { get; set; }
        public Guid ParentID => this.ParentCommentID ?? this.ThreadID;
        public Post Parent => (Post)this.ParentComment ?? this.Thread;
        [InverseProperty("ParentComment")]
        public virtual ICollection<Comment> ChildComments { get; set; }
    }
}