using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using SportSpot.ExceptionHandling;
using SportSpot.Swagger;
using SportSpot.V1.User;
using SportSpot.V1.User.Context;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.ConfigureFullSwaggerConfig();

string mongoDbConnection = builder.Configuration.GetValue<string>("MongoDBConnection") ?? throw new InvalidOperationException("MongoDBConnection is not set!");
string mongoDbDatabase = builder.Configuration.GetValue<string>("MongoDBDatabase") ?? throw new InvalidOperationException("MongoDBDatabase is not set!");

string sqlConnection = builder.Configuration.GetValue<string>("MariaDBConnection") ?? throw new InvalidOperationException("SQLDBConnection is not set!");
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

builder.Services.AddIdentity<AuthUserEntity, AuthRoleEntity>()
    .AddEntityFrameworkStores<AuthContext>().AddDefaultTokenProviders();

JwtConfiguration jwtConfiguration = new()
{
    Secret = builder.Configuration.GetValue<string>("JWT_Secret") ?? throw new InvalidOperationException("JWT_Secret is not set!"),
    ValidAudience = builder.Configuration.GetValue<string>("JWT_ValidAudience") ?? throw new InvalidOperationException("JWT_ValidAudience is not set!"),
    ValidIsUser = builder.Configuration.GetValue<string>("JWT_ValidIsUser") ?? throw new InvalidOperationException("JWT_ValidIsUser is not set!"),
    Duration = TimeSpan.FromMinutes(builder.Configuration.GetValue("JWT_Duration", 5))
};

builder.Services.AddSingleton(jwtConfiguration);

builder.Services.AddSingleton<IEventService, EventService>();

builder.Services.AddTransient<IClubRepository, ClubRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddTransient<IClubService, ClubService>();
builder.Services.AddTransient<IUserService, UserService>();

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
