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
        public static string StorageConnectionString => "SharedAccessSignature=sv=2015-12-11&ss=b&srt=co&sp=rwdl&st=2017-02-10T13%3A33%3A00Z&se=2017-04-14T12%3A33%3A00Z&sig=oKS5Is5YupWrTnBh2jB1DMpulaA4VN72m85ZzHEaBFQ%3D;BlobEndpoint=https://tests4dev.blob.core.windows.net/";

    }
}
