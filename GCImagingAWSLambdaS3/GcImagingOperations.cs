using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Imaging;

namespace GCImagingAWSLambdaS3
{
    public class GcImagingOperations
    {
        public static string GetConvertedImage(Stream stream)
        {
            using (var bmp = new GcBitmap())
            {
                bmp.Load(stream);
                // Add watermark
                using (var g = bmp.CreateGraphics(Color.White))
                {
                    g.DrawString("This is a watermarked string", new TextFormat
                    {
                        FontSize = 96,
                        ForeColor = Color.FromArgb(128, Color.Yellow),
                        Font = FontCollection.SystemFonts.DefaultFont
                    },
                    new RectangleF(0, 0, bmp.Width, bmp.Height),
                    TextAlignment.Center, ParagraphAlignment.Center, false);
                }
                // Convert to grayscale
                bmp.ApplyEffect(GrayscaleEffect.Get(GrayscaleStandard.BT601));
                // Resize to thumbnail
                var resizedImage = bmp.Resize(100, 100, InterpolationMode.NearestNeighbor);
                return GetBase64(resizedImage);
            }
        }
        
        #region helper
        private static string GetBase64(GcBitmap bmp)
        {
            using (Image image = Image.FromGcBitmap(bmp, true))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    bmp.SaveAsPng(m);
                    return Convert.ToBase64String(m.ToArray());
                }
            }
        }
        #endregion
    }
}
