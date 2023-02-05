using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQL Server DB");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn"));
    });
}
else
{
    Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

PrepareDb.PreparePopulation(app, builder.Environment.IsProduction());

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/Proto/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Proto/platforms.proto"));
});

app.Run();
