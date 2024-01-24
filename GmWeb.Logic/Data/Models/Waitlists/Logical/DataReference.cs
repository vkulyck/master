using GmWeb.Logic.Data.Context.Lookup;
using GmWeb.Logic.Data.Context.Profile;
using GmWeb.Logic.Data.Models.Shared;
using GmWeb.Logic.Enums;
using System;
using System.Linq;

namespace GmWeb.Logic.Data.Models.Waitlists
{

    public static class ReferenceTables
    {
        public static string CSP => "tblClientServiceProfile";
        public static string Client => "tblClient";
        public static string ExtendedClient => "ExtendedClient";
    }
    public abstract class DataReference : ContextualDataModel<ProfileContext>
    {
        public abstract string VariablePrefix { get; }
        public string VariableName { get; set; }
        public string LookupTableName { get; set; }
        public string FullyQualifiedVariableName => $"{this.VariablePrefix}_{this.VariableName.Replace(" ", "_")}";
        private int? _dataSourceID;
        public int? DataSourceID
        {
            get => this._dataSourceID;
            set
            {
                if (this._dataSourceID != value)
                {
                    this._dataSourceID = value;
                    this.DataSource = null;
                }
            }
        }
        public DataSource DataSource { get; private set; }
        protected DynamicFieldValue LookupValue { get; set; } = new DynamicFieldValue();
        protected abstract string ParentTableName { get; }
        public string Guid { get; set; } = GmWeb.Logic.Utility.Extensions.ModelExtensions.GenerateGuid();

        public override void PopulateReferences(ProfileContext cache)
        {
            this.PopulateDataSource(cache);
            this.PopulateLookupMetadata(cache);
        }

        protected void PopulateDataSource(ProfileContext cache)
        {
            if (this.DataSource != null)
                return;
            if (this.DataSourceID == null)
                throw new NullReferenceException($"The {this.GetType().Name}.DataSource cannot be populated when {this.GetType().Name}.DataSourceID is null.");
            this.DataSource = cache.DataSources.Single(x => x.DataSourceID == this.DataSourceID);
        }

        protected void PopulateLookupMetadata(ProfileContext cache)
        {
            this.LookupValue.Name = this.DataSource.Field;
            this.LookupValue.DataType = this.DataSource.DataType;
            if (this.DataSource.DataType != DataType.L)
                return;
            if (this.LookupValue.Options != null)
                return;
            this.LookupValue.Options = LookupCache.Instance.GetLookupOptions(this.LookupTableName).ToList();
        }

        public virtual void ResolveLookup(ProfileContext cache, int? RowID)
        {
            this.DataSource.RowID = RowID;
            // TODO: Generalize these lookups
            if (this.DataSource.RowID == null)
                return;
            string table = this.DataSource.Table;
            if (table == ReferenceTables.CSP)
            {
                var set = cache.ClientServiceProfiles.AsQueryable();
                set = set.Where(x => x.ParentTableID == this.DataSource.RowID);
                set = set.Where(x => x.ParentTableName == this.ParentTableName);
                set = set.Where(x => x.Name == this.DataSource.Field);
                var row = set.SingleOrDefault();
                this.LookupValue.RawValue = row?.Value;
            }
            else if (table == ReferenceTables.Client)
            {
                var set = cache.Clients.AsQueryable();
                set = set.Where(x => x.ClientID == this.DataSource.RowID);
                var row = set.SingleOrDefault();
                if (row == null)
                    return;
                var property = row.GetType().GetProperty(this.DataSource.Field);
                this.LookupValue.RawValue = property.GetValue(row)?.ToString();
            }
            else if (table == ReferenceTables.ExtendedClient)
            {
                var set = cache.ExtendedClients.AsQueryable();
                set = set.Where(x => x.ClientID == this.DataSource.RowID);
                var row = set.SingleOrDefault();
                if (row == null)
                    return;
                if (row.ProfileFieldMap.TryGetValue(this.DataSource.Field, out var result))
                {
                    this.LookupValue = result;
                }
                //else throw new Exception($"Client {this.DataSource.RowID} has no value assigned for field '{this.DataSource.Field}'");
            }
            else throw new ArgumentException($"No lookup could be performed with the given parameters.");
        }
    }
}
