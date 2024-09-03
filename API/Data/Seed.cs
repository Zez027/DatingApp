using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
       public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync())
            {
                Console.WriteLine("Users already exist, skipping seeding.");
                return;
            }

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                
                user.UserName = user.UserName.ToLower();
                user.PassWordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PassWordSalt = hmac.Key;

                context.Users.Add(user);  // Ensure that users are being added to the context
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Users seeded successfully.");
        }
    }
}