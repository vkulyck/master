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
    protected override EntityTypeBuilder<UserConfig> OnUserConfigsCreating(ModelBuilder modelBuilder, IConfiguration config)
    {
        var entityBuilder = base.OnUserConfigsCreating(modelBuilder, config);
        entityBuilder.ConfigureBitValueColumn(x => x.IsStarred);
        return entityBuilder;
    }
}