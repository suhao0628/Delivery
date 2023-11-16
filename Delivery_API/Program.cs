using Delivery_API;
using Delivery_API.Data;
using Delivery_API.Services;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Connect to DB
builder.Services.AddDbContext<AppDbContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers().
    AddJsonOptions(options =>
{
    //serialization of Enum as String
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); ;


builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IBasketService, BasketService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //Version control
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Delivery_API",
        Version = "v1",
        Description = ""
    });
    //Generate comment
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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




//Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfigurations>();

builder.Services.AddAuthentication
    (authoption => {
        authoption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        authoption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        authoption.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        authoption.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
.AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        ValidateIssuer = true,
        ValidateLifetime = jwtConfig.ValidateLifetime,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SigningKey)),
    };
});




var app = builder.Build();

//SeedData
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    Initializer.Initialize(services);
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication ();
app.UseAuthorization();

app.MapControllers();

app.Run();
