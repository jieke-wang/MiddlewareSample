using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MiddlewareSample.Middlewares {
    public class MyMiddleware2 {
        private readonly RequestDelegate _next;
        private readonly ILogger<MyMiddleware2> _logger;

        public MyMiddleware2 (RequestDelegate next, ILogger<MyMiddleware2> logger) {
            _next = next;
            _logger = logger;

            _logger.LogInformation ($"初始化{nameof(MyMiddleware2)}");
        }

        public async Task InvokeAsync (HttpContext context) {
            await context.Response.WriteAsync ($"开始执行{nameof(MyMiddleware2)}\n");
            _logger.LogInformation ($"开始执行{nameof(MyMiddleware2)}");
            await _next (context);
            _logger.LogInformation ($"结束执行{nameof(MyMiddleware2)}");
            await context.Response.WriteAsync ($"结束执行{nameof(MyMiddleware2)}\n");
        }
    }
}