using System.Data;
using System.Diagnostics;
using System.Globalization;
using AwtadStudy.Database;
using AwtadStudy.FirebaseAdmin;
using AwtadStudy.Universities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

// set default CultureInfo to avoid bugs with string comparisons/parsing
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

NpgsqlDataSourceBuilder dataSourceBuilder = new(builder.Configuration.GetConnectionString("Database"));
dataSourceBuilder.MapEnum<University>();

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

// Register FirebaseService Admin SDK dependency as a singleton.
// A single instance of the service is created and shared across the entire application.
builder.Services.AddSingleton<FirebaseService>();

// Register the FirebaseAuth Service
builder.Services.AddScoped<IFirebaseAuth, FirebaseAuthService>();

builder.Services.AddScoped<IUniversityServiceFactory, UniversityServiceFactory>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    
    // docs say to reload types after migrating on startup
    if (db.Database.GetDbConnection() is NpgsqlConnection con)
    {
        switch (con.State)
        {
            case ConnectionState.Closed:
                await con.OpenAsync();
                break;

            case ConnectionState.Open:
                break;

            default:
                throw new UnreachableException();
        }

        try
        {
            await con.ReloadTypesAsync();
        }
        finally
        {
            await con.CloseAsync();
        }
    }
}

// The middleware to run for each HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();