using AutoMapper;
using LamMocBao.Mappers;
using LamMocBaoWeb.Authentication;
using LamMocBaoWeb.Middleware;
using LamMocBaoWeb.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Services.Caching;
using Services.DbContexts;
using Services.ModelLoaders;
using Services.Services;
using Services.Services.Interfaces;
using Shared.Models.Identify;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace LamMocBao
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<Services.Services1.LMBBackgroundHost>();
            ConfigureDI(services);
            AddAutoMapper(services);
            AddAzureConfig(services);
            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            AddAuthentication(services);
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<InMemoryCache>();
            AddDbContext(services);
            services.AddAntiforgery(options => {
                options.SuppressXFrameOptionsHeader = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
            });
        }

        private void AddDbContext(IServiceCollection services)
        {
            var serviceConfig = services.BuildServiceProvider().GetService<IServiceConfig>();
            if (serviceConfig.IsTestMode)
            {
                services.AddScoped<IDbContext, SqlLiteContext>();
                services.AddScoped<DbContextOptionsBuilder, DbContextOptionsBuilder>();
                services.AddEntityFrameworkSqlite().AddDbContext<SqlLiteContext>();

                using (var client = services.BuildServiceProvider().GetService<IDbContext>() as SqlLiteContext)
                {
                    client.Database.EnsureCreated();
                    client.Database.Migrate();
                    EnsureIndexSqlite(serviceConfig, client.Database);
                }
            }
            else
            {
                services.AddScoped<IDbContext, ApplicationDbContext>();
                Action<DbContextOptionsBuilder> buildConnection = optionsBuilder =>
                {
                    optionsBuilder.UseSqlServer(serviceConfig.ConnectionString, d => d.MigrationsAssembly("Services"));
                };
                services.AddEntityFrameworkSqlServer().AddDbContext<DbContext>(optionsBuilder =>
                {
                    buildConnection(optionsBuilder);
                });


                using (var client = services.BuildServiceProvider().GetService<IDbContext>() as ApplicationDbContext)
                {
                    client.Database.EnsureCreated();
                    client.Database.Migrate();
                    EnsureIndexSql(serviceConfig, client.Database);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            RegisterRoutes(app);

            app.UseMiddleware<GrantSystemSettingMiddleware>();
            app.UseMiddleware<SetCultureMiddleware>();
            app.UseMiddleware<SetCommonSettingMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine($"{Environment.WebRootPath}", "js")),
                RequestPath = "/js"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine($"{Environment.WebRootPath}", "lib")),
                RequestPath = "/lib"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine($"{Environment.WebRootPath}", "css")),
                RequestPath = "/css"
            });
            app.UseAuthorization();
            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized || response.StatusCode == (int)HttpStatusCode.Forbidden)
                    response.Redirect("/Auth/LogOn");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };
            app.UseCookiePolicy(cookiePolicyOptions);
            

        }

        private void RegisterRoutes(IApplicationBuilder app)
        {;
            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/error/not-found";
                    await next();
                }
            });
            app.UseRouting();
        }

        private void ConfigureDI(IServiceCollection services)
        {
            RegisterServices(services);
            services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();

            services.AddScoped<ICachingLoader, ModelCachingLoader>();
            services.AddScoped<ICachingLoader, IdentityCachingLoader>();

            services.AddScoped<IModelLoader, CustomerDesiringLoader>();
            services.AddScoped<IModelLoader, ProductLoader>();
            services.AddScoped<IModelLoader, ProductTypeLoader>();
            services.AddScoped<IModelLoader, ProductImageLoader>();
            services.AddScoped<IModelLoader, OrderLoader>();
            services.AddScoped<IModelLoader, CustomerCommentPostLoader>();
            services.AddScoped<IModelLoader, NewsPaperPostPostLoader>();
            services.AddScoped<IModelLoader, KnowledgePostLoader>();
            services.AddScoped<IModelLoader, DefaultLoader>();

            //services.AddScoped<IFileServices, AzureFileServices>();
            services.AddScoped<IFileServices, LocallyFileServices>();
            services.AddTransient<IPingService, PingService>(d =>
            {
                var memoryCache = d.GetService<InMemoryCache>(); 
                var config = d.GetService<IServiceConfig>();
                IDbContext dbContext = config.IsTestMode ?
                            new SqlLiteContext(memoryCache, config) :
                            new ApplicationDbContext(memoryCache, config);
                return new PingService(dbContext, config);
            });

            services.AddTransient<IAnalysisService>(d =>
            {
                var memoryCache = d.GetService<InMemoryCache>();
                var config = d.GetService<IServiceConfig>();
                IDbContext dbContext = config.IsTestMode ? 
                            new SqlLiteContext(memoryCache, config) :
                            new ApplicationDbContext(memoryCache, config);
                return new AnalysisService(dbContext, memoryCache, config);
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            RegisterComponentsByPattern(services, "Service");

            services.AddTransient<ISystemSettingService, SystemSettingService>();
        }

        private void RegisterComponentsByPattern(IServiceCollection services,
            string pattern)
        {
            var baseType = typeof(Service<>);
            var type = typeof(IService<>);
            var classTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(p => p.GetTypes())
                .Where(t => t.Namespace == type.Namespace && t.IsClass && !t.IsGenericType && t.Name.EndsWith(pattern))
                .ToList();

            foreach (var classType in classTypes)
            {
                var interfaceTypes = classType.GetInterfaces().Where(d => d != type).ToList();
                foreach (var interfaceType in interfaceTypes)
                {
                    services.AddScoped(interfaceType, classType);
                }
            }
        }
        private void AddAutoMapper(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private void AddAzureConfig(IServiceCollection services)
        {
            //Func<IServiceProvider, ServiceConfig> serviceConfigFunc = x => x.GetRequiredService<IOptionsSnapshot<ServiceConfig>>().Value;
            Func<IServiceProvider, ServiceConfig> serviceConfigFunc = x => Configuration.Get<ServiceConfig>();
            services.AddSingleton<IServiceConfig>(serviceConfigFunc);
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
                .AddDefaultTokenProviders()
                .RegisterDBStores();
            IdentityModelEventSource.ShowPII = true;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration["LMB:JwtIssuer"],
                ValidateAudience = true,
                ValidAudience = Configuration["LMB:JwtIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["LMB:SecurityKey"])),
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // remove delay of token when expire
            };
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = tokenValidationParameters;
                });
        }

        private void EnsureIndexSql(IServiceConfig _config, DatabaseFacade databaseFacade)
        {
            foreach (var groupIndex in _config.Indexes)
            {
                // Collection: groupIndex.Key, Field: groupIndex.Value
                var sqls = groupIndex.Value.Select(field => $@"IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = '{groupIndex.Key}_{field.Key}' AND object_id = OBJECT_ID('dbo.{groupIndex.Key}'))
                    BEGIN
                        CREATE INDEX {groupIndex.Key}_{field.Key} ON {groupIndex.Key} ({field.Key})
                    END").ToList();
                foreach (var sql in sqls)
                {
                    databaseFacade.ExecuteSqlRaw(sql);
                }
            }
        }

        private void EnsureIndexSqlite(IServiceConfig _config, DatabaseFacade databaseFacade)
        {
            foreach (var groupIndex in _config.Indexes)
            {
                // Collection: groupIndex.Key, Field: groupIndex.Value
                var sqls = groupIndex.Value.Select(field => $@"CREATE INDEX IF NOT EXISTS {groupIndex.Key}_{field.Key} ON {groupIndex.Key} ({field.Key} ASC)").ToList();
                foreach (var sql in sqls)
                {
                    databaseFacade.ExecuteSqlRaw(sql);
                }
            }
        }
    }
}
