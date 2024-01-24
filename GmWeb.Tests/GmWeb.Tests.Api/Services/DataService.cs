using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommonDbContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using GmWeb.Tests.Api.Mocking;
using System.Collections.Generic;
using Newtonsoft.Json;
using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Tests.Api.Data;
using GmWeb.Tests.Api.Extensions;
using Startup = GmWeb.Web.Api.Startup;
using AsyncContext = Nito.AsyncEx.AsyncContext;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Mapping;

namespace GmWeb.Tests.Api.Services;

public class DataService
{
    public DataEntities Entities { get; private set; }
    public UserManager<GmIdentity> Manager { get; private set; }
    public CommonDbContext ComCtx { get; private set; }
    public GmIdentityContext IdCtx { get; private set; }

    public DataService(DataEntities entities, UserManager<GmIdentity> manager, CommonDbContext commonContext, GmIdentityContext idContext)
    {
        this.Entities = entities;
        this.Manager = manager;
        this.ComCtx = commonContext;
        this.IdCtx = idContext;
    }

    public async Task Initialize(IServiceProvider services)
    {
        var initalizers = new ContextInitializer[]
        {
            services.GetService<IdentityContextInitializer>(),
            services.GetService<CommonContextInitializer>(),
            // For now it is sufficient to allow one agency per user, but this may change
            // as we move forward with new clients and platforms. The claim initializer
            // should remain commented until/unless a new storage model is selected.
            // - jmenashe 2022-04-12
            // services.GetService<DataClaimInitializer>(),
        };
        foreach (var initializer in initalizers)
            await initializer.Initialize();
    }
}
