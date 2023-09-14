//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.SCIM;
using Microsoft.SCIM.WebHostSample.Provider;
using Newtonsoft.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IMonitor monitoringBehavior = new ConsoleMonitor();
IProvider providerBehavior = new InMemoryProvider();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(ConfigureAuthenticationOptions).AddJwtBearer(config =>
{
    ConfigureJwtBearerOptons(config, builder.Environment, builder.Configuration);
});
builder.Services.AddControllers().AddNewtonsoftJson(ConfigureMvcNewtonsoftJsonOptions);

builder.Services.AddSingleton(typeof(IProvider), providerBehavior);
builder.Services.AddSingleton(typeof(IMonitor), monitoringBehavior);

ConfigureSwagger(builder.Services);


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(
    (IEndpointRouteBuilder endpoints) =>
    {
        endpoints.MapDefaultControllerRoute();
    });

app.Run();

static void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "SCIM Reference Code",
            Description = "The reference code provided in this repository will help you get started building a SCIM endpoint. It contains guidance on how to implement basic requirements for CRUD operations on a user and group object (also known as resources in SCIM) and optional features of the standard such as filtering and pagination.",
            Contact = new OpenApiContact
            {
                Name = "SCIM Reference Code on GitHub",
                Email = string.Empty,
                Url = new Uri("https://github.com/AzureAD/SCIMReferenceCode/wiki"),
            },
            License = new OpenApiLicense
            {
                Name = "AzureAD/SCIMReferenceCode is licensed under the MIT License",
                Url = new Uri("https://github.com/AzureAD/SCIMReferenceCode/blob/master/LICENSE"),
            }
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        options.CustomOperationIds(e => $"{string.Join("", e.ActionDescriptor.RouteValues.Values)}");
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Description = "Token to access this API",
                Name = "Authorization",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { GetSecurityScheme(), Array.Empty<string>() }
                });
    });
}

static OpenApiSecurityScheme GetSecurityScheme()
{
    return new()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };
}

static void ConfigureAuthenticationOptions(AuthenticationOptions options)
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}

static void ConfigureJwtBearerOptons(JwtBearerOptions options, IWebHostEnvironment environment, ConfigurationManager configuration)
{
    if (environment.IsDevelopment())
    {
        options.TokenValidationParameters =
           new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = false,
               ValidateIssuerSigningKey = false,
               ValidIssuer = configuration["Token:TokenIssuer"],
               ValidAudience = configuration["Token:TokenAudience"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:IssuerSigningKey"]))
           };
    }
    else
    {
        options.Authority = configuration["Token:TokenIssuer"];
        options.Audience = configuration["Token:TokenAudience"];
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = AuthenticationFailed
        };
    }
}

static Task AuthenticationFailed(AuthenticationFailedContext arg)
{
    // For debugging purposes only!
    string authenticationExceptionMessage = $"{{AuthenticationFailed: '{arg.Exception.Message}'}}";

    arg.Response.ContentLength = authenticationExceptionMessage.Length;
    arg.Response.Body.WriteAsync(
        Encoding.UTF8.GetBytes(authenticationExceptionMessage),
        0,
        authenticationExceptionMessage.Length);

    return Task.FromException(arg.Exception);
}

static void ConfigureMvcNewtonsoftJsonOptions(MvcNewtonsoftJsonOptions options)
{
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
}

