using Amazon.DynamoDBv2.DataModel;
using Lab03Mahdi.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Models
{
    [DynamoDBTable("Comment")]
    public class Comments
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }
        public List<Comment> AllComments { get; set; }

    }

    public class Comment
    {
        public Guid Guid { get; set; }
        public string SenderId { get; set; }
        public string RecieverId { get; set; }
        public string RecieverName { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Likes { get; set; }

        public Comment()
        {
            Guid = Guid.NewGuid();
        }
    }

}
