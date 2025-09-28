using ArtAuction.Hubs;
using ArtAuction.Interface;
using ArtAuction.Models;
using ArtAuction.Repositories;
using ArtAuction.Repository;
using ArtAuction.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure; // ? add this
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ? Configure QuestPDF license (must be before app.Run)
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ArtAuctionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NewConn")));

// Register repositories & services
builder.Services.AddSignalR();

builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IArtwork, ArtworkRepository>();
builder.Services.AddScoped<ArtworkService>();

builder.Services.AddScoped<IBid, BidRepository>();
builder.Services.AddScoped<BidService>();

builder.Services.AddScoped<ITransaction, TransactionRepository>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddScoped<IReceipt, ReceiptRepository>();
builder.Services.AddScoped<ReceiptService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

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
        ValidateAudience = false
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
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
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // serve wwwroot files (receipts)

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<AuctionHub>("/auctionhub");
app.MapControllers();

app.Run();
