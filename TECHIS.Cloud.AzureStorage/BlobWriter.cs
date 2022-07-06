using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Azure;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;


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
            if (EnsureContainer())
            {
                (GetBlockBlob(blobFileName)).Upload(ms);
            }
        }
        public void WriteToBlob(byte[] data, string blobFileName)
        {
            if (EnsureContainer())
            {
                (GetBlockBlob(blobFileName)).Upload(new BinaryData(data));
            }
        }

        public async Task WriteToBlobAsync(Stream ms, string blobFileName)
        {
            if (await EnsureContainerAsync())
            {
                await (GetBlockBlob(blobFileName)).UploadAsync(ms).ConfigureAwait(false);
            }
        }
        public async Task WriteToBlobAsync(byte[] data, string blobFileName)
        {
            if (await EnsureContainerAsync())
            {
                await (GetBlockBlob(blobFileName)).UploadAsync(new BinaryData(data)).ConfigureAwait(false);
            }
        }
        #endregion


    }
}
