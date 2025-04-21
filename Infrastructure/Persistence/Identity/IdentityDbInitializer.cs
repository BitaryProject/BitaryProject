using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence.Identity
{
    public static class IdentityDbInitializer
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IdentityContext>>();

            // Define required roles based on the Role enum
            var requiredRoles = new[] { 
                Role.PetOwner.ToString(), 
                Role.Doctor.ToString(), 
                Role.Admin.ToString() 
            };

            // Create roles based on the Role enum
            foreach (var roleName in requiredRoles)
            {
                try
                {
                    // Check if the role already exists
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        // Create the role if it doesn't exist
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                        logger.LogInformation("Created role: {RoleName}", roleName);
                    }
                    else
                    {
                        logger.LogDebug("Role already exists: {RoleName}", roleName);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error creating role: {RoleName}", roleName);
                }
            }
        }
    }
} 