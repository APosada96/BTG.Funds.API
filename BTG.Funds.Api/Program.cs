using BTG.Funds.Application.Services;
using BTG.Funds.Domain.Interfaces;
using BTG.Funds.Domain.Models;
using BTG.Funds.Domain.Services;
using BTG.Funds.Infrastructure.Repositories;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoUrl = builder.Configuration.GetConnectionString("MongoDb");
    var client = new MongoClient(mongoUrl);
    return client.GetDatabase("BTGFundsDB");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// Dependency Injection
builder.Services.AddScoped<IRepository<Fund>>(sp =>
    new MongoRepository<Fund>(sp.GetRequiredService<IMongoDatabase>(), "Funds"));

builder.Services.AddScoped<IRepository<UserAccount>>(sp =>
    new MongoRepository<UserAccount>(sp.GetRequiredService<IMongoDatabase>(), "UserAccounts"));

builder.Services.AddScoped<IRepository<Transaction>>(sp =>
    new MongoRepository<Transaction>(sp.GetRequiredService<IMongoDatabase>(), "Transactions"));

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<FundService>();
builder.Services.AddScoped<TransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
