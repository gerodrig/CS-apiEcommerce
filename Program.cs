using System.Text;
using Asp.Versioning;
using cs_apiEcommerce.Constants;
using cs_apiEcommerce.Models;
using cs_apiEcommerce.Repository;
using cs_apiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string dbConnection = builder.Configuration.GetConnectionString("SqlConnection") ?? "";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnection));

//?Implementing Cache
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = true;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//* Add AspCoreNet Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//* Add Authentication services
string? secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("Secret key is not configured");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false

    };
});

//? Add Cache profiles to Add controllers
builder.Services.AddControllers(option =>
{
    option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
    option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
});
builder.Services.AddEndpointsApiExplorer();

//?The following swaggerGen was edited so it can be used with authentication
builder.Services.AddSwaggerGen(
    options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Our API uses JWT Authentication with the Bearer scheme.\n\n" +
                  "Enter the generated token from login below.\n\n" +
                  "Example: \"12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header

            },
            new List<string>()
            }
        });

        //? Swagger Documentation for V1
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "API Ecommerce",
            Description = "API to manage products, categories and users",
            TermsOfService = new Uri("http://example.com/terms"),
            Contact = new OpenApiContact
            {
            Name = "GerarICS",
            Url = new Uri("https://gerar.ca")
            },
            License = new OpenApiLicense
            {
            Name = "Use License",
            Url = new Uri("http://example.com/license")
            }
        });
        //? Swagger Documentation for V2
        options.SwaggerDoc("v2", new OpenApiInfo
        {
            Version = "v2",
            Title = "API Ecommerce",
            Description = "API to manage products, categories and users",
            TermsOfService = new Uri("http://example.com/terms"),
            Contact = new OpenApiContact
            {
            Name = "GerarICS",
            Url = new Uri("https://gerar.ca")
            },
            License = new OpenApiLicense
            {
            Name = "Use License",
            Url = new Uri("http://example.com/license")
            }
        });
    }
);

//? Add API versioning
var apiVersionBuilder = builder.Services.AddApiVersioning(option =>
{
    //?Default version
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
    //* Following option is to combine versioning so it can be used via query, param and path
    // option.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //? ?apiversion in queryparam
});

//? Add Api explorer so Swagger can show API version
apiVersionBuilder.AddApiExplorer(option =>
{
    option.GroupNameFormat = "'v'VVV"; //* v1, v2, v3...
    option.SubstituteApiVersionInUrl = true; //* api/v{version}/products
});

//? Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin,
    builder =>
    {
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //? ADD options to the swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}

//? Enable static files in the server
app.UseStaticFiles();

app.UseHttpsRedirection();

//?Add Middleware for CORS  
app.UseCors(PolicyNames.AllowSpecificOrigin);

//? Add caching After CORS as per documentation
//* https://learn.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-9.0
app.UseResponseCaching();

//?Enable authorization middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();
