using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Lab03Mahdi.Areas.Identity.Data;
using Lab03Mahdi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lab03Mahdi.Controllers
{
    public class CourseController : Controller
    {
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        private AppUser currentUser;
        private Courses courses;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public CourseController(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.CACentral1);
            context = new DynamoDBContext(client);
            courses = new Courses();
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    CommentViewModel vm = new CommentViewModel();
        //    vm.returnUrl = Request.Query["returnUrl"];
        //    return View(vm);
        //}

        [HttpGet]
        public IActionResult ShowCourses()
        {
            courses = context.LoadAsync<Courses>("general").Result;
            List<Course> courseList = courses.AllCourses;
            return View(courseList);
        }

        [HttpGet]
        public IActionResult Register(string id, string returnUrl)
        {
            GetStudentAsync().Wait();
            courses = context.LoadAsync<Courses>("general").Result;
            UserData userData = context.LoadAsync<UserData>(currentUser.Id).Result;
            Course  c = courses.AllCourses.Where(x => x.Guid == Guid.Parse(id)).FirstOrDefault();
            if(currentUser.UserType == UserType.STUDENT)
            {
                if (c.Students == null)
                {
                    c.Students = new List<string>();
                }
                if (!c.Students.Contains(currentUser.Id))
                {
                    c.Students.Add(currentUser.Id);
                    context.SaveAsync(courses);
                }
            } else
            {
                if (c.Teachers == null)
                {
                    c.Teachers = new List<string>();
                }
                if (!c.Teachers.Contains(currentUser.Id))
                {
                    c.Teachers.Add(currentUser.Id);
                    context.SaveAsync(courses);
                }
            }
            if(userData == null)
            {
                userData = new UserData();
                userData.UserId = currentUser.Id;
            }
            
            if(userData.CourseList == null)
            {
                userData.CourseList = new List<string>();
            }
            if (!userData.CourseList.Contains(c.Guid.ToString())){
                userData.CourseList.Add(c.Guid.ToString());
                context.SaveAsync(userData);
            }

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Drop(string id, string returnUrl)
        {
            GetStudentAsync().Wait();
            courses = context.LoadAsync<Courses>("general").Result;
            UserData userData = context.LoadAsync<UserData>(currentUser.Id).Result;
            Course c = courses.AllCourses.Where(x => x.Guid == Guid.Parse(id)).FirstOrDefault();
            string courseCode = c.Guid.ToString();

            if (currentUser.UserType == UserType.STUDENT)
            {
                if (c.Students != null && c.Students.Contains(currentUser.Id))
                {
                    c.Students.Remove(currentUser.Id);
                    context.SaveAsync(courses);
                }
            } else
            {
                if (c.Teachers != null && c.Teachers.Contains(currentUser.Id))
                {
                    c.Teachers.Remove(currentUser.Id);
                    context.SaveAsync(courses);
                }
            }

            
            if (userData.CourseList != null && userData.CourseList.Contains(courseCode))
            {
                userData.CourseList.Remove(courseCode);
                if(userData.CourseList.Count < 1)
                {
                    userData.CourseList = null;
                }
                context.SaveAsync<UserData>(userData);
            }

            return LocalRedirect(returnUrl);
        }

        private async Task<AppUser> GetStudentAsync()
        {
            currentUser = await _userManager.GetUserAsync(User);
            return currentUser;
        }
    }
}
