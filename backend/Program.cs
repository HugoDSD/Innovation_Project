using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InnovationProject.Models;
using InnovationProject.Data;
using InnovationProject.Services;
using InnovationProject.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. SERVICE CONFIGURATION (the dependency container)
// ==========================================

// Add the API controllers
builder.Services.AddControllers();

// PostgreSQL database configuration
builder.Services.AddDbContext<AppDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Security configuration (Identity)
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDb>();
builder.Services.AddScoped<IEvaluationService, EvaluationService>();



// --- JWT AUTHENTICATION CONFIGURATION ---
var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("Missing JWT secret key.");

builder.Services.AddAuthentication(options =>
{
    // Tell the API to look for a "Bearer" token in requests by default
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = false,   // Disabled for local development
        ValidateAudience = false, // Disabled for local development
        ClockSkew = TimeSpan.Zero // Removes the default 5 min tolerance on expiration
    };
});




// --- CORS CONFIGURATION (allows the local frontend) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================================
// 2. APPLICATION BUILD
// ==========================================
builder.Services.AddScoped<IAuthService, AuthService>(); // Tell ASP.NET Core to provide an AuthService instance (the concrete class implementing IAuthService) whenever an IAuthService is requested
var app = builder.Build();

// ==========================================
// 3. HTTP PIPELINE CONFIGURATION (the middlewares)
// ==========================================


// app.UseMiddleware<ExceptionHandlingMiddleware>();  // Uncomment this line once the ExceptionHandlingMiddleware class is added to the project

// Enable CORS (before authentication)
app.UseCors("FrontendDev");

// Enable security in the pipeline
app.UseAuthentication(); // verifies who the caller is, notably via the token
app.UseAuthorization();  // checks whether they are allowed

// ==========================================
// 4. ROUTE CONFIGURATION (Endpoints)
// ==========================================


/*
    With this line, the API automatically generates and exposes a whole group of pre-built routes to handle security.
    We already get ready-to-use routes to:
    - Register (/register)
    - Log in (/login)
    - Log out (/logout)
    - Manage roles (add, remove, etc.)
    - Manage users (list, delete, etc.)
    - Manage sessions (view active sessions, end them, etc.)
    - Manage tokens (generate, revoke, etc.)
    - Manage passwords (change, reset, etc.)
*/



// Expose the routes of our own controllers
app.MapControllers();

// Start the server
app.Run();