using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageClient
{
    /// <summary>
    /// Class represents blob storage for Azure platform
    /// </summary>
    public class AzureBlobStorageClient : BlobStorageClientBase, IBlobStorageClient, IBlobLease
    {
        #region Members
        private CloudBlobClient _azureBlobClient = null;
        #endregion Members

        #region Constructors
        /// <summary>
        /// Initilizes a new instance of <see cref="AzureBlobStorageClient"/>
        /// </summary>
        /// <param name="accountInfo">Storage account info</param>
        public AzureBlobStorageClient(StorageAccountInfo accountInfo): base(accountInfo)
        {
            Initialize(accountInfo);
        }
        #endregion Constructors

        #region IBlobStorageClient Methods


        /// <summary>
        /// Downloads a blob from the blob storage. Blob stream and metadata is set in the passed in blob parameter
        /// </summary>
        /// <param name="blob">Blob to be downloaded</param>
        /// <param name="fetchMetadata">Whether to fetch blob metadata or not. In some storage platforms fetching metadata requires a separate call.</param>
        public void DownloadBlob(Blob blob, bool fetchMetadata = true)
        {
            // 4. Get access to container
            CloudBlobContainer blobContainer = _azureBlobClient.GetContainerReference(blob.ContainerName);

            // 5. Upload blob
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blob.BlobName);


            blockBlob.DownloadToStream(blob.BlobStream);

            if (fetchMetadata)
            {
                // Fetch metadata
                FetchMetadata(blob, blockBlob);
            }
        }


        /// <summary>
        /// Upload a blob to the storage.
        /// </summary>
        /// <param name="blob">Blob to be uploaded</param>
        /// <returns></returns>
        public bool UploadBlob(Blob blob)
        {
            try
            {
            // 4. Get access to container
            CloudBlobContainer blobContainer = _azureBlobClient.GetContainerReference(blob.ContainerName);

            // 5. Upload blob
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blob.BlobName);

            // 6. Set blob metadata
            SetBlobMetadata(blob,blockBlob);

            // Set Request options
            BlobRequestOptions blobRequestOptions = new BlobRequestOptions
            {
                ParallelOperationThreadCount = 2,
                SingleBlobUploadThresholdInBytes = 1024 * 1024 // 1 MB If blob size < 1 MB, upload as a single file. If blob size  > 1 MB, chunk into blocks
            };
            blockBlob.StreamWriteSizeInBytes = 1024 * 1024;

            _azureBlobClient.DefaultRequestOptions = blobRequestOptions;

            using (FileStream fileStream = new FileStream(blob.FilePath, FileMode.Open))
            {
                blockBlob.UploadFromStream(fileStream);
            }
            blobResponse.IsSuccess = true;
                BlobUri blobUri = new BlobUri
                {
                    PrimaryUri = blockBlob.StorageUri.PrimaryUri.ToString(),
                    SecondaryUri = blockBlob.StorageUri.SecondaryUri.ToString()
                };
                blob.BlobUri = blobUri;

                blobResponse.Blob = blob;
            catch (Exception exObj)
            {
                blobResponse.IsSuccess = false;
                blobResponse.FailureMessage = exObj.Message;
            }
            return blobResponse;
        }


        /// <summary>
        /// Gets Blob metadata.
        /// </summary>
        /// <param name="blob">Blob whose metadata is to be retreived</param>
        public void GetBlobMetadata(Blob blob)
        {
            // 4. Get access to container
            CloudBlobContainer blobContainer = _azureBlobClient.GetContainerReference(blob.ContainerName);

            // 5. Upload blob
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blob.BlobName);

           FetchMetadata(blob, blockBlob);
        }

        /// <summary>
        ///  Overwrite the blob.
        ///  Takes a snapshot of a BLOB, which preserves the content of a BLOB at a particular time.
        ///  We could, for example, use this to keep a historical record of changes to a file over time,
        ///  with the ability to retrieve an older version of that file.
        /// </summary>
        /// <param name="originalBlob">Blob to be overwritten</param>
        /// <param name="newBlob">New blob to replace the original blob</param>
        /// <returns></returns>
        public bool ChangeBlob(Blob originalBlob, Blob newBlob)
        {
            // 4. Get access to container
            CloudBlobContainer blobContainer = _azureBlobClient.GetContainerReference(originalBlob.ContainerName);

            // 5. Upload blob
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(originalBlob.BlobName);

            // Create a snapshot
            blockBlob.CreateSnapshot();

            // upload the new blob data overwriting the current blob
            using (Stream file = new FileStream(newBlob.FilePath, FileMode.Open))
            {
                blockBlob.UploadFromStream(file);
            }
            return true;
        }

        public AcessToken GetDelegatedAccessToken(Blob blob,AccessPolicy policy)
        {
            // 4. Get access to container
            CloudBlobContainer blobContainer = _azureBlobClient.GetContainerReference(blob.ContainerName);

            // Create permissions on the olicy
            SharedAccessBlobPolicy blobPolicy = new SharedAccessBlobPolicy()
            {
                Permissions = GetPermissions(policy.Permissions),
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(policy.ExpiryHours)
            };

            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
            containerPermissions.SharedAccessPolicies.Add(policy.PolicyName, blobPolicy);
            // Set permissions on the container
            blobContainer.SetPermissions(containerPermissions);

            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blob.BlobName);
            string sasToken = blockBlob.GetSharedAccessSignature(blobPolicy); 
            string sasUri = string.Format(CultureInfo.InvariantCulture, "{0}{1}", blockBlob.Uri, sasToken);

            AcessToken accessToken = new AcessToken
            {
                Token = sasUri
            };

            return accessToken;
        }

       

        #endregion IBlobStorageClient Methods

        #region IBlobLease Methods
        /// <summary>
        /// Tries to acquire lease on blob.
        /// </summary>
        /// <param name="leaseRequest">Lease request</param>
        /// <returns>Lease response</returns>
        public LeaseResponse AcquireLease(LeaseRequest leaseRequest)
        {
            Blob blob = leaseRequest.Blob;
            // 4. Get access to container
            CloudBlobContainer blobContainer = _azureBlobClient.GetContainerReference(blob.ContainerName);

            // 5. Upload blob
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blob.BlobName);

            TimeSpan? timeSpan = GetTimeSpan(leaseRequest);
            string leaseId = blockBlob.AcquireLease(timeSpan, string.IsNullOrEmpty(leaseRequest.LeaseId) ? null : leaseRequest.LeaseId);

            LeaseResponse leaseResponse = new LeaseResponse
            {
                IsSucces = true,
                LeaseId = leaseId
            };

            return leaseResponse;
        }

        
        #endregion IBlobLease Methods

        #region Support Methods

        private void Initialize(StorageAccountInfo accountInfo)
        {
            // 1. Storage credentials
            StorageCredentials credentials = new StorageCredentials(accountInfo.StorageAccountName, accountInfo.AccessKey);

            // 2. Storage account
            CloudStorageAccount storageAccount = new CloudStorageAccount(credentials, useHttps: true);

            // 3. Blob client
            _azureBlobClient = storageAccount.CreateCloudBlobClient();
        }
        private void SetBlobMetadata(Blob blob, CloudBlockBlob blockBlob)
        {
            foreach (var item in blob.Metadata.GetMetadata())
            {
                blockBlob.Metadata.Add(item.Key, item.Value);
            }

            blockBlob.Properties.ContentType = blob.ContentType;
        }

        private void FetchMetadata(Blob blob, CloudBlockBlob blockBlob)
        {
            blockBlob.FetchAttributes();

            BlobMetadata blobMetadata = new BlobMetadata();
            foreach (var item in blockBlob.Metadata)
            {
                blobMetadata.Add(item.Key, item.Value); blob.Metadata = blobMetadata;
            }
        }

        private SharedAccessBlobPermissions GetPermissions(Permission permissions)
        {
            SharedAccessBlobPermissions blobPermissions = SharedAccessBlobPermissions.None;
            if (permissions.HasFlag(Permission.Read))
            {
                blobPermissions |= SharedAccessBlobPermissions.Read;
            }
            if (permissions.HasFlag(Permission.Write))
            {
                blobPermissions |= SharedAccessBlobPermissions.Write;
            }
            if (permissions.HasFlag(Permission.List))
            {
                blobPermissions |= SharedAccessBlobPermissions.List;
            }
            return blobPermissions;
        }

        private TimeSpan? GetTimeSpan(LeaseRequest leaseRequest)
        {
            switch (leaseRequest.TimeSpanType)
            {
                case TimeSpanType.Milliseconds:
                    return TimeSpan.FromMilliseconds(leaseRequest.LeasePeriod);
                case TimeSpanType.Seconds:
                    return TimeSpan.FromSeconds(leaseRequest.LeasePeriod) ;
                case TimeSpanType.Minutes:
                    return TimeSpan.FromMinutes(leaseRequest.LeasePeriod);
                case TimeSpanType.Hours:
                    return TimeSpan.FromHours(leaseRequest.LeasePeriod);
                default:
                    return TimeSpan.FromSeconds(leaseRequest.LeasePeriod);
            }
        }
        #endregion Support Methods
    }
}
