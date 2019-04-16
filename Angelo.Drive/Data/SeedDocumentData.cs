using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Abstractions;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Drive.Data
{
    // Keith: Moved this over to Drive, as all FileDocuments have an associated physical file, and I cannot seed the files the documents independently
    // and in parallel, as I have no way to look up the DocIDs, as they aren't created when the Drive seeding runs, as it's going at the same time as 
    // Web's seeding
    public class SeedDocumentData //: IStartupAction
    {
        #region Dependencies
        private string _systemRoot;
        private ConnectDbContext _connectDb;
        private IFolderManager<FileDocument> _folderManager;
        private IDocumentService<FileDocument> _documentService;
        // Can't use these, as their seeding is done in parallel (Web) with Drive
        //private IFolderManager<FileDocument> _folderManager;
        //private IDocumentService<FileDocument> _documentService;
        private IDocumentUploadService<FileDocument> _uploadService;
        #endregion // Dependencies
        #region Constructors
        public SeedDocumentData(ConnectDbContext connectDb, string systemRoot, IFolderManager<FileDocument> folderManager, IDocumentService<FileDocument> documentService, IDocumentUploadService<FileDocument> uploadService)
        {
            _systemRoot = systemRoot;
            _connectDb = connectDb;
            _folderManager = folderManager;
            _documentService = documentService;
            _uploadService = uploadService;
        }
        #endregion // Constructors
        #region IStartupMethod implementation
        public async void Invoke()
        {
            //var dbCreationInProgress = true;
            //var library = new DocumentLibrary();

            //do
            //{
            //    try
            //    {
            //        using (var command = _connectDb.Database.GetDbConnection().CreateCommand())
            //        {
            //            command.CommandText = "SELECT COUNT(*) TotalTables FROM sys.tables";
            //            _connectDb.Database.OpenConnection();
            //            using (var result = command.ExecuteReader())
            //            {
            //                while (result.Read())
            //                {
            //                    if ((int)result.GetValue(0) >= 86)
            //                    {
            //                        if (_connectDb.DocumentLibraries.Any() && _connectDb.FileDocuments.Any())
            //                        {
            //                            library = _connectDb.DocumentLibraries.FirstOrDefault(x => x.OwnerId == "xxxxDriveFlagForReSeedingFilesxxxx");
            //                            dbCreationInProgress = false;
            //                        }
            //                    }
            //                }
                         
            //            }
            //        }

            //    }
            //    catch (Exception)
            //    {
            //        dbCreationInProgress = true;
            //    }

            //    await Task.Delay(5000);
               
            //} while (dbCreationInProgress);
            

            ////One last check for the seeding flag, if present, it means that connect was re-seeeded, so drives needs
            ////to sync with documentids and physical files. None of the current physical files are valid anymore as everything
            ////its starting from scratch again.
            //if (library != null)
            //{
            //    //after seeding, then remove the connect flag to avoid re-seeding everytime restarting Drive
            //    _connectDb.DocumentLibraries.Remove(library);
            //    _connectDb.SaveChanges();

            //    SeedAllDocumentFiles();

                
            //}
                


        }
        #endregion // IStartupMethod implementation
        
     
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        private async void SeedAllDocumentFiles()
        {
            //clear root folder since we are starting from scratch, whatever files are currently there are not valid anymore
            var rootDocumentFolder = Path.Combine(_systemRoot, "clients");
            if (Directory.Exists(rootDocumentFolder))
            {
                DeleteDirectory(rootDocumentFolder);
            }


            foreach (var doc in _connectDb.FileDocuments)
            {
                //attempt to get the library location
                var libraryLocation = string.Empty;
                libraryLocation = await _documentService.GetDocumentLibraryLocation(doc.DocumentId);

                EnsurePhysicalFile(doc, libraryLocation);
            }
        }

        private async void EnsurePhysicalFile(FileDocument doc, string libraryLocation)
        {
            var stream = await _uploadService.GetFileStreamAsync(doc, libraryLocation);
            if (stream == null)
            {
                using (stream = CreateTemporarySeedData(doc))
                {
                    if (stream.Length > 0)
                        await _uploadService.SetFileStreamAsync(doc, stream, libraryLocation);
                }
            }
            else
            {
                stream.Dispose();
            }
        }

        #region Data generation
        private Stream CreateTemporarySeedData(FileDocument doc)
        {
            var result = new MemoryStream();

            if (LoadResourceStream("Angelo.Drive.Assets.images", doc.FileName, result))
                return result;

            if (LoadResourceStream("Angelo.Drive.Assets.videos", doc.FileName, result))
                return result;

            //CreateSeedFile(doc, result);

            return result;
        }

        private bool LoadResourceStream(string resourcePath, string filename, Stream file)
        {
            var result = false;
            var assembly = Assembly.GetEntryAssembly();
            var resourceName = $"{resourcePath}.{filename}";
            using (Stream resource = assembly.GetManifestResourceStream(resourceName))
            {
                if (resource != null)
                {
                    resource.CopyTo(file);
                    result = true;
                }
            }
            return result;
        }

        private void CreateSeedFile(FileDocument doc, Stream output)
        {
            var fileExtension = doc.FileExtension;
            switch ((fileExtension ?? string.Empty).ToLowerInvariant())
            {
                case ".jpg":
                    WriteBinary(output, CreateSeedJpeg(doc));
                    break;
                case ".png":
                    WriteBinary(output, CreateSeedPng(doc));
                    break;
                case ".mp3":
                case ".avi":
                    WriteBinary(output, CreateSeedAvi(doc));
                    break;
                case ".ppt":
                case ".book":
                case ".exe":
                case ".pdf":
                case ".m4p":
                case ".mov":
                    WriteBinary(output, CreateSeedQuicktime(doc));
                    break;
                //case ".zip":
                // Don't want to seed these
                case ".dll":
                    WriteBinary(output, CreateSeedBinary(doc));
                    break;
                case ".txt":
                    WriteString(output, CreateSeedText(doc));
                    break;
                case ".sln":
                    WriteString(output, CreateSeedSolution(doc));
                    break;
                case ".csv":
                    WriteString(output, CreateSeedCsv(doc));
                    break;
                case ".cs":
                    WriteString(output, CreateSeedCSharp(doc));
                    break;
                case ".md":
                    WriteString(output, CreateSeedMarkdown(doc));
                    break;
                case ".xml":
                    WriteString(output, CreateSeedXml(doc));
                    break;
                case ".config":
                    WriteString(output, CreateSeedConfig(doc));
                    break;
                case ".zip":
                    break;
                default:
                    WriteString(output, CreateSeedXml(doc));
                    //throw new NotSupportedException($"Unknown file type: '{fileExtension}'.");
                    break;
            }
        }
            
        private static void WriteBinary(Stream output, byte[] data)
        {
            output.Write(data, 0, data.Length);
        }

        private static void WriteString(Stream output, string data)
        {
            using (var writer = new StreamWriter(output, System.Text.Encoding.ASCII, 1024, true))
            {
                writer.Write(data);
            }
        }

        private static byte[] CreateSeedBinary(FileDocument doc)
        {
            var length = new Random().Next(20, 100);
            var data = new byte[length];
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                data[i] = (byte) random.Next();
            }

            return data;
        }

        private byte[] CreateSeedJpeg(FileDocument doc)
        {
            var path = Path.Combine(_systemRoot, "..", "..", "Assets", "images", "cat-video-capture.jpg");
            return File.ReadAllBytes(path);
        }

        private byte[] CreateSeedPng(FileDocument doc)
        {
            var path = Path.Combine(_systemRoot, "..", "..", "Assets", "images", "slideshow1.png");
            return File.ReadAllBytes(path);
        }

        private byte[] CreateSeedQuicktime(FileDocument doc)
        {
            var path = Path.Combine(_systemRoot, "..", "..", "Assets", "videos", "chris1.MOV");
            return File.ReadAllBytes(path);
        }

        private byte[] CreateSeedAvi(FileDocument doc)
        {
            var path = Path.Combine(_systemRoot, "..", "..", "Assets", "videos", "computer1.AVI");
            return File.ReadAllBytes(path);
        }

        private static string CreateSeedText(FileDocument doc)
        {
            var data = $"Sample text for file {doc.Title}";
            return data;
        }

        private static string CreateSeedSolution(FileDocument doc)
        {
            var data = "Microsoft Visual Studio Solution File, Format Version 12.00\r\n# Visual Studio 14\r\nVisualStudioVersion = 14.0.25420.1\r\nMinimumVisualStudioVersion = 14.0\r\nGlobal\r\nEndGlobal\r\n";
            return data;
        }

        private static string CreateSeedCsv(FileDocument doc)
        {
            var data = "Column1,Column2,Column2\r\nOne,Two,Three\r\nRed,White,Blue\r\nUno,Dos,Tres";
            return data;
        }

        private static string CreateSeedCSharp(FileDocument doc)
        {
            var data = "using System;\r\nnamespace Global\r\n{\r\n    public class Foo {}\r\n}\r\n";
            return data;
        }
        
        private static string CreateSeedMarkdown(FileDocument doc)
        {
            var data = "# Heading\r\n\r\nTest paragraph 1\r\n\r\nTest paragraph 2\r\n\r\n";
            return data;
        }

        private static string CreateSeedConfig(FileDocument doc)
        {
            var data = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<configuration>\r\n  <system.webServer>\r\n    <handlers>\r\n      <add name=\"aspNetCore\" path=\"*\" verb=\"*\" modules=\"AspNetCoreModule\" resourceType=\"Unspecified\"/>\r\n    </handlers>\r\n    <aspNetCore processPath=\"%LAUNCHER_PATH%\" arguments=\"%LAUNCHER_ARGS%\" stdoutLogEnabled=\"false\" stdoutLogFile=\".\\logs\\stdout\" forwardWindowsAuthToken=\"false\"/>\r\n  </system.webServer>\r\n</configuration>";
            return data;
        }

        private static string CreateSeedXml(FileDocument doc)
        {
            var data = $"<root><element1/><element2 attr1=\"test\"></element2></root>";
            return data;
        }
        #endregion // Data generation
    }
}
