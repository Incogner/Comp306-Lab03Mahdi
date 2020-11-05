using Lab03Mahdi.Areas.Identity.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Models
{
    public class Teacher : AppUser
    {

        public List<Appointment> Appointments { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Course> Courses { get; set; }
        
        public Teacher()
        {
            this.UserType = UserType.TEACHER;
            Appointments = new List<Appointment>();
            Comments = new List<Comment>();
            Courses = new List<Course>();
        }
    }

}
