using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartFood.Domain;
using SmartFood.Domain.Models;
using System.Text;
using FluentValidation;
using Microsoft.OpenApi.Models;
using SmartFood.API.Contracts.Auth.Requests;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SmartFood.Common.Configuration;
using SmartFood.API;
using SmartFood.Infrastructure.Services;
using SmartFood.Infrastructure.Services.Interfaces;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets(typeof(Program).Assembly);

JwtSettings jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ??
                          throw new Exception("JWT configuration is missing");

AdminSettings adminSettings = builder.Configuration.GetSection("AdminSettings").Get<AdminSettings>() ??
                              throw new Exception("Admin configuration is missing");

FridgeConnectionSettings fridgeConnectionSettings = builder.Configuration.GetSection("FridgeConnectionSettings").Get<FridgeConnectionSettings>() ??
                              throw new Exception("IoT devices (Smart Fridges) connection configuration is missing");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(adminSettings);
builder.Services.AddSingleton(fridgeConnectionSettings);
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MsSql"),
        o => o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(o =>
    {
        o.Password.RequireDigit = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireUppercase = false;
        o.Password.RequiredLength = 6;
        o.Password.RequiredUniqueChars = 0;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,

            ValidAudience = jwtSettings.ValidAudience,
            ValidIssuer = jwtSettings.ValidIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

const string CORS_POLICY = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORS_POLICY,
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins("http://localhost:4200");
            corsPolicyBuilder.AllowAnyMethod();
            corsPolicyBuilder.AllowAnyHeader();
        });
});

builder.Services.AddControllers(c => c.ModelValidatorProviders.Clear()).AddOData(opt =>
{
    opt.AddRouteComponents("odata", GetEdmModel());
});
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header

            },
            new List<string>()
        }
    });
});

builder.Services.AddHostedService<AdminInitializerHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CORS_POLICY);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandlerMiddleware();

app.MapControllers();


//app.UseSpa(c => c.UseProxyToSpaDevelopmentServer("http://localhost:4200/"));

app.Run();


static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<User>("Users").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Dish>("Dishes").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Supplier>("Suppliers").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Organization>("Organizations").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Filial>("Filials").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Fridge>("Fridges").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<FridgeInstallationRequest>("FridgeInstallationRequests").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<FridgeUsageRequest>("FridgeUsageRequests").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<StoredDish>("StoredDishes").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<Order>("Orders").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<FridgeDeinstallationRequest>("FridgeDeinstallationRequests").EntityType.Count().Filter().Expand().Select();
    builder.EntitySet<TechInspectionRequest>("TechInspectionRequests").EntityType.Count().Filter().Expand().Select();

    builder.EnableLowerCamelCase();
    return builder.GetEdmModel();
}