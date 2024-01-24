using GmWeb.Logic.Data.Context.Profile;
using GmWeb.Logic.Data.Models.Profile;
using GmWeb.Logic.Data.Models.Shared;
using System;
using System.Collections.Generic;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class VariableCache : IDisposable
    {
        public Flow Flow { get; private set; }
        public FlowPath FlowPath { get; private set; }
        public IReadOnlyDictionary<string, object> Assignments => this.ValueAssignments;
        protected Dictionary<string, object> ValueAssignments { get; } = new Dictionary<string, object>();
        public ProfileContext Context { get; private set; }
        public ExtendedClient Client { get; private set; }
        private bool EnableDispose { get; set; } = false;

        public VariableCache(FlowPath Path, bool EnableDispose)
            : this(DataContext: Path.Cache.DataContext, EnableDispose: EnableDispose, Flow: Path.Flow, Path: Path, Client: null) { }
        public VariableCache(ProfileContext DataContext, bool EnableDispose = false, Flow Flow = null, FlowPath Path = null, ExtendedClient Client = null)
        {
            this.Context = DataContext;
            this.EnableDispose = EnableDispose;
            this.FlowPath = Path;
            this.Flow = Flow;
            this.Client = Client;
        }

        public VariableCache Initialize()
        {
            foreach (var item in this.Flow.Data)
            {
                item.ResolveLookup(this.Context, item.CategoryID);
                this.SetValue(item.FullyQualifiedVariableName, item.ConfiguredValue);
            }
            IEnumerable<FlowStep> steps;
            if (this.FlowPath == null)
                steps = this.Flow.Steps; // If no path is specified then try to lookup every query variable
            else steps = this.FlowPath.Steps;
            foreach (var step in steps)
            {
                foreach (var q in step.Queries)
                {
                    q.ResolveLookup(this.Context, this.Client?.ClientID); // Ground against a Client/ClientProfile lookup via ClientID
                    this.SetValue(q.FullyQualifiedVariableName, q.ClientResponse);
                    q.ResetResponse();
                }
            }
            return this;
        }

        public object GetValue(string variable)
        {
            if (this.ValueAssignments.TryGetValue(variable, out object value))
                return value;
            return null;
        }

        public void SetValue(string variable, object value)
        {
            var dfv = value as DynamicFieldValue;
            if (dfv == null)
            {
                this.ValueAssignments[variable] = value;
            }
            else
            {
                this.ValueAssignments[variable] = dfv.ConvertedValue;
            }

        }

        public void Dispose()
        {
            if (!this.EnableDispose)
                return;
            this.Context.Dispose();
        }
    }
}
