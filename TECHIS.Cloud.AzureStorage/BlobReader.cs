using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace TECHIS.Cloud.AzureStorage
{
    public class BlobReader:BlobAccess
    {
        #region Public Methods 

        public new BlobReader Connect(string containerUri, Encoding encoding = null)
        {
            base.Connect(containerUri, encoding);
            return this;
        }

        public new BlobReader Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding);
            return this;
        }

        public virtual string ReadText(string blobFileName)
        {
            string text = null;

            if (EnsureContainer())
                text = GetTextFromBlob(GetBlockBlob(blobFileName));

            return text;
        }

        public virtual async Task<string> ReadTextAsync(string blobFileName)
        {
            string text = null;

            if (EnsureContainer())
                text = await GetTextFromBlobAsync(GetBlockBlob(blobFileName));

            return text;
        }

        public virtual async Task ReadDataAsync(string blobFileName, Stream output)
        {
            if (EnsureContainer())
            {
                try
                {
                    await GetBlockBlob(blobFileName).DownloadToStreamAsync(output, null, DefaultBlobRequestOptions, null);
                }
                catch (Exception ex) when (IsFileNotFound(ex))
                {
                    //do nothing, thus no data is written to stream
                }
            }
        }
        public virtual void ReadData(string blobFileName, Stream output)
        {
            ReadDataAsync(blobFileName, output).Wait();
            //if (EnsureContainer())
            //{
            //    try
            //    {
            //        GetBlockBlob(blobFileName).DownloadToStream(output, null, DefaultBlobRequestOptions, null);
            //    }
            //    catch (Exception ex) when (IsFileNotFound(ex))
            //    {
            //        //do nothing, thus no data is written to stream
            //    }
            //}
        }

        #endregion

        #region Protected 
        protected virtual string GetTextFromBlob(CloudBlob dataBlob)
        {
            //string text;
            //using (var memoryStream = new MemoryStream())
            //{
            //    try
            //    {
            //        dataBlob.DownloadToStream(memoryStream, null, DefaultBlobRequestOptions, null);
            //        text = Encoding.GetString(memoryStream.ToArray());
            //    }
            //    catch (Exception ex) when (IsFileNotFound(ex))
            //    {
            //        text = null;
            //    }
            //}

            //return text;

            return GetTextFromBlobAsync(dataBlob).Result;
        }
        protected virtual async Task<string> GetTextFromBlobAsync(CloudBlob dataBlob)
        {
            string text;
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await dataBlob.DownloadToStreamAsync(memoryStream, null, DefaultBlobRequestOptions, null);
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
            bool setDefault = false;

            StorageException exception = ex as StorageException;
            if ( ex is AggregateException)
            {
                exception = ex.InnerException as StorageException;
            }
            else if(ex is StorageException)
            {
                exception = ex as StorageException;
            }

            if (exception!=null)
            {
                switch (exception.RequestInformation.HttpStatusCode)
                {
                    case (int)HttpStatusCode.NotFound:
                        setDefault = true;
                        break;
                }
            }
            

            return setDefault;
        }
        #endregion


    }
}
