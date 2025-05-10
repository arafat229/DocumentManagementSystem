using DocumentManagementSystem.Helpers;
using DocumentManagementSystem.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace DocumentManagementSystem.Data
{
    public class SeedData
    {
        private readonly static List<string> roles = new List<string>
        {
 
         Privileges.canCreateUsers,
         Privileges.canViewUsers,
         Privileges.canDeleteUsers,
         Privileges.canUpdateUsers,
         Privileges.canCreateRoles,
         Privileges.canViewRoles,
         Privileges.canUpdateRoles,
         Privileges.canDeleteRoles,
         Privileges.canCreateSetting,
         Privileges.canViewSettings,
         Privileges.canUpdateSettings,
         Privileges.canDeleteSetting,
         Privileges.canViewDashboard,
         Privileges.canViewReport

        };
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();
            try
            {
                if (!userManager.Users.Any() && !roleManager.Roles.Any())
                {

                    #region Roles
                    SetRole(context, roleManager, Constants.SuperAdminRole);
                    SetRole(context, roleManager, Constants.ManagerRole);
                    #endregion

                    #region
                    var managerUser = new User
                    {
                        UserName = Constants.ManagerRole,
                        Email = "manager@app.com",
                        PhoneNumber = "0000000001",
                        CreatedAt = DateTime.UtcNow,
                        FullName = Constants.ManagerRole,
                        IsActive = true

                    };
                    var existingManager = userManager.FindByEmailAsync(managerUser.Email).Result;
                    //create manager if not exist 
                    if (existingManager is null)
                    {
                        var res = userManager.CreateAsync(managerUser, "manager@Password").GetAwaiter().GetResult();

                        if (res.Succeeded)
                        {

                            var managerRole = roleManager.FindByNameAsync(Constants.ManagerRole).Result;

                            if (managerRole != null)
                            {
                                userManager.AddToRoleAsync(managerUser, managerRole.Name).Wait();

                            }
                        }
                    }


                    #endregion
                }
                context.SaveChangesAsync();
            }
            catch (Exception ex)
            {


                Console.WriteLine(ex.StackTrace);
                 logger.LogError(ex,"An error occured while seeding the database");
            }

           
        }
      
            private static void SetRole(ApplicationDBContext context, RoleManager<Role>roleManager,string roleName)
        {
            var adminRole = new Role { Name = roleName.ToTitleCase(),NormalizedName=roleName.ToUpper() };
            var existingAdminRole = context.Roles.FirstOrDefault(x => x.Name == adminRole.Name);

            if(existingAdminRole == null)
             {
                var res = roleManager.CreateAsync(adminRole).Result;
                if(res.Succeeded)
                {

                    roles.Distinct().ToList().ForEach(r => roleManager.AddClaimAsync(adminRole,
                        new Claim(Constants.Privilege, r)).Wait());

                }
                else
                {
                    foreach(var r in roles)
                    {


                        var exist = context.RoleClaims.FirstOrDefault(x => x.RoleId == existingAdminRole.Id && r == x.ClaimValue);
                        if(exist == null)
                        {
                            var newClaim = new RoleClaim { RoleId = existingAdminRole.Id, ClaimValue = r, ClaimType = Constants.Privilege };
                            context.RoleClaims.Add(newClaim);
                           context.SaveChangesAsync();
                        }
                    }
                        

                }
             }

            }
        




    };
}

