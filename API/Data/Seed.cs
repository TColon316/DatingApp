using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            // Check if ANY users already exist in the Users table, and if so then break out of method
            //  (We do NOT want to run this method to seed the Users table with pre-determined users)
            if (await context.Users.AnyAsync()) return;

            // Read the User seed data in the UserSeedData.json file
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

            // Check if "users" has data or not
            if (users == null) return;

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                // Add Users to the database (not fully added yet though)
                context.Users.Add(user);
            }

            // Now the Users are saved to the database
            await context.SaveChangesAsync();
        }
    }
}