using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using MiddlewareSample.Middlewares.Helper;

namespace MiddlewareSample.Middlewares {
    public class CompressMiddleware : IMiddleware, IDisposable {
        private readonly AsyncLocal<MemoryStream> _inputStream;
        private readonly AsyncLocal<MemoryStream> _outputStream;
        private readonly ILogger<CompressMiddleware> _logger;

        public CompressMiddleware (ILogger<CompressMiddleware> logger) {
            _logger = logger;
            _logger.LogInformation ($"初始化{nameof(CompressMiddleware)}");
            _inputStream = new ();
            _outputStream = new ();
        }

        public void Dispose () {
            _inputStream.Value?.Dispose ();
            _outputStream.Value?.Dispose ();
        }

        public async Task InvokeAsync (HttpContext context, RequestDelegate next) {
            context.Request.EnableBuffering ();
            Stream originalInputStream = context.Request.Body;
            Stream originalOutputStream = context.Response.Body;

            try {
                if (context.Request.ContentLength > 0 && !(context.Request.HasFormContentType && context.Request.Form.Files.Any ())) {
                    context.Request.Body = _inputStream.Value = new MemoryStream ();
                    await originalInputStream.CopyToAsync (_inputStream.Value);
                }
                await LogRequestAsync (context.Request);
                context.Request.Body.Seek (0, SeekOrigin.Begin);

                _outputStream.Value = new MemoryStream ();
                context.Response.Body = _outputStream.Value;
                await next (context);
                await LogResponseAsync (context.Response);

                context.Response.Headers["Content-Encoding"] = "gzip";
                context.Response.Body = originalOutputStream;
                await CompressAsync (context.Response);
            } catch (Exception ex) {
                _logger.LogError (ex, nameof (CompressMiddleware));
                context.Response.Body = originalOutputStream;
            } finally {
                context.Request.Body = originalInputStream;
            }
        }

        private async Task LogRequestAsync (HttpRequest request) {
            Dictionary<string, object> requestData = new () { { "Url", request.GetDisplayUrl () }
            };

            Dictionary<string, string> requestHeaders = new ();
            foreach (var header in request.Headers) {
                requestHeaders[header.Key] = header.Value;
            }
            requestData["Headers"] = requestHeaders;

            if (_inputStream.Value == null && request.HasFormContentType) {
                if (request.Form.Keys.Count > 0) {
                    Dictionary<string, string> formData = new ();
                    foreach (var formKey in request.Form.Keys) {
                        formData[formKey] = request.Form[formKey];
                    }
                    requestData["FormData"] = formData;
                }

                if (request.Form.Files.Count > 0) {
                    Dictionary<string, List<object>> formFile = new ();
                    foreach (var file in request.Form.Files) {
                        object fileData = new {
                            file.ContentDisposition,
                            file.ContentType,
                            file.FileName,
                            file.Headers,
                            file.Length,
                            file.Name,
                        };

                        if (formFile.TryGetValue (file.Name, out List<object> fileDatas) == false)
                            formFile[file.Name] = fileDatas = new List<object> ();
                        fileDatas.Add (fileData);
                    }
                    requestData["FormFile"] = formFile;
                }
            } else if (_inputStream.Value != null) {
                _inputStream.Value.Seek (0, SeekOrigin.Begin);
                TextReader tr = new StreamReader (_inputStream.Value, Encoding.UTF8);
                requestData["Body"] = await tr.ReadToEndAsync ();
            }

            _logger.LogInformation ($"requestData: {JsonSerializerHelper.Serialize (requestData)}");
        }

        private async Task LogResponseAsync (HttpResponse response) {
            Dictionary<string, object> responseData = new ();

            Dictionary<string, string> responseHeaders = new ();
            foreach (var header in response.Headers) {
                responseHeaders[header.Key] = header.Value;
            }
            responseData["Headers"] = responseHeaders;

            _outputStream.Value.Seek (0, SeekOrigin.Begin);
            TextReader tr = new StreamReader (_outputStream.Value, Encoding.UTF8);
            responseData["Body"] = await tr.ReadToEndAsync ();

            _logger.LogInformation ($"responseData: {JsonSerializerHelper.Serialize (responseData)}");
        }

        private async Task CompressAsync (HttpResponse response) {
            _outputStream.Value.Seek (0, SeekOrigin.Begin);
            using MemoryStream compressedStream = new ();
            using GZipStream gzipStream = new (compressedStream, CompressionMode.Compress);
            await _outputStream.Value.CopyToAsync (gzipStream);
            await gzipStream.FlushAsync ();
            byte[] buffer = compressedStream.ToArray ();
            response.ContentLength = buffer.Length;
            await response.BodyWriter.WriteAsync (buffer);
            await response.BodyWriter.FlushAsync ();
        }
    }
}