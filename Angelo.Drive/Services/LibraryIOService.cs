using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Runtime.InteropServices;
using Angelo.Drive.Tools;
using System.Drawing;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Angelo.Drive.Services
{
    public class LibraryIOService : IDocumentDownloadService<FileDocument>,
                                    IDocumentUploadService<FileDocument>,
                                    IDocumentThumbnailService<FileDocument>
    {
        
        private string _systemRootPath;
        private string _systemCachePath;
        public const string ThumbnailCachePath = "thumbnails";
        private const int ThumbnailWidth = 600;
        private const int ThumbnailResolution = 72;

        public object WindowsThumbnailProvider { get; private set; }

        public LibraryIOService(string systemRootPath, string systemCachePath)
        {
            _systemRootPath = systemRootPath;
            _systemCachePath = systemCachePath;
        }

        public async Task CopyFolderAsync(string folderId, string destinationFolderId)
        {
            // No-op...folders do not physically exist
        }

        //public async Task<FileDocument> CopyDocumentAsync(FileDocument source, FileDocument destination)
        //{
        //    var sourcePath = GetPhysicalPath(source);
        //    var destinationPath = GetPhysicalPath(destination);

        //    File.Copy(sourcePath, destinationPath);

        //    // Flush the destination cache entry
        //    await SetCachedThumbnailAsync(destination, null);

        //    return destination;
        //}

        //public void DeleteDocument(FileDocument document)
        //{
        //    var filePath = GetPhysicalPath(document);

        //    if (File.Exists(filePath))
        //    {
        //        System.IO.File.Delete(filePath);
        //    }
            
        //}

        private string GetPhysicalPath(IDocument document, string libraryLocation)
        {
            return GetPhysicalPath(document.DocumentId, libraryLocation);
        }

        private string GetPhysicalPath(string documentId, string libraryLocation)
        {
            var physicalFilePath =  Path.Combine(_systemRootPath, libraryLocation, documentId);
            EnsurePathExists(Path.Combine(_systemRootPath, libraryLocation));
            return physicalFilePath;
        }

        //private string GetThumbPhysicalPath(IDocument document, string libraryLocation)
        //{
        //    return GetThumbPhysicalPath(document.DocumentId, libraryLocation);
        //}
        private string GetThumbPhysicalPath(string documentId, string libraryLocation)
        {
            var physicalThumbFilePath = Path.Combine(_systemRootPath, ThumbnailCachePath, libraryLocation, documentId);
            EnsurePathExists(Path.Combine(_systemRootPath, ThumbnailCachePath, libraryLocation));
            return physicalThumbFilePath;
        }

        private string GetCachePath(string documentId)
        {
            return Path.Combine(_systemCachePath, ThumbnailCachePath, documentId);
        }

        public async Task SetFileStreamAsync(FileDocument document, Stream fileStream, string libraryLocation)
        {
            if (fileStream.Length > 0)
            {
                fileStream.Position = 0;

                using (var output = new FileStream(GetPhysicalPath(document, libraryLocation), FileMode.Create))
                {
                    await fileStream.CopyToAsync(output);
                }
            }


           // await GenerateThumbnailForImage(document, libraryLocation);
            // Flush the destination cache entry
           // await SetCachedThumbnailAsync(document, null);
        }

        //private async Task GenerateThumbnailForImage(FileDocument document, string locationPath)
        //{
            
        //    //create a thumbnail versions of the immage
        //    var imageThumbnail = await GetThumbnailAsync(document, 600, 600, locationPath);

        //    if (imageThumbnail.Length > 0)
        //    {
        //        imageThumbnail.Position = 0;

        //        using (var output = new FileStream(GetThumbPhysicalPath(document.DocumentId, locationPath), FileMode.Create))
        //        {
        //            await imageThumbnail.CopyToAsync(output);
        //        }
        //    }

            
        //}

        public async Task<Stream> GetFileStreamAsync(FileDocument document, string libraryLocation)
        {
            var filePath = GetPhysicalPath(document, libraryLocation);

            Stream result;
            if (File.Exists(filePath))
            {
                result = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                result = null;
            }

            return result;
        }

        public async Task<Stream> GetThumbnailAsync(FileDocument document, int width, int height, string libraryLocation)
        {
            Stream result = null;
            var filePath = GetPhysicalPath(document, libraryLocation);

            if (File.Exists(filePath))
            {
                result = GetDocumentThumbnail(filePath, width, height);
            }
            return result;
        }

        internal void DeleteFileDocument(string documentId, string location)
        {
            var physicalFilePath = GetPhysicalPath(documentId, location);

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
        }

        public Stream CropImage(string documentId, int x, int y, int width, int height, string physicalLocation)
        {
            var filePath = GetPhysicalPath(documentId, physicalLocation);

            using (var myImage = Image.FromFile(filePath))
            {

                try
                {
                    //check the image height against our desired image height
                    if (myImage.Height < height)
                    {
                        height = myImage.Height;
                    }

                    if (myImage.Width < width)
                    {
                        width = myImage.Width;
                    }

                    //create a bitmap window for cropping
                    Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                    bmPhoto.SetResolution(72, 72);

                    //create a new graphics object from our image and set properties
                    Graphics grPhoto = Graphics.FromImage(bmPhoto);
                    grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    //now do the crop
                    grPhoto.DrawImage(myImage, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);

                    // Save out to memory and get an image from it to send back out the method.
                    MemoryStream mm = new MemoryStream();
                    bmPhoto.Save(mm, ImageFormat.Jpeg);
                    myImage.Dispose();
                    bmPhoto.Dispose();
                    grPhoto.Dispose();
                   
                    return mm;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error cropping image, the error was: " + ex.Message);
                }

            }
        }

        private Stream GetDocumentThumbnail(string filePath, int width, int height)
        {
            var thumbnail = ThumbnailProvider.GetThumbnail(filePath, width, height, ThumbnailOptions.None);

            var result = new MemoryStream();
            thumbnail.Save(result, System.Drawing.Imaging.ImageFormat.Png);

            return result;
        }

        private Stream GetPresentationThumbnail(string filePath, int width, int height)
        {
            return GetDocumentThumbnail(filePath, width, height);
        }

        private Stream GetImageThumbnail(string filePath, int targetWidth, int height, System.Drawing.Imaging.ImageFormat format)
        {

            var myImage = Image.FromFile(filePath);
            //calculate dimensions
            int iwidth, iheight;
            if (myImage.Width > myImage.Height)
            {
                iwidth = targetWidth;
                iheight = Convert.ToInt32(myImage.Height * targetWidth / (double)myImage.Width);
            }
            else
            {
                iwidth = Convert.ToInt32(myImage.Width * targetWidth / (double)myImage.Height);
                iheight = targetWidth;
            }


            var thumbnail = myImage.GetThumbnailImage(iwidth, iheight, new Image.GetThumbnailImageAbort(() => false), IntPtr.Zero);

            //var thumbnail = Image.FromFile(filePath).GetThumbnailImage(width, height, new Image.GetThumbnailImageAbort(() => false), IntPtr.Zero);

            var result = new MemoryStream();
            thumbnail.Save(result, format);

            return result;
        }

        private Stream GetAudioThumbnail(string filePath, int width, int height)
        {
            return GetDocumentThumbnail(filePath, width, height);
        }

        private Stream GetEBookThumbnail(string filePath, int width, int height)
        {
            return GetDocumentThumbnail(filePath, width, height);
        }

        private Stream GetVideoThumbnail(string filePath, int width, int height)
        {            
            return GetDocumentThumbnail(filePath, width, height);
        }

        private Stream GetCachedThumbnail(IDocument document, string libraryLocation)
        {
            var path = GetThumbPhysicalPath(document.DocumentId, libraryLocation);
             if (!File.Exists(path)) return null;

            return File.OpenRead(path);
        }

        private Task SetCachedThumbnailAsync(IDocument document, Stream data)
        {
            var path = GetCachePath(document.DocumentId);

            if (data == null)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (IOException) { }
                }
            }
            else
            {
                using (var file = File.Exists(path) ? File.OpenWrite(path) : File.Create(path))
                {
                    data.Position = 0;
                    data.CopyTo(file);
                }
            }

            return Task.CompletedTask;
        }

        private void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
