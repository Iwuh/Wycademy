using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.IO;

namespace Wycademy.Core.Models
{
    // Enables EF migrations to create instances of WycademyContext.
    class WycademyContextFactory : IDesignTimeDbContextFactory<WycademyContext>
    {
        // EF Core doesn't currently support passing arbitrary command line arguments into the args array, so until that happens we just hardcode the ini file's location.
        private const string INI_LOCATION = @".\..\Wycademy\WycademyConfig.ini";

        public WycademyContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddIniFile(Path.GetFullPath(INI_LOCATION), optional: false, reloadOnChange: false)
                .Build();

            var connectionString = new NpgsqlConnectionStringBuilder()
            {
                Host = "localhost",
                Port = int.Parse(config["Database:Port"]),
                Database = config["Database:Name"],
                Username = config["Database:User"],
                Passfile = config["Database:Passfile"]
            };

            return new WycademyContext(new DbContextOptionsBuilder<WycademyContext>().UseNpgsql(connectionString.ToString()).Options);
        }
    }
}
