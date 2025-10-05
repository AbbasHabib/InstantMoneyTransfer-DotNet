
using InstantTransfers.Infrastructure;
using InstantTransfers.Models;
using InstantTransfers.Services.Implementations;
using InstantTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    poolSize: 128);

builder.Services.AddRouting(options =>
    {
        options.LowercaseUrls = true;
    });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// builder.Services.AddAuthorization();
// builder.Services.AddAuthentication();

// builder.Services.AddIdentityApiEndpoints<User>()
//     .AddEntityFrameworkStores<AppDbContext>();


// User Services
builder.Services.AddScoped<IAccountService, AccountService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // ApplyMigrations(app);
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();
// app.MapIdentityApi<User>();

app.Run();


// static void ApplyMigrations(IApplicationBuilder app)
// {
//     using var scope = app.ApplicationServices.CreateScope();
//     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     dbContext.Database.Migrate();
// }
