using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SuperPartner
{
    /// <summary>
    /// Swageer setting
    /// </summary>
    public static class SwaggerSetting
    {
        /// <summary>
        /// Config
        /// </summary>
        /// <param name="services">Services</param>
        public static void Confige(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            // Detail see https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Super Partner Api",
                    Description = "",
                    Version = "v1",
                    Contact = new OpenApiContact()
                    {
                        Name = "Robert",
                        Email = "robert_luoqing@hotmail.com",
                        Url = new Uri("https://github.com/robert-luoqing"),
                    },
                });
                c.AddSecurityDefinition("token",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter token into the field",
                        Name = "token",
                        Type = SecuritySchemeType.ApiKey
                    });
                //c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                //    { "token", Enumerable.Empty<string>() },
                //});
                var tokenScheme = new OpenApiSecurityScheme
                {
                    Description = "Token Authorization",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                };

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                     { tokenScheme, new List<string>()}
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SuperPartner.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SuperPartner.Model.xml"));
            });
        }
    }
}
