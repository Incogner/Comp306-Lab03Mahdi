using Lab03Mahdi.Areas.Identity.Data;
using Lab03Mahdi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.ViewModels
{
    public class AppointmentViewModel
    {
        public AppUser Teacher { get; set; }
        public string ReturnUrl { get; set; }
        public Appointment Appointment { get; set; }
    }
}
