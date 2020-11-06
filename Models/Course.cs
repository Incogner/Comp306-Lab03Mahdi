using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Models
{
    [DynamoDBTable("Course")]
    public class Courses
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }
        public List<Course> AllCourses { get; set; }
        

        //public static explicit operator Course(Dictionary<string, AttributeValue> v)
        //{
        //    Course course = new Course();
        //    foreach(var att in v)
        //    {
        //        switch (att.Key)
        //        {
        //            case "UserId":
        //                course.UserId = att.Value.S;
        //                break;
        //            case "CourseName":
        //                course.CourseName = att.Value.S;
        //                break;
        //            case "Semester":
        //                course.Semester = att.Value.S;
        //                break;
        //            case "CourseDescription":
        //                course.CourseDescription = att.Value.S;
        //                break;
        //            case "DateAdded":
        //                course.DateAdded = DateTime.Parse(att.Value.S);
        //                break;
        //        }
        //    }
        //    return course;
        //}
    }


    public class Course
    {
        public Guid Guid { get; set; }
        public string CourseName { get; set; }
        public string Semester { get; set; }
        public string CourseDescription { get; set; }
        public DateTime DateAdded { get; set; }
        public List<string> Students { get; set; }
        public List<string> Teachers { get; set; }

        public Course()
        {
            Guid = Guid.NewGuid();
        }
    }
}
