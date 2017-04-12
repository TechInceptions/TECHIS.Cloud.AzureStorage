using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Test.Cloud.AzureStorage
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CloudBlobContainer container = null;
            BlobResultSegment results = container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.None, 1, null, null, null).Result;

            int count = 0;

            foreach (IListBlobItem blobItem in results.Results)

            {

                Assert.IsInstanceOfType(blobItem, typeof(CloudPageBlob));

                Assert.IsTrue(blobNames.Remove(((CloudPageBlob)blobItem).Name));

                count++;

            }
        }
    }
}
