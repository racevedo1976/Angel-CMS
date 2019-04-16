using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Services;
using Angelo.Connect.Models;

using Angelo.Connect.SlideShow.Data;
using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.SlideShow.Services;
using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Plugins;

using System.Net.Http;
using System.Threading;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using Angelo.Connect.Extensions;
using System.IO;
using System.Reflection;

namespace Angelo.Connect.SlideShow.Data
{
    public class SlideShowDocSeed : IPluginStartupAction
    {
        private IFolderManager<Slide> _slideShowManager;
        private IDocumentService<Slide> _slideService;

        // For the underlying document (physical file) seeding
        private IDocumentService<FileDocument> _documentService;
        private IFolderManager<FileDocument> _folderManager;

        private SlideShowDbContext _readDbSlideShow;
        private DbContextOptions<SlideShowDbContext> _writeDbSlideShow;

        private ConnectDbContext _readDbConnect;
        private DbContextOptions<ConnectDbContext> _writeDbConnect;

        private SlideShowService _slideShowService;

        private ConnectDocumentOptions _connectDocumentOptions;

        private string _userId_Admin = "AFCF7980-4BA7-4DD2-879D-599D058F7E73";
        private string _userId_Jane = "EECEFCC1-8050-4A0F-A5A5-D7ED19A078A8";
        private string _userId_Joe = "13B2D0D1-F8A6-487E-9D60-A1E89DCC610B";

        private string _documentId1 = "C1FEF685F54D4B56";
        private string _documentId2 = "E5FB74BE7459424D";
        private string _documentId3 = "C9491DB54CA32ADF";
        private string _documentId4 = "A312E95D51294851";

        public SlideShowDocSeed
        (
            IFolderManager<Slide> slideShowManager,
            IDocumentService<Slide> slideService,
            IFolderManager<FileDocument> folderManager,
            IDocumentService<FileDocument> documentService,
            SlideShowService slideShowService,
            SlideShowDbContext readDbSlideShow,
            DbContextOptions<SlideShowDbContext> writeDbSlideShow,
            ConnectDbContext readDbConnect,
            DbContextOptions<ConnectDbContext> writeDbConnect,
            ConnectDocumentOptions connectDocumentOptions)  // For getting Drive URL
        {
            _slideShowManager = slideShowManager;
            _slideService = slideService;
            _folderManager = folderManager;
            _documentService = documentService;
            _slideShowService = slideShowService;
            _readDbSlideShow = readDbSlideShow;
            _writeDbSlideShow = writeDbSlideShow;
            _readDbConnect = readDbConnect;
            _writeDbConnect = writeDbConnect;
            _connectDocumentOptions = connectDocumentOptions;
        }

        public void Invoke()
        {
            //var documentType = typeof(Slide).FullName;

            //if (!_readDbConnect.Folders.Any(x => x.DocumentType == documentType))
            //{
            //    //SeedData();
            //}
        }

        public void SeedData()
        {
            var isSeeded = _readDbSlideShow.Slides.Count() > 0;
            if (!isSeeded)
            {
                foreach (var user in new[] { _userId_Admin, _userId_Jane, _userId_Joe })
                {
                    CreateSlideShow("Basic sample slideshow", user, null);
                    //CreateSlideShow("Parallax sample slideshow", user, CreateParallaxEffect());
                    //CreateSlideShow("Ken Burns sample slidesho", user, CreateKenBurnsEffect());
                }
            }
       }

