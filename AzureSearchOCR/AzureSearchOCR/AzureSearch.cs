using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureSearchOCR
{
    public class AzureSearch
    {
        public static void CreateIndex(SearchServiceClient serviceClient, string indexName)
        {
            if (serviceClient.Indexes.Exists(indexName))
            {
                serviceClient.Indexes.Delete(indexName);
            }
            var definition = new Index()
            {
                Name = indexName,
                Fields = new[]
                {
                    new Field("fileId", DataType.String)                       { IsKey = true },
                    new Field("fileName", DataType.String)                     { IsSearchable = true, IsFilterable = false, IsSortable = false, IsFacetable = false },
                    new Field("ocrText", DataType.String)                      { IsSearchable = true, IsFilterable = false, IsSortable = false, IsFacetable = false }
                }
            };
            serviceClient.Indexes.Create(definition);
        }

        public static void UploadDocuments(SearchIndexClient indexClient, string fileId, string fileName, string ocrText)
        {
            List<IndexAction> indexOperations = new List<IndexAction>();
            var doc = new Document();
            doc.Add("fileId", fileId);
            doc.Add("fileName", fileName);
            doc.Add("ocrText", ocrText);
            indexOperations.Add(IndexAction.Upload(doc));
            try
            {
                indexClient.Documents.Index(new IndexBatch(indexOperations));
            }
            catch (IndexBatchException e)
            {
                Console.WriteLine(
                "Failed to index some of the documents: {0}",
                       String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }

        public static void SearchDocuments(SearchIndexClient indexClient, string searchText)
        {
            try
            {
                var sp = new SearchParameters();
                DocumentSearchResult<OCRTextIndex> response = indexClient.Documents.Search<OCRTextIndex>(searchText, sp);
                foreach (SearchResult<OCRTextIndex> result in response.Results)
                {
                    Console.WriteLine("File ID: {0}", result.Document.fileId);
                    Console.WriteLine("File Name: {0}", result.Document.fileName);
                    Console.WriteLine("Extracted Text: {0}", result.Document.ocrText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed search: {0}", e.Message.ToString());
            }
        }
    }
}