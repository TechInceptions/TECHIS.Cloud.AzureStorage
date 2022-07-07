using System;
using System.IO;
using System.Threading.Tasks;

using TECHIS.Cloud.AzureStorage;
using Xunit;

namespace Test.Cloud.AzureStorage
{
    
    public class TestBlobReaderWriter
    {
        [Fact]
        public void ReadText()
        {
            string data = (new BlobReader()).Connect(Connector.GetContainerUri()).ReadText(FixedFileName);

            Assert.False(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [Fact]
        public void ReadTextNoFile()
        {
            string data = (new BlobReader()).Connect(Connector.GetContainerUri()).ReadText(NonExistingFileName);

            Assert.True(string.IsNullOrEmpty(data));
        }

        [Fact]
        public void WriteText()
        {
            BlobWriter br = new BlobWriter();
            var containerUri = Connector.GetContainerUri();
            var fileName = "l1/l3";
            byte[] data = System.Text.Encoding.UTF8.GetBytes("Test data");
            br.Connect(containerUri).WriteToBlob(data, fileName);

            //Assert.False(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [Fact]
        public void ReadTextAsync()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = FixedFileName;
            string data = br.Connect(containerUri).ReadTextAsync(fileName).Result;

            Assert.False(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [Fact]
        public async Task ReadDataAsync()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = FixedFileName;
            string? data = null;

            Task task;
            using (MemoryStream ms = new MemoryStream() )
            {
                task = br.Connect(containerUri).ReadDataAsync(fileName, ms);
               await task;
                data = new string( br.Encoding.GetChars(ms.ToArray()));
            }
            Assert.False(string.IsNullOrEmpty(data), "Failed to read blob file");
        }

        [Fact]
        public void ReadData()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = FixedFileName;
            string? data = null;
            
            using (MemoryStream ms = new MemoryStream())
            {
                br.Connect(containerUri).ReadData(fileName, ms);
                
                data = new string(br.Encoding.GetChars(ms.ToArray()));
            }
            Assert.False(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [Fact]
        public void ReadTextNoFileAsync()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = NonExistingFileName;
            string data = br.Connect(containerUri).ReadTextAsync(fileName).Result;

            Assert.True(string.IsNullOrEmpty(data));
        }
        [Fact]
        public async Task ReadDataNoFileAsync()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = NonExistingFileName;
            string? data = null;

            Task task;
            using (MemoryStream ms = new MemoryStream())
            {
                task = br.Connect(containerUri).ReadDataAsync(fileName, ms);
               await task;
                data = new string(br.Encoding.GetChars(ms.ToArray()));
            }
            Assert.True(string.IsNullOrEmpty(data));
        }
        [Fact]
        public async Task WriteTextAsync()
        {
            BlobWriter br = new BlobWriter();
            string containerUri = Connector.GetContainerUri();
            var fileName = "l1/l3";
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"Test data: {DateTime.UtcNow.ToString() }");
            var task = br.Connect(containerUri).WriteToBlobAsync(data, fileName);
           await task;
            Assert.True(task.IsCompleted, "Failed to Write Async blob file");
        }
        [Fact]
        public async Task WriteTextAsync2()
        {
            BlobWriter br = new BlobWriter();
            var fileName = "l1/l3";
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"Test data: {DateTime.UtcNow.ToString()}");
            var task = br.Connect(Connector.StorageConnectionString, "test").WriteToBlobAsync(data, fileName);
            await task;
            Assert.True(task.IsCompleted, "Failed to Write Async blob file");
        }

        private string FixedFileName => "FixedDirectory/S2715H.inf";
        private string NonExistingFileName => "FD/43CEAA72B980FA1D305A.txt";



    }
}
