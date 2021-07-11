using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiddlewareSample.Middlewares.Options;

namespace MiddlewareSample.Middlewares {
    public class MyMiddleware5 {
        private readonly RequestDelegate _next;
        private readonly ILogger<MyMiddleware5> _logger;
        private readonly IOptions<MyOptions> _options;

        public MyMiddleware5 (RequestDelegate next, ILogger<MyMiddleware5> logger, IConfigureOptions<MyOptions> configureOptions, IOptions<MyOptions> options) {
            _next = next;
            _logger = logger;
            _options = options;
            configureOptions.Configure (_options.Value);
            _logger.LogInformation ($"初始化{nameof(MyMiddleware5)}");
        }

        public async Task InvokeAsync (HttpContext context) {
            await context.Response.WriteAsync ($"开始执行{nameof(MyMiddleware5)}\n");
            _logger.LogInformation ($"开始执行{nameof(MyMiddleware5)}");
            string options = Helper.JsonSerializerHelper.Serialize (_options.Value);
            _logger.LogInformation ($"{nameof(MyMiddleware5)}: {options}");
            await context.Response.WriteAsync ($"{nameof(MyMiddleware5)}: {options}\n");
            await _next (context);
            _logger.LogInformation ($"结束执行{nameof(MyMiddleware5)}");
            await context.Response.WriteAsync ($"结束执行{nameof(MyMiddleware5)}\n");
        }
    }
}