using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Security.Claims;

namespace GmWeb.Web.Common.Utility.Extensions
{
    public static class CookieExtensions
    {
        public static void RemoveCookies(this HttpContextBase context, params string[] keys)
            => context.ApplicationInstance.Context.RemoveCookies(keys);

        public static void RemoveCookies(this HttpContext context, params string[] keys)
        {
            foreach (var key in keys)
            {
                var cookie = context.Request.Cookies[key];
                if (cookie != null)
                {
                    cookie = new HttpCookie(key);
                    cookie.Expires = DateTime.Now.AddHours(-1);
                    context.Response.Cookies.Add(cookie);
                }
            }
        }

        public static bool HasCookies(this HttpContextBase context, params string[] keys)
            => context.ApplicationInstance.Context.HasCookies(keys);

        public static bool HasCookies(this HttpContext context, params string[] keys)
        {
            foreach (var key in keys)
            {
                var cookie = context.Request.Cookies[key];
                if (cookie == null)
                    return false;
            }
            return true;
        }


        public static ClaimsPrincipal DecryptOwinSession(string ticket)
        {
            ticket = ticket.Replace('-', '+').Replace('_', '/');

            var padding = 3 - ((ticket.Length + 3) % 4);
            if (padding != 0)
                ticket = ticket + new string('=', padding);

            var bytes = Convert.FromBase64String(ticket);

            bytes = System.Web.Security.MachineKey.Unprotect(bytes,
                "Microsoft.Owin.Security.Cookies.CookieAuthenticationMiddleware",
                    "ApplicationCookie", "v1");

            using (var memory = new MemoryStream(bytes))
            {
                using (var compression = new GZipStream(memory,
                                                    CompressionMode.Decompress))
                {
                    using (var reader = new BinaryReader(compression))
                    {
                        reader.ReadInt32();
                        string authenticationType = reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();

                        int count = reader.ReadInt32();

                        var claims = new Claim[count];
                        for (int index = 0; index != count; ++index)
                        {
                            string type = reader.ReadString();
                            type = type == "\0" ? ClaimTypes.Name : type;

                            string value = reader.ReadString();

                            string valueType = reader.ReadString();
                            valueType = valueType == "\0" ?
                                           "http://www.w3.org/2001/XMLSchema#string" :
                                             valueType;

                            string issuer = reader.ReadString();
                            issuer = issuer == "\0" ? "LOCAL AUTHORITY" : issuer;

                            string originalIssuer = reader.ReadString();
                            originalIssuer = originalIssuer == "\0" ?
                                                         issuer : originalIssuer;

                            claims[index] = new Claim(type, value,
                                                   valueType, issuer, originalIssuer);
                        }

                        var identity = new ClaimsIdentity(claims, authenticationType,
                                                      ClaimTypes.Name, ClaimTypes.Role);

                        var principal = new ClaimsPrincipal(identity);

                        return principal;
                    }
                }
            }
        }
    }
}
