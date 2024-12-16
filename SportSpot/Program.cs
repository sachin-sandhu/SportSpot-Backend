using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.IdentityModel.Tokens;
using SportSpot.Events.Extensions;
using SportSpot.Events.Services;
using SportSpot.ExceptionHandling;
using SportSpot.Swagger;
using SportSpot.V1.Extensions;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Services;
using SportSpot.V1.Media.BlurHash;
using SportSpot.V1.Media.Repositories;
using SportSpot.V1.Media.Services;
using SportSpot.V1.Request;
using SportSpot.V1.Session.Chat.Repositories;
using SportSpot.V1.Session.Chat.Services;
using SportSpot.V1.Session.Repositories;
using SportSpot.V1.Session.Services;
using SportSpot.V1.Storage;
using SportSpot.V1.User.Context;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Dtos.Auth.OAuth;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.OAuth;
using SportSpot.V1.User.Services;
using SportSpot.V1.WebSockets.Middleware;
using SportSpot.V1.WebSockets.Services;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.ConfigureFullSwaggerConfig();

string mongoDbConnection = builder.Configuration.GetValue<string>("MongoDBConnection") ?? throw new InvalidOperationException("MongoDBConnection is not set!");
string mongoDbDatabase = builder.Configuration.GetValue<string>("MongoDBDatabase") ?? throw new InvalidOperationException("MongoDBDatabase is not set!");

string sqlConnection = builder.Configuration.GetValue<string>("MariaDBConnection") ?? throw new InvalidOperationException("MariaDBConnection is not set!");
ServerVersion sqlVersion = ServerVersion.Create(new Version(10, 5, 4), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb);

if (builder.Configuration.GetValue("MariaDBCheckSchema", true))
{
    DbContextOptionsBuilder<AuthContext> contextBuilder = new();
    contextBuilder.UseMySql(sqlConnection, sqlVersion);
    using AuthContext dbContext = new(contextBuilder.Options);
    await dbContext.Database.EnsureCreatedAsync();
    await dbContext.Database.MigrateAsync();
}

await builder.Services.AddMongoDb(mongoDbConnection, mongoDbDatabase);

builder.Services.AddDbContextFactory<AuthContext>(options => options.UseMySql(sqlConnection, sqlVersion));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    string configuration = builder.Configuration.GetValue<string>("Location_Cache_DB_Connection") ?? throw new InvalidOperationException("Location_Cache_DB_Connection is not set!");
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSingleton<IDistributedCache>(sp =>
{
    IConnectionMultiplexer multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    var options = new RedisCacheOptions
    {
        ConnectionMultiplexerFactory = () => Task.FromResult(multiplexer),
        InstanceName = builder.Configuration.GetValue<string>("Location_Cache_DB_Name") ?? throw new InvalidOperationException("Location_Cache_DB_Name is not set!")
    };
    return new RedisCache(options);
});

builder.Services.AddIdentity<AuthUserEntity, AuthRoleEntity>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AuthContext>().AddDefaultTokenProviders();

JwtConfigurationDto jwtConfiguration = new()
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

builder.Services.AddSingleton(new OAuthConfigurationDto
{
    GoogleUserInformationEndpoint = builder.Configuration.GetValue<string>("OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT") ?? throw new InvalidOperationException("OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT is not set!")
});

builder.Services.AddSingleton(new LocationConfigDto
{
    AzureMapsReverseLocationEndpoint = builder.Configuration.GetValue<string>("AZURE_MAPS_REVERSE_LOCATION_ENDPOINT") ?? throw new InvalidOperationException("AZURE_MAPS_REVERSE_LOCATION_ENDPOINT is not set!"),
    AzureMapsSearchEndpoint = builder.Configuration.GetValue<string>("AZURE_MAPS_SEARCH_ENDPOINT") ?? throw new InvalidOperationException("AZURE_MAPS_SEARCH_ENDPOINT is not set!"),
    AzureMapsSubscriptionKey = builder.Configuration.GetValue<string>("AZURE_MAPS_SUBSCRIPTION_KEY") ?? throw new InvalidOperationException("AZURE_MAPS_SUBSCRIPTION_KEY is not set!")
});

TokenValidationParameters tokenValidationParameters = new()
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidAudience = jwtConfiguration.ValidAudience,
    ValidIssuer = jwtConfiguration.ValidIsUser,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret)),
    NameClaimType = JwtRegisteredClaimNames.Sub,
    RoleClaimType = ClaimTypes.Role
};

builder.Services.AddHttpClient();
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<IRequest, Request>();

builder.Services.AddTransient<ILocationCacheService, LocationCacheService>();
builder.Services.AddTransient<ILocationService, LocationService>();

builder.Services.AddTransient<IOAuthFactory, DefaultOAuthFactory>();
builder.Services.AddTransient<IBlobClient, AzureStorageClient>();
builder.Services.AddTransient<IBlurHashFactory, DefaultBlurHashFactory>();
builder.Services.AddTransient<IMediaRepository, MediaRepository>();
builder.Services.AddTransient<IMediaService, MediaService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ISessionRepository, SessionRepository>();
builder.Services.AddTransient<ISessionService, SessionService>();

builder.Services.AddSingleton<IConnectionService, ConnectionService>();
builder.Services.AddScoped<IWebSocketService, WebSocketService>();

builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.RegisterEvents();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = tokenValidationParameters;
});

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        int httpsPort = builder.Configuration.GetValue("ASPNETCORE_HTTPS_PORTS", 8080);
        options.ListenAnyIP(httpsPort, listenOptions =>
        {
            string certFileName = builder.Configuration.GetValue<string>("CERT_FILE") ?? throw new InvalidOperationException("CERT_FILE is not set!");
            string certKeyFileName = builder.Configuration.GetValue<string>("CERT_KEY_FILE") ?? throw new InvalidOperationException("CERT_KEY_FILE is not set!");
            X509Certificate2 certificate = X509Certificate2.CreateFromPemFile(certFileName, certKeyFileName);
            listenOptions.UseHttps(certificate);
        });
    });
}

var app = builder.Build();

app.Services.RegisterEvents();

if (app.Environment.IsDevelopment())
{
    app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// WebSockets
WebSocketOptions webSocketOptions = new()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
};

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseWebSockets(webSocketOptions);
app.UseMiddleware<CustomWebSocketMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.RunAsync();
