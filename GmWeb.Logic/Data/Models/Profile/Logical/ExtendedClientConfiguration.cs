using GmWeb.Logic.Data.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonSerializerSettings = Newtonsoft.Json.JsonSerializerSettings;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;

namespace GmWeb.Logic.Data.Models.Profile.Logical
{
    public class ExtendedClientConfiguration : IEntityTypeConfiguration<ExtendedClient>
    {
        /*
         * Original ExtendedClient configuration written as a DbQuery selector for EF Core 3.1.6:
         * 
            modelBuilder.Query<ExtendedClient>().ToQuery(() =>
                from c in this.Clients.AsQueryable()
                join csp in this.ClientServiceProfiles.AsQueryable() on c.ClientID equals csp.ParentTableID
                where csp.ParentTableName == "CLI" && csp.ParentTableID != null
                group new { Client = c, DFV = new DynamicFieldValue { Name = csp.Name, DataType = csp.DataType, RawValue = csp.Value } } by csp.ParentTableID into g
                select new ExtendedClient
                {
                    ClientID = g.Key.Value,
                    FirstName = g.Select(x => x.Client.FirstName).FirstOrDefault(),
                    LastName = g.Select(x => x.Client.LastName).FirstOrDefault(),
                    ProfileFields = g.Select(x => x.DFV).ToList()
                }
            );
        */
        public void Configure(EntityTypeBuilder<ExtendedClient> builder)
        {
            var serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            builder.HasKey(x => x.ClientID);
            // TODO: Create a matching ProfileFields column with JSON data type. This column should contain
            // a JSON array with items of the form '{ Name: "<ClientPropertyName>", Ra
            builder.Property(x => x.ProfileFields)
                .HasConversion(DynamicFieldValue.DfvToJsonConversion, DynamicFieldValue.JsonToDfvConversion)
            ;
        }
    }
}
