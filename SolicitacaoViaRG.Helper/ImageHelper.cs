
using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace SolicitacaoViaRG.Helper
{
    public static class ImageHelper
    {
        public static string SaveImage(string base64Image, string identifier, string imageDirectory)
        {
            var agora = DateTime.Now.ToString("ddMMyyyy_hhmmss");
            var imageBytes = Convert.FromBase64String(base64Image);

            var fileName = $"{identifier}_{agora}.jpeg";
            var filePath = Path.Combine(imageDirectory, fileName);

            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                var image = Image.Load(ms);
                image.Save(filePath, new JpegEncoder());
            }

            return filePath;
        }

        public static bool IsBase64Image(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
            || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
            return false;

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    var format = Image.DetectFormat(ms);
                    return format.Name ==  "JPEG" || format.Name == "PNG";
            
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

