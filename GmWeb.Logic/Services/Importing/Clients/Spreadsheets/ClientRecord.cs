using GmWeb.Logic.Utility.Csv;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using CarmaContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using ClientModel = GmWeb.Logic.Data.Models.Carma.User;

namespace GmWeb.Logic.Services.Importing.Clients.Spreadsheets;

public class ClientRecord : CsvRecord, IClientSource
{
    public string FullName { get; set; }
    public string BuildingCode { get; set; }
    public int UnitNumber { get; set; }

    public async Task<ClientModel> GetModelFromSourceAsync(ClientImportOptions options, CarmaContext context)
    {
        var client = new ClientModel 
        { 
            AgencyID = options.AgencyID
        };
        client.Profile.Residence.BuildingCode = this.BuildingCode;
        client.Profile.Residence.UnitNumber = this.UnitNumber.ToString();
        this.PartitionFullName(client);
        await context.Users.AddAsync(client);
        await context.SaveAsync();
        client.GenerateLookupID(client.UserID);
        return client;
    }

    protected bool PartitionFullName(ClientModel model)
    {
        var regex = new Regex
        (
            @"^(?<LastName>[A-Z][\w-]+)[, ]+(?<FirstName>\w.+)$"
        );
        var match = regex.Match(this.FullName);
        if (!match.Success)
            return false;
        model.FirstName = match.Groups["FirstName"].Value;
        model.LastName = match.Groups["LastName"].Value;
        return true;
    }
}
