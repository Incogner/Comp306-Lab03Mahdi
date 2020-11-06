using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
    [Route("~/Teachers/[action]")]
    public class TeacherController : Controller
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

        public TeacherController(SignInManager<AppUser> signInManager,
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
            bucketRegion = RegionEndpoint.CACentral1;
            s3Client = new AmazonS3Client(bucketRegion);
            userData = new UserData();
        }

        public async Task<IActionResult> DashAsync()
        {
            await GetUserAsync();
            await LoadData();
            TeacherDash vm = new TeacherDash();
            vm.Appointments = appointments.AllAppointments;
            vm.Comments = comments.AllComments;
            vm.Courses = courses;
            vm.TeacherUser = currentUser;
            vm.UserData = userData;

            return View("TeacherDash", vm);
        }

        private async Task<AppUser> GetUserAsync()
        {
            currentUser = await _userManager.GetUserAsync(User);
            return currentUser;
        }
        

        private async Task LoadData()
        {
            appointments = context.LoadAsync<Appointments>(currentUser.Id).Result;
            comments = context.LoadAsync<Comments>(currentUser.Id).Result;
            userData = context.LoadAsync<UserData>(currentUser.Id).Result;
            if(userData != null && userData.CourseList != null)
            {
                foreach(string s in userData.CourseList)
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

            await context.SaveAsync(comments);
            await context.SaveAsync(appointments);
            await context.SaveAsync(userData);
        }

        // Complete Upload File Method with conditions
        [HttpPost]
        public IActionResult UploadResume(TeacherDash vm)
        {
            string[] accepted = { ".doc", ".docx", ".pdf", ".txt", ".odt" };
            int pointIndex = vm.ResumeFile.FileName.LastIndexOf('.');
            string docFormat = vm.ResumeFile.FileName.Substring(pointIndex, vm.ResumeFile.FileName.Length - pointIndex);

            if (vm == null || vm.ResumeFile == null || !accepted.Contains(docFormat))
            {
                return RedirectToAction("Dash");
            }
            if(currentUser == null)
            {
                GetUserAsync().Wait();
            }

            string resumeKey = currentUser.Id + "-resume"+docFormat;

            UploadFileAsync(resumeKey, vm.ResumeFile).Wait();

            userData = context.LoadAsync<UserData>(currentUser.Id).Result;
            if(userData == null)
            {
                userData = new UserData();
                userData.UserId = currentUser.Id;
            }
            userData.ResumeLink = S3Link.Create(context, bucketName, resumeKey, bucketRegion);
            userData.ResumeTitle = currentUser.Email + " - Resume - "+DateTime.Now.ToLocalTime().ToString("dddd dd MMMM");
            userData.ResumeStringLink = "http://" + bucketName + ".s3.ca-central-1.amazonaws.com/" + resumeKey;

            var response = s3Client.GetObjectAsync(bucketName, resumeKey).Result;

            if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                context.SaveAsync(userData);
                return RedirectToAction("Dash");
            } else
            {
                return LocalRedirect("~/error");
            }
        }

        [HttpPost]
        public IActionResult UploadImage(TeacherDash vm)
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
    }
}
