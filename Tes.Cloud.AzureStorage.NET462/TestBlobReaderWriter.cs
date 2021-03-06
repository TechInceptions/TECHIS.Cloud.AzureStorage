﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TECHIS.Cloud.AzureStorage;

namespace Test.Cloud.AzureStorage
{
    [TestClass]
    public class TestBlobReaderWriter462
    {
        [TestMethod]
        public void ReadText462()
        {
            string data = (new BlobReader()).Connect(Connector.GetContainerUri()).ReadText(FixedFileName);

            Assert.IsFalse(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [TestMethod]
        public void ReadTextNoFile462()
        {
            string data = (new BlobReader()).Connect(Connector.GetContainerUri()).ReadText(NonExistingFileName);

            Assert.IsTrue(string.IsNullOrEmpty(data));
        }

        [TestMethod]
        public void WriteText462()
        {
            BlobWriter br = new BlobWriter();
            var containerUri = Connector.GetContainerUri();
            var fileName = "l1/l3";
            byte[] data = System.Text.Encoding.UTF8.GetBytes("Test data");
            br.Connect(containerUri).WriteToBlob(data, fileName);

            //Assert.IsFalse(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [TestMethod]
        public void ReadTextAsync462()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = FixedFileName;
            string data = br.Connect(containerUri).ReadTextAsync(fileName).Result;

            Assert.IsFalse(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [TestMethod]
        public void ReadDataAsync462()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = FixedFileName;
            string data = null;

            Task task = null;
            using (MemoryStream ms = new MemoryStream() )
            {
                task = br.Connect(containerUri).ReadDataAsync(fileName, ms);
                task.Wait();
                data = new string( br.Encoding.GetChars(ms.ToArray()));
            }
            Assert.IsFalse(string.IsNullOrEmpty(data), "Failed to read blob file");
        }

        [TestMethod]
        public void ReadData462()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = FixedFileName;
            string data = null;
            
            using (MemoryStream ms = new MemoryStream())
            {
                br.Connect(containerUri).ReadData(fileName, ms);
                
                data = new string(br.Encoding.GetChars(ms.ToArray()));
            }
            Assert.IsFalse(string.IsNullOrEmpty(data), "Failed to read blob file");
        }
        [TestMethod]
        public void ReadTextNoFileAsync462()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = NonExistingFileName;
            string data = br.Connect(containerUri).ReadTextAsync(fileName).Result;

            Assert.IsTrue(string.IsNullOrEmpty(data));
        }
        [TestMethod]
        public void ReadDataNoFileAsync462()
        {
            BlobReader br = new BlobReader();
            var containerUri = Connector.GetContainerUri();
            var fileName = NonExistingFileName;
            string data = null;

            Task task = null;
            using (MemoryStream ms = new MemoryStream())
            {
                task = br.Connect(containerUri).ReadDataAsync(fileName, ms);
                task.Wait();
                data = new string(br.Encoding.GetChars(ms.ToArray()));
            }
            Assert.IsTrue(string.IsNullOrEmpty(data));
        }
        [TestMethod]
        public void WriteTextAsync462()
        {
            BlobWriter br = new BlobWriter();
            string containerUri = Connector.GetContainerUri();
            var fileName = "l1/l3";
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"Test data: {DateTime.UtcNow.ToString() }");
            var task = br.Connect(containerUri).WriteToBlobAsync(data, fileName);
            task.Wait();
            Assert.IsTrue(task.IsCompleted, "Failed to Write Async blob file");
        }
        [TestMethod]
        public void WriteTextAsync2462()
        {
            BlobWriter br = new BlobWriter();
            var fileName = "l1/l3";
            byte[] data = System.Text.Encoding.UTF8.GetBytes($"Test data: {DateTime.UtcNow.ToString()}");
            var task = br.Connect(Connector.StorageConnectionString, "test").WriteToBlobAsync(data, fileName);
            task.Wait();
            Assert.IsTrue(task.IsCompleted, "Failed to Write Async blob file");
        }

        private string FixedFileName => "FixedDirectory/S2715H.inf";
        private string NonExistingFileName => "FD/43CEAA72B980FA1D305A.txt";



    }
}
