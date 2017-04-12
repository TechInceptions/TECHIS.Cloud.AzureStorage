
using System;

using System.Diagnostics;

using System.Net;

using System.Threading;

using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;

using Microsoft.WindowsAzure.Storage.Blob;

using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System.Text;

namespace TECHIS.Cloud.AzureStorage
{
    public  class BlobLeaseAgent:BlobAccess
    {
        private CloudPageBlob _LeaseBlob;
        private readonly string _LeaseBlobName;
        private readonly int _LeaseDurationSeconds;
        #region Connect 

        public new BlobLeaseAgent Connect(string containerUri, Encoding encoding = null)
        {
            base.Connect(containerUri, encoding);
            if (EnsureContainer())
            {
                _LeaseBlob = BlobContainer.GetPageBlobReference(_LeaseBlobName);
            }
            return this;
        }

        public new BlobLeaseAgent Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null)
        {
            base.Connect(azureStorageConnectionString, containerName, encoding);
            if (EnsureContainer())
            {
                _LeaseBlob = BlobContainer.GetPageBlobReference(_LeaseBlobName);
            }
            return this;
        }
        #endregion


        public BlobLeaseAgent(string leaseBlobName, int leaseDurationSeconds)
        {
            _LeaseBlobName = leaseBlobName;
            _LeaseDurationSeconds = leaseDurationSeconds;
        }

        public async Task ReleaseLeaseAsync(string leaseId)
        {
            try
            {
                await _LeaseBlob.ReleaseLeaseAsync(new AccessCondition { LeaseId = leaseId });
            }
            catch (StorageException e)
            {
                // Lease will eventually be released.
                Trace.TraceError(e.Message);
            }
        }

        public async Task<string> AcquireLeaseAsync(CancellationToken token)
        {
            bool blobNotFound = false;
            try
            {
                return await _LeaseBlob.AcquireLeaseAsync(TimeSpan.FromSeconds(_LeaseDurationSeconds), null, null, null, null, token);
            }
            catch (StorageException storageException)
            {
                Trace.TraceError(storageException.Message);
                if (storageException.InnerException is WebException webException)
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
                await CreateBlobAsync();
                return await AcquireLeaseAsync(token);
            }
            
            return null;
        }

        public async Task<bool> RenewLeaseAsync(string leaseId, CancellationToken token)

        {

            try

            {

                await _LeaseBlob.RenewLeaseAsync(new AccessCondition { LeaseId = leaseId },null,null, token);

                return true;

            }

            catch (StorageException storageException)

            {

                // catch (WebException webException)

                Trace.TraceError(storageException.Message);



                return false;

            }

        }

        private async Task CreateBlobAsync()
        {
            /* Container must already exist
             * await _LeaseBlob.Container.CreateIfNotExistsAsync(token);*/

            if (!await _LeaseBlob.ExistsAsync())
            {
                try
                {
                    await _LeaseBlob.CreateAsync(0);
                }
                catch (StorageException e)
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
    }
}