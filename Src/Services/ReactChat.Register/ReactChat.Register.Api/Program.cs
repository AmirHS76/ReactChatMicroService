using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactChat.Register.Domain.Entities;
using ReactChat.Register.Infrastructure.Persistence;
using ReactChat.Shared.Messaging.Config;
using ReactChat.Shared.Messaging.Connections;
using ReactChat.Shared.Messaging.Messaging;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(sp =>
{
    var settings = configuration
                     .GetSection("RabbitMQ")
                     .Get<RabbitMQSettings>();
    var factory = new RabbitMQConnector(settings!);
    return new RabbitMQPublisher(factory);
});
var settings = configuration
                     .GetSection("RabbitMQ")
                     .Get<RabbitMQSettings>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/user/v1/register", async (AppDbContext db, [FromServices] HttpClient http, [FromServices] RabbitMQPublisher publisher, string email, string password, string username) =>
{
    var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
    if (user != null)
        return Results.BadRequest("User already exists");
    var newUser = new User
    {
        Email = email,
        RegisteredAt = DateTime.Now,
        UserName = username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
    };
    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    await publisher.PublishAsync(newUser, string.Empty, RabbitMQQueues.UserRegistrationQueue);

    return Results.Created();
});

app.UseHttpsRedirection();


app.Run();