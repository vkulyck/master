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
using Xunit;
using Newtonsoft.Json;
using System.Text;
using System.Dynamic;
using Startup = GmWeb.Web.Api.Startup;
using GmWeb.Tests.Api.Mocking;
using GmWeb.Tests.Api.Extensions;

using User = GmWeb.Logic.Data.Models.Carma.User;
using ClientDetailsDTO = GmWeb.Web.Api.Models.Common.UserDetailsDTO;

namespace GmWeb.Tests.Api.Tests
{
    [Collection(nameof(ControllerTestCollection))]
    public class StaffControllerTests : ControllerTestBase<StaffControllerTests>
    {
        public StaffControllerTests(TestApplicationFactory factory) : base(factory)
        {
        }
    }
}
