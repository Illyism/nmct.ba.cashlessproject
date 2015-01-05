namespace nmct.ba.cashlessproject.web.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using nmct.ba.cashlessproject.web.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<nmct.ba.cashlessproject.web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "nmct.ba.cashlessproject.web.Models.ApplicationDbContext";
        }

        protected override void Seed(nmct.ba.cashlessproject.web.Models.ApplicationDbContext context)
        {
            string roleAdmin = "Administrator";
            string roleNormalUser = "User";
            IdentityResult roleResult;

            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!RoleManager.RoleExists(roleNormalUser))
                roleResult = RoleManager.Create(new IdentityRole(roleNormalUser));

            if (!RoleManager.RoleExists(roleAdmin))
                roleResult = RoleManager.Create(new IdentityRole(roleAdmin));

            if (!context.Users.Any(u => u.Email.Equals("hello@illyism.com")))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser()
                {
                    Name = "Ismanalijev",
                    FirstName = "Ilias",
                    Email = "hello@illyism.com",
                    UserName = "hello@illyism.com",
                };

                manager.Create(user, "P@ssw0rd");
                manager.AddToRole(user.Id, roleAdmin);
            }
        }
    }
}
