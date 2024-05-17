using despesas_backend_api_net_core.CommonDependenceInject;
using Business.CommonDependenceInject;
using Repository.CommonDependenceInject;
using Microsoft.EntityFrameworkCore;
using CrossCutting.CommonDependenceInject;
using Repository;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning();
builder.Services.AddSwaggerApiVersioning();
                                                                                                                                                 
if (builder.Environment.IsProduction()) 
{
    builder.Services.CreateDataBaseInMemory();
}
else if (builder.Environment.EnvironmentName.Equals("Azure"))
{
    builder.Services.AddDbContext<RegisterContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AzureMsSqlConnectionString")));
}    
else if (builder.Environment.EnvironmentName.Equals("MySqlServer"))
{
    builder.Services.AddDbContext<RegisterContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("MySqlConnectionString")));
}
else if (builder.Environment.EnvironmentName.Equals("DatabaseInMemory"))
{
    builder.Services.CreateDataBaseInMemory();
}
else 
{
    builder.Services.AddDbContext<RegisterContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnectionString")));
}

// Add CommonInjectDependences 
builder.Services.AddDataSeeders();
builder.Services.AddAuthConfigurations(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddCrossCuttingConfiguration();
builder.Services.AddHyperMediaHATEOAS();

var app = builder.Build();

// Configure the HTTP request pipeline
app.AddSupporteCulturesPtBr();
app.AddSwaggerApiVersioning();
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute("DefaultApi", "{version=apiVersion}/{controller=values}/{id?}");
app.UseDefaultFiles();
app.UseStaticFiles();
app.RunDataSeeders();
app.Run();