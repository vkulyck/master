using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Services.Deltas;

namespace GmWeb.Logic.Data.Context.Carma;

public partial class CarmaContext
{
    protected override EntityTypeBuilder<Note> OnNotesCreating(ModelBuilder modelBuilder, IConfiguration config)
    {
        var entityBuilder = base.OnNotesCreating(modelBuilder, config);
        entityBuilder.ConfigureBitValueColumn(x => x.IsFlagged);
        return entityBuilder;
    }
}

public partial class Notes
{
    public async Task<int> DeleteAsync(Guid noteID)
    {
        int count = await this.CountAsync(x => x.NoteID == noteID);
        this.EntitySet.RemoveAll(x => x.NoteID == noteID);
        return count;
    }
}
