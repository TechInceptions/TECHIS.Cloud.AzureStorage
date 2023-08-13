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
        public async Task TestGetLastModifiedContainerRootAsync()
        {
            string? path = null;
            var list = await ConnectedManager.GetLastModifiedDatesAsync(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task TestGetLastModifiedContainerChildAsync()
        {
            string path = "PackageRepo";
            var list = await ConnectedManager.GetLastModifiedDatesAsync(path);


            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.True(list.All(p => p.Name.Contains($"{path}/")), "failed to list only items in child folder");
        }

        [Fact]
        public async Task TestGetLastModifiedContainerSubChildAsync()
        {
            string path = "Samples/graphics/graphics";
            var list = await ConnectedManager.GetLastModifiedDatesAsync(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.True(list.All(p => p.Name.Contains($"{path}/")), "failed to list only items in child folder");
        }


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

        [Fact]
        public void TestFileExists()
        {
            string path = "Samples/graphics/graphics/icons/masthead.jpg";
            var list = ConnectedManager.List(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.Collection(list, p => Assert.Equal(path, p));

        }

        [Fact]
        public async Task TestFileExistsAsync()
        {
            string path = "Samples/graphics/graphics/icons/masthead.jpg";
            var list = await ConnectedManager.ListAsync(path);

            Assert.NotNull(list);
            Assert.NotEmpty(list);
            Assert.Collection(list, p => Assert.Equal(path, p));

        }

        [Fact]
        public async Task TestFileNOTExistsAsync()
        {
            string path = "Samples/graphics/graphics/icons/146CBAAF64364C59A31F06186C7AAAEE.jpg";
            var list = await ConnectedManager.ListAsync(path);

            Assert.NotNull(list);
            Assert.Empty(list);
     
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
