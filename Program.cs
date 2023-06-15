using despesas_backend_api_net_core.Infrastructure.Data.Common;
using despesas_backend_api_net_core.Infrastructure.ExtensionMethods;
using despesas_backend_api_net_core.Infrastructure.Security.Configuration;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

// Add Cors Configuration 
builder.Services.AddCors(c =>
{
    c.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();

    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "API REST Despesas Pessoais",
            Version = "v1"
        });
});


/*Configuração Database Production*/
string MySqlConnectionString = "";
string filePath = "MYSQL_ConnectionString.txt";
if (File.Exists(filePath))
{
    MySqlConnectionString = File.ReadAllText(filePath);
    builder.Services.AddDbContext<RegisterContext>(options =>
    options.UseMySQL(MySqlConnectionString));
}

ConfigureAutorization(builder.Services, builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "API Despesas Pessoais V1");
});

app.UseSwaggerUI(c =>
{
    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "API Despesas Pessoais V1");
});
//}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseStaticFiles();


app.MapControllers();
app.Run();

static void ConfigureAutorization(IServiceCollection services, IConfiguration configuration)
{
    SigningConfigurations signingConfigurations = new SigningConfigurations();
    services.AddSingleton(signingConfigurations);

    TokenConfiguration tokenConfigurations = new TokenConfiguration();

    new ConfigureFromConfigurationOptions<TokenConfiguration>(
        configuration.GetSection("TokenConfigurations")
    )
    .Configure(tokenConfigurations);

    services.AddSingleton(tokenConfigurations);


    services.AddAuthentication(authOptions =>
    {
        authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(bearerOptions =>
    {
        Microsoft.IdentityModel.Tokens.TokenValidationParameters paramsValidation = bearerOptions.TokenValidationParameters;
        paramsValidation.IssuerSigningKey = signingConfigurations.Key;
        paramsValidation.ValidAudience = tokenConfigurations.Audience;
        paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

        // Validates the signing of a received token
        paramsValidation.ValidateIssuerSigningKey = true;

        // Checks if a received token is still valid
        paramsValidation.ValidateLifetime = true;

        // Tolerance time for the expiration of a token (used in case
        // of time synchronization problems between different
        // computers involved in the communication process)
        paramsValidation.ClockSkew = TimeSpan.Zero;
    });

    // Enables the use of the token as a means of
    // authorizing access to this project's resources
    services.AddAuthorization(auth =>
    {
        auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
            .RequireAuthenticatedUser().Build());
    });
}