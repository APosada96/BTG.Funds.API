using BTG.Funds.Application.Services;
using BTG.Funds.Domain.Interfaces;
using BTG.Funds.Domain.Models; // asegúrate que tus entidades están en este namespace
using BTG.Funds.Domain.Services;
using BTG.Funds.Infrastructure.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace BTG.Funds.Tests;

public class FundServiceTests : IAsyncLifetime
{
    private FundService _service = default!;
    private MongoDbRunner _runner = default!;
    private IMongoDatabase _db = default!;

    public async Task InitializeAsync()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _db = client.GetDatabase("TestDB");

        IRepository<Fund> fundRepo = new MongoRepository<Fund>(_db, "Funds");
        IRepository<UserAccount> userRepo = new MongoRepository<UserAccount>(_db, "UserAccounts");
        IRepository<Transaction> txRepo = new MongoRepository<Transaction>(_db, "Transactions");
        INotificationService notif = new NotificationService();

        // ✅ Inicializar el servicio
        _service = new FundService(fundRepo, userRepo, txRepo, notif);

        // Sembrar datos base
        await _db.GetCollection<Fund>("Funds").InsertManyAsync(new[]
        {
        new Fund { Name = "Fondo1", MinimumAmount = 50000, Category = "FPV" },
        new Fund { Name = "Fondo2", MinimumAmount = 250000, Category = "FIC" }
    });

        await _db.GetCollection<UserAccount>("UserAccounts").InsertOneAsync(new UserAccount
        {
            Balance = 500000
        });
    }

    public Task DisposeAsync()
    {
        _runner.Dispose();
        return Task.CompletedTask;
    }


    [Fact(DisplayName = "Debería suscribir correctamente cuando hay saldo suficiente")]
    public async Task Subscribe_ShouldReduceBalance_WhenSufficientFunds()
    {
        // Arrange
        var fund = await _db.GetCollection<Fund>("Funds")
                            .Find(f => f.Name == "Fondo1")
                            .FirstOrDefaultAsync();

        // Act
        var result = await _service.SubscribeAsync(fund.Id, new NotificationPreference
        {
            Type = "Email",
            Destination = "test@btg.com"
        });

        // Assert
        Assert.Equal("Suscripción exitosa.", result);

        var user = await _db.GetCollection<UserAccount>("UserAccounts").Find(_ => true).FirstAsync();

        Assert.Equal(450000, user.Balance);
    }


    [Fact(DisplayName = "Debería reembolsar el saldo al cancelar una suscripción activa")]
    public async Task Cancel_ShouldRefundBalance_WhenSubscribed()
    {
        // Arrange
        var fund = await _db.GetCollection<Fund>("Funds")
                            .Find(f => f.Name == "Fondo1")
                            .FirstOrDefaultAsync();

        var users = _db.GetCollection<UserAccount>("UserAccounts");
        var user = await users.Find(_ => true).FirstAsync();

        user.SubscribedFunds.Add(fund.Id);
        user.Balance -= fund.MinimumAmount;

        await users.ReplaceOneAsync(u => u.Id == user.Id, user);

        // Act
        var result = await _service.CancelAsync(fund.Id);

        // Assert
        Assert.Equal("Cancelación exitosa.", result);

        var updated = await users.Find(_ => true).FirstAsync();

        Assert.Equal(500000, updated.Balance);
    }


    [Fact(DisplayName = "Debería lanzar excepción cuando el saldo es insuficiente")]
    public async Task Subscribe_ShouldThrow_WhenInsufficientBalance()
    {
        try
        {
            // Arrange
            var users = _db.GetCollection<UserAccount>("UserAccounts");
            await users.DeleteManyAsync(_ => true);
            await users.InsertOneAsync(new UserAccount { Balance = 100000 });

            var funds = _db.GetCollection<Fund>("Funds");
            await funds.DeleteManyAsync(_ => true);
            await funds.InsertManyAsync(new[]
            {
                new Fund { Name = "Fondo1", MinimumAmount = 50000, Category = "FPV" },
                new Fund { Name = "Fondo2", MinimumAmount = 250000, Category = "FIC" }
            });

            var fund = await funds.Find(f => f.Name == "Fondo2").FirstOrDefaultAsync();
            Assert.NotNull(fund);
            Assert.False(string.IsNullOrEmpty(fund.Id)); // 👈 asegúrate que tiene Id

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.SubscribeAsync(fund!.Id, new NotificationPreference()));

            Assert.Contains("No tiene saldo disponible", ex.Message);
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
      
    }


}
