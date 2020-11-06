using Amazon.DynamoDBv2.DataModel;
using Lab03Mahdi.Areas.Identity.Data;
using Lab03Mahdi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.ViewModels
{
    public class TeacherDash 
    {
        public List<Course> Courses { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Appointment> Appointments { get; set; }
        public UserData UserData { get; set; }
        public AppUser TeacherUser { get; set; }
        public IFormFile ResumeFile { get; set; }

        public IFormFile ImageFile { get; set; }
        public string ReturnUrl { get; set; }
    }
}