        private void CreateSlideShow(string title, string userId, IEffect<Slide> effect)
        {
            // Create the slideshow (folder)
            var folder = _slideShowManager.CreateFolderAsync(new Folder()
            {
                Title = title, 
                FolderType = typeof(Slide).FullName,
                OwnerId = userId
            }).Result;

            // Create the widget
            //var slideShow = CreateSlideShow(folder);

            var imageFolder = GetImageFolder(userId);
            // Add 3 slides to the slideshow
            foreach (var image in new[] { "/img/SeedImages/ants.png", "/img/SeedImages/beach.png", "/img/SeedImages/cliffs.png" })
            {
                // Add the FileDocument (logical and physical) to Connect/Drive
                var document = CreateFileDocumentAsync(userId, image, imageFolder).Result;

                // Create the slide
                var i = image.LastIndexOf('/') + 1;
                var topic = image.Substring(i, image.LastIndexOf('.') - i);
                var slide = CreateSlide(document, effect, $"http://www.google.com/search?q={topic}");

                // Add the slide that references this image
                _slideService.CreateAsync(slide);
                _slideShowManager.AddDocumentAsync(slide, folder);
            }
        }

        //private Models.SlideShow CreateSlideShow(IFolder slideshow)
        //{
        //    var result = new Models.SlideShow()
        //    {
        //        Id = KeyGen.NewGuid(),
        //        Title = slideshow.Title,
        //        FolderId = slideshow.Id,
        //        Type = SlideShowType.Standard
        //    };

        //    return _slideShowService.CreateSlideShowAsync(result).Result;
        //}

        private static readonly string DocumentType = typeof(FileDocument).FullName;

        private Folder GetImageFolder(string userId)
        {
            // Get
            SpinWait.SpinUntil(() => IsImageFolderSeeded(userId));

            var result = _readDbConnect.Folders
                .Include(x => x.ParentFolder)
                .SingleOrDefault(x => x.OwnerId == userId
                                && x.ParentFolder != null
                                && string.IsNullOrEmpty(x.ParentFolder.ParentId)
                                && x.DocumentType == DocumentType
                                && string.Equals(x.Title, "Images", StringComparison.CurrentCultureIgnoreCase));
            if (result == null)
            {
                throw new InvalidOperationException("Unable to find Images folder. Is Core seeding not complete?");
            }

            return result;
        }

        private bool IsImageFolderSeeded(string userId)
        {
            // NOT WORKING FOR SOME REASON
   //         if (!_readDbConnect.Database.TableExists("plugin.Folder")) return false;
            try
            {
                return _readDbConnect.Folders
                    .Include(x => x.ParentFolder)
                    .Any(x => x.OwnerId == userId
                                && x.ParentFolder != null
                                && string.IsNullOrEmpty(x.ParentFolder.ParentId)
                                && x.DocumentType == DocumentType
                                && string.Equals(x.Title, "Images", StringComparison.CurrentCultureIgnoreCase));
            }
            catch (SqlException)
            {
                // HACK: This is sub-optimal, but I can't find another way to see if FileDocuments exists yet. (DB.EnsureCreated always returns false)
                return false;
            }
        }

        private async Task<FileDocument> CreateFileDocumentAsync(string userId, string image, Folder destination)
        {
            var fileName = image.Substring("/assets/images/".Length);
            var result = new FileDocument()
            {
                DocumentId = KeyGen.NewGuid(),
                Title = fileName,
                FileName = fileName,
                FileType = FileType.Image
            };

            // upload it
            await UploadDriveDocumentAsync(result, image, destination);

            return result;
            // Current, Drive does this with the upload (taking that out later)
            //using (var db = new ConnectDbContext(_writeDbConnect))
            //{
            //    // Create it
            //    db.FileDocuments.Add(result);

            //    // Add it to a folder
            //    db.FolderItems.Add(new FolderItem()
            //    {
            //        Id = KeyGen.NewGuid(),
            //        Folder = destination,
            //        FolderId = destination.Id,
            //        DocumentId = result.DocumentId
            //    });

            //    db.SaveChanges();
            //}
        }

        private async Task UploadDriveDocumentAsync(FileDocument document, string filePath, IFolder destination)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var @namespace = typeof(SlideShowPlugin).Namespace;
            filePath = filePath.Replace('\\', '/').TrimStart('/');
            filePath = @namespace + "." + filePath;
            filePath = filePath.Replace('/', '.').Replace("SlideShow", "Slideshow");    // HACK/TODO: Why is the manifest name "Slideshow", not "SlideShow"?

