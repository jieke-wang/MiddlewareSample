using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiddlewareSample.Middlewares.Options;

namespace MiddlewareSample.Middlewares {
    public class MyMiddleware3 {
        private readonly RequestDelegate _next;
        private readonly ILogger<MyMiddleware3> _logger;
        private readonly IOptions<MyOptions> _options;

        public MyMiddleware3 (RequestDelegate next, ILogger<MyMiddleware3> logger, IOptions<MyOptions> options) {
            _next = next;
            _logger = logger;
            _options = options;
            _logger.LogInformation ($"初始化{nameof(MyMiddleware3)}");
        }

        public async Task InvokeAsync (HttpContext context) {
            await context.Response.WriteAsync ($"开始执行{nameof(MyMiddleware3)}\n");
            _logger.LogInformation ($"开始执行{nameof(MyMiddleware3)}");
            string options = Helper.JsonSerializerHelper.Serialize (_options.Value);
            _logger.LogInformation ($"{nameof(MyMiddleware3)}: {options}");
            await context.Response.WriteAsync ($"{nameof(MyMiddleware3)}: {options}\n");
            await _next (context);
            _logger.LogInformation ($"结束执行{nameof(MyMiddleware3)}");
            await context.Response.WriteAsync ($"结束执行{nameof(MyMiddleware3)}\n");
        }
    }
}