using System.Data.Common;
using Battleship.Api.Persistence;
using Battleship.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Battleship.Tests.Integration;

public sealed class TestApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection = CreateOpenConnection();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BattleshipDbContext>>();
            services.RemoveAll<IRandomGenerator>();

            services.AddDbContext<BattleshipDbContext>(options =>
                options.UseSqlite(_connection));

            services.AddSingleton<IRandomGenerator>(_ =>
                new SequenceRandomGenerator(
                    0, 1, 1, // Carrier: horizontal, x=1, y=1
                    0, 1, 2, // Battleship: horizontal, x=1, y=2
                    0, 1, 3, // Cruiser: horizontal, x=1, y=3
                    0, 1, 4, // Submarine: horizontal, x=1, y=4
                    0, 1, 5  // Destroyer: horizontal, x=1, y=5
                ));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }

    private static DbConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        return connection;
    }

    private sealed class SequenceRandomGenerator(params int[] values) : IRandomGenerator
    {
        private readonly Queue<int> _values = new(values);

        public int Next(int minValueInclusive, int maxValueExclusive)
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("No deterministic random values remain.");
            }

            return _values.Dequeue();
        }
    }
}