using Microsoft.AspNetCore.Identity;
using GXReservationAPI.Constants;
using GXReservationAPI.Model;

namespace GXReservationAPI.Data
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            await CreateUserWithRole(userManager, "admin@gmail.com", "Admin123!", Roles.Admin);
            await CreateUserWithRole(userManager, "employee@gmail.com", "Employee123!", Roles.Employee);

        }

        public static async Task CreateUserWithRole(
            UserManager<AppUser> userManager,
            string email,
            string password,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new AppUser
                {
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Assign the user to the specified role
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Failed to create user with email {email}. Errors: {string.Join(",", result.Errors)}");
                }
            }
        }

    }
}
