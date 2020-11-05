using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab03Mahdi.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab03Mahdi.Controllers
{
    [Route("~/Teachers/[action]")]
    public class TeacherController : Controller
    {
        
        public IActionResult Dash()
        {

            return View("TeacherDash");
        }
    }
}
