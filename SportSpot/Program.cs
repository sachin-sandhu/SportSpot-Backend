using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SportSpot.ExceptionHandling;
using SportSpot.Swagger;
using SportSpot.V1.Context;
using SportSpot.V1.Storage;
using SportSpot.V1.User;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.ConfigureFullSwaggerConfig();

string mongoDbConnection = builder.Configuration.GetValue<string>("MongoDBConnection") ?? throw new InvalidOperationException("MongoDBConnection is not set!");
string mongoDbDatabase = builder.Configuration.GetValue<string>("MongoDBDatabase") ?? throw new InvalidOperationException("MongoDBDatabase is not set!");

string sqlConnection = builder.Configuration.GetValue<string>("MariaDBConnection") ?? throw new InvalidOperationException("MariaDBConnection is not set!");
ServerVersion sqlVersion = ServerVersion.Create(new Version(10, 5, 4), ServerType.MariaDb);

if (builder.Configuration.GetValue("MariaDBCheckSchema", true))
{
    DbContextOptionsBuilder<AuthContext> contextBuilder = new();
    contextBuilder.UseMySql(sqlConnection, sqlVersion);
    using AuthContext dbContext = new(contextBuilder.Options);
    await dbContext.Database.EnsureCreatedAsync();
    await dbContext.Database.MigrateAsync();
}

builder.Services.AddDbContextFactory<DatabaseContext>(optionsBuilder => optionsBuilder.UseMongoDB(mongoDbConnection, mongoDbDatabase));
builder.Services.AddDbContextFactory<AuthContext>(options => options.UseMySql(sqlConnection, sqlVersion));

builder.Services.AddIdentity<AuthUserEntity, AuthRoleEntity>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AuthContext>().AddDefaultTokenProviders();

JwtConfiguration jwtConfiguration = new()
{
    Secret = builder.Configuration.GetValue<string>("JWT_Secret") ?? throw new InvalidOperationException("JWT_Secret is not set!"),
    ValidAudience = builder.Configuration.GetValue<string>("JWT_ValidAudience") ?? throw new InvalidOperationException("JWT_ValidAudience is not set!"),
    ValidIsUser = builder.Configuration.GetValue<string>("JWT_ValidIsUser") ?? throw new InvalidOperationException("JWT_ValidIsUser is not set!"),
    Duration = TimeSpan.FromMinutes(builder.Configuration.GetValue("JWT_Duration", 5))
};

builder.Services.AddSingleton(jwtConfiguration);

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration.GetValue<string>($"AZURE_BLOB_STORAGE") ?? throw new InvalidOperationException("AZURE_BLOB_STORAGE is not set!"));
});

builder.Services.AddSingleton(provider =>
{
    BlobServiceClient blobServiceClient = provider.GetRequiredService<BlobServiceClient>();
    string containerName = builder.Configuration.GetValue<string>("AZURE_BLOB_CONTAINER") ?? throw new InvalidOperationException("AZURE_BLOB_CONTAINER is not set!");
    BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
    blobContainerClient.CreateIfNotExists();
    return blobContainerClient;
});

builder.Services.AddSingleton<IEventService, EventService>();

builder.Services.AddTransient<IBlobClient, AzureStorageClient>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.RegisterEvents();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = jwtConfiguration.ValidAudience,
        ValidIssuer = jwtConfiguration.ValidIsUser,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret))
    };
});

var app = builder.Build();

app.Services.RegisterEvents();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.RunAsync();
