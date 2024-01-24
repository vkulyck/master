using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace GmWeb.Logic.Data.Context
{
    public static class EntityExtensions
    {
        public static void RemoveAll<TEntity>(this DbSet<TEntity> set) where TEntity : class => set.RemoveRange(set);
        public static void RemoveAll<TEntity>(this DbSet<TEntity> set, Expression<Func<TEntity, bool>> predicate) where TEntity : class => set.RemoveRange(set.Where(predicate));
    }
}
