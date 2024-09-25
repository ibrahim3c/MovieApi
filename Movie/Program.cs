using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Movie.Authorization.PermissionBased;
using Movie.Authorization.PolicyBased;
using Movie.Configs;
using Movie.Data;
using Movie.Extensions;
using Serilog;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

#region MyConfigs



 
    builder.Services.MyAddDbContext(builder.Configuration).AddCorsExtensions().AddInjectionExtensions().AddAuthenticationExtensions(builder.Configuration);
    var logger=new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
    builder.Logging.AddSerilog(logger);


    // optionPattern Way1
    var TestConfig =builder.Configuration.GetSection("TestCofigs").Get<TestConfigs>();
    builder.Services.AddSingleton(TestConfig);

    // optionPattern Way2
    var testo = new TestConfigs();
    builder.Configuration.GetSection("TestCofigs").Bind(testo);
    builder.Services.AddSingleton(testo);

    // optionPattern Way2
    builder.Services.Configure<TestConfigs>(builder.Configuration.GetSection("TestCofigs"));


    
    //JWT
    builder.Services.Configure<JwtConfigs>(builder.Configuration.GetSection("JWT"));



//policy based authorization
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("isMale", policy =>
        {
            policy.RequireClaim("Gender", "Male");
        });
        options.AddPolicy("IsPassed", policy =>
        {
            policy.AddRequirements(new DegreeRequirement(50));

        });
    });

#endregion

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionBasedAuthorizationFilter>();
});
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    // how to add authoization token to swagger
    builder.Services.AddSwaggerGen(options =>
        {
            //
            options.SwaggerDoc("v1", new OpenApiInfo
            {

                Version = "v1",
                Title = "MyMovie",
                Description = "My First api",
                TermsOfService = new Uri("http://www.google.com"),
                Contact = new OpenApiContact
                {
                    Name = "Ibrahim",
                    Email = "test@gmail.com"
                },
                License = new OpenApiLicense
                {
                    Name = "My Licence",
                    Url = new Uri("http://www.googl.com")

                }
            });


            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                In = ParameterLocation.Header,
                BearerFormat="JWT",
                Description = "Enter your JWT Key"
            });


            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference=new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        },
                        Name="Bearer",
                        In=ParameterLocation.Header
                    },
                    new List<string>()
                }

            });

            /*
             
            Summary of Differences:
                AddSecurityDefinition: Defines how Swagger should expect the security token 
                                                     (e.g., where it goes in the request, how it should be formatted).

                AddSecurityRequirement: Specifies that the defined security scheme is required to access the API endpoints,
                                                     enforcing that users must provide a token as per the defined scheme.
             
             */
        });

        var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

