using System;
using System.IO;
using System.Text;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace AzureSearchOCR
{
    namespace AzureSearchOCR
    {
        public class VisionHelper
        {
            private readonly IVisionServiceClient visionClient;
            public VisionHelper(string subscriptionKey)
            {
                this.visionClient = new VisionServiceClient(subscriptionKey);
            }
            public OcrResults RecognizeText(string imagePathOrUrl, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect)
            {
                this.ShowInfo("Recognizing");
                OcrResults ocrResult = null;
                string resultStr = string.Empty;
                try
                {
                    if (File.Exists(imagePathOrUrl))
                    {
                        using (FileStream stream = File.Open(imagePathOrUrl, FileMode.Open))
                        {
                            ocrResult = this.visionClient.RecognizeTextAsync(stream, languageCode, detectOrientation).Result;
                        }
                    }
                    else
                    {
                        this.ShowError("Invalid image path or Url");
                    }
                }
                catch (ClientException e)
                {
                    if (e.Error != null)
                    {
                        this.ShowError(e.Error.Message);
                    }
                    else
                    {
                        this.ShowError(e.Message);
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    this.ShowError("Error: " + ex.Message.ToString());
                    return null;
                }
                return ocrResult;
            }
            public string GetRetrieveText(OcrResults results)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (results != null && results.Regions != null)
                {
                    stringBuilder.Append("Text: ");
                    stringBuilder.AppendLine();
                    foreach (var item in results.Regions)
                    {
                        foreach (var line in item.Lines)
                        {
                            foreach (var word in line.Words)
                            {
                                stringBuilder.Append(word.Text);
                                stringBuilder.Append(" ");
                            }
                            stringBuilder.AppendLine();
                        }
                        stringBuilder.AppendLine();
                    }
                }
                return stringBuilder.ToString();
            }
            private void ShowInfo(string workStr)
            {
                Console.WriteLine(string.Format("{0}......", workStr));
                Console.ResetColor();
            }
            private void ShowError(string errorMessage)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errorMessage);
                Console.ResetColor();
            }
            private void ShowResult(string resultMessage)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(resultMessage);
                Console.ResetColor();
            }
        }
    }
}