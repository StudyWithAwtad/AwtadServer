using AwtadStudy.FirebaseAdmin;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register FirebaseService Admin SDK dependency as a singleton.
// A single instance of the service is created and shared across the entire application.
builder.Services.AddSingleton<FirebaseService>();

// Register the FirebaseAuth Service
builder.Services.AddScoped<IFirebaseAuth, FirebaseAuthService>();

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

