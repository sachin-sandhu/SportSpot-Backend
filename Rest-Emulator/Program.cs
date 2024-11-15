using Rest_Emulator.Factories;
using Rest_Emulator.Services;
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
builder.Services.AddSingleton<GoogleOAuthModeService>();
builder.Services.AddSingleton<IModeFactory, DefaultModeFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.Run();