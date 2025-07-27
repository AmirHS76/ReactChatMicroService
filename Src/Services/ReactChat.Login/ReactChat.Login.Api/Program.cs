using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using ReactChat.Login.Application.DTOs;
using ReactChat.Login.BackgroundTasks;
using ReactChat.Login.Domain.Entities;
using ReactChat.Login.Infrastructure.Messaging;
using ReactChat.Login.Infrastructure.Persistence;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddSingleton<RabbitMQConsumer>();
builder.Services.AddHostedService<RabbitMqHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("OpenApi");
}

app.MapPost("/api/v1/login", async (AppDbContext db, string email, string password) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null) return Results.BadRequest("User not found");
    if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        return Results.Ok("Fake-JWT-Token");
    return Results.BadRequest("Password is incorrect");
});

app.MapPost("/api/v1/newUser", async (AppDbContext db, NewUserDTO newUser) =>
{
    var user = await db.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);
    if (user != null)
        return Results.BadRequest("User already exists");
    db.Users.Add(new User
    {
        Email = newUser.Email,
        PasswordHash = newUser.PasswordHash,
        IsActive = true
    });
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/api/v1/testRabbit", async (string message) =>
{
    var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672, UserName = "admin", Password = "admin" };
    using var connection = await factory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();

    await channel.QueueDeclareAsync(queue: "test", durable: false, exclusive: false, autoDelete: false, arguments: null);

    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "test", body: body);
    return Results.Ok($"{message} sent");
});

app.UseHttpsRedirection();

app.Run();
