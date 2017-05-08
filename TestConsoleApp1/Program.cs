using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECHIS.Cloud.AzureStorage;
using Test.Cloud.AzureStorage;

namespace TestConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("STart");
            var data = ReadData462();
            Console.WriteLine(data);

            Console.WriteLine("Hit enter to exit ...");
            Console.ReadLine();
        }
        public static string ReadData462()
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
            return data;
        }

        private static string FixedFileName => "FixedDirectory/S2715H.inf";
        private static string NonExistingFileName => "FD/43CEAA72B980FA1D305A.txt";

    }
}
