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
            string errorMessage = null;
            string leaseName = "testleasesc3";
            int durationSeconds = 15;

            BlobLeaseAgent bla = (new BlobLeaseAgent(leaseName, durationSeconds)).Connect(Connector.GetContainerUri());

            string leaseId = null;
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    leaseId = bla?.AcquireLeaseAsync(cts.Token).Result;
                }
                catch (Exception xc)
                {
                    errorMessage = xc.Message;
                }
            }

            bool hasLease = !string.IsNullOrEmpty(leaseId);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(hasLease);

            if (hasLease)
            {
                var task = bla.ReleaseLeaseAsync(leaseId);
                task.Wait();
            }
        }
        [TestMethod]
        public void TestReAcquireLease()
        {
            string errorMessage = null;
            string leaseName = "testleasesc3";
            int durationSeconds = 22;
            int renewCount = 3;

            BlobLeaseAgent bla = (new BlobLeaseAgent(leaseName, durationSeconds)).Connect(Connector.GetContainerUri());

            string leaseId = null;
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    leaseId = bla?.AcquireLeaseAsync(cts.Token).Result;
                }
                catch (Exception xc)
                {
                    errorMessage = xc.Message;
                }

                for (int i = 0; i < renewCount; i++)
                {
                    leaseId = bla.AcquireLeaseAsync(cts.Token, durationSeconds, leaseId).Result;
                    Thread.Sleep((durationSeconds - 5) * 1000);
                }
            }

            bool hasLease = !string.IsNullOrEmpty(leaseId);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(hasLease);

            if (hasLease)
            {
                bla.ReleaseLeaseAsync(leaseId).Wait();
            }
        }
    }
}
