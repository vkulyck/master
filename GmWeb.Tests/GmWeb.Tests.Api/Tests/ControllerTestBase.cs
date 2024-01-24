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
using GmWeb.Tests.Api.Services;
using System.Collections.Generic;
using Newtonsoft.Json;
using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Tests.Api.Data;
using GmWeb.Tests.Api.Extensions;
using Startup = GmWeb.Web.Api.Startup;
using AsyncContext = Nito.AsyncEx.AsyncContext;
using GmWeb.Web.Common.Auth;
using GmWeb.Logic.Utility.Mapping;


namespace GmWeb.Tests.Api.Tests
{
    [Collection("Sequential")]
    public abstract class ControllerTestBase<TControllerTests> : ITestApplicationFactoryFixture, IAsyncLifetime
        where TControllerTests : ControllerTestBase<TControllerTests>
    {
        protected WebApplicationFactory<FakeStartup> Factory { get; set; }
        protected IServiceScope Scope { get; set; }
        protected ILoggerFactory LoggerFactory { get; private set; }
        protected ILogger<TControllerTests> Logger { get; private set; }
        protected DataService DataService { get; private set; }
        protected DataEntities Entities => this.DataService.Entities;
        protected UserManager<GmIdentity> Manager => this.DataService.Manager;
        protected CommonDbContext ComCtx => this.DataService.ComCtx;
        protected HttpClient Client { get; set; }
        protected CookieContainer CookieContainer { get; set; }
        protected HttpResponseHeaders HeaderContainer { get; set; }
        protected EntityMappingFactory Mapper => EntityMappingFactory.Instance;
        private Bogus.Faker _faker;
        protected Bogus.Faker Faker => this._faker ?? (this._faker = new Bogus.Faker());
        public ControllerTestBase(TestApplicationFactory factory)
        {
            this.GetServices(factory);
        }

        protected virtual void GetServices(WebApplicationFactory<FakeStartup> factory)
        {
            this.Factory = factory;
            this.Scope = this.Factory.Services.CreateScope();
            this.DataService = this.Scope.ServiceProvider.GetService<DataService>();
            this.LoggerFactory = this.Scope.ServiceProvider.GetService<ILoggerFactory>();
            this.Logger = this.LoggerFactory.CreateLogger<TControllerTests>();
            this.Client = this.Factory.CreateAdminClient(this.Entities);
        }

        protected async Task InitializeServicesAsync(WebApplicationFactory<FakeStartup> factory)
        {
            this.GetServices(factory);
            await this.InitializeAsync();
        }

        protected async Task<GmIdentity> RefreshIdentityAsync()
        {
            this.GetServices(this.Factory);
            var identity = await this.Manager.FindByEmailAsync(this.Entities.AdminEmail);
            return identity;
        }

        protected async Task<string> RequestDataAsync(
            string Controller, string Action, HttpMethod Method,
            object RequestData = null,
            HttpStatusCode? ExpectedStatus = null
        )
        {
            var requestUri = this.Client.BaseAddress.JoinUri(Controller, Action).AbsoluteUri;
            var response = await this.Client.ProcessRequestAsync(Method, requestUri, RequestData);

            if (ExpectedStatus.HasValue)
                Assert.Equal(ExpectedStatus, response.StatusCode);
            this.HeaderContainer = response.Headers;
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            string responseData = await response.Content.ReadAsStringAsync();
            return responseData;
        }
        protected async Task<TResult> RequestDataAsync<TResult>(
            string Controller, string Action, HttpMethod Method,
            object RequestData = null,
            HttpStatusCode? ExpectedStatus = null
        )
        {
            var responseData = await this.RequestDataAsync(
                Controller: Controller, Action: Action, Method: Method,
                RequestData: RequestData,
                ExpectedStatus: ExpectedStatus
            );
            if (ExpectedStatus == HttpStatusCode.NotFound && responseData is null)
                return default;
            Assert.NotEmpty(responseData);
            var responseModel = JsonConvert.DeserializeObject<TResult>(responseData);
            return responseModel;
        }
        protected async Task<TTResult> ValidateRequestDataErrorAsync<TTResult>(
            string Controller, string Action, HttpMethod Method,
            object RequestData = null,
            HttpStatusCode? ExpectedStatus = null
        )
        {
            var apiResponse = await this.RequestDataAsync<TTResult>(
                Controller: Controller, Action: Action, Method: Method,
                RequestData: RequestData,
                ExpectedStatus: ExpectedStatus
            );
            return apiResponse;
        }

        protected async Task ValidateRequestDataErrorAsync(
            string Controller, string Action, HttpMethod Method,
            object RequestData = null,
            string[] ExpectedErrorMessages = null,
            HttpStatusCode? ExpectedStatus = null
        ) 
        {
            var apiResponse = await this.RequestDataAsync<ErrorResponse>(
                Controller, Action, Method, RequestData, ExpectedStatus
            );
            Assert.NotEmpty(apiResponse.Errors);
            if (ExpectedErrorMessages != null)
            {
                for (int i = 0; i < ExpectedErrorMessages.Length; i++)
                {
                    Assert.NotNull(apiResponse.Errors[i]);
                    Assert.Equal(ExpectedErrorMessages[i], apiResponse.Errors[i].Message);
                }
            }
        }

        public async Task InitializeAsync()
        {
            await this.DataService.Initialize(this.Scope.ServiceProvider);
        }

        public async Task DisposeAsync()
        {
            GC.SuppressFinalize(this);
            this.Scope?.Dispose();
            await Task.CompletedTask;
        }
    }
}
