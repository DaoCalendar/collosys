using System;
using System.IO;
using System.Reflection;

namespace ColloSys.UserInterface.Areas.Developer.Models.Excel2Db
{
    public class ResourceReader
    {
        public static Stream GetEmbeddedResourceAsStream(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceName = String.Format("AngularUI.ExcelData.{0}", name);
            return asm.GetManifestResourceStream(resourceName);
        }

        public static FileStream GetEmbeddedResourceAsFileStream(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var file = String.Format("AngularUI.ExcelData.{0}", name);
           
            using (var stream = asm.GetManifestResourceStream(file))
            {
                var tempPath = Path.GetTempPath();
                string path = Path.Combine(tempPath, name);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                var fileStream = SaveStreamToFile(path, stream); // Save New Created File
                return fileStream;
            }

        }
        private static FileStream SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0) return null;

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                return fileStream;
            }

        }

    }
}
