using HPlusSport.API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string productsCorsPolicy = "ProductsCorsPolicy";

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(productsCorsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ShopConnection")
    ?? throw new InvalidOperationException("Connection string 'ShopConnection' was not found.");

builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MariaDbServerVersion(new Version(10, 6, 23)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(productsCorsPolicy);

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ShopContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        logger.LogWarning(
            ex,
            "The application started, but the database could not be reached. " +
            "Confirm MariaDB/MySQL is running and that the ShopConnection setting points to the correct host, port, database, user, and password.");
    }
}

app.Run();
