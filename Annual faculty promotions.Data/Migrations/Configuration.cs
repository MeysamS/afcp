using System.Data.Entity.Validation;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public class Configuration : DbMigrationsConfiguration<Annual_faculty_promotions.Data.AfpContext>
    {

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Annual_faculty_promotions.Data.AfpContext context)
        {
            try
            {
                //  This method will be called after migrating to the latest version.

                //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
                //  to avoid creating duplicate seed data. E.g.
                //
                //    context.People.AddOrUpdate(
                //      p => p.FullName,
                //      new Person { FullName = "Andrew Peters" },
                //      new Person { FullName = "Brice Lambson" },
                //      new Person { FullName = "Rowan Miller" }
                //    );
                //
                var role = context.Roles.FirstOrDefault(x => x.Name == "User");
                if (role == null)
                    context.Roles.Add(new CustomRole("User", "کاربر"));

                role = context.Roles.FirstOrDefault(x => x.Name == "MG");
                if (role == null)
                    context.Roles.Add(new CustomRole("MG", "مدیرگروه"));

                role = context.Roles.FirstOrDefault(x => x.Name == "RD");
                if (role == null)
                    context.Roles.Add(new CustomRole("RD", "رئیس دانشکده"));

                role = context.Roles.FirstOrDefault(x => x.Name == "MA");
                if (role == null)
                    context.Roles.Add(new CustomRole("MA", "معاون آموزشی"));

                role = context.Roles.FirstOrDefault(x => x.Name == "RK");
                if (role == null)
                    context.Roles.Add(new CustomRole("RK", "رئیس کارگزینی"));

                role = context.Roles.FirstOrDefault(x => x.Name == "Admin");
                if (role == null)
                    context.Roles.Add(new CustomRole("Admin", "مدیر"));

                var bul = context.BaseUserLogins.Where(b => b.CodeMeli == 1 && b.CodeEstekhdam == "1").FirstOrDefault();
                if (bul == null)
                    context.BaseUserLogins.Add(new BaseUserLogin { CodeEstekhdam = "1", CodeMeli = 1,Active=true });

                var uni = context.UnivercityStructures.Where(x => x.Name == "دانشگاه").ToList();
                if (!uni.Any())
                {
                    context.UnivercityStructures.Add(new UnivercityStructure
                    {
                        HasChild = true,
                        Level = 1,
                        Name = "دانشگاه"
                    });
                }

                context.SaveChanges();
                //--------------------------------------------------------------------------
                var stage = context.Stages.FirstOrDefault(x => x.StageNumber == 1);
                if (stage == null)
                {
                    role = context.Roles.SingleOrDefault(r => r.Name == "User");
                    context.Stages.Add(new Stage { Name = "کاربر", StageNumber = 1, Role = role, Id = role.Id });
                }
                stage = context.Stages.FirstOrDefault(x => x.StageNumber == 2);
                if (stage == null)
                {
                    role = context.Roles.SingleOrDefault(r => r.Name == "MG");
                    context.Stages.Add(new Stage { Name = "مدیر گروه", StageNumber = 2, Role = role, Id = role.Id });
                }
                stage = context.Stages.FirstOrDefault(x => x.StageNumber == 3);
                if (stage == null)
                {
                    role = context.Roles.SingleOrDefault(r => r.Name == "RD");
                    context.Stages.Add(new Stage
                    {
                        Name = "رئیس دانشکده",
                        StageNumber = 3,
                        Role = role,
                        Id = role.Id
                    });
                }
                stage = context.Stages.FirstOrDefault(x => x.StageNumber == 4);
                if (stage == null)
                {
                    role = context.Roles.SingleOrDefault(r => r.Name == "MA");
                    context.Stages.Add(new Stage { Name = "معاون آمورشی", StageNumber = 4, Role = role, Id = role.Id });
                }
                stage = context.Stages.FirstOrDefault(x => x.StageNumber == 5);
                if (stage == null)
                {
                    role = context.Roles.SingleOrDefault(r => r.Name == "RK");
                    context.Stages.Add(new Stage { Name = "رئیس کارگزینی", StageNumber = 5, Role = role, Id = role.Id });
                }

                //===============================================
                var bulId = context.BaseUserLogins.Where(b => b.CodeMeli == 1 && b.CodeEstekhdam == "1").Select(u => u.BaseUserLoginId).FirstOrDefault();
                var getUser = context.Users.Where(x => x.Email == "admin@mail.com").SingleOrDefault();
                if (getUser == null)
                {
                    var hasher=new PasswordHasher();
                    var newUser = new AppUser
                    {
                        Email = "admin@mail.com",
                        UserName = "admin@mail.com",
                        EmailConfirmed = true,
                        PasswordHash = hasher.HashPassword("admin"),
                        SecurityStamp = Guid.NewGuid().ToString(),
                        BaseUserLoginId = bulId
                    };
                    context.Users.Add(newUser);
                    context.SaveChanges();
                }

                var userId = context.Users.Where(x => x.Email == "admin@mail.com").Select(s => s.Id).SingleOrDefault();
                var roleAdminId = context.Roles.Where(r => r.Name == "Admin").Select(s => s.Id).SingleOrDefault();
                var universe = context.UnivercityStructures.Where(x => x.Name == "دانشگاه").FirstOrDefault();
                if ((userId > 0) && (roleAdminId > 0) && (universe != null))
                {
                    var userrole =
                        context.UserRoles.Where(u => u.UserId == userId && u.RoleId == roleAdminId).FirstOrDefault();
                    if (userrole == null)
                        context.UserRoles.Add(new CustomUserRole
                        {
                            Department = universe,
                            RoleId = roleAdminId,
                            UserId = userId
                        });
                }
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                //foreach (var eve in e.EntityValidationErrors)
                //{
                //    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //    foreach (var ve in eve.ValidationErrors)
                //    {
                //        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //            ve.PropertyName, ve.ErrorMessage);
                //    }
                //}
                throw;
            }
        }
    }
}
