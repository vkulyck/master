using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models;
using CarmaContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using ClientModel = GmWeb.Logic.Data.Models.Carma.User;

namespace GmWeb.Logic.Services.Importing.Clients;

public interface IModelSource<TModel, TContext, TOptions>
    where TModel : BaseDataModel
    where TContext : IBaseDataContext
    where TOptions : ImportOptions
{
    Task<TModel> GetModelFromSourceAsync(TOptions options, TContext context);
}
public interface IClientSource : IModelSource<ClientModel, CarmaContext, ClientImportOptions> { }