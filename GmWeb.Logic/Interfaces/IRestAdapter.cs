using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;
using HttpMethod = System.Net.Http.HttpMethod;
using StatusCode = System.Net.HttpStatusCode;
using Newtonsoft.Json;
using GmWeb.Logic.Utility.Extensions.Http;
using GmWeb.Logic.Utility.Web;
using ApiIdentityResult = GmWeb.Logic.Utility.Identity.Results.ApiIdentityResult;

namespace GmWeb.Logic.Interfaces;

public partial interface IRestAdapter
{
    public ILogger Logger { get; }
    public Uri BaseClientUri { get; }
    public T ConvertResponse<T>(string data)
        where T : class
    {
        if (typeof(T) == typeof(string))
            return data as T;
        if (typeof(T) == typeof(IdentityResult))
        {
            var apiResult = JsonConvert.DeserializeObject<ApiIdentityResult>(data);
            var idResult = (IdentityResult)apiResult;
            var tResult = idResult as T;
            return tResult;
        }
        else
        {
            var result = JsonConvert.DeserializeObject<T>(data);
            return result;
        }
    }

    private async Task<(string Data, bool Success)> RequestAsync(string subpath, object query, HttpMethod method, bool refresh)
    {
        using var httpClient = await this.CreateHttpClient(refresh: refresh);
        if (httpClient == null)
            return default;
        string endpointUri = new Uri(this.BaseClientUri, subpath).AbsoluteUri;
        var response = await httpClient.ProcessRequestAsync(method, endpointUri, query);
        var data = await response.Content.ReadAsStringAsync();
        return (data, response.IsSuccessStatusCode);
    }

    public async Task<TSuccess> RequestAsync<TSuccess>(string subpath, object query, HttpMethod method, bool refresh)
        where TSuccess : class
    {
        if(typeof(TSuccess) == typeof(IdentityResult))
        {
            var result = await this.RequestAsync<TSuccess, TSuccess>(subpath, query, method, refresh);
            return result.Success ?? result.Error;
        }
        else
        {
            var result = await this.RequestAsync<TSuccess, string>(subpath, query, method, refresh);
            return result.Success;
        }
    }
    public async Task<(TSuccess Success, TError Error)> RequestAsync<TSuccess, TError>(string subpath, object query, HttpMethod method, bool refresh)
        where TSuccess : class
        where TError : class
    {
        (string Data, bool Success) result = default;
        try
        {
            result = await this.RequestAsync(subpath, query, method, refresh);
            if (result.Success)
                return (this.ConvertResponse<TSuccess>(result.Data), default);
            else
                return (default, this.ConvertResponse<TError>(result.Data));
        }
        catch (Exception ex)
        {
            if (this.Logger is not null)
            {
                if(result.Data is null)
                    this.Logger.LogError(ex, ex.Message);
                else
                    this.Logger.LogError(ex, ex.Message, result.Data);
            }
        }
        return default;
    }
    public Task<HttpClient> CreateHttpClient() => CreateHttpClient(refresh: false);
    public Task<HttpClient> CreateHttpClient(bool refresh);
}
