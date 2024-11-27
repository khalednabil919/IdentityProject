using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using BusinessLogic;
using Core.Auth;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using BusinessLogic.Helper;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Components.Authorization;
using DataTransferObject.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Autofac.Core;
using Microsoft.OpenApi.Models;
using BusinessLogic.MiddleWares;
using DataTransferObject.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.ComponentModel.DataAnnotations;
using BusinessLogic.CustomAuthorizationPolicy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region Json newtonsoft and json patch

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddControllersWithViews(options =>
{
    options.InputFormatters.Insert(0, new ServiceCollection()
        .AddLogging()
        .AddMvc()
        .AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>()
        .Value
        .InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First());
});
#endregion
#region Connection String
string DataAccess = builder.Configuration.GetConnectionString("APIConnection");
builder.Services.AddDbContext<DataAccess>(options =>
{
    options.UseSqlServer(DataAccess, b =>
    {
        b.MigrationsAssembly(typeof(DataAccess).Assembly.FullName);
    });
});
#endregion
#region addIdentity

builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 7;
    opt.Password.RequireDigit = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequiredUniqueChars = 2;
    opt.Password.RequireNonAlphanumeric = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<DataAccess>()
    .AddDefaultTokenProviders();
#endregion
#region add JWT
var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidIssuer = builder.Configuration["JWT:Issuer"],
    ValidAudience = builder.Configuration["JWT:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
    ClockSkew = TimeSpan.FromSeconds(20)
};
builder.Services.AddSingleton(tokenValidationParams);

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = tokenValidationParams;
    });

#endregion
#region autoFac Configure IOC Container for other Projects

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new BusinessLogicModule());
    builder.RegisterModule(new InfrastructureModule());
});

builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
}).CreateMapper());
#endregion
#region Add Cors
builder.Services.AddCors();
#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddAuthorizationCore(options =>
{
    //user should have role of Create and edit in roles in token roles;[Create,edit]
    options.AddPolicy("FirstPolicy", policy =>
    {
        //roles array should contain Create and Edit 
        //roles refer to word use by identity and this word in my code contains roles of the user and claims of that user and claims
        //related to that roles
        policy.RequireRole("Create");
        policy.RequireRole("edit");
    });

    //first policy: user should have Create or edit claim under Role Claim Type
    //second policy: user should have Create and edit claim under Role Claim Type
    options.AddPolicy("CreateRegionPolicy", policy =>
    {
        //first policy
        //token contain ClaimValue: Create or edit With Claim Type:Role
        policy.RequireClaim("Role", "Create", "Edit");

        //second policy
        //token should contain ClaimValue: Create and edit With Claim Type:Role 
        //policy.RequireClaim("Role", "Create")
        //      .RequireClaim("Role","Edit");
    });

    //user should have role Visitor and Super Admin  and ClaimValue: Create With Claim Type:Role 
    options.AddPolicy("SuperAdminandVisitorandCreate", policy =>
    {
        policy.RequireRole("Visitor")
              .RequireRole("Super Admin")
              .RequireClaim("Role", "Create");
    });

    //user should have role visitor and ClaimValue: Create With Claim Type:Role  OR user should have role Super Admin
    options.AddPolicy("SuperAdminORVisitorandCreate", policy =>
    {
        policy.RequireAssertion(context =>
                            context.User.IsInRole("Visitor") && context.User.HasClaim(c => c.Type == "Role" && c.Value == "Create") ||
                            context.User.IsInRole("Super Admin"));
    });

    options.AddPolicy("AdminCantEditHimSelf", policy =>
    policy.Requirements.Add(new ManageAdminRolesAndClaimsRequirement()));
    
    //options.InvokeHandlersAfterFailure = false;


});
builder.Services.AddScoped<IAuthorizationHandler, CanEditOnlyAdminRolesAndClaimsHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AnotherCustomerPolicyHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseMiddleware<ValidateToken>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
