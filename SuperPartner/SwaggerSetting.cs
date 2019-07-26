using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SuperPartner
{
    public static class SwaggerSetting
    {
        public static void Confige(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            // Detail see https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Super Partner Api",
                    Description = "",
                    Version = "v1",
                    Contact = new Contact()
                    {
                        Name = "Robert",
                        Email = "robert_luoqing@hotmail.com",
                        Url = "https://github.com/robert-luoqing",
                    },
                });
                c.AddSecurityDefinition("token",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter token into the field",
                        Name = "token",
                        Type = "apiKey"
                    });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "token", Enumerable.Empty<string>() },
                });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SuperPartner.xml"));
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SuperPartner.Model.xml"));
            });
        }
    }
}
