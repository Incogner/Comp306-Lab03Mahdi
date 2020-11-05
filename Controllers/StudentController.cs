using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Lab03Mahdi.Areas.Identity.Data;
using Lab03Mahdi.Models;
using Lab03Mahdi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lab03Mahdi.Controllers
{
    [Route("~/Students/[action]")]
    public class StudentController : Controller
    {
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        private List<Teacher> teachers;
        private Comments comments;
        private Courses courses;
        private Appointments appointments;
        private AppUser currentUser;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<StudentController> _logger;
        

        public StudentController(SignInManager<AppUser> signInManager,
            ILogger<StudentController> logger,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.CACentral1);
            context = new DynamoDBContext(client);
            courses = new Courses();
            appointments = new Appointments();
            comments = new Comments();
        }

        public async Task<IActionResult> DashAsync()
        {
            await GetStudentAsync();
            await LoadData();
            StudentDash vm = new StudentDash();
            vm.Appointments = appointments.AllAppointments;
            vm.Comments = comments.AllComments;
            vm.Courses = courses.AllCourses;
            return View("StudentDash", vm);
        }

        private async Task<AppUser> GetStudentAsync()
        {
            currentUser = await _userManager.GetUserAsync(User);
            return currentUser;
        }


        private async Task LoadData()
        {
            appointments = context.LoadAsync<Appointments>(currentUser.Id).Result;
            comments = context.LoadAsync<Comments>(currentUser.Id).Result;
            courses = context.LoadAsync<Courses>(currentUser.Id).Result;

            if(comments.AllComments == null)
            {
                comments.AllComments = new List<Comment>();
            }
            if(appointments.AllAppointments == null)
            {
                appointments.AllAppointments = new List<Appointment>();
            }
            if (courses.AllCourses == null)
            {
                courses.AllCourses = new List<Course>();
            }

            comments.UserId = currentUser.Id;
            courses.UserId = currentUser.Id;
            appointments.UserId = currentUser.Id;
            

            if (comments.AllComments.Count < 1)
            {
                for (int x = 0; x < 5; x++)
                {
                    Comment yak = new Comment();
                    yak.Content = "Comment content 1" + x;
                    yak.Likes = 1 + x;
                    yak.SenderId = currentUser.Id;
                    yak.RecieverId = "Reciver no 1" + x;
                    yak.TimeStamp = DateTime.Now.ToLocalTime();
                    comments.AllComments.Add(yak);
                }
                       
            }

            if (appointments.AllAppointments.Count < 1)
            {
                for (int x = 0; x < 7; x++)
                {
                    Appointment yak1 = new Appointment();
                    yak1.Location = "appointment location 1" + x;
                    yak1.Confirmed = true;
                    yak1.StudentId = currentUser.Id;
                    yak1.TeacherId = "Teacher 1" + x;
                    yak1.ScheduledTime = DateTime.Now.ToLocalTime();
                    appointments.AllAppointments.Add(yak1);
                }
                        
            }

            if (courses.AllCourses.Count < 1)
            {
                for (int x = 0; x < 6; x++)
                {
                    Course yak2 = new Course();
                    yak2.CourseName = "Course 1" + x;
                    yak2.CourseDescription = "Course Description 1" + x;
                    yak2.DateAdded = DateTime.Now.ToLocalTime();
                    yak2.Semester = x.ToString();
                    courses.AllCourses.Add(yak2);
                }
                        
            }
            await context.SaveAsync(courses);
            await context.SaveAsync(comments);
            await context.SaveAsync(appointments);
        }

        //private async Task<List<Course>> LoadCoursesAsync()
        //{
        //    if (courses == null)
        //    {
        //        courses = new List<Course>();
        //    }

        //    try
        //    {
        //        await GetStudentAsync();
        //        var response = client.BatchGetItemAsync(GetRequest("Course", currentUser.Id));
        //        var result = response.Result.Responses;

        //        foreach (var results in result.Values)
        //        {
        //            foreach (var item in results)
        //            {
        //                Course course = (Course)item;
        //                courses.Add(course);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e.Message);
        //    }
        //    return courses;
        //}

        //private BatchGetItemRequest GetRequest(string tableName, string userId)
        //{
        //    var req = new BatchGetItemRequest
        //    {
        //        RequestItems = new Dictionary<string, KeysAndAttributes>()
        //        {
        //            { tableName,
        //              new KeysAndAttributes
        //              {
        //                Keys = new List<Dictionary<string, AttributeValue>>()
        //                {
        //                  new Dictionary<string, AttributeValue>()
        //                  {
        //                    { "UserId", new AttributeValue { S = userId } }
        //                  }
        //                }
        //              }
        //            }
        //        }
        //    };
        //    return req;
        //}
    }
}
