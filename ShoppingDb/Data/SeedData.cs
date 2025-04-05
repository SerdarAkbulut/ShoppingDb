using Microsoft.AspNetCore.Identity;
using ShoppingApi.Entity;

namespace ShoppingApi.Data
{
    public class SeedData
    {
        public static async  void Initialize(IApplicationBuilder app) 
        { 
            var userManager=app.ApplicationServices
                .CreateScope()
                .ServiceProvider.GetRequiredService<UserManager<User>>();

            var roleManager=app.ApplicationServices.CreateScope()
                .ServiceProvider
                .GetRequiredService<RoleManager<Role>>();

            if (!roleManager.Roles.Any()) 
            {
            var customer=new Role { Name="Customer"};
            var admin=new Role { Name= "Admin" };

                await roleManager.CreateAsync(customer);
                await roleManager.CreateAsync(admin);
            }

            if (!userManager.Users.Any())
            {
                var customer = new User { Name = "Kullanıcı", UserName = "kullanici", Email = "kullanici@gmail.com" };
                var admin = new User { Name = "Admin", UserName = "admin", Email = "admin@gmail.com" };

                await userManager.CreateAsync(customer,"Kullanici_123");
                await userManager.AddToRoleAsync(customer, "Customer");

                await userManager.CreateAsync(admin, "Admin_123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
