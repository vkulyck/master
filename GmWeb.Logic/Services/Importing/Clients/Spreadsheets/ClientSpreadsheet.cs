using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Utility.Csv;
using GmWeb.Logic.Data.Context.Carma;
using ClientModel = GmWeb.Logic.Data.Models.Carma.User;
using MappingFactory = GmWeb.Logic.Utility.Mapping.EntityMappingFactory;

namespace GmWeb.Logic.Services.Importing.Clients.Spreadsheets;
public class ClientSpreadsheet : CsvMapReader<ClientRecord, ClientRecordMap>, IClientReader
{
    protected MappingFactory Mapper => MappingFactory.Instance;
    public ClientImportOptions Settings { get; }
    protected CarmaContext Context { get; }
    public ClientSpreadsheet(ClientImportOptions settings, CarmaContext context)
        : base(settings.SourceClientSpreadsheet)
    {
        this.Settings = settings;
        this.Context = context;
    }

    public async IAsyncEnumerable<ClientModel> GetModelsFromSourcesAsync()
    {
        await foreach (var record in this.GetRecordsAsync())
        {
            var clientModel = await record.GetModelFromSourceAsync(this.Settings, this.Context);
            yield return clientModel;
        }
    }
}
