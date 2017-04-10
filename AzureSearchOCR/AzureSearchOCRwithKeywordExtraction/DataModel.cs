using Microsoft.Azure.Search.Models;

namespace AzureSearchOCRwithKeywordExtraction
{
    [SerializePropertyNamesAsCamelCase]
    public class OCRTextIndex
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public string ocrText { get; set; }
        public string[] keyPhrases { get; set; }
    }
}