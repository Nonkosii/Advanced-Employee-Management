using Microsoft.EntityFrameworkCore;
using ServerLibrary.Configurations;
using ServerLibrary.Data;
using ServerLibrary.Service.Interface;
using ServerLibrary.Service.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();

builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection") 
?? throw new InvalidOperationException("ConnectionString is down")));

builder.Services.Configure<JwtSection>(builder.Configuration.GetSection("JwtSection"));
builder.Services.AddScoped<IUserAccount, UserAccount>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm",
        builder => builder
        .WithOrigins("http://localhost:5068", "https://localhost:7261")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors("AllowBlazorWasm");
app.UseAuthorization();

app.MapControllers();

app.Run();
