using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Context.Profile;
using System.Collections.Generic;

namespace GmWeb.Logic.Data.Models.Waitlists
{
    public class ClientFilter
    {
        public ProfileContext DataCache { get; private set; }

        public ClientFilter(ProfileContext cache)
        {
            this.DataCache = cache;
        }

        public FilterResultData Evaluate(IEnumerable<string> predicates, Flow waitlist, FlowPath path = null)
        {
            var results = new FilterResultData();
            foreach (var client in this.DataCache.ExtendedClients)
            {
                results.TotalCount++;
                // TODO: Can we pass this.DataCache into the VariableCache constructor to avoid creating another instance?
                var cache = new VariableCache(this.DataCache.CreateNew(), Flow: waitlist, Path: path, Client: client).Initialize();
                bool? result = null;
                foreach (string predicate in predicates)
                {
                    var expression = new NCalc.Expression(predicate);
                    result = expression.ValidateAndEvaluate(EntityAssignments: cache.Assignments);
                    if (!result.HasValue || !result.Value)
                        break;
                }
                if (result.HasValue && result.Value)
                    results.Clients.Add(client);
            }

            return results;
        }
    }
}
