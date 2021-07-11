using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MiddlewareSample.Middlewares {
    public class MyMiddlewareFactory : IMiddlewareFactory {
        private readonly IServiceProvider _iServiceProvider;
        private readonly ILogger _logger;

        public MyMiddlewareFactory (IServiceProvider serviceProvider, ILogger<MyMiddlewareFactory> logger) {
            this._iServiceProvider = serviceProvider;
            this._logger = logger;

            _logger.LogInformation ($"初始化{nameof(MyMiddlewareFactory)}");
        }

        public IMiddleware Create (Type middlewareType) {
            _logger.LogInformation ($"{nameof(MyMiddlewareFactory)} 创建 {middlewareType.FullName}");
            return (IMiddleware) this._iServiceProvider.GetService (middlewareType);
        }

        public void Release (IMiddleware middleware) {
            _logger.LogInformation ($"{nameof(MyMiddlewareFactory)} 释放 {middleware.GetType().FullName}");
            if (middleware is IDisposable disposable) disposable.Dispose ();
        }
    }
}