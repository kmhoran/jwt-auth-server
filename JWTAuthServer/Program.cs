using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JWTAuthServer.Data;
using JWTAuthServer.Interfaces;
using JWTAuthServer.Models;
using JWTAuthServer.UserAuth;

string stage = Environment.GetEnvironmentVariable("STAGE");
bool isDebug = stage == null || stage.ToLower() != "prod";


if (isDebug)
{
    Console.WriteLine("-----------------------------------------");
    Console.WriteLine("DEBUG");
    Console.WriteLine("-----------------------------------------");
    Console.WriteLine($"TOKEN SECRET: {Constants.TokenSecret}");
    Console.WriteLine($"DB PASSWORD: {Constants.DBPassword}");
    Console.WriteLine($"ISSUER: {Constants.Issuer}");
    // Console.WriteLine($"AUDIENCE: {Constants.Audience}");
    Console.WriteLine("-----------------------------------------");
}


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Constants.Issuer,
            // ValidAudience = Constants.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.TokenSecret))
        };
    });
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSingleton<IDataRepo, LiteDbDataRepo>();
builder.Services.AddSingleton<IUserAuthentication, UserAuthentication>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

