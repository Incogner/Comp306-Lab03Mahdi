using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Lab03Mahdi.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AppUser class
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public string Picture { get; set; }
        public string Resume { get; set; }
        public UserType UserType { get; set; }
    }

    public enum UserType
    {
        ADMIN,
        TEACHER,
        STUDENT,
        ANONYMOUS
    }
}
