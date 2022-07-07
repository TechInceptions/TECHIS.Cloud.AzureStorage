using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using TECHIS.Core;

namespace TECHIS.Cloud.AzureStorage
{
    public abstract class BlobAccess
    {
        #region Fields 
        //private readonly RetryOptions _DefaultLinearRetry = new RetryOptions(null) { Mode= RetryMode.Fixed };  //new LinearRetry(new TimeSpan(0, 0, 3), 3);
        //private readonly BlobRequestOptions _DefaultBlobRequestOptions;
        private ConnectionSettings          _ConnectionSettings;
        private BlobServiceClient _BlobClient;
        private string _ConnectionString;

        //private CloudStorageAccount         _StorageAccount;
        private bool                        _IsAccountValid;
        private bool                        _IsClientValid;
        //private Encoding                    _Encoding = Encoding.UTF8;
        #endregion

        public BlobAccess()
        {
            //_DefaultBlobRequestOptions = new BlobRequestOptions { RetryPolicy = _DefaultLinearRetry };
        }

        #region Properties  


        public bool IsAccountValid
        {
            get
            {
                return _IsAccountValid;
            }

            private set
            {
                _IsAccountValid = value;
            }
        }



        protected ConnectionSettings ConnectionSettings
        {
            get
            {
                return _ConnectionSettings;
            }
        }
        

        public bool IsValidContainer
        {
            get;

            protected set;
        }

        protected virtual BlobContainerClient BlobContainer
        {
            get;
            private set;
        }

        public bool IsClientValid
        {
            get
            {
                return _IsClientValid;
            }
        }

        //public virtual LinearRetry DefaultLinearRetry
        //{
        //    get
        //    {
        //        return _DefaultLinearRetry;
        //    }
        //}

        //public virtual BlobRequestOptions DefaultBlobRequestOptions
        //{
        //    get
        //    {
        //        return _DefaultBlobRequestOptions;
        //    }
        //}

        public virtual Encoding Encoding { get; protected set; } = Encoding.UTF8;
        #endregion

        #region Private Methods 
        private bool CreateClient()
        {
            if (IsAccountValid)
            {
                _BlobClient = new BlobServiceClient(_ConnectionString);
                _IsClientValid = true;
            }
            else
            {
                _IsClientValid = false;
            }

            return IsClientValid;
        }

        protected bool CreateContainerFromBlobClient()
        {
            if (!IsClientValid)
            {
                CreateClient();
            }

            if (IsClientValid)
            {

                // Retrieve a reference to a container.
                BlobContainer = _BlobClient.GetBlobContainerClient(ConnectionSettings.ContainerName);

                // Create the container if it doesn't already exist.
                 BlobContainer.CreateIfNotExists();
                IsValidContainer = true;
            }

            return IsValidContainer;
        }
        protected async Task<bool> CreateContainerFromBlobClientAsync()
        {
            if (!IsClientValid)
            {
                CreateClient();
            }

            if (IsClientValid)
            {

                // Retrieve a reference to a container.
                BlobContainer = _BlobClient.GetBlobContainerClient(ConnectionSettings.ContainerName);

                // Create the container if it doesn't already exist.
                await BlobContainer.CreateIfNotExistsAsync();

                IsValidContainer = true;
            }

            return IsValidContainer;
        }

        #endregion

        #region Public Methods 

        protected virtual void Connect(string containerUri, Encoding encoding = null)
        {
            InputValidator.ArgumentNullOrEmptyCheck(containerUri, nameof(containerUri));
            _ConnectionSettings = new ConnectionSettings { AzureContainerUriWithSas = containerUri };
            _IsAccountValid = false;

            if (encoding!=null)
            {
                Encoding = encoding;
            }
        }

        protected virtual void Connect(string azureStorageConnectionString, string containerName, Encoding encoding = null)
        {
            InputValidator.ArgumentNullOrEmptyCheck(azureStorageConnectionString, nameof(azureStorageConnectionString));      
                 
            if (encoding != null)
            {
                Encoding = encoding;
            } 
            try
            {
                _ConnectionSettings = new ConnectionSettings { AzureStorageConnectionString = azureStorageConnectionString, ContainerName=containerName };

                if (!string.IsNullOrEmpty(_ConnectionSettings.AzureStorageConnectionString))
                {
                    //_StorageAccount = CloudStorageAccount.Parse(_ConnectionSettings.AzureStorageConnectionString);
                    _ConnectionString = _ConnectionSettings.AzureStorageConnectionString;
                    _IsAccountValid = true;
                }
            }
            catch
            {
                _IsAccountValid = false;
                throw;
            }
        }

        #endregion

        #region Protected 

        protected async Task<bool> EnsureContainerAsync()
        {
            bool success;
            if ((!IsValidContainer) && (!ConnectToContainer()))
            {
                success = await CreateContainerFromBlobClientAsync();
            }
            else
            {
                success = true;
            }

            if (!success)
            {

                if (!IsValidContainer)
                {
                    throw new InvalidOperationException(Errors.STORE_CONTAINER_INVALID);
                }

                if (!IsAccountValid)
                {
                    throw new InvalidOperationException(Errors.STORE_CONNECTION_INVALID);
                }

                if (!IsClientValid)
                {
                    throw new InvalidOperationException(Errors.STORE_CLIENT_INVALID);
                }
            }

            return success;
        }
        protected bool EnsureContainer()
        {
            bool success;
            if ((!IsValidContainer) && (!ConnectToContainer()))
            {
                success = CreateContainerFromBlobClient();
            }
            else
            {
                success = true;
            }

            if (! success)
            {

                if (!IsValidContainer)
                {
                    throw new InvalidOperationException(Errors.STORE_CONTAINER_INVALID);
                }

                if (!IsAccountValid)
                {
                    throw new InvalidOperationException(Errors.STORE_CONNECTION_INVALID);
                }

                if (!IsClientValid)
                {
                    throw new InvalidOperationException(Errors.STORE_CLIENT_INVALID);
                }
            }

            return success;
        }
        /// <summary>
        /// Resets all fields to default values.
        /// After reset is called, you will need to call Connect to re-connect to the store.
        /// </summary>
        protected virtual void Reset() {
            _IsAccountValid = false;
            _ConnectionSettings = null;
        }

        protected virtual bool ConnectToContainer(string containerUri = null)
        {
            bool success = false;
            try
            {
                if (string.IsNullOrEmpty(containerUri) && ConnectionSettings != null)
                {
                    containerUri = ConnectionSettings.AzureContainerUriWithSas;
                }
                if (!string.IsNullOrEmpty(containerUri))
                {
                    BlobContainer               = new BlobContainerClient(new Uri(containerUri)); // CloudBlobContainer(new Uri(containerUri));
                    success                     = true;
                    IsValidContainer            = true;
                }
            }
            catch
            {
                IsValidContainer = false;
                throw;
            }

            return success;
        }


        protected BlobClient GetBlockBlob(string blobFileName)
        {
            return BlobContainer.GetBlobClient(blobFileName);
        }
        #endregion


    }
}
