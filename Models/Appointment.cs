using Amazon.DynamoDBv2.DataModel;
using Lab03Mahdi.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Models
{
    [DynamoDBTable("Appointment")]
    public class Appointments
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }
        
        public List<Appointment> AllAppointments { get; set; }
        
    }

    public class Appointment
    {
        public Guid Guid { get; set; }
        public string StudentId { get; set; }
        public string TeacherId { get; set; }
        public bool Confirmed { get; set; }
        public string Location { get; set; }
        public DateTime ScheduledTime { get; set; }
        public Appointment()
        {
            Guid = Guid.NewGuid();
        }
    }

}
