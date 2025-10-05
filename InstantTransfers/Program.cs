
using InstantTransfers.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")),
    poolSize: 128);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // ApplyMigrations(app);
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();


// static void ApplyMigrations(IApplicationBuilder app)
// {
//     using var scope = app.ApplicationServices.CreateScope();
//     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     dbContext.Database.Migrate();
// }
