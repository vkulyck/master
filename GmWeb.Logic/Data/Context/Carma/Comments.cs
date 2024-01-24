using GmWeb.Logic.Services.Deltas;
using GmWeb.Logic.Data.Models.Carma;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GmWeb.Logic.Data.Context.Carma;

#region Thread Model Configuration

public partial class CarmaContext
{
    protected override EntityTypeBuilder<Comment> OnCommentsCreating(ModelBuilder modelBuilder, IConfiguration config)
    {
        var entityBuilder = base.OnCommentsCreating(modelBuilder, config);
        entityBuilder.ConfigureDeltaModified(x => x.ContentModified);
        entityBuilder.ConfigureDeltaValue(x => x.Content);
        return entityBuilder;
    }
}

#endregion

public partial class Comments
{
    public Comment Insert(Comment comment, User author)
    {
        comment.ThreadID = Guid.NewGuid();
        comment.AuthorID = author.UserID;
        comment.AgencyID = author.AgencyID;
        comment.Created = DateTimeOffset.Now;
        this.DataContext.DeltaService.UpdateDeltaFields(comment);
        return this.Insert(comment);
    }
}
