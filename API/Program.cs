using Microsoft.EntityFrameworkCore;
using API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DB
DotNetEnv.Env.Load();
// setting DbContext db connection
builder.Services.AddDbContext<GoShopContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.Use(async (context, next) =>
{
   await next();

   if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
   {
       context.Request.Path = "/index.html";
       await next();
   }
});

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
