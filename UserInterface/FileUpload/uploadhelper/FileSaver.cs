using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace AngularUI.FileUpload.uploadhelper
{
    public class FileSaver
    {
        public static MultipartFormDataStreamProvider GetMultipartProvider()
        {
            var uploadFolder = Path.GetTempPath();
            return new MultipartFormDataStreamProvider(uploadFolder);
        }

        // Extracts Request FormatData as a strongly typed model
        public static T GetFormData<T>(MultipartFormDataStreamProvider result) where T : class
        {
            if (!result.FormData.HasKeys()) return null;
            var values = result.FormData.GetValues(0);
            if (values == null) return null;
            var unescapedFormData = Uri.UnescapeDataString(values.FirstOrDefault() ?? String.Empty);
            if (String.IsNullOrEmpty(unescapedFormData)) return null;
            return JsonConvert.DeserializeObject<T>(unescapedFormData);
        }

        private static string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        private static string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        public static FileInfo MoveToTemp(MultipartFormDataStreamProvider result)
        {
            var originalFileName = GetDeserializedFileName(result.FileData.First());
            var uploadedFileInfo = new FileInfo(result.FileData.First().LocalFileName);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            var folder = Directory.CreateDirectory(Path.GetTempPath() + @"\" + timestamp);
            if (folder.Exists) folder.Create();
            var filename = folder.FullName + @"\" + originalFileName;
            uploadedFileInfo.MoveTo(filename);
            return uploadedFileInfo;
        }

    }
}