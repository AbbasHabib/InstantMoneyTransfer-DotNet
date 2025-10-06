using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InstantTransfers.DTOs.Account;
using InstantTransfers.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InstantTransfers.Tests;

[Collection("Database collection")]
public class AccountControllerTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _application;
    private readonly HttpClient _client;

    public AccountControllerTests(TestFixture fixture)
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

    [Fact]
    public async Task GetAll_ShouldReturnAllAccounts()
    {
        // Arrange
        SeedAccounts();

        // Act
        var response = await _client.GetAsync("/api/account");
        response.EnsureSuccessStatusCode();
        var accounts = await response.Content.ReadFromJsonAsync<List<AccountResponseDto>>();

        // Assert
        accounts.Should().HaveCount(3);
        accounts![0].Balance.Should().Be(1000);
        accounts[1].Balance.Should().Be(2000);
        accounts[2].Balance.Should().Be(3000);
    }

    [Fact]
    public async Task GetById_ShouldReturnAccount_WhenExists()
    {
        // Arrange
        SeedAccounts();

        // Act
        var response = await _client.GetAsync("/api/account/2");
        response.EnsureSuccessStatusCode();
        var account = await response.Content.ReadFromJsonAsync<AccountResponseDto>();

        // Assert
        account.Should().NotBeNull();
        account!.Id.Should().Be(2);
        account.Balance.Should().Be(2000);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        SeedAccounts();

        // Act
        var response = await _client.GetAsync("/api/account/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ShouldAddNewAccount()
    {
        // Arrange
        var dto = new AccountCreateDto
        (
            UserId: 10,
            InitialBalance: 5000
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/account", dto);
        response.EnsureSuccessStatusCode();
        var account = await response.Content.ReadFromJsonAsync<AccountResponseDto>();

        // Assert
        account.Should().NotBeNull();
        account!.UserId.Should().Be(10);
        account.Balance.Should().Be(5000);

        // Verify in DB
        var db = _application.DbContext;
        db.Accounts.Count().Should().Be(1);
    }

    [Fact]
    public async Task Update_ShouldModifyExistingAccount()
    {
        // Arrange
        SeedAccounts();
        var dto = new AccountUpdateDto
        (
            9999
        );

        // Act
        var response = await _client.PutAsJsonAsync("/api/account/1", dto);
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<AccountResponseDto>();

        // Assert
        updated.Should().NotBeNull();
        updated!.Balance.Should().Be(9999);
    }

    [Fact]
    public async Task Delete_ShouldRemoveAccount_WhenExists()
    {
        // Arrange
        SeedAccounts();

        // Act
        var response = await _client.DeleteAsync("/api/account/1");
        response.EnsureSuccessStatusCode();
        var message = await response.Content.ReadAsStringAsync();

        // Assert
        message.Should().Contain("deleted account id 1 successfully");

        var db = _application.DbContext;
        db.Accounts.Should().HaveCount(2);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/account/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
