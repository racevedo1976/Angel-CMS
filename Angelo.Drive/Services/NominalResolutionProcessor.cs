using Angelo.Drive.Abstraction;
using System;
using System.Threading.Tasks;
using Angelo.Connect.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Drawing.Drawing2D;

namespace Angelo.Drive.Services
{
    public class NominalResolutionProcessor : IImagePostProcessor
    {
        private string _systemRootPath;
        private const int maxImageWidth = 1900;
        private const int maxResolution = 72;
        private string _processedDirFolder = "processedImages";
        private string _lowResFolder = "lowRes";
        public NominalResolutionProcessor(string systemRootPath)
        {
            _systemRootPath = systemRootPath;
        }
        public async Task Invoke(string librarypath, FileDocument document)
        {
            var physicalOriginalFile = GetOriginalFilePath(librarypath, document.DocumentId);
            var physicalLowResPath = GetLowResFilePath(librarypath, document.DocumentId);
            
            ImageFormat format;

            switch (document.FileExtension)
            {
                case ".jpg":
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case ".png":
                    format = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                case ".gif":
                    format = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case ".bmp":
                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                case ".tiff":
                    format = System.Drawing.Imaging.ImageFormat.Tiff;
                    break;
                default:
                    format = System.Drawing.Imaging.ImageFormat.Png;
                    break;
            }

            var lowResImage = GetImageResizedVersion(physicalOriginalFile, format);

            if (lowResImage.Length > 0)
            {
                //if the nominal res version still is greater than original. PNG scenario.
                if (lowResImage.Length > (new FileInfo(physicalOriginalFile).Length))
                {
                    File.Copy(physicalOriginalFile, physicalLowResPath, true);
                }
                else
                {
                    lowResImage.Position = 0;

                    using (var output = new FileStream(physicalLowResPath, FileMode.Create))
                    {
                        await lowResImage.CopyToAsync(output);
                    }
                }

                
            }

            //return Task.CompletedTask;

        }

        private Stream GetImageResizedVersion(string filePath, ImageFormat format)
        {
            Bitmap bImageFile = new Bitmap(filePath);
            //using (var myImage = Image.FromFile(filePath))
            //{
            var result = new MemoryStream();

            if (bImageFile.Width < maxImageWidth)
            {
                bImageFile.Save(result, format);
                return result;
            }

            // calculate dimensions
            var newImageSize = ResizeFit(bImageFile);


            var destRect = new Rectangle(0, 0, newImageSize.Width, newImageSize.Height);
            var destImage = new Bitmap(newImageSize.Width, newImageSize.Height);

            destImage.SetResolution(bImageFile.HorizontalResolution, bImageFile.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(bImageFile, destRect, 0, 0, bImageFile.Width, bImageFile.Height,
                        GraphicsUnit.Pixel,
                        wrapMode);
                }
            }

            using (destImage)
            {
                destImage.Save(result, format);

            }

            bImageFile.Dispose();

            return result;
            //  }




            //using (var myImage = Image.FromFile(filePath))
            //{

            //    var result = new MemoryStream();
            //    var qualityParamId = Encoder.ColorDepth;
            //    var encoderParameters = new EncoderParameters(1);
            //    encoderParameters.Param[0] = new EncoderParameter(qualityParamId, maxResolution);

            //    var codec = ImageCodecInfo.GetImageDecoders()
            //        .FirstOrDefault(dec => dec.FormatID == format.Guid);

            //    if (myImage.Width < maxImageWidth)
            //    {
            //        myImage.Save(result, codec, encoderParameters);
            //        return result;
            //    }


            //    //calculate dimensions
            //    var newImageSize = ResizeFit(myImage);

            //    using (var thumbnail = myImage.GetThumbnailImage(newImageSize.Width, newImageSize.Height, new Image.GetThumbnailImageAbort(() => false), IntPtr.Zero))
            //    {

            //        thumbnail.Save(result, codec, encoderParameters);

            //        return result;
            //    }
            //}
        }

        private Size ResizeFit(Image original)
        {
            var widthRatio = (double)maxImageWidth / (double)original.Width;
            var heightRatio = (double)maxImageWidth / (double)original.Height;
            var minAspectRatio = Math.Min(widthRatio, heightRatio);
            if (minAspectRatio > 1)
                return new Size(original.Width, original.Height);
            return new Size((int)(original.Width * minAspectRatio), (int)(original.Height * minAspectRatio));
        }

        private string GetOriginalFilePath(string librarypath, string fileName)
        {
            return Path.Combine(_systemRootPath, librarypath, fileName);

        }

        private string GetLowResFilePath(string librarypath, string fileName)
        {
            var physicalThumbFilePath = Path.Combine(_systemRootPath, _processedDirFolder, librarypath, _lowResFolder);
            EnsurePathExists(physicalThumbFilePath);
            return Path.Combine(physicalThumbFilePath, fileName);
        }

        private void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public Task Delete(string librarypath, string documentId)
        {
            var physicalFilePath = GetLowResFilePath(librarypath, documentId);

            if (File.Exists(physicalFilePath))
            {
                try
                {
                    File.Delete(physicalFilePath);
                }
                catch (Exception ex)
                {
                    var x = ex.Message;
                }

            }

            return Task.CompletedTask;
        }

        public Stream GetFileStream(string librarypath, string documentId)
        {
            var filePath = GetLowResFilePath(librarypath, documentId);
            if (!File.Exists(filePath)) return null;

            return File.OpenRead(filePath);
        }
    }
}