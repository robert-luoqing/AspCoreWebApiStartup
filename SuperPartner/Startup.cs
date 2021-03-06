﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SuperPartner.Biz.Common;
using SuperPartner.DataLayer.DataContext;
using SuperPartner.Filters;
using SuperPartner.Model.Common;
using SuperPartner.Permission.TokenHandler;
using SuperPartner.Utils.Loggers;
using SuperPartner.Utils.Redis;
using Microsoft.EntityFrameworkCore;
using SuperPartner.DataLayer.Common;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using SuperPartner.Permission.DataContext;
using Microsoft.Extensions.Logging.Console;
using SuperPartner.Permission.Authorization;
using Microsoft.Extensions.Hosting;

namespace SuperPartner
{
    /// <summary>
    /// Startup entry
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// Console logger provider.
        /// </summary>
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddFilter("SampleApp.Program", LogLevel.Debug)
                       .AddConsole();
            });

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Startup constructor
        /// </summary>
        /// <param name="configuration">Configuation</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = this.Configuration.GetSection("AppSettings");
            var settingModel = appSettings.Get<SpConfiguration>();
            services.AddSingleton(settingModel);

            // Auto inject business classes
            services.Scan(scan => scan
                .FromAssemblyOf<ManagerBase>()
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Manager")))
                .AsSelf()
                .WithTransientLifetime());

            // Auto inject dao classes
            services.Scan(scan => scan
                .FromAssemblyOf<DaoBase>()
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Dao")))
                .AsSelf()
                .WithTransientLifetime());

            // Register component
            services.AddSingleton<CommonRedisHelper>(new CommonRedisHelper(settingModel.CommonRedis, settingModel.RedisPrefix));

            // Token handler, The default is save token and associate data in memoery.
            services.AddSingleton<ITokenHandler>(new MemoryTokenHandler());
            // You can use redis as token storage
            // services.AddSingleton<ITokenHandler>(new RedisTokenHandler(new PermissionRedisHelper(settingModel.PermissionRedis, settingModel.RedisPrefix), "token"));

            // It will initial in SpAuthFilter, It simplify the reference of request object
            // It includes token, associate object of token, token handler instance etc.
            services.AddScoped<BizContext>();
            services.AddScoped<DaoContext>();

            services.AddDbContext<PermissionDataContext>
                 (options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SpFrameworkDatabase"), b => b.MigrationsAssembly("SuperPartner"))
                        .UseLoggerFactory(MyLoggerFactory)
                );
            services.AddDbContext<SpFrameworkContext>
                (options =>
                       options.UseSqlServer(Configuration.GetConnectionString("SpFrameworkDatabase"))
                        .UseLoggerFactory(MyLoggerFactory)
                );

            // Permission implement
            // The inject will be use for UI
            services.AddScoped(typeof(IAuthorizationStorageProvider), typeof(AuthorizationStorageProvider));
            var optionsBuilder = new DbContextOptionsBuilder<PermissionDataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SpFrameworkDatabase"));
            var dataContext = new PermissionDataContext(optionsBuilder.Options);
            var authorizationHandler = new AuthorizationHandler(new AuthorizationStorageProvider(dataContext));
            // User cache to save performance. But the ClearCache must invoke if any change in AuthorizationStorageProvider
            authorizationHandler.UseCache = true;
            // It should be define as singleton
            services.AddSingleton(typeof(IAuthorizationHandler), authorizationHandler);

            services.AddControllers(options =>
            {
                options.Filters.Add(new SpExceptionFilter());
                options.Filters.Add(new SpAuthFilter());
            });

            // swagger configuration
            SwaggerSetting.Confige(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Logger.ConfigurationLog4Net();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
