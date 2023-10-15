/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MtdKey.OrderMaker.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtdKey.OrderMaker.AppConfig;
using MtdKey.OrderMaker.Services;
using MtdKey.OrderMaker.Areas.Identity.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using MtdKey.Cipher;
using MtdKey.OrderMaker.Core;

namespace MtdKey.OrderMaker
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Domain = Configuration.GetConnectionString("Domain");
                options.Cookie.Name = $".OrderMaker.{Configuration.GetConnectionString("ClientName")}";
            });

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("OrderMakerIdentity"), new MySqlServerVersion(new Version(8, 0))));

            services.AddDbContext<OrderMakerContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("OrderMakerData"), new MySqlServerVersion(new Version(8, 0))));
            services.AddHostedService<MigrationService>();

            services.AddDataProtection()
            .SetApplicationName($"{Configuration.GetConnectionString("ClientName")}")
            .PersistKeysToDbContext<IdentityDbContext>();

            services.AddDefaultIdentity<WebAppUser>(config =>
            {
                config.SignIn.RequireConfirmedEmail = false;
                config.User.RequireUniqueEmail = true;

            }).AddRoles<WebAppRole>()
             .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            Program.TemplateConnectionStaring = Configuration.GetConnectionString("Template");

            services.AddMemoryCache();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RoleAdmin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RoleUser", policy => policy.RequireRole("User", "Admin"));
                options.AddPolicy("RoleGuest", policy => policy.RequireRole("Guest", "User", "Admin"));
            });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                    })
                .AddRazorPagesOptions(options =>
                    {
                        options.Conventions.AuthorizeFolder("/");
                        options.Conventions.AuthorizeAreaFolder("Workplace", "/", "RoleUser");
                        options.Conventions.AuthorizeAreaFolder("Identity", "/Users", "RoleAdmin");
                        options.Conventions.AuthorizeAreaFolder("Config", "/", "RoleAdmin");
                    });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddScoped<UserHandler>();
            services.AddTransient<ConfigHandler>();
            services.AddTransient<IEmailSenderBlank, EmailSender>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<ConfigSettings>(Configuration.GetSection("ConfigSettings"));
            services.Configure<LimitSettings>(Configuration.GetSection("LimitSettings"));

            services.AddAesMangerService(options =>
            {
                options.SecretKey = Configuration.GetValue<string>("AesOptions:SecretKey");
                options.KeySize = Configuration.GetValue<int>("AesOptions:KeySize");
            });


            services.AddCoreServices();

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddScoped<DataConnector>();
            services.AddMvc(options => options.EnableEndpointRouting = false);


#if DEBUG
            services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

            //services.Configure<RequestLocalizationOptions>(options =>
            //{
            //    var supportedCultures = new[]
            //    {
            //        new CultureInfo("en-US"),
            //        new CultureInfo("ru-RU"),
            //    };

            //    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
            //    options.SupportedCultures = supportedCultures;
            //    options.SupportedUICultures = supportedCultures;
            //    options.RequestCultureProviders = new[] { new CookieRequestCultureProvider() };
            //});            
        }

        public void Configure(IApplicationBuilder app)
        {
            //var config = serviceProvider.GetRequiredService<IOptions<ConfigSettings>>();
            //var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            //var cultureInfo = new CultureInfo(config.Value.CultureInfo);
            //locOptions.Value.DefaultRequestCulture = new RequestCulture(cultureInfo);
            //app.UseRequestLocalization(locOptions.Value);

            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }           
            
            app.UseStaticFiles();
            
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseUsersMiddleware();
            app.UseLocalizerMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.UseMvc();

        }
    }
}
