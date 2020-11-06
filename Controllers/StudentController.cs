using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Transfer;
using Lab03Mahdi.Areas.Identity.Data;
using Lab03Mahdi.Models;
using Lab03Mahdi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lab03Mahdi.Controllers
{
    [Route("~/Students/[action]")]
    public class StudentController : Controller
    {
        private const string bucketName = "lab03-resumes";
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        private Comments comments;
        private List<Course> courses;
        private Appointments appointments;
        private AppUser currentUser;
        private UserData userData;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<StudentController> _logger;
        private readonly RegionEndpoint bucketRegion;
        private static IAmazonS3 s3Client;


        public StudentController(SignInManager<AppUser> signInManager,
            ILogger<StudentController> logger,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.CACentral1);
            context = new DynamoDBContext(client);
            courses = new List<Course>();
            appointments = new Appointments();
            comments = new Comments();
            userData = new UserData();
            bucketRegion = RegionEndpoint.CACentral1;
            s3Client = new AmazonS3Client(bucketRegion);
        }

        public async Task<IActionResult> DashAsync()
        {
            await GetUserAsync();
            await LoadData();
            StudentDash vm = new StudentDash();
            vm.Appointments = appointments.AllAppointments;
            vm.Comments = comments.AllComments;
            vm.Courses = courses;
            vm.StudentUser = currentUser;
            vm.UserData = userData;
            return View("StudentDash", vm);
        }

        public IActionResult ShowTeacher(string id, string returnUrl)
        {
            if(id == null || id == "")
            {
                return RedirectToAction("Dash");
            }
            StudentDash vm = new StudentDash();
            AppUser teacher = _userManager.FindByIdAsync(id).Result;
            UserData teacherData = context.LoadAsync<UserData>(teacher.Id).Result;
            Comments teacherComments = context.LoadAsync<Comments>(teacher.Id).Result;
            if(teacherData != null && teacherData.PictureStringLink != null)
            {
                teacher.Picture = teacherData.PictureStringLink;
                teacher.Resume = teacherData.ResumeStringLink??"";
            }
            vm.TeacherUser = teacher;
            if(teacherComments != null && teacherComments.AllComments != null)
            {
                vm.Comments = teacherComments.AllComments;
            }
            return View(vm);
        }

        [HttpPost]
        public IActionResult ShowBook(string teacherId)
        {

            AppointmentViewModel vm = new AppointmentViewModel();
            vm.ReturnUrl = "/Students/ShowTeacher?id=" + teacherId;
            vm.Teacher = _userManager.FindByIdAsync(teacherId).Result;


            return View("Book", vm);
        }

        [HttpPost]
        public IActionResult Book(AppointmentViewModel vm)
        {
            GetUserAsync().Wait();
            if (vm.Appointment == null)
            {
                return RedirectToAction("ShowBook", vm.Appointment.TeacherId);
            }

            vm.ReturnUrl = "/Students/ShowTeacher?id=" + vm.Appointment.TeacherId;
            vm.Teacher = _userManager.FindByIdAsync(vm.Appointment.TeacherId).Result;
            vm.Appointment.StudentId = currentUser.Id;
            vm.Appointment.Confirmed = true;

            appointments = context.LoadAsync<Appointments>(currentUser.Id).Result;
            if (appointments == null)
            {
                appointments = new Appointments();
                appointments.UserId = vm.Teacher.Id;
            }
            if (appointments.AllAppointments == null)
            {
                appointments.AllAppointments = new List<Appointment>();
            }

            appointments.AllAppointments.Add(vm.Appointment);
            context.SaveAsync(appointments);

            Appointments teacherAppointments = context.LoadAsync<Appointments>(vm.Teacher.Id).Result;
            if(teacherAppointments == null)
            {
                teacherAppointments = new Appointments();
                teacherAppointments.UserId = vm.Teacher.Id;
            }
            if(teacherAppointments.AllAppointments == null)
            {
                teacherAppointments.AllAppointments = new List<Appointment>();
            }
            teacherAppointments.AllAppointments.Add(vm.Appointment);
            context.SaveAsync(teacherAppointments);

            return LocalRedirect(vm.ReturnUrl);
        }

        private async Task<AppUser> GetUserAsync()
        {
            currentUser = await _userManager.GetUserAsync(User);
            return currentUser;
        }


        private async Task LoadData()
        {
            //Courses courses2 = new Courses();
            //courses2.UserId = "general";
            //courses2.AllCourses = new List<Course>();
            //if (courses2.AllCourses.Count < 1)
            //{
                
            //        Course yak2 = new Course();
            //        yak2.CourseName = "Advanced Functions";
            //        yak2.CourseDescription = "This course is to recamp high school classes";
            //        yak2.DateAdded = DateTime.Now.ToLocalTime();
            //        yak2.Semester = "1";
            //        courses2.AllCourses.Add(yak2);

            //    Course yak23 = new Course();
            //    yak23.CourseName = "Communication 01";
            //    yak23.CourseDescription = "English course";
            //    yak23.DateAdded = DateTime.Now.ToLocalTime();
            //    yak23.Semester = "2";
            //    courses2.AllCourses.Add(yak23);

            //    Course yak24 = new Course();
            //    yak24.CourseName = "Java Development";
            //    yak24.CourseDescription = "Java Programming and Eclipse introduction";
            //    yak24.DateAdded = DateTime.Now.ToLocalTime();
            //    yak24.Semester = "3";
            //    courses2.AllCourses.Add(yak24);

            //    Course yak25 = new Course();
            //    yak25.CourseName = "C# programming 2";
            //    yak25.CourseDescription = "Advance C# desktop programming course";
            //    yak25.DateAdded = DateTime.Now.ToLocalTime();
            //    yak25.Semester = "3";
            //    courses2.AllCourses.Add(yak25);

            //    Course yak26 = new Course();
            //    yak26.CourseName = "JavaScript Gaming";
            //    yak26.CourseDescription = "Elctive course for Software Engineering";
            //    yak26.DateAdded = DateTime.Now.ToLocalTime();
            //    yak26.Semester = "4";
            //    courses2.AllCourses.Add(yak26);

            //    Course yak27 = new Course();
            //    yak27.CourseName = "Database Functions";
            //    yak27.CourseDescription = "Oracle Pl/SQL course";
            //    yak27.DateAdded = DateTime.Now.ToLocalTime();
            //    yak27.Semester = "5";
            //    courses2.AllCourses.Add(yak27);

            //    Course yak28 = new Course();
            //    yak28.CourseName = "Data Structure";
            //    yak28.CourseDescription = "Algorith and data structure is the fundation of programming";
            //    yak28.DateAdded = DateTime.Now.ToLocalTime();
            //    yak28.Semester = "6";
            //    courses2.AllCourses.Add(yak28);

            //    await context.SaveAsync(courses2);
            //}
            appointments = context.LoadAsync<Appointments>(currentUser.Id).Result;
            comments = context.LoadAsync<Comments>(currentUser.Id).Result;
            userData = context.LoadAsync<UserData>(currentUser.Id).Result;
            if (userData != null && userData.CourseList != null)
            {
                foreach (string s in userData.CourseList)
                {
                    Course c = context.LoadAsync<Courses>("general").Result.AllCourses.Where(x => x.Guid == Guid.Parse(s)).FirstOrDefault();
                    courses.Add(c);
                }
            }
            if (userData == null)
            {
                userData = new UserData();
            }
            if (userData.CourseList == null)
            {
                userData.CourseList = new List<string>();
            }
            if (comments == null)
            {
                comments = new Comments();
            }
            if (comments.AllComments == null)
            {
                comments.AllComments = new List<Comment>();
            }
            if (appointments == null)
            {
                appointments = new Appointments();
            }
            if (appointments.AllAppointments == null)
            {
                appointments.AllAppointments = new List<Appointment>();
            }

            userData.UserId = currentUser.Id;
            comments.UserId = currentUser.Id;
            appointments.UserId = currentUser.Id;
            

            //if (comments.AllComments.Count < 1)
            //{
            //    for (int x = 0; x < 5; x++)
            //    {
            //        Comment yak = new Comment();
            //        yak.Content = "Comment content 1" + x;
            //        yak.Likes = 1 + x;
            //        yak.SenderId = currentUser.Id;
            //        yak.RecieverId = "Reciver no 1" + x;
            //        yak.TimeStamp = DateTime.Now.ToLocalTime();
            //        comments.AllComments.Add(yak);
            //    }
                       
            //}

            //if (appointments.AllAppointments.Count < 1)
            //{
            //    for (int x = 0; x < 7; x++)
            //    {
            //        Appointment yak1 = new Appointment();
            //        yak1.Location = "appointment location 1" + x;
            //        yak1.Confirmed = true;
            //        yak1.StudentId = currentUser.Id;
            //        yak1.TeacherId = "Teacher 1" + x;
            //        yak1.ScheduledTime = DateTime.Now.ToLocalTime();
            //        appointments.AllAppointments.Add(yak1);
            //    }
                        
            //}
            await context.SaveAsync(userData);
            await context.SaveAsync(comments);
            await context.SaveAsync(appointments);
        }

        [HttpPost]
        public IActionResult UploadImage(StudentDash vm)
        {
            string[] accepted = { ".png", ".jpg", ".jpeg", ".gif" };
            int pointIndex = vm.ImageFile.FileName.LastIndexOf('.');
            string docFormat = vm.ImageFile.FileName.Substring(pointIndex, vm.ImageFile.FileName.Length - pointIndex);

            if (vm == null || vm.ImageFile == null || !accepted.Contains(docFormat))
            {
                return RedirectToAction("Dash");
            }
            if (currentUser == null)
            {
                GetUserAsync().Wait();
            }

            string imageKey = currentUser.Id + "-image" + docFormat;

            UploadFileAsync(imageKey, vm.ImageFile).Wait();

            userData = context.LoadAsync<UserData>(currentUser.Id).Result;
            if (userData == null)
            {
                userData = new UserData();
                userData.UserId = currentUser.Id;
            }
            userData.PictureLink = S3Link.Create(context, bucketName, imageKey, bucketRegion);
            userData.PictureTitle = currentUser.Email + " - Image - " + DateTime.Now.ToLocalTime().ToString("dddd dd MMMM");
            userData.PictureStringLink = "http://" + bucketName + ".s3.ca-central-1.amazonaws.com/" + imageKey;

            var response = s3Client.GetObjectAsync(bucketName, imageKey).Result;

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                context.SaveAsync(userData);
                return RedirectToAction("Dash");
            }
            else
            {
                return LocalRedirect("~/error");
            }
        }

        private static async Task UploadFileAsync(string keyId, IFormFile file)
        {
            try
            {
                //var fileToUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read)
                var fileTransferUtility =
                    new TransferUtility(s3Client);
                Stream stream = new MemoryStream();



                using (file.CopyToAsync(stream))
                {
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = keyId,
                        CannedACL = S3CannedACL.PublicRead,
                        InputStream = stream
                    };
                    await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

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
