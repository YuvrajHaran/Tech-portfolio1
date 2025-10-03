using System.Security.Cryptography;
using System.Text;
using PortfolioAPI.Models;

namespace PortfolioAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PortfolioDbContext context)
        {
            // Ensure database created/migrated already (call before this if you want)
            context.Database.EnsureCreated();

            // Seed Admin if none
            if (!context.Admins.Any())
            {
                var admin = new Admin
                {
                    Username = "admin",
                    PasswordHash = ComputeSha256Hash("admin123") // default password: admin123
                };
                context.Admins.Add(admin);
            }

            // Seed Projects if none
            if (!context.Projects.Any())
            {
                context.Projects.AddRange(
                    new Project { Title = "Sample Project 1", Description = "Demo project 1", ImageUrl = "", GithubLink = "", DemoLink = "" },
                    new Project { Title = "Sample Project 2", Description = "Demo project 2", ImageUrl = "", GithubLink = "", DemoLink = "" }
                );
            }

            context.SaveChanges();
        }

        private static string ComputeSha256Hash(string raw)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
