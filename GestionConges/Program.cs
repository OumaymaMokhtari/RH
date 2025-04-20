using GestionConges.Data;
using GestionConges.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer; // 🔴 À supprimer
using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens; // 🔴 À supprimer
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Ajouter DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter services
builder.Services.AddScoped<IDemandeCongeService, DemandeCongeService>();
builder.Services.AddScoped<CongeCalculatorService>();


/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });
*/


// builder.Services.AddAuthorization();

// CORS pour React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Ajouter les contrôleurs
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Dev Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");

//  Middleware d’authentification JWT
// app.UseAuthentication();

app.UseAuthorization(); 

app.MapControllers();
app.UseStaticFiles();
app.Run();
