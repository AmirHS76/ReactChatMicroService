using Microsoft.EntityFrameworkCore;
using ReactChat.Login.BackgroundTasks;
using ReactChat.Login.Infrastructure.Persistence;
using ReactChat.Shared.Messaging.Config;
using ReactChat.Shared.Messaging.Connections;
using ReactChat.Shared.Messaging.Messaging;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddSingleton(sp =>
{
    var settings = configuration
                     .GetSection("RabbitMQ")
                     .Get<RabbitMQSettings>();
    var factory = new RabbitMQConnector(settings!);
    return new RabbitMQConsumer(factory);
});


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


app.UseHttpsRedirection();

app.Run();
