using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using OpenTelemetry.WebApi2.Extensions;

namespace OpenTelemetry.WebApi2
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenTelemetry.WebApi2", Version = "v1" });
            });
            services.AddStackExchangeRedisCache(options =>
            {
                var connString =
                    $"{Configuration["Redis:Host"]}:{Configuration["Redis:Port"]}";

                options.Configuration = connString;
            });
            services.AddOpenTelemetryTracing((sp, builder) =>
            {
                RedisCache cache = (RedisCache)sp.GetRequiredService<IDistributedCache>();
                builder.AddAspNetCoreInstrumentation()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebApi2"))
                    .AddRedisInstrumentation(cache.GetConnection())
                    .AddJaegerExporter(opts =>
                    {
                        opts.AgentHost = Configuration["Jaeger:AgentHost"];
                        opts.AgentPort = Convert.ToInt32(Configuration["Jaeger:AgentPort"]);
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenTelemetry.WebApi2 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
