
using InstantTransfers.Infrastructure;
using InstantTransfers.Models;
using InstantTransfers.Services.Implementations;
using InstantTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );

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
builder.Services.AddScoped<ITransactionService, TransactionService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();


public partial class Program { }
