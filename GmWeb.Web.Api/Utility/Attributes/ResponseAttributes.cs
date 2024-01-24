using Microsoft.AspNetCore.Mvc;
using System;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace GmWeb.Web.Api.Utility.Attributes
{
    public class UseStatusCodeAttribute : ProducesResponseTypeAttribute
    {
        public UseStatusCodeAttribute(HttpStatusCode code) : base((int)code) { }
    }
    public class UseSuccessModel : ProducesResponseTypeAttribute
    {
        public UseSuccessModel(Type modelType) : base(modelType, (int)HttpStatusCode.OK) { }
    }
    public class UseBadRequestModelAttribute : ProducesResponseTypeAttribute
    {
        public UseBadRequestModelAttribute() : base(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest) { }
    }
    public class UseNotFoundModelAttribute : ProducesResponseTypeAttribute
    {
        public UseNotFoundModelAttribute() : base(typeof(ErrorResponse), (int)HttpStatusCode.NotFound) { }
    }
    public class UseForbiddenModelAttribute : ProducesResponseTypeAttribute
    {
        public UseForbiddenModelAttribute() : base(typeof(ErrorResponse), (int)HttpStatusCode.Forbidden) { }
    }
    public class UseUnauthorizedModelAttribute : ProducesResponseTypeAttribute
    {
        public UseUnauthorizedModelAttribute() : base(typeof(ErrorResponse), (int)HttpStatusCode.Unauthorized) { }
    }
}
