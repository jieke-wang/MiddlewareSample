using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiddlewareSample.Middlewares.Options;

namespace MiddlewareSample.Middlewares {
    public class MyMiddleware4 {
        private readonly RequestDelegate _next;
        private readonly ILogger<MyMiddleware4> _logger;
        private readonly IOptions<MyOptions> _options;

        public MyMiddleware4 (RequestDelegate next, ILogger<MyMiddleware4> logger, IOptions<MyOptions> options) {
            _next = next;
            _logger = logger;
            _options = options;
            _logger.LogInformation ($"初始化{nameof(MyMiddleware4)}");
        }

        public async Task InvokeAsync (HttpContext context) {
            await context.Response.WriteAsync ($"开始执行{nameof(MyMiddleware4)}\n");
            _logger.LogInformation ($"开始执行{nameof(MyMiddleware4)}");
            string options = Helper.JsonSerializerHelper.Serialize (_options.Value);
            _logger.LogInformation ($"{nameof(MyMiddleware4)}: {options}");
            await context.Response.WriteAsync ($"{nameof(MyMiddleware4)}: {options}\n");
            await _next (context);
            _logger.LogInformation ($"结束执行{nameof(MyMiddleware4)}");
            await context.Response.WriteAsync ($"结束执行{nameof(MyMiddleware4)}\n");
        }
    }
}