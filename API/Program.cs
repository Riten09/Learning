using System.Text;
using API.Data;
using API.Extension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//Adding EF DB context as service using our extension method 
builder.Services.AddApplicationService(builder.Configuration);
// Adding IdentityService from extension method
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors(builder=> builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
