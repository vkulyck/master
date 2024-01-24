using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
using GmWeb.Logic.Enums;
using GmWeb.Logic.Data.Models.Carma;
using GmWeb.Logic.Utility.Extensions.Collections;
using GmWeb.Logic.Utility.Performance.Paging;
using User = GmWeb.Logic.Data.Models.Carma.User;
using UserDTO = GmWeb.Web.Common.Models.Carma.UserDTO;
using UserDetailsDTO = GmWeb.Web.Api.Models.Common.UserDetailsDTO;

namespace GmWeb.Tests.Api.Tests
{
    [Collection(nameof(ControllerTestCollection))]
    public class ClientControllerTests : ControllerTestBase<ClientControllerTests>
    {
        public ClientControllerTests(TestApplicationFactory factory) : base(factory)
        {
        }

        protected async Task<UserDetailsDTO> ValidateDetails(GmIdentity identity, User user)
        {
            var reqUser = await this.RequestDataAsync<UserDetailsDTO>(
                Controller: "Client", Action: "", Method: HttpMethod.Get,
                RequestData: new { user.UserID },
                ExpectedStatus: HttpStatusCode.OK
            );

            Assert.Equal(reqUser.AccountID, identity.Id);
            Assert.Equal(reqUser.AccountID, user.AccountID);
            Assert.Equal(reqUser.FirstName, user.FirstName);
            Assert.Equal(reqUser.LastName, user.LastName);
            Assert.Equal(reqUser.LanguageCode, user.LanguageCode);
            Assert.Equal(reqUser.Gender, user.Gender);
            Assert.Equal(reqUser.Email, identity.Email);
            Assert.Equal(reqUser.Email, user.Email);
            return reqUser;
        }

        [Fact]
        public async Task ValidateEmptyDetails()
        {
            var noidData = await this.RequestDataAsync<ErrorResponse>(
                Controller: "Client", Action: "", Method: HttpMethod.Get,
                RequestData: null,
                ExpectedStatus: HttpStatusCode.BadRequest
            );

            Assert.NotNull(noidData);
            Assert.False(noidData.Success);
            Assert.Collection(noidData.Errors, error =>
            {
                Assert.Null(error.StackTrace);
                Assert.Equal("The requested data could not be retrieved.", error.Message);
            });
        }

        [Fact]
        public async Task ValidateAdminDetails()
        {
            var identity = this.Entities.AdminIdentity;
            var staffer = this.Entities.AdminStaffer;

            var okData = await this.ValidateDetails(identity, staffer);
            Assert.Empty(okData.Activities);
        }

        [Fact]
        public async Task ValidateMemberDetails()
        {
            var identity = this.Entities.MemberIdentity;
            var client = this.Entities.MemberClient;

            var okData = await this.ValidateDetails(identity, client);
            Assert.Collection(okData.Activities, activity =>
            {
                Assert.True(true);
            });
        }

        [Fact]
        public async Task TestStarredUsers()
        {
            var lookups = await this.ComCtx.Users
                .Where(x => x.UserRole == Logic.Enums.UserRole.Client)
                .Where(x => x.AgencyID == this.Entities.Agency.AgencyID)
                .Select(x => new { x.UserID, x.LookupID })
                .Shuffle()
                .ToListAsync()
            ;
            var totalClientCount = lookups.Count;
            int starredClientCount = 10;
            Dictionary<int, UserDetailsDTO>
                detailMap = new(),
                starredMap = new()
            ;
            for(int i = 0; i < starredClientCount + 5; i++)
            {
                var lookup = lookups[i];
                bool isStarred = i < starredClientCount;
                var reqUpdate = new { lookup.UserID, IsStarred = isStarred };
                var respUpdate = await this.RequestDataAsync<UserDTO>(
                    Controller: "Client", Action: "Update", Method: HttpMethod.Put,
                    RequestData: reqUpdate,
                    ExpectedStatus: HttpStatusCode.OK
                );
                var reqDetails = new { lookup.LookupID };
                var respDetails = await this.RequestDataAsync<UserDetailsDTO>(
                    Controller: "Client", Action: string.Empty, Method: HttpMethod.Get,
                    RequestData: reqDetails,
                    ExpectedStatus: HttpStatusCode.OK
                );
                Assert.Equal(reqUpdate.UserID, respUpdate.UserID);
                Assert.Equal(reqUpdate.IsStarred, respUpdate.IsStarred);
                Assert.Equal(reqUpdate.UserID, respDetails.UserID);
                Assert.Equal(reqUpdate.IsStarred, respDetails.IsStarred);

                detailMap[respDetails.UserID] = respDetails;
                if (isStarred)
                    starredMap[respDetails.UserID] = respDetails;
            }
            var clients = await this.ComCtx.Users 
                //.Include(x => x.ParentConfigs)
                .Where(x => x.UserRole == Logic.Enums.UserRole.Client)
                .Where(x => x.AgencyID == this.Entities.Agency.AgencyID)
                .ToListAsync()
            ;
            await clients.ForEachAsync(async(x) => await x.LoadParentConfigAsync(owner: this.Entities.AdminStaffer, context: this.ComCtx));

            var reqClients = new { this.Entities.Agency.AgencyID };
            var respClients = await this.RequestDataAsync<ExtendedPagedList<UserDTO, string>>(
                    Controller: "Client", Action: "List", Method: HttpMethod.Get,
                    RequestData: reqClients,
                    ExpectedStatus: HttpStatusCode.OK
                );
            Assert.Equal(respClients.TotalItemCount, totalClientCount);

            foreach(var client in respClients.Items)
            {
                if (detailMap.TryGetValue(client.UserID, out var details))
                {
                    Assert.Equal(details.IsStarred, client.IsStarred);
                }
            }

            var reqStarredClients = new
            {
                AgencyID = this.Entities.Agency.AgencyID,
                Include = UserConfigStatus.Starred
            };
            var respStarredClients = await this.RequestDataAsync<ExtendedPagedList<UserDTO, string>>(
                    Controller: "Client", Action: "List", Method: HttpMethod.Get,
                    RequestData: reqStarredClients,
                    ExpectedStatus: HttpStatusCode.OK
                );

            Assert.Equal(starredMap.Count, respStarredClients.Items.Count);
            foreach (var client in respStarredClients.Items)
            {
                Assert.True(client.IsStarred);
                Assert.True(starredMap.ContainsKey(client.UserID));
            }

            var reqUnstarredClients = new
            {
                AgencyID = this.Entities.Agency.AgencyID,
                Exclude = UserConfigStatus.Starred
            };
            var respUnstarredClients = await this.RequestDataAsync<ExtendedPagedList<UserDTO, string>>(
                    Controller: "Client", Action: "List", Method: HttpMethod.Get,
                    RequestData: reqUnstarredClients,
                    ExpectedStatus: HttpStatusCode.OK
                );

            foreach (var client in respUnstarredClients.Items)
            {
                Assert.False(client.IsStarred);
                Assert.False(starredMap.ContainsKey(client.UserID));
            }
        }
    }
}
