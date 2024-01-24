using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GmWeb.Web.Api.Utility;

/// <summary>
/// Attribute for enabling Brotli/GZip/Deflate compression for specied action
/// </summary>
public class ResponseCompressionAttribute : ActionFilterAttribute
{
    private Stream _originStream = null;
    private MemoryStream _ms = null;
    private readonly DecompressionMethods _enabled, _required;

    public ResponseCompressionAttribute()
    {
        _enabled = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
        _required = DecompressionMethods.None;
    }
    public ResponseCompressionAttribute(DecompressionMethods EnabledMethods, DecompressionMethods RequiredMethods)
    {
        _enabled = EnabledMethods;
        _required = RequiredMethods;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        HttpRequest request = context.HttpContext.Request;
        string acceptEncoding = request.Headers["Accept-Encoding"];
        if (string.IsNullOrEmpty(acceptEncoding)) return;
        acceptEncoding = acceptEncoding.ToUpperInvariant();
        HttpResponse response = context.HttpContext.Response;

        bool
            allowGzip = acceptEncoding.Contains("GZIP", StringComparison.OrdinalIgnoreCase) && _enabled.HasFlag(DecompressionMethods.GZip),
            requireGzip = _required.HasFlag(DecompressionMethods.GZip),

            allowDeflate = acceptEncoding.Contains("DEFLATE", StringComparison.OrdinalIgnoreCase) && _enabled.HasFlag(DecompressionMethods.Deflate),
            requireDeflate = _required.HasFlag(DecompressionMethods.Deflate),

            allowBrotli = acceptEncoding.Contains("BR", StringComparison.OrdinalIgnoreCase) && _enabled.HasFlag(DecompressionMethods.Brotli),
            requireBrotli = _required.HasFlag(DecompressionMethods.Brotli)
        ;

        if (requireBrotli || allowBrotli)
        {
            if (!(response.Body is BrotliStream))// avoid twice compression.
            {
                _originStream = response.Body;
                _ms = new MemoryStream();
                response.Headers.Add("Content-encoding", "br");
                response.Body = new BrotliStream(_ms, CompressionLevel.Optimal);
            }
        }
        else if(allowGzip || requireGzip)
        {
            if (!(response.Body is GZipStream))
            {
                _originStream = response.Body;
                _ms = new MemoryStream();
                response.Headers.Add("Content-Encoding", "gzip");
                response.Body = new GZipStream(_ms, CompressionLevel.Optimal);
            }
        }
        else if (allowDeflate || requireDeflate)
        {
            if (!(response.Body is DeflateStream))
            {
                _originStream = response.Body;
                _ms = new MemoryStream();
                response.Headers.Add("Content-encoding", "deflate");
                response.Body = new DeflateStream(_ms, CompressionLevel.Optimal);
            }
        }
        base.OnActionExecuting(context);
    }

    public override async void OnResultExecuted(ResultExecutedContext context)
    {
        if ((_originStream != null) && (_ms != null))
        {
            HttpResponse response = context.HttpContext.Response;
            await response.Body.FlushAsync();
            _ms.Seek(0, SeekOrigin.Begin);
            response.Headers.ContentLength = _ms.Length;
            await _ms.CopyToAsync(_originStream);
            response.Body.Dispose();
            _ms.Dispose();
            response.Body = _originStream;
        }
        base.OnResultExecuted(context);
    }
}