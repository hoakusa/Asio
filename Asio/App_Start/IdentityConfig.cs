using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using Asio.Models;

namespace IdentitySample.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                //RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole,string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext> 
    {
        protected override void Seed(ApplicationDbContext context) {
            var groups = new List<Group>
            {
                new Group {Name="DPI1SN"},
                new Group {Name="DPI2SN"},
                new Group {Name="DPI3SN"}
            };
            groups.ForEach(s => context.Groups.Add(s));
            context.SaveChanges();

            var semesters = new List<Semester>
            {
                new Semester {Name="S2014"},
                new Semester {Name="K2015"},
                new Semester {Name="S2015"},
                new Semester {Name="S2016"},
            };
            semesters.ForEach(s => context.Semesters.Add(s));
            context.SaveChanges();

            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db) {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null) {
                role = new IdentityRole(roleName);                
                var roleresult = roleManager.Create(role);

            }

            var user = userManager.FindByName(name);
            if (user == null) {
                user = new ApplicationUser { UserName = name};
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name)) {
                var result = userManager.AddToRole(user.Id, role.Name);
            }

            //Create Role Student + Teacher if doesnt exist
            string[] roles = new string[] {"Student", "Teacher"};

            for (var i = 0; i < roles.Length; i++) {
                role = roleManager.FindByName(roles[i]);
                if (role == null)
                {
                    role = new IdentityRole(roles[i]);
                    var roleresult = roleManager.Create(role);
                }
            }

            // Create default User
            var users = new List<ApplicationUser>
            {
                new ApplicationUser {UserName="t3mare00", FirstName="Mao", LastName="Reider"},
                new ApplicationUser {UserName="t2clba00", FirstName="Claude", LastName="Banta"},
                new ApplicationUser {UserName="t1ribr00", FirstName="Rina", LastName="Brun"},
                new ApplicationUser {UserName="t3nawi00", FirstName="Nanci", LastName="Willcutt"},
                new ApplicationUser {UserName="t2arot00", FirstName="Art", LastName="Otte"},
                new ApplicationUser {UserName="t1amnu00", FirstName="Amalia", LastName="Nugent"},
                new ApplicationUser {UserName="t3mafl00", FirstName="Marianela", LastName="Flavell"},
                new ApplicationUser {UserName="t2saes00", FirstName="Sacha", LastName="Ester"},
                new ApplicationUser {UserName="t1tene00", FirstName="Temeka", LastName="Newberry"},
                new ApplicationUser {UserName="t3debe00", FirstName="Dennis", LastName="Beachum"},
                new ApplicationUser {UserName="t2dosc00", FirstName="Doretta", LastName="Schwabe"},
                new ApplicationUser {UserName="t1teke00", FirstName="Terri", LastName="Kester"},
                new ApplicationUser {UserName="t3kebe00", FirstName="Kelle", LastName="Belnap"},
                new ApplicationUser {UserName="t2jipi00", FirstName="Jillian", LastName="Pirtle"},
                new ApplicationUser {UserName="t1aube00", FirstName="Audry", LastName="Bello"},
                new ApplicationUser {UserName="ojdati00", FirstName="Damaris", LastName="Tippit"},
                new ApplicationUser {UserName="ojjest00", FirstName="Jenise", LastName="Starrett"},
                new ApplicationUser {UserName="ojgica00", FirstName="Giovanna", LastName="Campos"},
                new ApplicationUser {UserName="ojrost00", FirstName="Rosia", LastName="Sturgeon"},
                new ApplicationUser {UserName="ojlase00", FirstName="Lamonica", LastName="Seyler"},
            };

            const string passwordUser = "Abcdef1234";

            for (var i = 0; i < users.Count; i++)
            {
                user = userManager.FindByName(users[i].UserName);
                if (user == null)
                {
                    user = new ApplicationUser { UserName = users[i].UserName, FirstName = users[i].FirstName, LastName = users[i].LastName };
                    var result = userManager.Create(user, passwordUser);
                    result = userManager.SetLockoutEnabled(user.Id, false);
                }

                string rolename = "";
                
                if (i < 15)
                {
                    // Set first 15 users as students
                    rolename = "Student";
                    var student = new Student { User = user };
                    db.Students.Add(student);
                    db.SaveChanges();
                }
                else {
                    // Set last 5 users as teachers
                    rolename = "Teacher";
                    var teacher = new Teacher { User = user };
                    db.Teachers.Add(teacher);
                    db.SaveChanges();
                }

                rolesForUser = userManager.GetRoles(user.Id);
                if (!rolesForUser.Contains(rolename))
                {
                    var result = userManager.AddToRole(user.Id, rolename);
                }

            }

            //Seed Course
            var courses = new List<Course>
            {
                new Course {Code="KY100", Name="IT English 1", Credits=3, TeacherId=1, SemesterId=1},
                new Course {Code="KY101", Name="IT English 2", Credits=3, TeacherId=2, SemesterId=2},
                new Course {Code="KY102", Name="Finish 1", Credits=3, TeacherId=3, SemesterId=3},
                new Course {Code="KY103", Name="Finish 2", Credits=3, TeacherId=4, SemesterId=4},
                new Course {Code="KY104", Name="Finish 3", Credits=3, TeacherId=5, SemesterId=1},
                new Course {Code="KY105", Name="Animation", Credits=6, TeacherId=1, SemesterId=2},
                new Course {Code="KY106", Name="Cloud Service", Credits=6, TeacherId=2, SemesterId=3},
                new Course {Code="KY107", Name="Usability Testing", Credits=6, TeacherId=3, SemesterId=4},
                new Course {Code="KY108", Name="Internet Programming", Credits=6, TeacherId=4, SemesterId=1},
                new Course {Code="KY109", Name="Physics", Credits=3, TeacherId=5, SemesterId=2}
            };
            try
            {
                courses.ForEach(s => db.Courses.Add(s));
                db.SaveChanges();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}