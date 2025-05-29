using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyntraClone.API.Data;
using MyntraClone.API.Models;
using MyntraClone.API.Services;
using System.Text;
using MyntraClone.API.Services.FileService;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ✅ Step 1: Register EF Core with SQL Server and connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Step 2: Add Controllers to support API endpoints
builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Step 3: Register custom services (DI container will inject these)
builder.Services.AddScoped<IProductService, ProductService>();    // Product service registration
builder.Services.AddScoped<ICategoryService, CategoryService>();  // Category service registration
builder.Services.AddScoped<ICartService, CartService>();          // Cart service registration
builder.Services.AddScoped<IOrderService, OrderService>();        // Order service registration
builder.Services.AddScoped<IWishlistService, WishlistService>();  // Wishlist service registration
builder.Services.AddScoped<IUserService, UserService>();          // User service registration
builder.Services.AddScoped<IFileService, LocalFileService>();

// ✅ Step 4: Register JwtService for generating JWT tokens
builder.Services.AddScoped<JwtService>();

// ✅ Step 5: Register IPasswordHasher<User> for secure password hashing
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ✅ Step 6: Add JWT Authentication middleware - UPDATED CONFIGURATION
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                // Validate the server that created the token
            ValidateAudience = true,              // Validate the recipient of the token is authorized to receive
            ValidateLifetime = true,              // Check if the token is not expired
            ValidateIssuerSigningKey = true,      // Verify signature to check if token wasn't tampered with
            ValidIssuer = builder.Configuration["Jwt:Issuer"],    // The same issuer used when creating token
            ValidAudience = builder.Configuration["Jwt:Issuer"],  // Using issuer as audience - FIXED 401 ERROR
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing"))
            ),
            // The below settings ensure proper mapping of claims - CRITICAL FOR AUTHORIZATION
            NameClaimType = ClaimTypes.Name,      // Maps the "name" claim to User.Identity.Name
            RoleClaimType = ClaimTypes.Role       // Maps the "role" claim to User.IsInRole() and [Authorize(Roles="...")]
        };

        // Add debugging events to troubleshoot auth issues - HELPS WITH DEBUGGING
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log auth failures to understand what's wrong
                Console.WriteLine($"Authentication failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Confirm when token validation succeeds
                Console.WriteLine($"Token validated for {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                // Log the incoming token
                Console.WriteLine($"Token received: {context.Token?.Substring(0, 20)}...");
                return Task.CompletedTask;
            }
        };
    });

// ✅ Step 7: Add Swagger (OpenAPI) to test APIs in browser
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add JWT support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {your token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// ✅ Step 8: Enable proper model state behavior for Swagger error handling
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false; // Ensure validation errors are shown in Swagger
});

// ✅ Step 9: Build the app object
var app = builder.Build();

// ✅ Step 10: Enable Swagger UI only during development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // UI to test endpoints
}

// ✅ Step 11: Enable Middleware & Map Controllers
app.UseHttpsRedirection();     // Force HTTPS

// Enable CORS
app.UseCors();

// Order matters: Authentication MUST come before Authorization
app.UseAuthentication();       // Enable JWT auth check
app.UseAuthorization();        // Enable [Authorize] attributes

app.MapControllers();          // Route HTTP requests to API controllers

app.UseStaticFiles(); // Serves wwwroot/images publicly

// ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅
// ✅ Admin Seeding Block (no change to existing code)

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var passwordHasher = new PasswordHasher<User>();

    context.Database.Migrate();

    if (!context.Users.Any(u => u.Email == "admin@example.com"))
    {
        var admin = new User
        {
            Name = "Super Admin",
            Email = "admin@example.com",
            Role = "Admin"
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");
        context.Users.Add(admin);
        context.SaveChanges();
    }
}

// ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅ ✅

app.Run(); // Start the app