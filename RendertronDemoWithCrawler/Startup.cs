using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RendertronDemoWithCrawler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder => builder.AllowAnyOrigin()
                                                                                               .AllowAnyMethod()
                                                                                               .AllowAnyHeader()
                                                                                               .AllowCredentials()
                                                                                               .WithOrigins("http://localhost:3001"));
            });
            services.AddControllers();

            // Add rendertron services
            services.AddRendertron(options =>
            {
                // rendertron service url
                options.RendertronUrl = "http://rendertron:3000/render/";

                // proxy url for application
                options.AppProxyUrl = "http://localhost:3001/";

                // prerender for firefox
                options.UserAgents.Add("firefox");

                // inject shady dom
                options.InjectShadyDom = true;

                // use http compression
                options.AcceptCompression = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use Rendertron middleware
            app.UseRendertron();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
