using Battleship.Api.Games;
using Battleship.Api.Persistence;
using Battleship.Api.Summaries;
using Battleship.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IRandomGenerator, SystemRandomSource>();
builder.Services.AddSingleton<IGameStore, InMemoryGameStore>();
builder.Services.AddSingleton<GameFactory>();

builder.Services.AddDbContext<BattleshipDbContext>(options => options.UseSqlite("Data Source=battleship.db"));

var app = builder.Build();

app.UseCors("Frontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BattleshipDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.MapGameEndpoints();
app.MapSummaryEndpoints();

app.Run();