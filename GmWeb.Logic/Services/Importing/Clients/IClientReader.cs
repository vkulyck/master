using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models;
using ClientModel = GmWeb.Logic.Data.Models.Carma.User;
using CarmaContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;

namespace GmWeb.Logic.Services.Importing.Clients;

public interface IModelReader<TModel, TContext, TOptions>
    where TModel : BaseDataModel
    where TContext : IBaseDataContext
    where TOptions : ImportOptions
{
    IAsyncEnumerable<TModel> GetModelsFromSourcesAsync();
}

public interface IClientReader : IModelReader<ClientModel, CarmaContext, ClientImportOptions> { }