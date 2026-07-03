using Coursera.Api.Middlewares;
using Coursera.Application;
using Coursera.Application.Common.Models;
using Coursera.Infrastructure;
using Coursera.Infrastructure.Data;
using Coursera.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = "Bearer";
    o.DefaultChallengeScheme = "Bearer";

    })
    .AddJwtBearer("Bearer", o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))

        };
    })
    // External provider schemes — registered for DI/config; actual token validation
    // is performed server-side in AuthService.ExternalLoginAsync via provider APIs.
    .AddGoogle(GoogleDefaults.AuthenticationScheme, o =>
    {
        o.ClientId = builder.Configuration["ExternalAuth:Google:ClientId"] ?? "REPLACE_WITH_GOOGLE_CLIENT_ID";
        o.ClientSecret = builder.Configuration["ExternalAuth:Google:ClientSecret"] ?? "REPLACE_WITH_GOOGLE_CLIENT_SECRET";
    })
    .AddFacebook(FacebookDefaults.AuthenticationScheme, o =>
    {
        o.AppId = builder.Configuration["ExternalAuth:Facebook:AppId"] ?? "REPLACE_WITH_FACEBOOK_APP_ID";
        o.AppSecret = builder.Configuration["ExternalAuth:Facebook:AppSecret"] ?? "REPLACE_WITH_FACEBOOK_APP_SECRET";
    });
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
// builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://byway-lime.vercel.app")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Byway API V1");
        c.RoutePrefix = "docs";
    });
if(app.Environment.IsDevelopment())
{
app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var logger = service.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        logger.LogInformation("Applying migrations...");
        db.Database.Migrate();
        logger.LogInformation("Migrations applied successfully.");

        var roleManager = service.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();

        logger.LogInformation("Seeding roles...");
        await RoleSeeder.SeedAsync(roleManager, userManager);
        logger.LogInformation("Seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during migration or seeding.");
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
