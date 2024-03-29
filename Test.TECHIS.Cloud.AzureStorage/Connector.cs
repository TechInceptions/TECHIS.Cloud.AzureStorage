﻿using System;
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
        internal static string ADControlledStorage = "https://techis4devad.blob.core.windows.net";
        internal static string ADControlledContainerUrl = "https://techis4devad.blob.core.windows.net/cloudfile";

        public static string GetContainerUri() => _defaultSecretStore.GetSecret("ContainerUri");

        /// <summary>
        /// Needs rights to create a container
        /// </summary>
        public static string StorageConnectionString => _defaultSecretStore.GetSecret("ConnectionString");

    }
}
