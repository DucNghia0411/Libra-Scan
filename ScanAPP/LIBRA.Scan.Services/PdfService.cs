using Docnet.Core.Models;
using LIBRA.Scan.Common.Setting;
using LIBRA.Scan.Data.InfraStructure;
using LIBRA.Scan.Services.Constracts;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services
{
    public class PdfService : IPdfService
    {
        private readonly IPageService _pageService;
        private readonly IImageService _imageService;

        public PdfService() 
        {
            _pageService = new PageService();
            _imageService = new ImageService();
        }

        public void ConvertImageToPdf(string imagePath, string pdfPath)
        {
            PdfDocument document = new PdfDocument();

            XImage image = XImage.FromFile(imagePath);
            double imageWidth = image.PixelWidth;
            double imageHeight = image.PixelHeight;

            PdfPage page = document.AddPage();
            page.Width = XUnit.FromPoint(imageWidth);
            page.Height = XUnit.FromPoint(imageHeight);

            XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.DrawImage(image, 0, 0, imageWidth, imageHeight);

            document.Save(pdfPath);
            document.Close();
        }

        public void ConvertImagesToPdf(string[] imagePaths, string pdfPath)
        {
            PdfDocument document = new PdfDocument();

            foreach (string imagePath in imagePaths)
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XImage image = XImage.FromFile(imagePath);

                double imageWidth = image.PixelWidth;
                double imageHeight = image.PixelHeight;

                double scaleFactor = 1;
                if (imageWidth > page.Width || imageHeight > page.Height)
                {
                    double scaleX = page.Width / imageWidth;
                    double scaleY = page.Height / imageHeight;
                    scaleFactor = Math.Min(scaleX, scaleY);
                }

                double x = (page.Width - (imageWidth * scaleFactor)) / 2;
                double y = (page.Height - (imageHeight * scaleFactor)) / 2;

                gfx.DrawImage(image, x, y, imageWidth * scaleFactor, imageHeight * scaleFactor);
            }

            document.Save(pdfPath);

            document.Close();
        }

        public bool ConvertBatchToPdf(long documentId)
        {
            try
            {
                var pages = _pageService.GetByDocumentId(documentId);
                foreach (var page in pages)
                {
                    var images = _imageService.GetByPageId(page.Id);
                    foreach (var image in images)
                    {
                        if (image.Path != null)
                        {
                            string path = image.Path;
                            string directory = Path.GetDirectoryName(path);
                            string newDirectory = directory.Replace(FolderSetting.TempData, FolderSetting.Transfer);

                            if (!Directory.Exists(newDirectory))
                                Directory.CreateDirectory(newDirectory);

                            string pdfPath = Path.Combine(newDirectory, Path.GetFileNameWithoutExtension(path)) + ".pdf";

                            ConvertImageToPdf(path, pdfPath);
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
