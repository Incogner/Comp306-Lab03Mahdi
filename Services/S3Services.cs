using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Services
{
    public class S3Services
    {
        //private const string bucketName = "lab03-resumes";
        ////private string keyName = "*** provide a name for the uploaded object ***";
        ////private string filePath = "*** provide the full path name of the file to upload ***";
        //// Specify your bucket region (an example region is shown).
        //private static readonly RegionEndpoint bucketRegion = RegionEndpoint.CACentral1;
        //private static IAmazonS3 client = new AmazonS3Client(bucketRegion);

        //public S3Services()
        //{
        //    client = new AmazonS3Client(bucketRegion);
        //    UploadFileAsync("asdasdasdasd", "asdasdasdasdasdasdasdasd").Wait();
        //}

        //public static async Task<PutObjectResponse> WritingAnObjectAsync()
        //{
        //    PutObjectResponse response = null;

        //    try
        //    {
        //        var putRequest = new PutObjectRequest
        //        {
        //            BucketName = bucketName,
        //            Key = "sds",
        //            FilePath = "sdsd",
        //            ContentType = "text/plain"
        //        };

        //        response = await client.PutObjectAsync(putRequest);
        //    }
        //    catch (AmazonS3Exception e)
        //    {
        //        //uploadlabel.Content = e.Message;
        //    }
        //    return response;
        //}

        //private static async Task UploadFileAsync(string filePath, string keyId)
        //{
        //    try
        //    {
        //        var fileTransferUtility =
        //            new TransferUtility(client);

        //        // Option 1. Upload a file. The file name is used as the object key name.
        //        //await fileTransferUtility.UploadAsync(filePath, bucketName);
        //        //Console.WriteLine("Upload 1 completed");

        //        // Option 2. Specify object key name explicitly.
        //        //await fileTransferUtility.UploadAsync(filePath, bucketName, keyName);
        //        //Console.WriteLine("Upload 2 completed");

        //        // Option 3. Upload data from a type of System.IO.Stream.
        //        using (var fileToUpload =
        //            new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //        {
        //            await fileTransferUtility.UploadAsync(fileToUpload,
        //                                       bucketName, keyId);
        //        }
        //        //Console.WriteLine("Upload 3 completed");

        //        // Option 4. Specify advanced settings.
        //        //var fileTransferUtilityRequest = new TransferUtilityUploadRequest
        //        //{
        //        //    BucketName = bucketName,
        //        //    FilePath = filePath,
        //        //    StorageClass = S3StorageClass.StandardInfrequentAccess,
        //        //    PartSize = 6291456, // 6 MB.
        //        //    Key = keyName,
        //        //    CannedACL = S3CannedACL.PublicRead
        //        //};
        //        //fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
        //        //fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

        //        //await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
        //        //Console.WriteLine("Upload 4 completed");
        //    }
        //    catch (AmazonS3Exception e)
        //    {
        //        Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }

        //}
    }


}
