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
using SuperPartner.Filters;
using SuperPartner.Model.Common;
using SuperPartner.Permission.TokenHandler;
using SuperPartner.Utils.Loggers;
using SuperPartner.Utils.Redis;

namespace SuperPartner
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
            var appSettings = this.Configuration.GetSection("AppSettings");
            var serviceCollection =  services.Configure<SpConfiguration>(appSettings);
            var settingModel = appSettings.Get<SpConfiguration>();

            services.Scan(scan => scan
                .FromAssemblyOf<ManagerBase>()
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Manager")))
                .AsSelf()
                .WithTransientLifetime());

            // Register component
            services.AddSingleton<CommonRedisHelper>(new CommonRedisHelper(settingModel.CommonRedis, settingModel.RedisPrefix));
            services.AddSingleton<PermissionRedisHelper>(new PermissionRedisHelper(settingModel.PermissionRedis, settingModel.RedisPrefix));

            // Token handler, The default is save token and associate data in memoery.
            services.AddSingleton<ITokenHandler>(new MemoryTokenHandler());
            // You can use redis as token storage
            //services.AddSingleton<ITokenHandler>(new RedisTokenHandler());

            // It will initial in SpAuthFilter, It simplify the reference of request object
            // It includes token, associate object of token, token handler instance etc.
            services.AddScoped<SpContext>();

            services.AddMvc(options=>
            {
                options.Filters.Add(new SpExceptionFilter());
                options.Filters.Add(new SpAuthFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
