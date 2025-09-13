using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (string roleName in roleNames)
        {
            bool roleExist = await roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        IdentityUser adminUser = await userManager.FindByEmailAsync("admin@exemplo.com");

        if (adminUser == null)
        {
            IdentityUser newAdminUser = new IdentityUser()
            {
                UserName = "admin@exemplo.com",
                Email = "admin@exemplo.com",
                EmailConfirmed = true,
            };

            IdentityResult result = await userManager.CreateAsync(newAdminUser, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
            }
        }
    }
}
