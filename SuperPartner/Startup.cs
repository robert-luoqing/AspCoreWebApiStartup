using System;
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

namespace SuperPartner
{
    public class Startup
    {
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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

            // Permission implement
            services.AddDbContext<PermissionDataContext>
                 (options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SpFrameworkDatabase"), b => b.MigrationsAssembly("SuperPartner"))
                        .UseLoggerFactory(MyLoggerFactory)
                );
            services.AddScoped(typeof(IAuthorizationStorageProvider), typeof(AuthorizationStorageProvider));
            services.AddScoped(typeof(IAuthorizationHandler), typeof(AuthorizationHandler));


            services.AddDbContext<SpFrameworkContext>
                (options =>
                       options.UseSqlServer(Configuration.GetConnectionString("SpFrameworkDatabase"))
                        .UseLoggerFactory(MyLoggerFactory)
                );

            services.AddMvc(options =>
            {
                options.Filters.Add(new SpExceptionFilter());
                options.Filters.Add(new SpAuthFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // swagger configuration
            SwaggerSetting.Confige(services);
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
