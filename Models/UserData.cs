using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Models
{
    [DynamoDBTable("UserData")]
    public class UserData
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }
        public S3Link ResumeLink { get; set; }
        public string ResumeStringLink { get; set; }
        public string ResumeTitle { get; set; }
        public S3Link PictureLink { get; set; }
        public string PictureStringLink { get; set; }
        public string PictureTitle { get; set; }
        public List<string> CourseList { get; set; }

    }
}
