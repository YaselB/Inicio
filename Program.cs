using System.Text;
using inicio.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>{
    c.SwaggerDoc("v1", new OpenApiInfo {Title ="MyGalleryPhotos" , Version = "v1"});
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
    In = ParameterLocation.Header,
    Description = "Please insert token",
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    BearerFormat = "JWT",
    Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },new string[]{}
        }
    });
    c.OperationFilter<FileUploadOperation >();
});



builder.Services.AddDbContext<GaleriaContexto>(options => options.UseNpgsql( "Server=localhost;Port=5432;Database=Galeria;User ID=postgres;Password=1234" ));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters(){
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey( Encoding.UTF8.GetBytes("super secret key"))
    };
});
builder.Services.AddAuthorization(option => {
option.AddPolicy("AdminOnly",policy =>
policy.RequireRole("Administrator"));
});

builder.Services.AddScoped<IGeneratedJwt,GeneratedJwt>();
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp",
            builder =>
            {
                builder.WithOrigins("http://localhost:3000") 
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });
var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json" , "MiFirstAplicacion v1"));
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
 
app.MapControllers();

app.Run();