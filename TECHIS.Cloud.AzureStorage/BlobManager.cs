using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TECHIS.Cloud.AzureStorage
{
    public class BlobManager:BlobAccess
    {

        #region Connection Methods 

        public new BlobManager Connect(string containerUri, Encoding encoding = null)
        {
            base.Connect(containerUri, encoding);
            return this;
        }

        public new BlobManager Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding);
            return this;
        }

        #endregion

        public void Delete(string fileName)
        {
            if (EnsureContainer())
                GetBlockBlob(fileName).DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult(); //.Wait();
        }
        public string[] List(string containerPath)
        {
            //if (containerPath == null)
            //{
            //    containerPath = string.Empty;
            //}
            //List<string> names = null;
            //if (EnsureContainer())
            //{
            //    BlobContinuationToken continuationToken = null;
            //    IEnumerable<IListBlobItem> results = new List<IListBlobItem>();
            //    do
            //    {
            //        results = BlobContainer.ListBlobs(containerPath, true, BlobListingDetails.None, DefaultBlobRequestOptions);
            //    }
            //    while (continuationToken != null);
            //    var containerUri = BlobContainer.Uri;
            //    names = results.ToList().ConvertAll(p => GetFileNameFromBlobURI(p.Uri));
            //}
            //if (names == null)
            //{
            //    names = new List<string>(0);
            //}

            //return names.ToArray();

            return ListAsync(containerPath).ConfigureAwait(false).GetAwaiter().GetResult(); //.Result;
        }
        public async Task DeleteAsync(string fileName)
        {
            if (EnsureContainer())
                await GetBlockBlob(fileName).DeleteAsync() ;
        }

        /// <summary>
        /// Initiates an asynchronous operation to return a list of names of blob items in the container.
        /// Parameter containerPath must include the complete name of an existing container in the account and a forward slash (/) at a minimum. containerPath may optionally include the blob name prefix as well.
        /// Here are some examples of valid prefixes:
        /// sample-container/: Returns all blobs in this container.
        /// sample-container/s: Returns all blobs in this container whose names begin with 's'.
        /// sample-container/media/s: Returns all blobs in this container whose names begin with 'media/s'.
        /// </summary>
        public async Task<string[]> ListAsync(string containerPath=null)
        {
            if (containerPath==null)
            {
                containerPath = string.Empty;
            }
            List<string> names = null;
            if (EnsureContainer())
            {
                BlobContinuationToken continuationToken = null;
                List<IListBlobItem> results = new List<IListBlobItem>();
                do
                {
                    var response = await BlobContainer.ListBlobsSegmentedAsync(containerPath, true, BlobListingDetails.None,null,  continuationToken, DefaultBlobRequestOptions,null);
                    continuationToken = response.ContinuationToken;
                    results.AddRange(response.Results);
                }
                while (continuationToken != null);
                var containerUri = BlobContainer.Uri;
                //names = results.ConvertAll(p => containerUri.MakeRelativeUri( p.Uri).ToString());
                names = results.Select(p => GetFileNameFromBlobURI(p.Uri) ).ToList();
            }
            if (names==null)
            {
                names = new List<string>(0);
            }

            return names.ToArray();
        }
        
        private string GetFileNameFromBlobURI(Uri blobUri)
        {
            var containerName = BlobContainer.Name; 
            string theFile = blobUri.ToString();
            int dirIndex = theFile.IndexOf(containerName);
            return theFile.Substring(dirIndex + containerName.Length + 1, theFile.Length - (dirIndex + containerName.Length + 1));
        }
    }
}
