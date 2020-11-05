using Lab03Mahdi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.ViewModels
{
    public class StudentDash
    {
        public List<Course> Courses { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Appointment> Appointments { get; set; }
    }
}
