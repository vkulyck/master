using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GmWeb.Logic.Data.Context.ModelConfigurators;

public interface IDynamicConverterConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    IDynamicConverterConfiguration<TEntity> With<TConverter>();
}
