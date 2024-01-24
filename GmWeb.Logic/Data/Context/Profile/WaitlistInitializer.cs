using GmWeb.Logic.Data.Models.Waitlists;
using GmWeb.Logic.Enums;
using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Data.Context.Profile
{
    public class WaitlistInitializer : BaseDataInitializer<ProfileCache, ProfileContext>
    {
        protected override string ConcreteConfigKeySuffix => nameof(WaitlistInitializer);

        public static void Seed(ProfileCache cache) => new WaitlistInitializer(cache).Seed();

        public WaitlistInitializer(ProfileCache cache) : base(cache) { }

        public override void OnSeed()
        {
            this.ClearEntities();

            var sources = this.CreateDataSources();
            this.Cache.DataSources.AddRange(sources);
            this.Cache.Save();

            var flows = this.CreateFlows();
            this.Cache.Flows.AddRange(flows);
            this.Cache.Save();

            var steps = this.CreateSteps();
            this.OrganizeSteps(steps, flows[0]);
            this.Cache.Save();
        }

        public override void ClearEntities()
        {
            this.Cache.Flows.RemoveAll();
            this.Cache.FlowSteps.RemoveAll();
            this.Cache.DataSources.RemoveAll();
            this.Cache.Save();
        }

        private List<Flow> CreateFlows()
        {
            var catSources = this.Cache.DataSources.AsEnumerable().Where(x => x.EntityType == EntityType.Category).ToDictionary(x => x.Field, x => x.DataSourceID);
            CategoryData CategoryDataFromField(string field, string variable = null)
            {
                if (variable == null)
                    variable = field;
                var cat = new CategoryData
                {
                    VariableName = variable,
                    DataSourceID = catSources[field]
                };
                if (field == "Targeted Population")
                    cat.LookupTableName = "lkpTargetPopulation";
                else if (field.Contains("Health Plan Type"))
                    cat.LookupTableName = "lkpHealthPlanType";
                return cat;
            }
            var flows = new List<Flow>
            {
                new Flow
                {
                    Name = "Demo Flow",
                    Data = new List<CategoryData>
                    {
                        CategoryDataFromField("Age"),
                        CategoryDataFromField("Family Size"),
                        CategoryDataFromField("City"),
                        CategoryDataFromField("Zip"),
                        CategoryDataFromField("Availability Deadline", "Deadline"),
                        CategoryDataFromField("Physical Health Plan Type"),
                        CategoryDataFromField("Dental Health Plan Type"),
                        CategoryDataFromField("Vision Health Plan Type"),
                    },
                    Results = new List<ResultSet>
                    {
                        new ResultSet
                        {
                            Name = "Client age above 30",
                            Predicates = new List<string>
                            {
                                "Client_Age <= Category_Age"
                            }
                        },
                        new ResultSet
                        {
                            Name = "Client age below 30 and family size not less than category",
                            Predicates = new List<string>
                            {
                                "Client_Age > 30",
                                "Client_Family_Size >= Category_Family_Size"
                            }
                        },
                        new ResultSet
                        {
                            Name = "Client age below 30 and family size matches category",
                            Predicates = new List<string>
                            {
                                "Client_Age > 30",
                                "Client_Family_Size == Category_Family_Size + 1"
                            }
                        },
                        new ResultSet
                        {
                            Name = "Clients in category city (SF)",
                            Predicates = new List<string>
                            {
                                "Client_City == Category_City"
                            }
                        }
                    }
                }
            };
            return flows;
        }

        private List<FlowStep> CreateSteps()
        {
            var cliSources = this.Cache.DataSources.AsEnumerable().Where(x => x.EntityType == EntityType.Client).ToDictionary(x => x.Field, x => x.DataSourceID);
            ClientQuery ClientQueryFromField(string field, string text, string variable = null)
            {
                if (variable == null)
                    variable = field;
                var q = new ClientQuery
                {
                    VariableName = variable,
                    DataSourceID = cliSources[field],
                    RequestText = text
                };
                if (field == "Targeted Population")
                    q.LookupTableName = "lkpTargetPopulation";
                else if (field.Contains("Health Plan Type"))
                    q.LookupTableName = "lkpHealthPlanType";
                return q;
            }
            var steps = new List<FlowStep>
            {
                new FlowStep
                {
                    Name = "Contact",
                    Queries = new List<ClientQuery>
                    {
                        ClientQueryFromField("Email", "Please enter your email address:"),
                        ClientQueryFromField("Phone", "Please enter your phone number:"),
                    }
                },
                new FlowStep
                {
                    Name = "Location",
                    Queries = new List<ClientQuery>
                    {
                        ClientQueryFromField("City", "What city do you reside in?"),
                        ClientQueryFromField("Zip", "What is your primary zip code?"),
                    }
                },
                new FlowStep
                {
                    Name = "Personal",
                    Queries = new List<ClientQuery>
                    {
                        ClientQueryFromField("Age", "What is your age?"),
                        ClientQueryFromField("Family Size", "How many people are in your family?"),
                        ClientQueryFromField("Unemployed", "Are you currently unemployed?"),
                    }
                },
                new FlowStep
                {
                    Name = "Insurance",
                    Queries = new List<ClientQuery>
                    {
                        ClientQueryFromField("Physical Health Plan Type", "What type of health insurance do you have?"),
                        ClientQueryFromField("Vision Health Plan Type", "What type of vision plan do you use?"),
                        ClientQueryFromField("Dental Health Plan Type", "What type of dental plan do you use?"),
                    }
                },
                new FlowStep
                {
                    Name = "Availability",
                    Queries = new List<ClientQuery>
                    {
                        ClientQueryFromField("Availability Deadline", "When is your deadline for participation?", "Deadline"),
                    }
                },
                new FlowStep
                {
                    Name = "End",
                    Queries = new List<ClientQuery>()
                }
            };
            return steps;
        }

        private void OrganizeSteps(List<FlowStep> steps, Flow flow)
        {
            steps.First().StepType = StepType.Initial;
            steps.Last().StepType = StepType.Terminal;
            steps.ForEach(x => x.Flow = flow);

            var lookup = steps.ToDictionary(x => x.Name);

            // contact, location, personal, insurance, availability, end
            this.CreateLink("Contact", "Personal", lookup);
            this.CreateLink("Personal", "Availability", lookup, Predicate: "Client_Age < 18");
            this.CreateLink("Personal", "Location", lookup);
            this.CreateLink("Location", "Availability", lookup, Predicate: "Client_City == \"San Francisco\"");
            this.CreateLink("Location", "Insurance", lookup);
            this.CreateLink("Insurance", "Availability", lookup, Predicate: "Client_Physical_Health_Plan_Type == Category_Physical_Health_Plan_Type");
            this.CreateLink("Availability", "End", lookup);

            for (int i = 0; i < steps.Count; i++)
            {
                // Add the steps in creation order
                this.Cache.FlowSteps.Add(steps[i]);
                this.Cache.Save();
            }
        }
        private List<DataSource> CreateDataSources()
        {
            var sources = new List<DataSource>
            {
                new DataSource
                {
                    Description = "The age requirement of a particular category.", EntityType = EntityType.Category,
                    DataType = DataType.N, Field = "Age", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The client's age.", EntityType = EntityType.Client,
                    DataType = DataType.N, Field = "Age", Table = ReferenceTables.Client
                },
                new DataSource
                {
                    Description = "The income limit for a particular category.", EntityType = EntityType.Category,
                    DataType = DataType.N, Field = "Family Income Level", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The target population for a particular category.", EntityType = EntityType.Category,
                    DataType = DataType.L, Field = "Targeted Population", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The client's family income level.", EntityType = EntityType.Client,
                    DataType = DataType.N, Field = "Family Income Level", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The family size required by a particular category.", EntityType = EntityType.Category,
                    DataType = DataType.N, Field = "Family Size", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The client's family size.", EntityType = EntityType.Client,
                    DataType = DataType.N, Field = "Family Size", Table = ReferenceTables.ExtendedClient
                },
                new DataSource
                {
                    Description = "The city of residence required by a particular category.", EntityType = EntityType.Category,
                    DataType = DataType.S, Field = "City", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The client's city of residence.", EntityType = EntityType.Client,
                    DataType = DataType.S, Field = "City", Table = ReferenceTables.ExtendedClient
                },
                new DataSource
                {
                    Description = "The zip code of residence required by a particular category.", EntityType = EntityType.Category,
                    DataType = DataType.S, Field = "Zip", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The client's zip code.", EntityType = EntityType.Client,
                    DataType = DataType.S, Field = "Zip", Table = ReferenceTables.ExtendedClient
                },
                new DataSource
                {
                    Description = "The client's population specifier.", EntityType = EntityType.Client,
                    DataType = DataType.L, Field = "Targeted Population", Table = ReferenceTables.CSP
                },


                new DataSource
                {
                    Description = "The preferred primary insurance plan type.", EntityType = EntityType.Category,
                    DataType = DataType.L, Field = "Physical Health Plan Type", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The preferred vision insurance plan type.", EntityType = EntityType.Category,
                    DataType = DataType.L, Field = "Vision Health Plan Type", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The preferred dental insurance plan type.", EntityType = EntityType.Category,
                    DataType = DataType.L, Field = "Dental Health Plan Type", Table = ReferenceTables.CSP
                },

                new DataSource
                {
                    Description = "The client's primary insurance plan.", EntityType = EntityType.Client,
                    DataType = DataType.L, Field = "Physical Health Plan Type", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The client's vision insurance plan.", EntityType = EntityType.Client,
                    DataType = DataType.L, Field = "Vision Health Plan Type", Table = ReferenceTables.CSP
                },
                new DataSource
                {
                    Description = "The client's dental insurance plan.", EntityType = EntityType.Client,
                    DataType = DataType.L, Field = "Dental Health Plan Type", Table = ReferenceTables.CSP
                },

                new DataSource
                {
                    Description = "The last date that services will be offered.", EntityType = EntityType.Category,
                    DataType = DataType.D, Field = "Availability Deadline", Table = ReferenceTables.CSP
                },

                new DataSource
                {
                    Description = "The client's deadline for receipt of services.", EntityType = EntityType.Client,
                    DataType = DataType.D, Field = "Availability Deadline", Table = ReferenceTables.CSP
                },

                new DataSource
                {
                    Description = "The client's contact email.", EntityType = EntityType.Client,
                    DataType = DataType.S, Field = "Email", Table = ReferenceTables.Client
                },

                new DataSource
                {
                    Description = "The client's contact phone number.", EntityType = EntityType.Client,
                    DataType = DataType.S, Field = "Phone", Table = ReferenceTables.Client
                },

                new DataSource
                {
                    Description = "The client's unemployment status.", EntityType = EntityType.Client,
                    DataType = DataType.B, Field = "Unemployed", Table = ReferenceTables.CSP
                },

                new DataSource
                {
                    Description = "The client's disability status.", EntityType = EntityType.Client,
                    DataType = DataType.B, Field = "Disabled", Table = ReferenceTables.CSP
                },

                new DataSource
                {
                    Description = "The category's unemployment status.", EntityType = EntityType.Category,
                    DataType = DataType.B, Field = "Unemployed", Table = ReferenceTables.CSP
                },

                new DataSource
                {
                    Description = "The category's disability status.", EntityType = EntityType.Category,
                    DataType = DataType.B, Field = "Disabled", Table = ReferenceTables.CSP
                },
            };
            return sources;
        }

        private void CreateLink(string sourceName, string targetName, Dictionary<string, FlowStep> lookup, string Predicate = null, string Name = null)
        {
            var source = lookup[sourceName];
            var target = lookup[targetName];
            this.CreateLink(source, target, Predicate: Predicate, Name: Name);
        }
        private void CreateLink(FlowStep source, FlowStep target, string Predicate = null, string Name = null)
        {
            if (Predicate == null)
                Predicate = "true";
            if (Name == null)
                Name = $"{source.Name} -> {target.Name}";
            var link = new FlowLink
            {
                Name = Name,
                Predicate = Predicate,
                SourceStepGuid = source.Guid,
                TargetStepGuid = target.Guid
            };
            source.Links.Add(link);
        }
    }
}