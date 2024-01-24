using GmWeb.Logic.Data.Context;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GmWeb.Logic.Data.Models
{
    public static class Extensions
    {
        public static BaseDataModel GetModel(this PropertyInfo property, BaseDataModel parent) => property.GetValue(parent) as BaseDataModel;
        public static IEnumerable<BaseDataModel> GetModels(this PropertyInfo property, BaseDataModel parent)
        {
            var collection = property.GetValue(parent) as IEnumerable;
            if (collection == null)
                return null;
            var models = collection.OfType<BaseDataModel>();
            return models;
        }

        public static ContextualDataModel<TContext> GetModel<TContext>(this PropertyInfo property, ContextualDataModel<TContext> parent)
            where TContext : IBaseDataContext => property.GetValue(parent) as ContextualDataModel<TContext>;

        public static IEnumerable<ContextualDataModel<TContext>> GetModels<TContext>(this PropertyInfo property, ContextualDataModel<TContext> parent)
            where TContext : IBaseDataContext
        {
            var collection = property.GetValue(parent) as IEnumerable;
            if (collection == null)
                return null;
            var models = collection.OfType<ContextualDataModel<TContext>>();
            return models;
        }
    }
}
