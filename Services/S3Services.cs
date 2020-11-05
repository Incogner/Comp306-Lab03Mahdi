using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Services
{
    public class S3Services
    {

        private static IAmazonS3 client = new AmazonS3Client(Amazon.RegionEndpoint.CACentral1);
    }
}
