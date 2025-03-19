using DocumentManagementSystem.Data;
using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Mapper;
using DocumentManagementSystem.Model;
using HashidsNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;



try
{
    var builder = WebApplication.CreateBuilder(args);

    //Serilog Initialization
    builder.Host.UseSerilog((ctx, lc) => lc
      .WriteTo.Console()
      .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddDbContext<ApplicationDBContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });


    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please Enter a valid Token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"

        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}

        }


    });

    });

    // User Management configuration for Token Generation
    builder.Services.Configure<IdentityOptions>(options =>
    {
        //Password settings
        options.Password = new PasswordOptions()
        {
            RequireDigit = true,
            RequireNonAlphanumeric = true,
            RequireUppercase = true,
            RequireLowercase = true,
            RequiredLength = 5,
            RequiredUniqueChars = 1

        };
        //Lockout setting
        options.Lockout = new LockoutOptions()
        {
            DefaultLockoutTimeSpan = TimeSpan.FromMinutes(600),
            MaxFailedAccessAttempts = 50,
            AllowedForNewUsers = true
        };
        //User Settings
        options.User = new UserOptions()
        {
            AllowedUserNameCharacters =
            "abcdefghjklmnopqrstuvwsyzABCDEFGHIJKLMNOPQRSTUWXYZ-._?!@#$%*&^",
            RequireUniqueEmail = true

        };
    });
    //Remove default claims from Jwt token
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

    //User Identity Configuration
    builder.Services.AddIdentityCore<User>().AddRoles<Role>()
        .AddEntityFrameworkStores<ApplicationDBContext>()
        .AddDefaultTokenProviders().AddRoleValidator<RoleValidator<Role>>()
        .AddRoleManager<RoleManager<Role>>().AddSignInManager<SignInManager<User>>();

    // Read Token From AppSetting
    var secret = builder.Configuration.GetSection("AppSettings:Token").Value ?? "";

    //Token Authentification setting
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Utils.StringToByteArray($"{secret}-{secret}")),
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                ValidAudience = builder.Configuration["AppSettings:Audience"],
                ClockSkew = TimeSpan.Zero

            };

          
        });
    //require Authentification globaly
    builder.Services.AddMvc(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    });
    
    //AutoMapper Configuration 
    builder.Services.AddAutoMapper(m => m.AddProfile<AutoMapperProfiles>());

    //configer idhasher
   builder.Services.AddSingleton<IHashids>(x => new Hashids(builder.Configuration
       .GetSection("AppSettins:Token").Value, 32));

    builder.Services.AddAuthorization();
    builder.Services.AddControllers();

    //Config Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "DocManagementSystem API", Version = "v1" });
        var secScheme = new OpenApiSecurityScheme()
        {
            Description = "Jwt Authorization: login with creds and copy and paste jwt token",
            Name = "Authorization",
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }


        };
        c.AddSecurityDefinition("Bearer", secScheme);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            { secScheme,Array.Empty<String>()}
        });

    });


    var app = builder.Build();

    // Set up the middleware

    app.UseSerilogRequestLogging(option =>
    {
        option.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "");
            diagnosticContext.Set("RequestFullPath", httpContext.Request.GetDisplayUrl());
            diagnosticContext.Set("RequestQueryString", httpContext.Request.QueryString.Value ?? "");
        };
        option.IncludeQueryInRequestPath = true;

    });


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // order matters
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");

}
finally
{
    Log.Information("Short down completely");
    Log.CloseAndFlush();
}
