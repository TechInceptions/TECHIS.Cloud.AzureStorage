using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TECHIS.Cloud.AzureStorage
{
    public static class Errors
    {
        private const int       LENGTH_ERRORCODE                    = 6;
        private const string    PREFIX                              = "Ex0";

        public const string     IOCCONTAINER_CONFIGURATION_ABSENT   = PREFIX + "010. Failed to load the configuration for the Unity IoC Container. Check config file";
        public const string     APPLICATIONNAME_REQUIRED            = PREFIX + "101. ApplicationName property must be assigned.";
        public const string     STORE_CONNECTION_INVALID            = PREFIX + "201. The connection to the cloud storage is invalid";
        public const string     STORE_CLIENT_INVALID                = PREFIX + "202. The cloud storage client is invalid";
        public const string     STORE_CONTAINER_INVALID             = PREFIX + "203. The cloud storage container is invalid";

        public static bool IsSource(this Exception exception, string errorDef)
        {
            bool val = false;
            if (exception!=null && (! string.IsNullOrEmpty(errorDef)) && errorDef.Length>=LENGTH_ERRORCODE)
            {
                exception = exception.GetBaseException();

                if (!string.IsNullOrEmpty( exception.Message) && exception.Message.Length>=LENGTH_ERRORCODE)
                {
                    string errorCode = errorDef.Substring(0, LENGTH_ERRORCODE);
                    val = exception.Message.Contains(errorCode);
                }
            }

            return val;
        }
    }
}
