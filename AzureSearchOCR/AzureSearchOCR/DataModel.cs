using Microsoft.Azure.Search.Models;

namespace AzureSearchOCR
{
    [SerializePropertyNamesAsCamelCase]
    public class OCRTextIndex
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public string ocrText { get; set; }
    }
}