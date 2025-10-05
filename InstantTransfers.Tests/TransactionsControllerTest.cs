using System.Net.Http.Json;
using FluentAssertions;
using InstantTransfers.DTOs.Transaction;
using InstantTransfers.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InstantTransfers.Tests;

public class TransactionsControllerTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _application;
    private readonly HttpClient _client;

    public TransactionsControllerTests(TestFixture fixture)
    {
        _application = new TestFixture();
        _client = _application.CreateClient();
    }

    private void SeedAccounts()
    {
        var db = _application.DbContext;
        db.Accounts.AddRange(
            new Account { Id = 1, UserId = 1, Balance = 1000 },
            new Account { Id = 2, UserId = 2, Balance = 2000 },
            new Account { Id = 3, UserId = 3, Balance = 3000 }
        );
        db.SaveChanges();
    }

    private void SeedTransactions()
    {
        var db = _application.DbContext;
        db.Transactions.AddRange(
            new Transaction
            {
                Id = 1,
                Amount = 100,
                FromAccountId = 1,
                ToAccountId = 2,
                Timestamp = new DateTime(2015, 12, 20, 10, 20, 30, DateTimeKind.Utc),
            },
            new Transaction
            {
                Id = 2,
                Amount = 200,
                FromAccountId = 2,
                ToAccountId = 3,
                Timestamp = new DateTime(2015, 12, 20, 11, 20, 30, DateTimeKind.Utc),
            }
        );
        db.SaveChanges();
    }

    [Fact]
    public void GetTransactions_ShouldReturnAllTransactions()
    {
        // Arrange
        SeedAccounts();
        SeedTransactions();

        // Act
        var response = _client.GetAsync("/api/transaction").GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var transaction = response.Content.ReadFromJsonAsync<List<TransactionResponseDto>>().GetAwaiter().GetResult();

        // Assert
        transaction.Should().HaveCount(2);
        transaction[0].Amount.Should().Be(100);
        transaction[1].Amount.Should().Be(200);
    }

    [Fact]
    public void CreateTransaction_ShouldSucceed_WhenSufficientBalance()
    {
        // Arrange
        SeedAccounts();

        var newTransaction = new TransactionCreateDto(
            1, 2, 500, DateTime.UtcNow
        );

        // Act
        var response = _client.PostAsJsonAsync("/api/transaction", newTransaction).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var created = response.Content.ReadFromJsonAsync<TransactionResponseDto>().GetAwaiter().GetResult();

        // Assert
        created.Should().NotBeNull();
        created.Amount.Should().Be(500);

        var db = _application.DbContext;
        var from = db.Accounts.AsNoTracking().First(a => a.Id == 1);
        var to = db.Accounts.AsNoTracking().First(a => a.Id == 2);
        from.Balance.Should().Be(500);
        to.Balance.Should().Be(2500);
    }

    [Fact]
    public void CreateTransaction_ShouldFail_WhenInsufficientBalance()
    {
        // Arrange
        SeedAccounts();

        var newTransaction = new TransactionCreateDto(
            1, 2, 2000, DateTime.UtcNow
        );

        // Act
        var response = _client.PostAsJsonAsync("/api/transaction", newTransaction).GetAwaiter().GetResult();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public void CreateTransaction_ShouldBeIdempotent_WhenSameRequestId()
    {
        // Arrange
        SeedAccounts();

        var txDto = new TransactionCreateDto(1, 2, 100, DateTime.UtcNow);

        // Act
        var firstResponse = _client.PostAsJsonAsync("/api/transaction", txDto).GetAwaiter().GetResult();
        firstResponse.EnsureSuccessStatusCode();

        var secondResponse = _client.PostAsJsonAsync("/api/transaction", txDto).GetAwaiter().GetResult();

        // Assert
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public void CreateTransaction_ShouldBeIdempotent_WhenSameRequestIsRepeatedMultipleTimes()
    {
        // Arrange
        SeedAccounts();

        var txDto = new TransactionCreateDto(1, 2, 100, DateTime.UtcNow);

        // Act
        var firstResponse = _client.PostAsJsonAsync("/api/transaction", txDto).GetAwaiter().GetResult();
        firstResponse.EnsureSuccessStatusCode();

        var secondResponse = _client.PostAsJsonAsync("/api/transaction", txDto).GetAwaiter().GetResult();

        // Assert
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }


    [Fact]
    public void CreateTransaction_ShouldFail_WhenFromAccountEqualsToAccount()
    {
        // Arrange
        SeedAccounts();

        var txDto = new TransactionCreateDto(1, 1, 50, DateTime.UtcNow);

        // Act
        var response = _client.PostAsJsonAsync("/api/transaction", txDto).GetAwaiter().GetResult();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public void GetTransactionById_ShouldReturnTransaction()
    {
        // Arrange
        SeedAccounts();
        SeedTransactions();

        var db = _application.DbContext;
        var txId = db.Transactions.First().Id;

        // Act
        var response = _client.GetAsync($"/api/transaction/{txId}").GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var tx = response.Content.ReadFromJsonAsync<TransactionResponseDto>().GetAwaiter().GetResult();

        // Assert
        tx.Should().NotBeNull();
        tx.Id.Should().Be(txId);
    }
}
