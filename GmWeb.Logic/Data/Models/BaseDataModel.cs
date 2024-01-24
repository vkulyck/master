using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Utility.Primitives;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;


namespace GmWeb.Logic.Data.Models
{
    public interface IIntegerKeyModel : IPrimaryKeyModel<int>
    {
        new int PrimaryKey { get; }
        int IPrimaryKeyModel<int>.PrimaryKey => this.PrimaryKey;
    }
    public interface IGuidKeyModel : IPrimaryKeyModel<Guid>
    {
        new Guid PrimaryKey { get; }
        Guid IPrimaryKeyModel<Guid>.PrimaryKey => this.PrimaryKey;
    }
    public interface IHalfGuidKeyModel : IPrimaryKeyModel<ulong>
    {
        new HalfGuid PrimaryKey { get; }
        ulong IPrimaryKeyModel<ulong>.PrimaryKey => this.PrimaryKey.UID;
    }
    public interface IPrimaryKeyModel<TKey>
        where TKey : struct
    {
        TKey PrimaryKey { get; }
    }
    public interface ILongKeyModel : IPrimaryKeyModel<long>
    {
        new long PrimaryKey { get; }
        long IPrimaryKeyModel<long>.PrimaryKey => this.PrimaryKey;
    }
    public abstract class BaseDataModel
    {

    }
    public abstract class ContextualDataModel : ContextualDataModel<IBaseDataContext> { }
    public abstract class ContextualDataModel<TContext> : BaseDataModel
        where TContext : IBaseDataContext
    {
        [NotMapped]
        [JsonIgnore]
        public bool AreReferencesPopulated { get; protected set; } = false;

        [NotMapped]
        [JsonIgnore]
        public TContext Context { get; private set; }

        public virtual void PopulateReferences(TContext context)
        {
            this.Context = context;
            if (this.AreReferencesPopulated)
                return;
            this.AreReferencesPopulated = true;
            var properties = this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && x.CanWrite)
                .ToList()
            ;
            foreach (var property in properties)
            {
                var dm = property.GetModel(this);
                if (dm != null)
                {
                    dm.PopulateReferences(context);
                    continue;
                }
                var childModels = property.GetModels(this);
                if (childModels != null)
                {
                    foreach (var child in childModels)
                        child.PopulateReferences(context);
                }
            }
        }
    }
}
