using Lab03Mahdi.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lab03Mahdi.Models
{
    public class Student : AppUser
    {
        public List<Appointment> Appointments { get; set; }
        public List<Course> Courses { get; set; }
        public List<Comment> Comments { get; set; }

        public Student():base()
        {
            this.UserType = UserType.STUDENT;
            Appointments = new List<Appointment>();
            Comments = new List<Comment>();
            Courses = new List<Course>();

        }
    }
}
