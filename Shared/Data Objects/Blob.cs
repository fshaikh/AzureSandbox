using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class StorageAccountInfo
    {
        public string StorageAccountName { get; set; }
        public string AccessKey { get; set; }
    }

    public class BlobMetadata : IEnumerable
    {
        private Dictionary<string, string> _metadataContainer = new Dictionary<string, string>();

       

        public void Add(string key,string value)
        {
            _metadataContainer.Add(key, value);
        }

        public IEnumerator GetEnumerator()
        {
            return _metadataContainer.Values.GetEnumerator();
        }

        public Dictionary<string,string> GetMetadata()
        {
            return _metadataContainer;
        }
       
    }
    public class Blob
    {
        public string ContainerName { get; set; }

        public string BlobName { get; set; }
        public string FilePath { get; set; }

        public Stream  BlobStream{ get; set; }

        public BlobMetadata Metadata { get; set; }

        public string ContentType { get; set; }
        
         /// <summary>
        /// Gets/Sets the blob byte array
        /// </summary>
        public byte[] BlobData { get; set; }
        
        public BlobUri BlobUri { get; set; }
    }   

    public class AcessToken
    {
        public string Token { get; set; }
    }

    public enum Permission
    {
        None,
        Read,
        Write,
        Delete,
        List
    }
    public class AccessPolicy
    {
        public string PolicyName { get; set; }
        public int ExpiryHours { get; set; }
        public Permission Permissions { get; set; }

    }
    
     /// <summary>
    /// This class represents a blob response
    /// </summary>
    public class BlobResponse
    {
        public bool IsSuccess { get; set; }
        public string FailureMessage { get; set; }
        public BlobUri BlobUri { get; set; }
    }
    
     public class BlobUri
    {
        public string PrimaryUri { get; set; }
        public string SecondaryUri { get; set; }
    }
}
