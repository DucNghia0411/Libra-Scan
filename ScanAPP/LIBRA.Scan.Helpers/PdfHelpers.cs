using Docnet.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LIBRA.Scan.Data.InfraStructure;
using System.IO;
using LIBRA.Scan.Helpers.Constracts;

namespace LIBRA.Scan.Helpers
{
    public class PdfHelpers : IPdfHelpers
    {
        private readonly Fixture _fixture;

        public PdfHelpers() 
        {
            _fixture = new Fixture();
        }

        public bool ConvertPDFToImages(string filePath, string fileName, int dimOne, int dimTwo)
        {
            try
            {
                using var docReader = _fixture.DocNet.GetDocReader(filePath, new PageDimensions(dimOne, dimTwo));
                int total = docReader.GetPageCount();
                for (int i = 0; i <= total; i++)
                {
                    using var pageReader = docReader.GetPageReader(i);
                    byte[] rawBytes = pageReader.GetImage();
                    var width = pageReader.GetPageWidth();
                    var height = pageReader.GetPageHeight();
                    var characters = pageReader.GetCharacters();
                    using var bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    AddBytes(bmp, rawBytes);
                    DrawRectangles(bmp, characters);
                    using var stream = new MemoryStream();
                    bmp.Save(stream, ImageFormat.Png);
                    File.WriteAllBytes(fileName + $"{i}.png", stream.ToArray());
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void AddBytes(Bitmap bmp, byte[] rawBytes)
        {
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var pNative = bmpData.Scan0;

            Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
            bmp.UnlockBits(bmpData);
        }

        private static void DrawRectangles(Bitmap bmp, IEnumerable<Character> characters)
        {
            var pen = new System.Drawing.Pen(System.Drawing.Color.Red);

            using var graphics = Graphics.FromImage(bmp);

            foreach (var c in characters)
            {
                var rect = new System.Drawing.Rectangle(c.Box.Left, c.Box.Top, c.Box.Right - c.Box.Left, c.Box.Bottom - c.Box.Top);
                graphics.DrawRectangle(pen, rect);
            }
        }
    }
}
