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
        //public static string GetContainerUri()
        //{
        //    return "https://tests4dev.blob.core.windows.net/cloudfile?sv=2015-12-11&si=cloudfile-RWLD&sr=c&sig=qXnlg3DGNBrT8wWVAeeeqn8asP%2BJXjYdXH1os6mdaCU%3D";
        //}

        ///// <summary>
        ///// Needs rights to create a container
        ///// </summary>
        //public static string StorageConnectionString => "SharedAccessSignature=sv=2020-04-08&ss=btqf&srt=sco&st=2021-10-26T11%3A48%3A00Z&se=2040-10-29T11%3A48%3A00Z&sp=rwdxftlacup&sig=d02Dv2Xg03j8dZT1oX0gt6FmmF5jk88TXjOIbqNLgzA%3D;BlobEndpoint=https://tests4dev.blob.core.windows.net/;FileEndpoint=https://tests4dev.file.core.windows.net/;QueueEndpoint=https://tests4dev.queue.core.windows.net/;TableEndpoint=https://tests4dev.table.core.windows.net/;";


        private static DefaultSecretStore _defaultSecretStore = new DefaultSecretStore("https://techis-proj-azurestorage.vault.azure.net/");
        public static string GetContainerUri() => _defaultSecretStore.GetSecret("ContainerUri");

        /// <summary>
        /// Needs rights to create a container
        /// </summary>
        public static string StorageConnectionString => _defaultSecretStore.GetSecret("ConnectionString");

    }
}
