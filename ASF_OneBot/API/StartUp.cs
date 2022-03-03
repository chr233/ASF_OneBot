using ASF_OneBot.API.Integration;
using ASF_OneBot.API.Middlewares;
using ASF_OneBot.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using static ASF_OneBot.Utils;

namespace ASF_OneBot.API
{
    internal sealed class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode", Justification = "PathString is a primitive, it's unlikely to be trimmed to the best of our knowledge")]
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(env);

            // The order of dependency injection is super important, doing things in wrong order will break everything
            // https://docs.microsoft.com/aspnet/core/fundamentals/middleware

            // This one is easy, it's always in the beginning
            if (Global.GlobalConfig.Debug)
            {
                app.UseDeveloperExceptionPage();
            }

            // Add support for proxies, this one comes usually after developer exception page, but could be before
            app.UseForwardedHeaders();


            // Add support for response caching - must be called before static files as we want to cache those as well
            //app.UseResponseCaching();


            // Add support for response compression - must be called before static files as we want to compress those as well
            app.UseResponseCompression();

            // It's not apparent when UsePathBase() should be called, but definitely before we get down to static files
            // TODO: Maybe eventually we can get rid of this, https://github.com/aspnet/AspNetCore/issues/5898
            PathString pathBase = Configuration.GetSection("Kestrel").GetValue<PathString>("PathBase");

            if (!string.IsNullOrEmpty(pathBase) && (pathBase != "/"))
            {
                app.UsePathBase(pathBase);
            }

            // The default HTML file (usually index.html) is responsible for IPC GUI routing, so re-execute all non-API calls on /
            // This must be called before default files, because we don't know the exact file name that will be used for index page
            app.UseWhen(static context => !context.Request.Path.StartsWithSegments("/Api", StringComparison.OrdinalIgnoreCase), static appBuilder => appBuilder.UseStatusCodePagesWithReExecute("/"));

            // Add support for default root path redirection (GET / -> GET /index.html), must come before static files
            app.UseDefaultFiles();


            // Add support for additional localization mappings
            //app.UseMiddleware<LocalizationMiddleware>();

            // Add support for localization
            app.UseRequestLocalization();

            // Use routing for our API controllers, this should be called once we're done with all the static files mess

            app.UseRouting();


            // We want to protect our API with IPCPassword and additional security, this should be called after routing, so the middleware won't have to deal with API endpoints that do not exist
            app.UseWhen(static context => context.Request.Path.StartsWithSegments("/Api", StringComparison.OrdinalIgnoreCase), static appBuilder => appBuilder.UseMiddleware<ApiAuthenticationMiddleware>());

            string ipcPassword = Global.GlobalConfig.AccessToken ?? "";

            if (!string.IsNullOrEmpty(ipcPassword))
            {
                // We want to apply CORS policy in order to allow userscripts and other third-party integrations to communicate with ASF API, this should be called before response compression, but can't be due to how our flow works
                // We apply CORS policy only with IPCPassword set as an extra authentication measure
                app.UseCors();
            }

            // Add support for websockets that we use e.g. in /Api/NLog
            app.UseWebSockets();

            // Finally register proper API endpoints once we're done with routing

            app.UseEndpoints(static endpoints => endpoints.MapControllers());


            // Add support for swagger, responsible for automatic API documentation generation, this should be on the end, once we're done with API
            app.UseSwagger();

            // Add support for swagger UI, this should be after swagger, obviously
            app.UseSwaggerUI(
                static options => {
                    options.DisplayRequestDuration();
                    options.EnableDeepLinking();
                    options.ShowExtensions();
                    options.SwaggerEndpoint($"OneBot/swagger.json", $"{nameof(ASF_OneBot)} API");
                }
            );
        }

        [UnconditionalSuppressMessage("AssemblyLoadTrimming", "IL2026:RequiresUnreferencedCode", Justification = "HashSet<string> isn't a primitive, but we widely use the required features everywhere and it's unlikely to be trimmed to the best of our knowledge")]
        public void ConfigureServices(IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            // The order of dependency injection is super important, doing things in wrong order will break everything
            // Order in Configure() method is a good start

            // Add support for response caching
            //services.AddResponseCaching();


            // Add support for response compression
            services.AddResponseCompression();

            string ipcPassword = Global.GlobalConfig != null ? Global.GlobalConfig.AccessToken : "";



            if (!string.IsNullOrEmpty(ipcPassword))
            {
                // We want to apply CORS policy in order to allow userscripts and other third-party integrations to communicate with ASF API
                // We apply CORS policy only with IPCPassword set as an extra authentication measure
                services.AddCors(static options => options.AddDefaultPolicy(static policyBuilder => policyBuilder.AllowAnyOrigin()));
            }

            // Add support for swagger, responsible for automatic API documentation generation
            services.AddSwaggerGen(
                static options => {
                    options.AddSecurityDefinition(
                        nameof(Config.AccessToken), new OpenApiSecurityScheme {
                            Description = $"{nameof(Config.AccessToken)} authentication using request headers.",
                            In = ParameterLocation.Header,
                            Name = ApiAuthenticationMiddleware.HeadersField,
                            Type = SecuritySchemeType.ApiKey
                        }
                    );

                    options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement {
                        {
                            new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Id = nameof(Config.AccessToken),
                                    Type = ReferenceType.SecurityScheme
                                }
                            },

                            Array.Empty<string>()
                        }
                        }
                    );

                    options.CustomSchemaIds(static type => type.GetUnifiedName());
                    options.EnableAnnotations(true, true);

                    options.SchemaFilter<CustomAttributesSchemaFilter>();
                    options.SchemaFilter<EnumSchemaFilter>();

                    options.SwaggerDoc(
                        nameof(ASF_OneBot), new OpenApiInfo {
                            //Contact = new OpenApiContact {
                            //    Name = "233",
                            //    Url = new Uri("https://baidu.com")
                            //},

                            //License = new OpenApiLicense {
                            //    Name = "AGPL v3",
                            //    Url = new Uri("https://baidu.com")
                            //},

                            Title = $"{nameof(ASF_OneBot)} API",
                            Version = Global.Version.ToString()
                        }
                    );

                    //string xmlDocumentationFile = Path.Combine(AppContext.BaseDirectory, "ASF_OneBot.xml");

                    //if (File.Exists(xmlDocumentationFile))
                    //{
                    //    options.IncludeXmlComments(xmlDocumentationFile);
                    //}
                }
            );

            // Add support for Newtonsoft.Json in swagger, this one must be executed after AddSwaggerGen()
            //services.AddSwaggerGenNewtonsoftSupport();

            // We need MVC for /Api, but we're going to use only a small subset of all available features
            IMvcBuilder mvc = services.AddControllers();

            // Add support for controllers declared in custom plugins

            mvc.AddControllersAsServices();


            mvc.AddNewtonsoftJson(
                static options => {
                    // Fix default contract resolver to use original names and not a camel case
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                    if (Global.GlobalConfig.Debug)
                    {
                        options.SerializerSettings.Formatting = Formatting.Indented;
                    }


                }
            );
        }
    }
}
