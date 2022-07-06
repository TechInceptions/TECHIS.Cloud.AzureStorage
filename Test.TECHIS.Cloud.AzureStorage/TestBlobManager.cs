using System;
using System.IO;
using System.Threading.Tasks;

using TECHIS.Cloud.AzureStorage;
using System.Linq;
using Xunit;

namespace Test.Cloud.AzureStorage
{
    
    public class TestBlobManager
    {
        [Fact]
        public void TestDelete()
        {
            string path = "ForDelete";
            var list = ListFilesAsync(path);

            Assert.True(list != null && list.Length > 0, "Failed to get valid location for testing delete location");

            var initial1st = list[0];
            ConnectedManager.DeleteAsync(initial1st).Wait();

            var new1st = ListFilesAsync(path)[0];

            Assert.False(string.Equals(initial1st, new1st), "Failed to delete item");
        }

        [Fact]
        public void TestListContainerRoot()
        {
            string path = null;
            var list = ConnectedManager.ListAsync(path).Result;

            Assert.True(list != null && list.Length > 0, "failed to list items in container");
        }

        [Fact]
        public void TestListContainerChild()
        {
            string path = "PackageRepo";
            string[] list = ConnectedManager.ListAsync(path).Result;

            Assert.True(list != null && list.Length > 0 && list.All(p=>p.Contains($"{path}/") ), "failed to list only items in child folder");
        }

        [Fact]
        public void TestListContainerSubChild()
        {
            string path = "Samples/graphics/graphics";
            var list = ConnectedManager.ListAsync(path).Result;

            Assert.True(list != null && list.Length > 0 && list.All(p => p.Contains($"{path}/")), "failed to list only items in child folder");
        }

        private async Task<string[]> ListFilesAsync(string path)
        {
            return await ConnectedManager.ListAsync(path);
        }

        private BlobManager ConnectedManager=> (new BlobManager()).Connect(Connector.GetContainerUri());

        
    }
}
