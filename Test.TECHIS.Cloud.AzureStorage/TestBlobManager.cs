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
        //[Fact]
        //public async Task TestDelete()
        //{
        //    string path = "ForDelete";
        //    WriteTextFile(path);
        //    var list = await ListFilesAsync(path);

        //    Assert.NotNull(list);
        //    Assert.NotEmpty(list);

        //    var initial1st = list[0];
        //    await ConnectedManager.DeleteAsync(initial1st);

        //    var postList = await ListFilesAsync(path);
        //    Assert.Empty(postList);
        //}

        [Fact]
        public async Task TestListContainerRootAsync()
        {
            string? path = null;
            var list = await ConnectedManager.ListAsync(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task TestListContainerChildAsync()
        {
            string path = "PackageRepo";
            string[] list = await ConnectedManager.ListAsync(path);


            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.True(list.All(p => p.Contains($"{path}/")), "failed to list only items in child folder");
        }

        [Fact]
        public async Task TestListContainerSubChildAsync()
        {
            string path = "Samples/graphics/graphics";
            var list = await ConnectedManager.ListAsync(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.True(list.All(p => p.Contains($"{path}/")), "failed to list only items in child folder");
        }
        [Fact]
        public void TestListContainerRoot()
        {
            string? path = null;
            var list = ConnectedManager.List(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);   
        }

        [Fact]
        public void TestListContainerChild()
        {
            string path = "PackageRepo";
            string[] list = ConnectedManager.List(path);


            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.True(list.All(p=>p.Contains($"{path}/") ), "failed to list only items in child folder");
        }

        [Fact]
        public void TestListContainerSubChild()
        {
            string path = "Samples/graphics/graphics";
            var list = ConnectedManager.List(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.True(list.All(p => p.Contains($"{path}/")), "failed to list only items in child folder");
        }

        private async Task<string[]> ListFilesAsync(string path)
        {
            return await ConnectedManager.ListAsync(path);
        }
        //private static void WriteTextFile(string prefix)
        //{
        //    BlobWriter br = new();
        //    br.Connect(Connector.GetContainerUri()).WriteToBlob(System.Text.Encoding.UTF8.GetBytes("Test data"), $"{prefix}/l3.txt");
        //}
        private BlobManager ConnectedManager=> (new BlobManager()).Connect(Connector.GetContainerUri());

        
    }
}
