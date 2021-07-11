using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MiddlewareSample.Middlewares {
    public class MyMiddleware1 {
        private readonly RequestDelegate _next;

        public MyMiddleware1 (RequestDelegate next) {
            Console.WriteLine ($"初始化{nameof(MyMiddleware1)}");
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context) {
            await context.Response.WriteAsync ($"开始执行{nameof(MyMiddleware1)}\n");
            Console.WriteLine ($"\n\n开始执行{nameof(MyMiddleware1)}");
            await _next (context);
            Console.WriteLine ($"结束执行{nameof(MyMiddleware1)}\n\n");
            await context.Response.WriteAsync ($"结束执行{nameof(MyMiddleware1)}\n");
        }
    }
}