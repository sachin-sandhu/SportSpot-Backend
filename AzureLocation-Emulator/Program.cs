using AzureLocation_Emulator.Factories;
using AzureLocation_Emulator.Services;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


builder.Services.AddSingleton<SearchModeService>();
builder.Services.AddSingleton<ReverseModeService>();
builder.Services.AddSingleton<IModeFactory, DefaultModeFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    foreach (Type? type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsEnum && x.IsPublic).ToList())
    {
        if (type is null) continue;
        options.MapType(type, () => new OpenApiSchema
        {
            Type = "string",
            Enum = Enum.GetNames(type).Select(x => new OpenApiString(x)).ToList<IOpenApiAny>()
        });
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();