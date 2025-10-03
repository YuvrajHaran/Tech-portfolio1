using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortfolioAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ---- Add services ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext - uses DefaultConnection from appsettings.json
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsA32CharacterLongDevKeyForJWT!!!";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

// CORS (dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// ---- Ensure migrations & seed at startup ----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<PortfolioDbContext>();
        // Apply migrations if any
        db.Database.Migrate();

        // Seed data if needed
        DbInitializer.Initialize(db);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error applying migrations or seeding DB: " + ex);
        throw;
    }
}

// ---- Middleware ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable static files (wwwroot)
app.UseDefaultFiles(); // looks for index.html by default
app.UseStaticFiles();

app.UseCors("DevCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
