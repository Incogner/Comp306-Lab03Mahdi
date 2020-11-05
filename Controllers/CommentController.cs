using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Lab03Mahdi.Areas.Identity.Data;
using Lab03Mahdi.Models;
using Lab03Mahdi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;

namespace Lab03Mahdi.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private AmazonDynamoDBClient client;
        private DynamoDBContext context;
        private AppUser currentUser;
        private Comments comments;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public CommentController(SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.CACentral1);
            context = new DynamoDBContext(client);
            comments = new Comments();
        }
        [HttpGet]
        public IActionResult Create()
        {
            CommentViewModel vm = new CommentViewModel();
            vm.returnUrl = Request.Query["returnUrl"];
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(CommentViewModel model)
        {
            if(model != null && model.comment != null)
            {
                context.SaveAsync(model.comment);
                return LocalRedirect(model.returnUrl);

            } else
            {
                return LocalRedirect("~/");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(string id, string returnUrl)
        {
            await GetStudentAsync();
            comments = context.LoadAsync<Comments>(currentUser.Id).Result;
            Comment toDelete = comments.AllComments.FirstOrDefault(x => x.Guid == Guid.Parse(id));
            if(toDelete != null)
            {
                comments.AllComments.Remove(toDelete);
            }
            await context.SaveAsync(comments);
            Thread.Sleep(1000);
            return LocalRedirect(returnUrl);
        }

        private async Task<AppUser> GetStudentAsync()
        {
            currentUser = await _userManager.GetUserAsync(User);
            return currentUser;
        }


        public IActionResult Update()
        {
            return View();
        }
    }
}
