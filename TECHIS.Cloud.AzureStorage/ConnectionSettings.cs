using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TECHIS.Cloud.AzureStorage
{
    public class ConnectionSettings
    {
        public string AzureStorageConnectionString { get; set; }
        public string AzureContainerUriWithSas { get; set; }

        public string ContainerName { get; set; }
        public TokenCredential TokenCredential { get; internal set; }
    }
}
