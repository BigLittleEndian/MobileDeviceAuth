// ***********************************************************************************
// Simple in-house version of Confidential Client Authentication.
// Switching from ADAL to MSAL you will get exception trying to create
// Confidential Client on mobile platform so if your devices are considered
// confidential (like locked phones working only within company’s walls)
// this is simple implementation of JWT Bearer Token Authentication and Authorization.
// ***********************************************************************************

using Microsoft.OpenApi.Models;

using DeviceAuth.Security;

var builder = WebApplication.CreateBuilder(args);

JWTToken.Initialize(builder.Configuration); // Initialize our static helper

//
// Services
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to have JWT Bearer token login option
builder.Services.AddSwaggerGen(options =>
{
   options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
      Scheme = "Bearer",
      BearerFormat = "JWT",
      In = ParameterLocation.Header,
      Name = "Authorization",
      Description = "Please provide JWT Bearer Token",
      Type = SecuritySchemeType.Http
   });
   options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// Configure two authentication JWT Bearer schemes
builder.Services.AddAuthentication("JWTBearer01") // "JWTBearer01" is default and User.Identity.IsAuthenticated works only for this scheme
   .AddJwtBearer("JWTBearer01", JWTToken.GetOptions("JWTBearer01"))
   .AddJwtBearer("JWTBearer02", JWTToken.GetOptions("JWTBearer02"));

builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Let's put something in the root and exclude it from the Swagger
app.MapGet("/", () => "Hello this is DeviceAuth example.").ExcludeFromDescription();

app.Run();

