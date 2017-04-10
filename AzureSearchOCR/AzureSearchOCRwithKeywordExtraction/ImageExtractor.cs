using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;

namespace AzureSearchOCRwithKeywordExtraction
{
    public static class PdfImageExtractor
    {
        #region Methods
        #region Public Methods

        public static bool PageContainsImages(string filename, int pageNumber)
        {
            using (var reader = new PdfReader(filename))
            {
                var parser = new PdfReaderContentParser(reader);
                ImageRenderListener listener = null;
                parser.ProcessContent(pageNumber, (listener = new ImageRenderListener()));
                return listener.Images.Count > 0;
            }
        }
        public static Dictionary<string, System.Drawing.Image> ExtractImages(string filename)
        {
            var images = new Dictionary<string, System.Drawing.Image>();
            using (var reader = new PdfReader(filename))
            {
                var parser = new PdfReaderContentParser(reader);
                ImageRenderListener listener = null;
                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    parser.ProcessContent(i, (listener = new ImageRenderListener()));
                    var index = 1;

                    if (listener.Images.Count > 0)
                    {
                        Console.WriteLine("Found {0} images on page {1}.", listener.Images.Count, i);
                        foreach (var pair in listener.Images)
                        {
                            images.Add(string.Format("{0}_Page_{1}_Image_{2}{3}",
                                System.IO.Path.GetFileNameWithoutExtension(filename), i.ToString("D4"), index.ToString("D4"), pair.Value), pair.Key);
                            index++;
                        }
                    }
                }
                return images;
            }
        }
        public static Dictionary<string, System.Drawing.Image> ExtractImages(string filename, int pageNumber)
        {
            Dictionary<string, System.Drawing.Image> images = new Dictionary<string, System.Drawing.Image>();
            PdfReader reader = new PdfReader(filename);
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            ImageRenderListener listener = null;
            parser.ProcessContent(pageNumber, (listener = new ImageRenderListener()));
            int index = 1;
            if (listener.Images.Count > 0)
            {
                Console.WriteLine("Found {0} images on page {1}.", listener.Images.Count, pageNumber);
                foreach (KeyValuePair<System.Drawing.Image, string> pair in listener.Images)
                {
                    images.Add(string.Format("{0}_Page_{1}_Image_{2}{3}",
                        System.IO.Path.GetFileNameWithoutExtension(filename), pageNumber.ToString("D4"), index.ToString("D4"), pair.Value), pair.Key);
                    index++;
                }
            }
            return images;
        }
        #endregion Public Methods
        #endregion Methods
    }
    internal class ImageRenderListener : IRenderListener
    {
        #region Fields
        Dictionary<System.Drawing.Image, string> images = new Dictionary<System.Drawing.Image, string>();
        #endregion Fields
        #region Properties
        public Dictionary<System.Drawing.Image, string> Images
        {
            get { return images; }
        }
        #endregion Properties
        #region Methods
        #region Public Methods
        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderImage(ImageRenderInfo renderInfo)
        {
            PdfImageObject image = renderInfo.GetImage();
            PdfName filter = (PdfName)image.Get(PdfName.FILTER);
            if (filter != null)
            {
                System.Drawing.Image drawingImage = image.GetDrawingImage();
                string extension = ".";
                if (filter == PdfName.DCTDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.JPG.FileExtension;
                }
                else if (filter == PdfName.JPXDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.JP2.FileExtension;
                }
                else if (filter == PdfName.FLATEDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.PNG.FileExtension;
                }
                else if (filter == PdfName.LZWDECODE)
                {
                    extension += PdfImageObject.ImageBytesType.CCITT.FileExtension;
                }
                this.Images.Add(drawingImage, extension);
            }
        }
        public void RenderText(TextRenderInfo renderInfo) { }
        #endregion Public Methods
        #endregion Methods
    }
}