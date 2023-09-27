using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Services.Order;

var builder = WebApplication.CreateBuilder(args);

//CORS setting
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("*") // note the port is included 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IOrderService, OrderService>();

//DB
DotNetEnv.Env.Load();
// setting DbContext db connection
builder.Services.AddDbContext<GoShopContext>(options =>
    options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")));
// Add services to the container.

var app = builder.Build();

//CORS setting
app.UseCors("MyAllowedOrigins");

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

app.UseRouting(); // 必須在 UseEndpoints 之前

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<GoShopHub>("/GoShopHub"); // 使用SignalR Hub
});

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
