using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TECHIS.Cloud.AzureStorage;
using System.Linq;
using System.Threading;

namespace Test.Cloud.AzureStorage
{
    [TestClass]
    public class TestBlobLeaseAgent
    {

        [TestMethod]
        public void TestAcquireLease()
        {
            string errorMessage = null ;
            string leaseName = "testleasesc3";
            int durationSeconds = 15;
            var cts = new CancellationTokenSource();

            BlobLeaseAgent bla = (new BlobLeaseAgent(leaseName, durationSeconds)).Connect(Connector.GetContainerUri());

            string leaseId = null;
            try
            {
                leaseId = bla?.AcquireLeaseAsync(cts.Token).Result;
            }
            catch (Exception xc)
            {
                errorMessage = xc.Message;
            }

            bool hasLease = !string.IsNullOrEmpty(leaseId);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(hasLease);

            if(hasLease)
                bla.ReleaseLease(leaseId);

        }

    }
}
