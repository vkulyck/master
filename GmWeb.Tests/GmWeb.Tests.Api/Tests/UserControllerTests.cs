using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using System.Dynamic;
using Startup = GmWeb.Web.Api.Startup;
using GmWeb.Tests.Api.Mocking;
using GmWeb.Tests.Api.Extensions;
using GmWeb.Logic.Utility.Extensions.Http;

using User = GmWeb.Logic.Data.Models.Carma.User;
using UserDTO = GmWeb.Web.Common.Models.Carma.UserDTO;

namespace GmWeb.Tests.Api.Tests
{
    [Collection(nameof(ControllerTestCollection))]
    public class UserControllerTests : ControllerTestBase<UserControllerTests>
    {
        public UserControllerTests(TestApplicationFactory factory) : base(factory)
        {
        }

        protected async Task<UserDTO> ValidateDetails(HttpClient client, string endpoint, GmIdentity identity, User user)
        {
            var response = await client.GetAsync(endpoint);
            return await this.ValidateDetails(response, identity, user);
        }
        protected async Task<UserDTO> ValidateDetails(HttpResponseMessage response, GmIdentity identity, User user)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var okData = await response.ParseBodyAsync<UserDTO>();
            Assert.Equal(okData.AccountID, identity.Id);
            Assert.Equal(okData.AccountID, user.AccountID);
            Assert.Equal(okData.FirstName, user.FirstName);
            Assert.Equal(okData.LastName, user.LastName);
            Assert.Equal(okData.LanguageCode, user.LanguageCode);
            Assert.Equal(okData.Gender, user.Gender);
            Assert.Equal(okData.Email, identity.Email);
            Assert.Equal(okData.Email, user.Email);
            return okData;
        }

        [Fact]
        public async Task ValidateGetUser()
        {
            var webClient = this.Factory.CreateAdminClient(this.Entities);
            var identity = this.Entities.AdminIdentity;
            var staffer = this.Entities.AdminStaffer;

            await this.ValidateDetails(webClient, $"user/current", identity, staffer);
            await this.ValidateDetails(webClient, $"user/?UserID={staffer.UserID}", identity, staffer);
            await this.ValidateDetails(webClient, $"user/?AccountID={staffer.AccountID}", identity, staffer);
        }
    }
}
