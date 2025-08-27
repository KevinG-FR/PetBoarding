using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using PetBoarding_Api.Endpoints;
using PetBoarding_Api.Endpoints.Authentication;
using PetBoarding_Api.Endpoints.Pets;
using PetBoarding_Api.Endpoints.Prestations;
using PetBoarding_Api.Endpoints.Reservations;
using PetBoarding_Api.Endpoints.Users;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.OptionsSetup;

using PetBoarding_Application;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Account;

using PetBoarding_Domain.Accounts;
using PetBoarding_Domain.Users;

using PetBoarding_Infrastructure;
using PetBoarding_Infrastructure.Authentication;

using PetBoarding_Persistence;
using PetBoarding_Persistence.Migrations;
using PetBoarding_Persistence.Options;
using PetBoarding_Persistence.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddControllers();

// Configuration CORS pour permettre les requêtes depuis Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Port par défaut d'Angular
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PetBoarding API",
        Version = "v1"
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

// Configuration des policies d'autorisation
builder.Services.AddAuthorization(options =>
{
    // Ajouter une policy pour chaque permission
    foreach (PetBoarding_Domain.Accounts.Permission permission in Enum.GetValues<PetBoarding_Domain.Accounts.Permission>())
    {
        options.AddPolicy(permission.ToString(), policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("permissions", permission.ToString());
        });
    }
});

builder.Services.AddApplication()
                .AddInfrastructure()
                .AddPersistence();

builder.Services.AddEndpoint(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Add Endpoints. Don't forget to add new Endpoints here.
app.MapAuthenticationEndpoints();
app.MapUsersEndpoints();
app.MapReservationsEndpoints();
app.MapPrestationsEndpoints();
app.MapPetsEndpoints();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "PetBoarding API V1");
    c.RoutePrefix = "swagger";
});

app.ApplyMigrations();

// Utiliser CORS
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
