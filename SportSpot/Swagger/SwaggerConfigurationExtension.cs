using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.OpenApi.Models;

namespace SportSpot.Swagger
{
    public static class SwaggerConfigurationExtension
    {
        public static void ConfigureFullSwaggerConfig(this IServiceCollection col)
        {
            col.AddApiVersioning(
                options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1.0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(
                           new UrlSegmentApiVersionReader(),
                           new QueryStringApiVersionReader("api-version"),
                           new HeaderApiVersionReader("X-Version"),
                           new MediaTypeApiVersionReader("x-version"));
                })
            .AddMvc(options => options.Conventions.Add(new VersionByNamespaceConvention()))
            .AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            col.AddEndpointsApiExplorer();

            col.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
               {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        []
                    }
               });
            });
        }
    }
}
