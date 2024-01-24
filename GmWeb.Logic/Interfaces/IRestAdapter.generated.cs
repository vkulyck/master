
namespace GmWeb.Logic.Interfaces;
using ErrorResponse = GmWeb.Logic.Utility.Web.ErrorResponse;
using HttpMethod = System.Net.Http.HttpMethod;
using System.Threading.Tasks;

public partial interface IRestAdapter
{
    public async Task<TSuccess> RestRequestAsync<TSuccess>(string subpath, object data, HttpMethod method, bool refresh)
        where TSuccess : class
        => await ((IRestAdapter)this).RequestAsync<TSuccess>(subpath, data, method, refresh);

    public async Task<(TSuccess Success, ErrorResponse Error)> RestRequestWithErrorAsync<TSuccess>(string subpath, object data, HttpMethod method, bool refresh)
        where TSuccess : class
        => await ((IRestAdapter)this).RequestAsync<TSuccess, ErrorResponse>(subpath, data, method, refresh);

    public async Task<(TSuccess Success, TError Error)> RestRequestAsync<TSuccess, TError>(string subpath, object data, HttpMethod method, bool refresh)
        where TSuccess : class
        where TError : class
        => await ((IRestAdapter)this).RequestAsync<TSuccess, TError>(subpath, data, method, refresh);

   public async Task<ErrorResponse> GetAsync(string subpath, object data = null, bool refresh = false)
       => (await ((IRestAdapter)this).RequestAsync<string, ErrorResponse>(subpath, data, HttpMethod.Get, refresh)).Error;

   public async Task<TSuccess> GetAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess>(subpath, data, HttpMethod.Get, refresh);

   public async Task<(TSuccess Success, ErrorResponse Error)> GetWithErrorAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await this.RestRequestWithErrorAsync<TSuccess>(subpath, data: data, method: HttpMethod.Get, refresh: refresh);

   public async Task<(TSuccess Success, TError Error)> GetAsync<TSuccess,TError>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       where TError : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess, TError>(subpath, data, HttpMethod.Get, refresh);

   public async Task<ErrorResponse> PostAsync(string subpath, object data = null, bool refresh = false)
       => (await ((IRestAdapter)this).RequestAsync<string, ErrorResponse>(subpath, data, HttpMethod.Post, refresh)).Error;

   public async Task<TSuccess> PostAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess>(subpath, data, HttpMethod.Post, refresh);

   public async Task<(TSuccess Success, ErrorResponse Error)> PostWithErrorAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await this.RestRequestWithErrorAsync<TSuccess>(subpath, data: data, method: HttpMethod.Post, refresh: refresh);

   public async Task<(TSuccess Success, TError Error)> PostAsync<TSuccess,TError>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       where TError : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess, TError>(subpath, data, HttpMethod.Post, refresh);

   public async Task<ErrorResponse> PutAsync(string subpath, object data = null, bool refresh = false)
       => (await ((IRestAdapter)this).RequestAsync<string, ErrorResponse>(subpath, data, HttpMethod.Put, refresh)).Error;

   public async Task<TSuccess> PutAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess>(subpath, data, HttpMethod.Put, refresh);

   public async Task<(TSuccess Success, ErrorResponse Error)> PutWithErrorAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await this.RestRequestWithErrorAsync<TSuccess>(subpath, data: data, method: HttpMethod.Put, refresh: refresh);

   public async Task<(TSuccess Success, TError Error)> PutAsync<TSuccess,TError>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       where TError : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess, TError>(subpath, data, HttpMethod.Put, refresh);

   public async Task<ErrorResponse> DeleteAsync(string subpath, object data = null, bool refresh = false)
       => (await ((IRestAdapter)this).RequestAsync<string, ErrorResponse>(subpath, data, HttpMethod.Delete, refresh)).Error;

   public async Task<TSuccess> DeleteAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess>(subpath, data, HttpMethod.Delete, refresh);

   public async Task<(TSuccess Success, ErrorResponse Error)> DeleteWithErrorAsync<TSuccess>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       => await this.RestRequestWithErrorAsync<TSuccess>(subpath, data: data, method: HttpMethod.Delete, refresh: refresh);

   public async Task<(TSuccess Success, TError Error)> DeleteAsync<TSuccess,TError>(string subpath, object data = null, bool refresh = false)
       where TSuccess : class
       where TError : class
       => await ((IRestAdapter)this).RequestAsync<TSuccess, TError>(subpath, data, HttpMethod.Delete, refresh);


}
