global using GmWeb.Logic.Data.Models.Identity;
global using GmWeb.Logic.Data.Context.Identity;
global using GmWebOptions = GmWeb.Logic.Utility.Config.GmWebOptions;
global using GmWeb.Web.Common.Auth.Services.JwtAuth;
global using GmWeb.Logic.Utility.Web;
global using GmWeb.Logic.Utility.Identity.DTO;
global using Assert = GmWeb.Tests.Api.Extensions.GmAssert;

#region XUnit
global using CollectionAttribute = Xunit.CollectionAttribute;
global using FactAttribute = Xunit.FactAttribute;
global using CollectionDefinitionAttribute = Xunit.CollectionDefinitionAttribute;
global using ITestApplicationFactoryFixture = Xunit.IClassFixture<GmWeb.Tests.Api.Mocking.TestApplicationFactory>;
global using IAsyncLifetime = Xunit.IAsyncLifetime;
#endregion