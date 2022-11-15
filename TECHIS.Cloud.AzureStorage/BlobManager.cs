using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace TECHIS.Cloud.AzureStorage
{
    public class BlobManager:BlobAccess
    {

        #region Connection Methods 

        public new BlobManager Connect(string containerUri, Encoding encoding = null, TokenCredential tokenCredential = null)
        {
            base.Connect(containerUri, encoding, tokenCredential);
            return this;
        }

        public new BlobManager Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null, TokenCredential tokenCredential = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding, tokenCredential);
            return this;
        }

        #endregion

        public void Delete(string fileName)
        {
            if (EnsureContainer())
                GetBlockBlob(fileName).Delete();
        }
        public async Task DeleteAsymc(string fileName)
        {
            if (await EnsureContainerAsync())
                await GetBlockBlob(fileName).DeleteAsync(); 
        }

        public async Task DeleteAsync(string fileName)
        {
            if (await EnsureContainerAsync())
                await GetBlockBlob(fileName).DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Initiates an asynchronous operation to return a list of names of blob items in the container.
        /// Parameter containerPath must include the complete name of an existing container in the account and a forward slash (/) at a minimum. containerPath may optionally include the blob name prefix as well.
        /// Here are some examples of valid prefixes:
        /// /: Returns all blobs in this container.
        /// s: Returns all blobs in this container whose names begin with 's'.
        /// media/s: Returns all blobs in this container whose names begin with 'media/s'.
        /// </summary>
        public async Task<string[]> ListAsync(string prefix=null)
        {
            List<string> names = null;
            if (await EnsureContainerAsync())
            {
                if (string.IsNullOrWhiteSpace(prefix) || (prefix.Length==1 && prefix[0]=='/'))
                {
                    prefix = null;
                }

                var pageable = BlobContainer.GetBlobsAsync(prefix: prefix);
                var results = await GetListFromPageAsync(pageable);

                names = results .Where(blobItem => (blobItem.IsLatestVersion==null || blobItem.IsLatestVersion.Value) && !blobItem.Deleted)
                                .Select(blobItem => blobItem.Name).ToList();
            }
            if (names==null)
            {
                names = new List<string>(0);
            }

            return names.ToArray();
        }
        /// <summary>
        /// Initiates an asynchronous operation to return a list of names of blob items in the container.
        /// Parameter containerPath must include the complete name of an existing container in the account and a forward slash (/) at a minimum. containerPath may optionally include the blob name prefix as well.
        /// Here are some examples of valid prefixes:
        /// /: Returns all blobs in this container.
        /// s: Returns all blobs in this container whose names begin with 's'.
        /// media/s: Returns all blobs in this container whose names begin with 'media/s'.
        /// </summary>
        public string[] List(string prefix = null)
        {
            List<string> names = null;
            if ( EnsureContainer())
            {
                if (string.IsNullOrWhiteSpace(prefix) || (prefix.Length == 1 && prefix[0] == '/'))
                {
                    prefix = null;
                }

                var pageable = BlobContainer.GetBlobs(prefix: prefix);
                var results = GetListFromPage(pageable);

                //names = results.Where(blobItem => blobItem.IsLatestVersion != null && blobItem.IsLatestVersion.Value)
                //                .Select(blobItem => blobItem.Name).ToList();
                names = results.Where(blobItem => (blobItem.IsLatestVersion == null || blobItem.IsLatestVersion.Value) && !blobItem.Deleted)
                                .Select(blobItem => blobItem.Name).ToList();
            }
            if (names == null)
            {
                names = new List<string>(0);
            }

            return names.ToArray();
        }
        private async Task<List<T>> GetListFromPageAsync<T>(AsyncPageable<T> asyncPageable)
        {

            List<T> allInstances = new List<T>();

            var enumerator = asyncPageable.GetAsyncEnumerator();
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    allInstances.Add(enumerator.Current);
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }
            return allInstances;
        }

        private List<T> GetListFromPage<T>(Pageable<T> asyncPageable)
        {

            List<T> allInstances = new List<T>();

            var enumerator = asyncPageable.GetEnumerator();
            try
            {
                while ( enumerator.MoveNext())
                {
                    allInstances.Add(enumerator.Current);
                }
            }
            finally
            {
                enumerator.Dispose();
            }
            return allInstances;
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
