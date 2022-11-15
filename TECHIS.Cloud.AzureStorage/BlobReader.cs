using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Azure;
using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;


namespace TECHIS.Cloud.AzureStorage
{
    public class BlobReader:BlobAccess
    {
        #region Public Methods 

        public new BlobReader Connect(string containerUri, Encoding encoding = null, TokenCredential tokenCredential = null)
        {
            base.Connect(containerUri, encoding, tokenCredential);
            return this;
        }

        public new BlobReader Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null, TokenCredential tokenCredential = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding,tokenCredential);
            return this;
        }

        public virtual string ReadText(string blobFileName)
        {

            if (EnsureContainer())
                return GetTextFromBlob(GetBlockBlob(blobFileName));

            return null;
        }

        public virtual async Task<string> ReadTextAsync(string blobFileName)
        {
            
            if ( await EnsureContainerAsync())
                return await GetTextFromBlobAsync(GetBlockBlob(blobFileName)).ConfigureAwait(false);

            return null;
        }

        public virtual async Task ReadDataAsync(string blobFileName, Stream output)
        {
            if (await EnsureContainerAsync())
            {
                try
                {
                    await GetBlockBlob(blobFileName).DownloadToAsync(output).ConfigureAwait(false);
                }
                catch (Exception ex) when (IsFileNotFound(ex))
                {
                    //do nothing, thus no data is written to stream
                }
            }
        }

        public virtual void ReadData(string blobFileName, Stream output)
        {
            if (EnsureContainer())
            {
                try
                {
                    GetBlockBlob(blobFileName).DownloadTo(output);
                }
                catch (Exception ex) when (IsFileNotFound(ex))
                {
                    //do nothing, thus no data is written to stream
                }
            }
        }

        #endregion

        #region Protected 
        //protected virtual string GetTextFromBlob(BlobClient dataBlob)
        //{
        //    return GetTextFromBlob(dataBlob);
        //}
        protected virtual string GetTextFromBlob(BlobClient dataBlob)
        {
            string text;
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    dataBlob.DownloadTo(memoryStream);
                    text = Encoding.GetString(memoryStream.ToArray());
                }
                catch (Exception ex) when (IsFileNotFound(ex))
                {
                    text = null;
                }
            }

            return text;
        }
        protected virtual async Task<string> GetTextFromBlobAsync(BlobClient dataBlob)
        {
            string text;
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await dataBlob.DownloadToAsync(memoryStream).ConfigureAwait(false);
                    text = Encoding.GetString(memoryStream.ToArray());
                }
                catch(Exception ex) when( IsFileNotFound(ex) )
                {
                    text = null;
                }
            }

            return text;
        }

        protected virtual bool IsFileNotFound(Exception ex)
        {
            RequestFailedException exception = ex as RequestFailedException;
            if (exception is null && ex is AggregateException)
            {
                exception = ex.InnerException as RequestFailedException;
            }

            if (exception != null && exception.Status == (int)HttpStatusCode.NotFound)
            {
                return true;
            }

            return false;
        }
        #endregion


    }
}
