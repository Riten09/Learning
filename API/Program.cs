using API.Data;
using API.ExceptionMIddleware;
using API.Extension;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//Adding EF DB context as service using our extension method 
builder.Services.AddApplicationService(builder.Configuration);
// Adding IdentityService from extension method
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(builder=> builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//adding logic for DB migration 
using var scope = app.Services.CreateScope(); // creating scope to get the access to all the services in programe class
var services = scope.ServiceProvider; // creating services 
try
{
    var context = services.GetRequiredService<DataContext>(); // created db context 
    await context.Database.MigrateAsync(); // migrating the DB 
    await Seed.SeedUsers(context); 
}
catch (Exception ex)
{
    
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "AN error occurred during migration");
}

app.Run();
