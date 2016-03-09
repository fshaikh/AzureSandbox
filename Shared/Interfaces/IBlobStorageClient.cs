namespace Shared
{
    /// <summary>
    /// Interface for blob storage operations. Defines methods for blob upload, download, fetching blob metadata, etc.
    /// </summary>
    public interface IBlobStorageClient
    {
        /// <summary>
        /// Upload a blob to the storage.
        /// </summary>
        /// <param name="blob">Blob to be uploaded</param>
        /// <returns></returns>
        BlobResponse UploadBlob(Blob blob);

        /// <summary>
        /// Downloads a blob from the blob storage. Blob stream and metadata is set in the passed in blob parameter
        /// </summary>
        /// <param name="blob">Blob to be downloaded</param>
        /// <param name="fetchMetadata">Whether to fetch blob metadata or not. In some storage platforms fetching metadata requires a separate call.</param>
        void DownloadBlob(Blob blob,bool fetchMetadata = true);

        /// <summary>
        /// Gets Blob metadata.
        /// </summary>
        /// <param name="blob">Blob whose metadata is to be retreived</param>
        void GetBlobMetadata(Blob blob);

        /// <summary>
        ///  Overwrite the blob. On some storage platforms, this can be used to create snapshots/versions of a blob data.
        /// </summary>
        /// <param name="originalBlob">Blob to be overwritten</param>
        /// <param name="newBlob">New blob to replace the original blob</param>
        /// <returns></returns>
        bool ChangeBlob(Blob originalBlob,Blob newBlob);

        /// <summary>
        /// This method returns a delegated acess token for a blob.
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        AcessToken GetDelegatedAccessToken(Blob blob,AccessPolicy policy);
    }
}
