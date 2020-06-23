using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ContentNetworkSystem.Data;
using ContentNetworkSystem.Push;
using ContentNetworkSystem.Pull;
using ContentNetworkSystem.Models;
using ContentNetworkSystem.Data.GoogleSearchCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using System.IdentityModel.Tokens.Jwt;
using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.Diagnostics.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContentNetworkSystem
{
    public class Startup
    { 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = "dymekapps";
                options.ServiceName = "ContentNetworkSystem";
                options.Version = "0.01";
                //  options.Options = errorOptions; 
            });

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = Configuration.GetValue<string>("AuthorityServer");
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "CNS";
                    options.ClientSecret = "7FAEB0A9-301F-4425-ADFE-85062F8D419F";
                    options.ResponseType = "code";

                    options.SaveTokens = true;

                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("offline_access");
                    options.Scope.Add("profile");
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";

                })
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("AuthorityServer");
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters.RoleClaimType = "client_role";

                    options.Audience = "apiContentNetworkSystem";
                });

            services.AddDbContext<ContentNetworkSystemContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection"))); 

            services.AddRazorPages().AddNewtonsoftJson();
            services.AddControllers().AddNewtonsoftJson(options =>
                  {
                      options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                      options.SerializerSettings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                      options.SerializerSettings.SerializationBinder = new MyCustomSerializationBinder();
                   });
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>(); 
            services.AddHttpClient();

            services.AddTransient<EncryptionService>();

            services.AddTransient<IContentsService, ContentsService>();
            services.AddTransient<IGroupsService, GroupsService>();
            services.AddTransient<IProjectsService, ProjectsService>();
            services.AddTransient<INichesService, NichesService>();
            services.AddTransient<IKeywordsService, KeywordsService>();
            services.AddTransient<IYoutubeResultsService, YoutubeResultsService>();
            services.AddTransient<IImagesResultsService, ImagesResultsService>(); 
            services.AddTransient<SchedulerService>();

            //push
            services.AddTransient<WordpressService>();

            //pull
            services.AddTransient<TextGenerationService>();
            services.AddTransient<YouTubeService>();
            services.AddTransient<GoogleImagesService>();
            services.AddTransient<RandomContentService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
            });

            // Configure logging service.
            LoggerOptions loggerOptions = LoggerOptions.Create(logName: "ContentNetworkSystem");

            loggerFactory.AddGoogle(app.ApplicationServices, "dymekapps", loggerOptions);

            app.UseGoogleExceptionLogging();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
