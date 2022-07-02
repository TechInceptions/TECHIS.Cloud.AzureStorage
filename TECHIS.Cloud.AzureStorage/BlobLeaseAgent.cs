
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Text;

namespace TECHIS.Cloud.AzureStorage
{
    public  class BlobLeaseAgent:BlobAccess
    {
        #region Fields 
        private PageBlobClient _LeaseBlob;
        private readonly string _LeaseBlobName;
        private readonly int _LeaseDurationSeconds;
        #endregion

        #region Connect 

        public async Task<BlobLeaseAgent> ConnectAsync(string containerUri, Encoding encoding = null)
        {
            base.Connect(containerUri, encoding);
            if (await EnsureContainerAsync())
            {
                _LeaseBlob = BlobContainer.GetPageBlobClient(_LeaseBlobName);
            }
            return this;
        }

        public async Task<BlobLeaseAgent> ConnectAsync(string azureStorageConnectionString, string containerName, Encoding encoding = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding);
            if (await EnsureContainerAsync())
            {
                _LeaseBlob = BlobContainer.GetPageBlobClient(_LeaseBlobName);
            }
            return this;
        }
        public new BlobLeaseAgent Connect(string containerUri, Encoding encoding = null)
        {
            base.Connect(containerUri, encoding);
            if (EnsureContainer())
            {
                _LeaseBlob = BlobContainer.GetPageBlobClient(_LeaseBlobName);
            }
            return this;
        }

        public new BlobLeaseAgent Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding);
            if (EnsureContainer())
            {
                _LeaseBlob = BlobContainer.GetPageBlobClient(_LeaseBlobName);
            }
            return this;
        }
        #endregion

        #region Constructors 
        public BlobLeaseAgent(string leaseBlobName, int leaseDurationSeconds)
        {
            _LeaseBlobName = leaseBlobName;
            _LeaseDurationSeconds = leaseDurationSeconds;
        }
        #endregion

        #region Public Methods 
        public static BlobLeaseAgent Get(string leaseBlobName, int leaseDurationSeconds, string containerUri)
        {
            return (new BlobLeaseAgent(leaseBlobName, leaseDurationSeconds)).Connect(containerUri);
        }

        //public async Task ReleaseLeaseAsync(string leaseId)
        //{
        //    try
        //    {
        //        await _LeaseBlob.ReleaseLeaseAsync(new AccessCondition { LeaseId = leaseId }).ConfigureAwait(false);
        //    }
        //    catch (StorageException)
        //    {
        //        // Lease will eventually be released.
        //        //Trace.TraceError(e.Message);
        //    }
        //}
        public async Task ReleaseLeaseAsync(string leaseId)
        {
            try
            {
                await _LeaseBlob.GetBlobLeaseClient(leaseId).ReleaseAsync().ConfigureAwait(false);
            }
            catch (RequestFailedException _)
            {
                // Lease will eventually be released.
                //Trace.TraceError(e.Message);
            }
        }

        /// <summary>
        /// 15 seconds is the min allowed
        /// </summary>
        private const int MIN_LEASEDURATION = 15;
        public async Task<string> AcquireLeaseAsync(CancellationToken token, int? leaseDurationSeconds = null, string reAcquireLeaseId = null)
        {
            bool blobNotFound = false;
            try
            {
                int lds = leaseDurationSeconds ?? _LeaseDurationSeconds;
                if (lds<MIN_LEASEDURATION)
                {
                    lds = MIN_LEASEDURATION;
                }
                
                var r = await _LeaseBlob.GetBlobLeaseClient(reAcquireLeaseId).AcquireAsync(TimeSpan.FromSeconds(lds)).ConfigureAwait(false);

                return r.Value.LeaseId;
            }
            catch (RequestFailedException storageException)
            {
                //Trace.TraceError(storageException.Message);
                if (storageException.Status ==(int)HttpStatusCode.NotFound)
                {
                    blobNotFound = true;
                }
                else if (storageException.Status == (int)HttpStatusCode.Conflict)
                {
                    return null;
                }
                else if (storageException.InnerException is WebException webException)
                {
                    if (webException.Response is HttpWebResponse response)
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            blobNotFound = true;
                        }
                        if (response.StatusCode == HttpStatusCode.Conflict)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            if (blobNotFound)
            {
                await CreateBlobAsync().ConfigureAwait(false);
                return await AcquireLeaseAsync(token).ConfigureAwait(false);
            }
            
            return null;
        }

        public async Task<bool> RenewLeaseAsync(string leaseId, CancellationToken token)
        {
            try
            {
                await _LeaseBlob.GetBlobLeaseClient(leaseId).RenewAsync(null, token).ConfigureAwait(false);
                return true;
            }

            catch (RequestFailedException _)
            {
                // catch (WebException webException)
                //Trace.TraceError(storageException.Message);
                return false;
            }
        }
        #endregion

        #region Private Methods 
        private async Task CreateBlobAsync()
        {
            /* Container must already exist
             * await _LeaseBlob.Container.CreateIfNotExistsAsync(token);*/

            if (!await _LeaseBlob.ExistsAsync().ConfigureAwait(false))
            {
                try
                {
                    await _LeaseBlob.CreateAsync(0).ConfigureAwait(false);
                }
                catch (RequestFailedException e)
                {

                    if (e.Status != (int)HttpStatusCode.PreconditionFailed)
                    {
                        throw;
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException is WebException webException)
                    {
                        if (webException.Response is HttpWebResponse response)
                        {
                            if (response.StatusCode != HttpStatusCode.PreconditionFailed)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}