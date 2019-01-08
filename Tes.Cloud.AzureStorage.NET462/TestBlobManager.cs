using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TECHIS.Cloud.AzureStorage;
using System.Linq;

namespace Test.Cloud.AzureStorage
{
    [TestClass]
    public class TestBlobManager462
    {
        [TestMethod]
        public void TestDelete462()
        {
            string path = "ForDelete";
            var list = ListFilesAsync462(path);

            Assert.IsTrue(list != null && list.Length > 0, "Failed to get valid location for testing delete location");

            var initial1st = list[0];
            ConnectedManager.DeleteAsync(initial1st).Wait();

            var new1st = ListFilesAsync462(path)[0];

            Assert.IsFalse(string.Equals(initial1st, new1st), "Failed to delete item");
        }

        [TestMethod]
        public void TestListContainerRoot462()
        {
            string path = null;
            var list = ConnectedManager.ListAsync(path).Result;

            Assert.IsTrue(list != null && list.Length > 0, "failed to list items in container");
        }

        [TestMethod]
        public void TestListContainerChild462()
        {
            string path = "PackageRepo";
            string[] list = ConnectedManager.ListAsync(path).Result;

            Assert.IsTrue(list != null && list.Length > 0 && list.All(p=>p.Contains($"{path}/") ), "failed to list only items in child folder");
        }

        [TestMethod]
        public void TestListContainerSubChild462()
        {
            string path = "Samples/graphics/graphics";
            var list = ConnectedManager.ListAsync(path).Result;

            Assert.IsTrue(list != null && list.Length > 0 && list.All(p => p.Contains($"{path}/")), "failed to list only items in child folder");
        }

        private string[] ListFilesAsync462(string path)
        {
            return ConnectedManager.ListAsync(path).Result;
        }

        private BlobManager ConnectedManager=> (new BlobManager()).Connect(Connector.GetContainerUri());

        
    }
}
