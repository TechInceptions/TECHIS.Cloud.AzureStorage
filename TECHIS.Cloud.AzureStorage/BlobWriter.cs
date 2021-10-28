using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;


namespace TECHIS.Cloud.AzureStorage
{
    public class BlobWriter:BlobAccess
    {
        #region Public Methods 

        public new BlobWriter Connect(string containerUri, Encoding encoding = null)
        {
            base.Connect(containerUri, encoding);
            return this;
        }

        public new BlobWriter Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding);
            return this;
        }

        #endregion

        #region Public Methods 
        public void WriteToBlob(Stream ms, string blobFileName)
        {
            Task.Run(() => WriteToBlobAsync(ms, blobFileName)).Wait() ;
        }
        public void WriteToBlob(byte[] data, string blobFileName)
        {
            Task.Run(() => WriteToBlobAsync(data, blobFileName)).Wait();
        }

        public async Task WriteToBlobAsync(Stream ms, string blobFileName)
        {
            if (await EnsureContainerAsync())
            {
                await (GetBlockBlob(blobFileName)).UploadFromStreamAsync(ms, null, DefaultBlobRequestOptions, null).ConfigureAwait(false);
            }
        }
        public async Task WriteToBlobAsync(byte[] data, string blobFileName)
        {
            if (await EnsureContainerAsync())
            {
                await (GetBlockBlob(blobFileName)).UploadFromByteArrayAsync(data, 0, data.Length, null, DefaultBlobRequestOptions, null).ConfigureAwait(false);
            }
        }
        #endregion


    }
}
