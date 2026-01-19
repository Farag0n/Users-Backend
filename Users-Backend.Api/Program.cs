using System.Text;
using Users_Backend.Application.Interfaces;
using Users_Backend.Application.Services;
using Users_Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Users_Backend.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
//TODO ==================================================================
//TODO mejorar la arquitectura en la capa de Application y API
//TODO ==================================================================

// ===================== Database Connection =====================
//The connection string is obtained from infrastructure
builder.Services.AddInfrastructure(builder.Configuration);


// ===================== Dependency Injection =====================

// Application Services
builder.Services.AddScoped<IUserService, UserService>();

// Utility Services
builder.Services.AddScoped<TokenService>();



// ===================== JWT Configuration =====================
//Configures the authentication system to validate JWT tokens on HTTP requests.
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// ===================== Controllers and Swagger =====================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Real Estate API",
        Version = "v1"
    });

    // JWT en Swagger
    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Escribe SOLO el token JWT (sin 'Bearer ')"
    });

    options.AddSecurityRequirement(new()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===================== CORS =====================
var corsPolicyName = "AllowAll";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy
            .AllowAnyOrigin()       // Permite cualquier frontend
            .AllowAnyMethod()       // GET, POST, PUT, DELETE, etc.
            .AllowAnyHeader();      // Authorization, Content-Type, etc.
    });
});

// ===================== Construction and Pipeline =====================
var app = builder.Build();

// =========================Test the DB connection to have control over possible errors=========================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("Database connection successful.");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection error: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Users Management API v1");
        options.RoutePrefix = string.Empty;
    });
}

//Run this command to perform local tests if any issue appears:
//export ASPNETCORE_ENVIRONMENT=Local dotnet run --project ProductCatalog.Api

//app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();