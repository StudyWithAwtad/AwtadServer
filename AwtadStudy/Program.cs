using System.Globalization;
using AwtadStudy.FirebaseAdmin;
using AwtadStudy.Universities;

// set default CultureInfo to avoid bugs with string comparisons/parsing
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

// Register FirebaseService Admin SDK dependency as a singleton.
// A single instance of the service is created and shared across the entire application.
builder.Services.AddSingleton<FirebaseService>();

// Register the FirebaseAuth Service
builder.Services.AddScoped<IFirebaseAuth, FirebaseAuthService>();

builder.Services.AddSingleton<IUniversityServiceFactory, UniversityServiceFactory>();

var app = builder.Build();

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