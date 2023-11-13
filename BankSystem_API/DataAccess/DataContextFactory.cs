using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            // appsettings JSON Path depends on where it is saved.
            string JsonFilePath = "C:\\Users\\kentz\\Desktop\\BankSystem_API\\API\\appsettings.json";
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(JsonFilePath)
                .Build();
            var builder = new DbContextOptionsBuilder<DataContext>();
            var connectionString = configuration.GetConnectionString("DefaultSQLConnection");
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("DataAccess"));

            return new DataContext(builder.Options);
        }
    }
}
