using Mango.Services.ProductAPI;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Service;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//automapper
builder.Services.AddAutoMapper(typeof(MappingConfig));
//sql server
builder.Services.AddDbContext<ApplicationDbContext>(options => {

    options.UseSqlServer(builder.Configuration.GetConnectionString("CouponConnection"));
});

//config
var apiConfig = builder.Configuration.GetSection("ApiSettings:JwtOptions");
var secret = Encoding.ASCII.GetBytes(apiConfig.GetValue<string>("SecretKey"));
var issuer = apiConfig.GetValue<string>("Issuer");
var audience = apiConfig.GetValue<string>("Audience");
//authentication
builder.Services.AddAuthentication(options => { 

    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey=new SymmetricSecurityKey(secret),
    };
});

//injecting services
builder.Services.AddScoped<IProductService, ProductService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
