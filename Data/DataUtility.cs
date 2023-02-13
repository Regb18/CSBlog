using CSBlog.Models;
using CSBlog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CSBlog.Data
{
    // we don't have to instantiate this since it's static
    public static class DataUtility
    {
        private const string _adminRole = "Admin";
        private const string _moderatorRole = "Moderator";
        private const string _adminEmail = "racbarnes18@mailinator.com";
        private const string _moderatorEmail = "rbmoderator8@mailinator.com";

        public static DateTime GetPostGresDate(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            string? databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            return string.IsNullOrEmpty(databaseUrl) ? connectionString! : BuildConnectionString(databaseUrl);

        }

        private static string BuildConnectionString(string databaseUrl)
        {
            // these can be var's as well (implicit instantiation)
            Uri databaseUri = new(databaseUrl);
            string[] userInfo = databaseUri.UserInfo.Split(':');
            NpgsqlConnectionStringBuilder builder = new()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            // Service Provider gives us all of the services in program.cs and we choose the ones we want to use
            // Service: an Instance of ApplicationDbContext
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            // Service: an instance of Configuration Service
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

            // Service: an Instance of UserManager
            var configurationSvc = svcProvider.GetRequiredService<IConfiguration>();

            // Service: an instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Migration: This is the programattic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();


            // Seed Roles
            await SeedRolesAsync(roleManagerSvc);

            // Seed Users
            await SeedUsersAsync(userManagerSvc, dbContextSvc, configurationSvc);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // will give true if not admin
            if (!await roleManager.RoleExistsAsync(_adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_adminRole));
            }
            if (!await roleManager.RoleExistsAsync(_moderatorRole))
            {
                await roleManager.CreateAsync(new IdentityRole(_moderatorRole));
            }
        }


        private static async Task SeedUsersAsync(UserManager<BlogUser> userManager,
                                                 ApplicationDbContext context,
                                                 IConfiguration configuration)
        {
            // Seed Admin User
            if (!context.Users.Any(u => u.Email == _adminEmail))
            {
                BlogUser adminUser = new()
                {
                    Email = _adminEmail,
                    UserName = _adminEmail,
                    FirstName = "Reggie",
                    LastName = "Barnes",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, configuration["AdminPwd"] ?? Environment.GetEnvironmentVariable("AdminPwd")!);
                await userManager.AddToRoleAsync(adminUser, _adminRole);

            }

            // Seed Moderator User
            if (!context.Users.Any(u => u.Email == _moderatorEmail))
            {
                BlogUser moderatorUser = new()
                {
                    Email = _moderatorEmail,
                    UserName = _moderatorEmail,
                    FirstName = "Blog",
                    LastName = "Moderator",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(moderatorUser, configuration["ModeratorPwd"] ?? Environment.GetEnvironmentVariable("ModeratorPwd")!);
                await userManager.AddToRoleAsync(moderatorUser, _moderatorRole);

            }
        }
    }
}