            var url = $"{_connectDocumentOptions.DriveAuthority}/upload";
            using (var client = new HttpClient())
            {
                // var stream = assembly.GetManifestResourceStream();
                using (var fileStream = assembly.GetManifestResourceStream(filePath))
                {
                    if (fileStream == null) throw new InvalidOperationException($"File not found: '{filePath}'.");

                    document = await _documentService.CreateAsync(document);
                    try
                    {
                        await _folderManager.AddDocumentAsync(document, destination);
                        try
                        {
                            var fileSize = fileStream.Length;
                            await UploadDocumentAsync(document, fileStream);
                            await UpdateFileSizeAsync(document, fileSize);
                        }
                        catch (Exception exc)
                        {
                            await LogErrorAsync(exc);

                            await _folderManager.RemoveDocumentAsync(document, destination);

                            // Cascade so the document is deleted
                            throw;
                        }
                    }
                    catch (Exception exc)
                    {
                        // Log error
                        await LogErrorAsync(exc);

                        // Delete the FileDocument
                        await _documentService.DeleteAsync(document);
                    }
                }
            }
        }

        private Task LogErrorAsync(Exception exc)
        {
            return Task.FromResult(typeof(void));
        }

        private async Task UploadDocumentAsync(FileDocument document, Stream file)
        {
            var url = $"{_connectDocumentOptions.DriveAuthority}/upload";
            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                ByteArrayContent fileContent;
                using (var br = new BinaryReader(file, System.Text.Encoding.UTF8, true))
                {
                    fileContent = new ByteArrayContent(br.ReadBytes((int)file.Length));
                }

                content.Add(fileContent, "file", document.FileName);
                content.Add(new StringContent(document.DocumentId), "documentId");

                await client.PostAsync(url, content);
            }
        }

        private async Task UpdateFileSizeAsync(FileDocument document, long size)
        {
            document.ContentLength = size;
            await _documentService.UpdateAsync(document);
        }

        private static string GetSlideTitle(IDocument document)
        {
            var result = document.Title;

            switch (result.ToLowerInvariant())
            {
                case "ants.png":
                    result = "Ants Marching";
                    break;
                case "beach.png":
                    result = "Sleepy Beach";
                    break;
                case "cliffs.png":
                    result = "Misty Cliffs";
                    break;
                default:
                    break;
            }

            return result;
        }

        private Slide CreateSlide( FileDocument document, IEffect<Slide> effect, string linkUrl)
        {
            var result = new Slide()
            {
                ImageUrl = CreateImageUrl(document),
                ThumbnailUrl = CreateThumbnailUrl(document),
                DocumentId = KeyGen.NewGuid(),
                Duration = 4000,
                BackgroundFit = Fit.Cover,
                Direction = Direction.LeftToRight,
                Title = GetSlideTitle(document),
                SlideLinkUrl = linkUrl,
                LinkTarget = LinkTarget.NewWindow,
                IsLinkEnabled = !string.IsNullOrEmpty(linkUrl)
            };

            // Add effect (if any) to the first slide
            if (effect != null)
            {
                if (effect is Parallax)
                {
                    throw new NotImplementedException();
                }
                else if (effect is KenBurnsEffect)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new NotSupportedException($"Unable to persist effects of type '{effect.GetType().Name}'.");
                }
            }

            return result;
        }

        private string CreateImageUrl(IDocument document)
        {
            var url = _connectDocumentOptions.DriveAuthority;
            return $"{url}/download?id={document.DocumentId}";
        }

        private string CreateThumbnailUrl(IDocument document)
        {
            var url = _connectDocumentOptions.DriveAuthority;
            const int height = 60; const int width = 100;
            return $"{url}/download/image?id={document.DocumentId}&x={width}&y={height}";
        }

        private Parallax CreateParallaxEffect()
        {
            return new Parallax()
            {
            };
        }

        private KenBurnsEffect CreateKenBurnsEffect()
        {
            return new KenBurnsEffect()
            {
            };
        }
    }
}
