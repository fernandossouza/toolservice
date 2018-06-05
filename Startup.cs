using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using securityfilter.Services;
using securityfilter.Services.Interfaces;
using toolservice.Data;
using toolservice.Service;
using toolservice.Service.Interface;

namespace toolservice {
    public class Startup {

        public Startup (IConfiguration configuration) {
            this.Configuration = configuration;

        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddCors (o => o.AddPolicy ("CorsPolicy", builder => {
                builder.AllowAnyOrigin ()
                    .AllowAnyMethod ()
                    .AllowAnyHeader ();
            }));

            services.AddSingleton<IConfiguration> (Configuration);
            services.AddTransient<IEncryptService, EncryptService> ();

            if (!String.IsNullOrEmpty (Configuration["KeyFolder"]))
                services.AddDataProtection ()
                .SetApplicationName ("Lorien")
                .PersistKeysToFileSystem (new DirectoryInfo (Configuration["KeyFolder"]));

            services.AddSingleton<IThingService, ThingService> ();
            services.AddSingleton<IThingGroupService, ThingGroupService> ();
            services.AddTransient<IToolTypeService, ToolTypeService> ();
            services.AddTransient<IToolService, ToolService> ();
            services.AddTransient<IToolInformationService, ToolInformationService> ();
            services.AddTransient<IStateTransitionHistoryService, StateTransitionHistoryService> ();
            services.AddDbContext<ApplicationDbContext> (options =>
                options.UseNpgsql (Configuration.GetConnectionString ("ToolDB")));
            services.AddTransient<IStateManagementService, StateManagementService> ();
            services.AddTransient<IStateManagementService, StateManagementService> ();
            services.AddTransient<IAssociateToolService, AssociateToolService> ();
            services.AddResponseCaching ();
            services.AddMvc ((options) => {
                options.CacheProfiles.Add ("toolcache", new CacheProfile () {
                    Duration = Convert.ToInt32 (Configuration["CacheDuration"]),
                        Location = ResponseCacheLocation.Any
                });
            }).AddJsonOptions (options => {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            app.UseResponseCaching ();
            app.UseCors ("CorsPolicy");
            app.UseForwardedHeaders (new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseMvc ();

        }
    }
}