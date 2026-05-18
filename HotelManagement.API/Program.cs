using System.Security.Claims;
using System.Text;
using FluentValidation;
using HotelManagement.API.Filters;
using HotelManagement.API.Middlewares;
using HotelManagement.API.Modules.AmenityModule.Repositories;
using HotelManagement.API.Modules.AmenityModule.Services;
using HotelManagement.API.Modules.AuthModule.Services;
using HotelManagement.API.Modules.AuthModule.Validators;
using HotelManagement.API.Modules.HotelModule.Repositories;
using HotelManagement.API.Modules.HotelModule.Services;
using HotelManagement.API.Modules.PaymentModule.Repositories;
using HotelManagement.API.Modules.PaymentModule.Services;
using HotelManagement.API.Modules.ReservationModule.Repositories;
using HotelManagement.API.Modules.ReservationModule.Services;
using HotelManagement.API.Modules.ReviewModule.Repositories;
using HotelManagement.API.Modules.ReviewModule.Services;
using HotelManagement.API.Modules.RoomModule.Repositories;
using HotelManagement.API.Modules.RoomModule.Services;
using HotelManagement.API.Modules.RoomTypeModule.Repositories;
using HotelManagement.API.Modules.RoomTypeModule.Services;
using HotelManagement.Common.Data;
using HotelManagement.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HotelDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<HotelDbContext>()
    .AddDefaultTokenProviders();

// ============ JWT configuration setup ============
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection.GetValue<string>("Key") ??
                throw new ArgumentException("Secret key is required.");
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddControllers();

builder.Services.AddScoped(typeof(ValidationFilters<>));
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// ============ Services =============
builder.Services.AddScoped<IAuthTokenService, AuthTokenService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// ============ Repositories =============
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelManagement.API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type =  SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =  "JWT Authorization header using the Bearer scheme."
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAutoMapper(_ => {}, typeof(Program));

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

//using (var  scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
//    db.Database.Migrate();

//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    string[] roleNames = [ "Admin", "User" ];

//    foreach (var roleName in roleNames)
//        if (!await roleManager.RoleExistsAsync(roleName))
//            await roleManager.CreateAsync(new IdentityRole(roleName));
//}


