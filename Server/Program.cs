using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection") 
?? throw new InvalidOperationException("ConnectionString is down")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
