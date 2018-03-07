using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgriSystemCore.Authorize;
using AgriSystemCore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgriSystemCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AgriOptions>(Configuration);

            services.AddAuthentication("AgriSecurityScheme")
                   .AddCookie("AgriSecurityScheme", options =>
                   {

                       options.AccessDeniedPath = new PathString("/Authorization/Login");
                       options.Cookie = new CookieBuilder
                       {
                           //Domain = "",
                           HttpOnly = true,
                           Name = ".Agri.Security.Cookie",
                           Path = "/",
                           SameSite = SameSiteMode.Lax,
                           SecurePolicy = CookieSecurePolicy.SameAsRequest
                       };
                       options.Events = new CookieAuthenticationEvents
                       {
                           OnSignedIn = context =>
                           {
                               //Console.WriteLine("{0} - {1}: {2}", DateTime.Now, "OnSignedIn", context.Principal.Identity.Name);
                               return Task.CompletedTask;
                           },
                           OnSigningOut = context =>
                           {
                               //Console.WriteLine("{0} - {1}: {2}", DateTime.Now, "OnSigningOut", context.HttpContext.User.Identity.Name);
                               return Task.CompletedTask;
                           },
                           OnValidatePrincipal = context =>
                           {
                               //Console.WriteLine("{0} - {1}: {2}", DateTime.Now, "OnValidatePrincipal", context.Principal.Identity.Name);
                               return Task.CompletedTask;
                           }
                       };
                       //options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                       options.LoginPath = new PathString("/Authorization/Index");
                       options.ReturnUrlParameter = "RequestPath";
                       options.SlidingExpiration = true;
                   });

            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ManagerPolicy", policy => policy.Requirements.Add(new RoleRequirement(new List<string>() { "Manager" })));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RawDataPolicy", policy => policy.Requirements.Add(new RoleRequirement(new List<string>() { "RawData" })));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SensorDefinitionPolicy", policy => policy.Requirements.Add(new RoleRequirement(new List<string>() { "SensorDefinition" })));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AssemblyPolicy", policy => policy.Requirements.Add(new RoleRequirement(new List<string>() { "Assembly" })));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MonitoringPolicy", policy => policy.Requirements.Add(new RoleRequirement(new List<string>() { "Monitoring" })));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("HistoryPolicy", policy => policy.Requirements.Add(new RoleRequirement(new List<string>() { "History" })));
            });

            services.AddSingleton<IAuthorizationHandler, RoleHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Default/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Authorization}/{action=Index}/{id?}");
            });
        }
    }
}
