using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Launch : swagger


namespace DemoWebApp
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
                options.AddDefaultPolicy(builder =>
                    builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = Configuration.GetValue<string>("Authority");

                o.RequireHttpsMetadata = false;
                o.IncludeErrorDetails = true;

                o.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidIssuer = Configuration.GetValue<string>("Issuer"),

                    //ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5),

                };
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var exc = context.Exception;
                        Console.WriteLine(exc.Message);
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new KeyValuePairConverter());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DemoWebApp", Version = "v1" });
            });
            services.AddHealthChecks()
            .AddCheck("api_check", () =>
            {
                // example API check
                try
                {
                    return HealthCheckResult.Healthy();
                }
                catch (Exception)
                {
                    return HealthCheckResult.Unhealthy();
                }
            });

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.Use(async (context, next) => {
                context.Request.EnableBuffering();
                await next();
            });
            long requestCount = 0;
            app.Use(async (context, next) => {
                await next();
                logger.LogInformation("{}: {} req_len:{} -> resp_len:{}", ++requestCount, context.Request.Path, context.Request.ContentLength, context.Response.ContentLength);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoWebApp v1"));
                IdentityModelEventSource.ShowPII = true;
            }

            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseDefaultFiles();
            // uncomment if you want to support static files
            app.UseStaticFiles();

            

            var configJson = JsonConvert.SerializeObject(new
            {
                authority = Configuration.GetValue<string>("Authority"),
                issuer = Configuration.GetValue<string>("Issuer"),
                clientId = Configuration.GetValue<string>("ClientId"),
                token = Configuration.GetValue<string>("Token"),
            });

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("config.js", async context =>
                {
                    await context.Response.WriteAsync($"(function(){{app_config = {configJson}}})()");
                });

            });
            app.UseHealthChecks("/status");
        }
    }
}
