using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECHIS.Cloud.AzureStorage;

namespace Test.Cloud.AzureStorage
{
    public static class Connector
    {

        private static DefaultSecretStore _defaultSecretStore = new DefaultSecretStore("https://techis-proj-azurestorage.vault.azure.net/");
        public static string GetContainerUri()
        {
            return _defaultSecretStore.GetSecret("ContainerUri");
        }

        /// <summary>
        /// Needs rights to create a container
        /// </summary>
        public static string StorageConnectionString => _defaultSecretStore.GetSecret("ConnectionString");

    }
}
