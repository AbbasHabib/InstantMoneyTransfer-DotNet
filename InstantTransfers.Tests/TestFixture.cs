using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InstantTransfers.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;


public class TestFixture : WebApplicationFactory<Program>
{
    public AppDbContext DbContext { get; private set; }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));


            services.AddNpgsql<AppDbContext>("Host=localhost;Port=5432;Database=atlasbank_test;Username=postgres;Password=somepassword");

            DbContext = CreateDbContext(services);
            DbContext.Database.EnsureDeleted();
            DbContext.Database.Migrate();
        });
    }

    private static AppDbContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return dbContext;
    }
}