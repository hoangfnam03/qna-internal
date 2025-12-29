using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Load config (ưu tiên appsettings của WebApi nếu có)
            var basePath = Directory.GetCurrentDirectory();

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = config.GetConnectionString("DefaultConnection");

            // fallback nếu đang chạy từ solution root
            if (string.IsNullOrWhiteSpace(cs))
            {
                var webApiPath = Path.Combine(basePath, "src", "WebApi");
                if (Directory.Exists(webApiPath))
                {
                    config = new ConfigurationBuilder()
                        .SetBasePath(webApiPath)
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile("appsettings.Development.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();

                    cs = config.GetConnectionString("DefaultConnection");
                }
            }

            cs ??= "Server=localhost;Database=QnA_Dev;Trusted_Connection=True;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(cs)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
