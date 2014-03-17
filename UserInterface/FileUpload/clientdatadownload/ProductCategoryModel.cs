using System.Collections.Generic;
using ColloSys.DataLayer.FileUploader;

namespace AngularUI.FileUpload.clientdatadownload
{
    public class ProductCategoryModel
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public IEnumerable<string> Products { get; set; }
        public IEnumerable<string> Category { get; set; }
        public IEnumerable<FileDetail> FileDetails { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        public ProductCategoryModel()
        {
            Products = new List<string>();
            Category = new List<string>();
            FileDetails = new List<FileDetail>();
        }
    }
}