using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiddlewareSample.Middlewares;

namespace MiddlewareSample {
    public class Startup {
        private readonly IConfiguration _configuration;

        public Startup (IConfiguration configuration) {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices (IServiceCollection services) {
            services
                .AddMyServices ()
                .ConfigureMyOptions (_configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseMyMiddlewares ();

            app.UseRouting ();

            app.UseEndpoints (endpoints => {
                endpoints.MapGet ("/", async context => {
                    Console.WriteLine ("兜底中间件\n");
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync ("Hello World!, 兜底中间件\n");
                });
            });
        }
    }
}