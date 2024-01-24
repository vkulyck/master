using GmWeb.Logic.Data.Context.Profile;
using GmWeb.Logic.Data.Models.Shared;
using Newtonsoft.Json;
using System;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class ClientQuery : DataReference
    {
        public string RequestText { get; set; }
        public override string VariablePrefix => "Client";
        protected override string ParentTableName => "CLI";

        [JsonIgnore]
        public DynamicFieldValue ClientResponse => base.LookupValue;
        [JsonIgnore]
        public int? ClientID
        {
            get => this.DataSource.RowID;
            set => this.DataSource.RowID = value;
        }

        public override void ResolveLookup(ProfileContext cache, int? clientID = null)
        {
            this.ClientID = clientID;
            this.PopulateDataSource(cache);
            if (this.ClientResponse?.ConvertedValue != null)
                // If the query is being presented to the client via UI, use their answer
                return;
            else if (clientID.HasValue)
                base.ResolveLookup(cache, clientID);
            else throw new Exception($"Client query value cannot be retrieved without a ClientID, and no client response has been recorded.");
        }

        public void ResetResponse(int? clientID = null)
        {
            this.ClientID = clientID;
            this.ClientResponse.RawValue = null;
        }
    }
}
