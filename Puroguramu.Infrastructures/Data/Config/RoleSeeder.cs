using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Puroguramu.Infrastructures.Data.Config;

public static class RoleSeeder
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "teacher", "student" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
