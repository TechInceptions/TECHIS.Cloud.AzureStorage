using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Cloud.AzureStorage
{
    public static class Connector
    {
        public static string GetContainerUri()
        {
            return "https://tests4dev.blob.core.windows.net/cloudfile?sv=2015-12-11&si=cloudfile-RWLD&sr=c&sig=qXnlg3DGNBrT8wWVAeeeqn8asP%2BJXjYdXH1os6mdaCU%3D";
        }

        /// <summary>
        /// Needs rights to create a container
        /// </summary>
        public static string StorageConnectionString => "SharedAccessSignature=sv=2016-05-31&ss=b&srt=sco&sp=rwdlacu&st=2017-04-19T19%3A56%3A00Z&se=2019-08-31T19%3A56%3A00Z&sig=Z6q6gHRrC%2FmhUHzbQqM74RNPrmBTEa7Wp1BMVN4hsKQ%3D;BlobEndpoint=https://tests4dev.blob.core.windows.net/";

    }
}
