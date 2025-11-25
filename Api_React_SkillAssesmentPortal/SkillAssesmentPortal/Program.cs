using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Repositories.Implementations;
using SkillAssessmentPortal.Repositories.Interfaces;
using SkillAssessmentPortal.Services.Implementations;
using SkillAssessmentPortal.Services.Interfaces;
using System.Text;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ? Database Connection
builder.Services.AddDbContext<SkillAssessmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SkillAssessmentDbContext"))
);

// ? Repository Registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<ITestLevelRepository, TestLevelRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();

// ? Caching
builder.Services.AddMemoryCache();

// ? Service Registrations
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ITestLevelService, TestLevelService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ? JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]!)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            }
        };
    });

// ? Add Authorization
builder.Services.AddAuthorization();

// ✅ CORS Configuration for React Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")   
              .AllowAnyMethod()                       
              .AllowAnyHeader()                       
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromDays(1)); // Cache for 24 hours
    });
});



// ? Add API + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// ? Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactAppPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
