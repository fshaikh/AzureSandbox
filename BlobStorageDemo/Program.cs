using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Configuration;
using System.IO;
using Shared;
using AzureStorageClient;

namespace BlobStorageDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string storageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
            string accessKey = ConfigurationManager.AppSettings["AccessKey"];
            string container = ConfigurationManager.AppSettings["Container"];
            string blobName = ConfigurationManager.AppSettings["BlobName"];
            string filePath = ConfigurationManager.AppSettings["FilePath"];

            AzureBlobStorageClient blobStorageClient = new AzureBlobStorageClient(new StorageAccountInfo
            {
                StorageAccountName = storageAccountName,
                AccessKey = accessKey
            });

            //LeaseRequest leaseRequest = new LeaseRequest
            //{
            //    Blob = new Blob
            //    {
            //        ContainerName = container,
            //        BlobName = blobName
            //    },
            //    LeasePeriod = 60,
            //    TimeSpanType = TimeSpanType.Seconds
            //};

            //LeaseResponse leaseResponse = blobStorageClient.AcquireLease(leaseRequest);
            //Console.WriteLine(leaseResponse.LeaseId);

            //AcessToken token = blobStorageClient.GetDelegatedAccessToken(new Blob
            //{
            //    ContainerName = container,
            //    BlobName = blobName
            //},
            //new AccessPolicy
            //{
            //    ExpiryHours = 10,
            //    PolicyName = "SASPolicy",
            //    Permissions = Permission.Read | Permission.Write
            //});

            //blobStorageClient.GetBlobMetadata(new Blob
            //{
            //    ContainerName = container,
            //    BlobName = blobName
            //});

            blobStorageClient.UploadBlob(new Blob
            {
                ContainerName = container,
                BlobName = blobName,
                FilePath = filePath,
                //ContentType = "image/png",
                Metadata = GetBlobMetadata()
            });

            //using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            //{
            //    blobStorageClient.DownloadBlob(new Blob
            //    {
            //        BlobName = blobName,
            //        BlobStream = fileStream,
            //        ContainerName = container,
            //        FilePath = filePath
            //    });
            //}

            //blobStorageClient.ChangeBlob(new Blob
            //{
            //    ContainerName = container,
            //    BlobName = "SuperSports-Screen 1"
            //},
            //new Blob
            //{
            //    FilePath = @"C:\Users\Fshaikh\Pictures\SportsApp\Screen 1 - Select Sports - Copy.png"
            //});

            Console.ReadKey();
        }

        private static BlobMetadata GetBlobMetadata()
        {
            BlobMetadata metadata = new BlobMetadata();
            metadata.Add("Source", "Furqan Shaikh");
            return metadata;
        }
    }
}
