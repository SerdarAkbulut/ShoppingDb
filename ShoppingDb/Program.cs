using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingApi.Data;
using ShoppingApi.Entity;
using ShoppingApi.Mapping;
using ShoppingApi.Middlewares;
using ShoppingApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<DataContext>(options =>
{
    var config=builder.Configuration;
    var connectionString = config.GetConnectionString("defaultConnection");
    options.UseSqlServer(connectionString);
});
//builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddIdentity<User,Role>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;

    options.User.RequireUniqueEmail = true;

    // Burada bo�luk karakteri dahil edildi
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ / ";
});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey=true,
            IssuerSigningKey= new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWTSecurity:SecretKey"]!)),
            ValidateLifetime = true,

        };
    });


builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(2); // sadece şifre sıfırlama tokeni için geçerli
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
var app = builder.Build();

app.UseMiddleware<ExceptionHandling>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Demo API");
    });

}

app.UseHttpsRedirection();
app.UseCors(opt =>
{
      opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();

});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
SeedData.Initialize(app);
app.Run();
