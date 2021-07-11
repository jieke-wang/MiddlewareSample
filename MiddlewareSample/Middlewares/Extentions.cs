using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MiddlewareSample.Middlewares.Options;

namespace MiddlewareSample.Middlewares {
    public static class Extentions {
        public static IServiceCollection AddMyServices (this IServiceCollection services) {
            services.AddSingleton<CompressMiddleware> ();
            services.Replace (ServiceDescriptor.Singleton<IMiddlewareFactory, MyMiddlewareFactory> ());
            return services;
        }

        public static IServiceCollection ConfigureMyOptions (this IServiceCollection services, IConfiguration configuration) {
            services
                .AddOptions<MyOptions> ()
                .Bind (configuration.GetSection (MyOptions.OptionsKey))
                .Configure (options => {
                    options.Name += " - 来自Configure Action";
                });

            return services;
        }

        public static IApplicationBuilder UseMyMiddlewares (this IApplicationBuilder app) {
            app.UseMiddleware<CompressMiddleware> ();
            app.UseMiddleware<MyMiddleware1> ();
            app.UseMiddleware<MyMiddleware2> ();
            app.UseMiddleware<MyMiddleware3> ();
            app.UseMiddleware<MyMiddleware4> (Microsoft.Extensions.Options.Options.Create (new MyOptions () {
                Name = "jack 来自中间件参数"
            }));
            app.UseMiddleware<MyMiddleware5> (Microsoft.Extensions.Options.Options.Create (new MyOptions () {
                Name = "jack 来自中间件参数"
            }));
            return app;
        }
    }
}