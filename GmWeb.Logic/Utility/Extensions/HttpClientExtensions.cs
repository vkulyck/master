using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using GmWeb.Logic.Interfaces;

namespace GmWeb.Logic.Utility.Extensions.Http
{
    public static class HttpClientExtensions
    {
        public static readonly StringContent EmptyJsonObject = new("{}", Encoding.UTF8, MediaTypeNames.Application.Json);
        public static async Task DownloadFileAsync(this HttpClient client, Uri resource, string path)
        {
            var response = await client.GetAsync(resource);
            using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            await response.Content.CopyToAsync(fs);
        }
        // TODO: Remove
        public static async Task<TModel> ParseBodyAsync<TModel>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<TModel>(content);
            return model;
        }

        public static async Task<HttpResponseMessage> ProcessDeleteAsync(this HttpClient client, string uriBase, object data = null)
        {
            if (data == null)
                return await client.DeleteAsync(uriBase);
            var uri = uriBase.WithQuery(data);
            var response = await client.DeleteAsync(uri);
            return response;
        }
        private static async Task<HttpResponseMessage> ProcessGetAsync(this HttpClient client, string uriBase, object data)
        {
            if (data == null)
                return await client.GetAsync(uriBase);
            var uri = uriBase.WithQuery(data);
            var response = await client.GetAsync(uri);
            return response;
        }
        // TODO: Make private
        public static async Task<HttpResponseMessage> ProcessPostAsync(this HttpClient client, string uriBase, object data = null)
        {
            if (data == null)
                return await client.PostAsync(uriBase, content: EmptyJsonObject);
            var content = data.AsJson();
            var response = await client.PostAsync(uriBase, content);
            return response;
        }
        private static async Task<HttpResponseMessage> ProcessPutAsync(this HttpClient client, string uriBase, object data = null)
        {
            if (data == null)
                return await client.PutAsync(uriBase, content: null);
            var content = data.AsJson();
            var response = await client.PutAsync(uriBase, content);
            return response;
        }
        // TODO: Make private
        public static async Task<HttpResponseMessage> ProcessPatchAsync(this HttpClient client, string uriBase, object data = null)
        {
            if (data == null)
                return await client.PatchAsync(uriBase, content: null);
            var content = data.AsJson();
            var response = await client.PatchAsync(uriBase, content);
            return response;
        }

        public static async Task<HttpResponseMessage> ProcessRequestAsync(this HttpClient client, HttpMethod method, string uriBase, object data = null) => method.Method.ToUpper() switch
            {
                "GET" => await client.ProcessGetAsync(uriBase, data),
                "POST" => await client.ProcessPostAsync(uriBase, data),
                "PUT" => await client.ProcessPutAsync(uriBase, data),
                "DELETE" => await client.ProcessDeleteAsync(uriBase, data),
                "PATCH" => await client.ProcessPatchAsync(uriBase, data),
                _ => throw new NotImplementedException($"No request handler defined for HTTP method '{method}")
            };

        private static StringContent AsJson(this object data)
        {
            string regData = JsonConvert.SerializeObject(data);
            var content = new StringContent(regData, Encoding.UTF8, MediaTypeNames.Application.Json);
            return content;
        }
    }
    public static class UriParameterization
    {
        public static Uri WithQuery(this string uri, string QueryParamName, string QueryParamValue)
            => new Uri(uri, UriKind.RelativeOrAbsolute).WithQuery(QueryParamName, QueryParamValue);
        public static Uri WithQuery(this string uri, Dictionary<string, object> QueryParams)
            => new Uri(uri, UriKind.RelativeOrAbsolute).WithQuery(QueryParams);
        public static Uri WithQuery(this string uri, object data)
            => new Uri(uri, UriKind.RelativeOrAbsolute).WithQuery(data);
        public static Uri WithQuery(this string uri, QueryString qs)
            => new Uri(uri, UriKind.RelativeOrAbsolute).WithQuery(qs);

        public static Uri WithQuery(this Uri uri, string QueryParamName, string QueryParamValue)
            => uri.WithQuery(QueryString.Create(QueryParamName, QueryParamValue));

        public static Uri WithQuery(this Uri uri, Dictionary<string, object> QueryParams)
        {
            if (QueryParams == null || QueryParams.Count == 0)
                return uri;
            var nameValuePairs = QueryParams.ToDictionary(x => x.Key, x => x.Value?.ToString());
            var newQS = QueryString.Create(nameValuePairs);
            return uri.WithQuery(newQS);
        }
        public static Uri WithQuery(this Uri uri, object data)
        {
            if (data == null)
                return uri;
            string json = JsonConvert.SerializeObject(data);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (dict.Count == 0)
                return uri;
            var qs = QueryString.Create(dict);
            return uri.WithQuery(qs);
        }
        public static Uri WithQuery(this Uri uri, QueryString qs)
        {
            if (!qs.HasValue)
                return uri;
            var kind = uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative;
            var baseUri = uri;
            if (!string.IsNullOrWhiteSpace(uri.Query))
            {
                var initialQS = QueryString.FromUriComponent(uri.GetComponents(UriComponents.Query, UriFormat.Unescaped));
                qs = initialQS.Add(qs);
                baseUri = new Uri(uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped), kind);
            }

            var fullUri = new Uri($"{baseUri.OriginalString}{qs.ToUriComponent()}", kind);
            return fullUri;
        }
        public static Uri JoinUri(this Uri baseUri, params string[] components)
        {
            var trimmed = components.Select(x => Regex.Replace(x, @"^/+|/+$", string.Empty)).ToList();
            string joined = string.Join('/', trimmed);
            var uri = new Uri(baseUri, joined);
            return uri;
        }
    }
}
