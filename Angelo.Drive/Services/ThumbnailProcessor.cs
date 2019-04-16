using Angelo.Connect.Models;
using Angelo.Drive.Abstraction;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Drive.Services
{
    public class ThumbnailProcessor : IImagePostProcessor
    {
        private const int ThumbnailWidth = 600;
        private string _systemRootPath;
        private string _processedDirFolder = "processedImages";
        private string _thumbFolder = "thumb";

        public ThumbnailProcessor(string systemRootPath)
        {
            _systemRootPath = systemRootPath;
        }

        public async Task Invoke(string librarypath, FileDocument document)
        {
            var physicalOriginalFile = GetOriginalFilePath(librarypath, document.DocumentId);
            var physicalThumbPath = GetThumbnailPath(librarypath, document.DocumentId);


            ImageFormat format;

            switch (document.FileExtension)
            {
                case ".jpg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    break;
                case ".gif":
                    format = ImageFormat.Gif;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                case ".tiff":
                    format = ImageFormat.Tiff;
                    break;
                default:
                    format = ImageFormat.Png;
                    break;
            }

            var thumbnail = GetImageThumbnail(physicalOriginalFile, format);

            if (thumbnail.Length > 0)
            {
                //if the nominal res version still is greater than original. PNG scenario.
                if (thumbnail.Length > (new FileInfo(physicalOriginalFile).Length))
                {
                    File.Copy(physicalOriginalFile, physicalThumbPath, true);
                }
                else
                {

                    thumbnail.Position = 0;

                    using (var output = new FileStream(physicalThumbPath, FileMode.Create))
                    {
                        await thumbnail.CopyToAsync(output);
                    }
                }
            }


        }

        private Stream GetImageThumbnail(string filePath, ImageFormat format)
        {
            Bitmap bImageFile = new Bitmap(filePath);
            
            var result = new MemoryStream();

            if (bImageFile.Width < ThumbnailWidth)
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
                    graphics.DrawImage(bImageFile, destRect, 0, 0, bImageFile.Width, bImageFile.Height, GraphicsUnit.Pixel,
                        wrapMode);
                }
            }

            using (destImage)
            {
                destImage.Save(result, format);

            }

            bImageFile.Dispose();

            return result;
           
        }

        private Size ResizeFit(Image original)
        {
            var widthRatio = (double)ThumbnailWidth / (double)original.Width;
            var heightRatio = (double)ThumbnailWidth / (double)original.Height;
            var minAspectRatio = Math.Min(widthRatio, heightRatio);
            if (minAspectRatio > 1)
                return new Size(original.Width, original.Height);
            return new Size((int)(original.Width * minAspectRatio), (int)(original.Height * minAspectRatio));
        }

        private string GetOriginalFilePath(string librarypath, string fileName)
        {
            return Path.Combine(_systemRootPath, librarypath, fileName);
           
        }

        private string GetThumbnailPath(string librarypath, string fileName)
        {
            var physicalThumbFilePath = Path.Combine(_systemRootPath, _processedDirFolder, librarypath, _thumbFolder);
            EnsurePathExists(physicalThumbFilePath);
            return Path.Combine(physicalThumbFilePath , fileName);
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
            var physicalFilePath = GetThumbnailPath(librarypath, documentId);

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
            var thumbPath = GetThumbnailPath(librarypath, documentId);
            if (!File.Exists(thumbPath)) return null;

            var fileStream = new MemoryStream();
            using (FileStream fs = File.OpenRead(thumbPath))
            {
                fs.CopyTo(fileStream);
                return fileStream;
            }
            
        }
    }
}
