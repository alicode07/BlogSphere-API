using System.Text;
using BlogSphere.BLL.Interfaces;
using BlogSphere.BLL.Services;
using BlogSphere.DAL.Context;
using BlogSphere.DAL.Interfaces;
using BlogSphere.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine(">>> BUILD MARKER v3 <<<");
// Database (MySQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BlogSphereDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

// DAL
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// BLL
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = ctx =>
        {
            var ex = ctx.AuthenticateFailure;
            var info = ex == null
                ? "no exception (token missing or not read)"
                : ex.GetType().Name + ": " + ex.Message.Replace("\r", " ").Replace("\n", " ");
            Console.WriteLine("JWT CHALLENGE: " + info);
            ctx.Response.Headers["X-Auth-Error"] = info.Length > 300 ? info[..300] : info;
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogSphere API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token (no Bearer prefix needed).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();