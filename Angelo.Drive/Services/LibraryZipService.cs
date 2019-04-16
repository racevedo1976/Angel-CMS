using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Drive.Services
{
    // NOTE: Passing around KVPairs rather than Documents so that the ZipService doesn't need to take a dependency on the IOService.
    // I want to keep the functionality here as basic as possible, letting the lookup happen in the owning class.
    // Nomenclature:   Folder = virtual, Directory = physical.  Document = virtual, File = physical
    public class LibraryZipService
    {
        #region Nested classes
        private class TempDirectory : IDisposable
        {
            public string Path { get; set; }
            private string Id { get; } = Guid.NewGuid().ToString();
            public bool IsDisposeIgnored { get; set; }
            public void Dispose()
            {
                if (this.IsDisposeIgnored) return;

                Destroy(this.Id);
            }

            public TempDirectory()
            {
                Create(this.Id);
            }

            private static void Create(string name)
            {
                var appTemp = System.IO.Path.GetTempPath();
                var folder = System.IO.Path.Combine(appTemp, name);
                var result = Directory.CreateDirectory(folder);
            }

            private static void Destroy(string name)
            {
                var appTemp = System.IO.Path.GetTempPath();
                var folder = System.IO.Path.Combine(appTemp, name);

                Directory.Delete(folder, true);
            }
        }
        #endregion // Nested classes
        #region Public methods
        // NOTE: Callers must dispose of the result! (as opposed to passing the streams in)
        // NOTE2: Keeping async, as most stream libraries support it. SharpZipLib does not
        public async Task<Stream> ZipAsync(IEnumerable<KeyValuePair<string, Stream>> documents)
        {
            Ensure.NotNull(documents, $"{nameof(documents)} cannot be null.");
            Ensure.That<ArgumentException>(documents.Any(), $"{nameof(documents)} cannot be empty.");

            var result = new MemoryStream();
             
            using (var tempDir = CreateTemporaryDirectory(false))
            {
                // NOTE: Two options here. One is to use a ZipOutputStream and manually build the zip file. Two is to create a temp file
                // to serve as outputStream and copy it to the non-disposed result stream so that the result doesn't get disposed by FastZip.
                // The former is more efficient but more code-intensive while the latter has more IO and but is easier to implement.
                //ZipFast();
                //ZipEasy();
                await WriteTempFilesAsync(documents, tempDir.Path);
                await ZipEasyAsync(result, tempDir);
            }

            return result;
        }

        // Removing the returnsStream signature because I can't find a way to make it work with file-backings, as I can't clean
        // up the files until I don't need the streams any more, but they're in different scopes, so it won't work (the streams will
        // point to missing files). Returning to a system where the caller provides the context and deals with both creation *and*
        // cleanup.
        //public async Task<IEnumerable<Stream>> UnzipAsync(Stream input)
        public string Unzip(KeyValuePair<string, Stream> document)
        {
            Ensure.NotNullOrEmpty(document.Key, $"{nameof(document.Key)} cannot be null or empty.");
            Ensure.NotNull(document.Value, $"{nameof(document.Value)} cannot be null.");

            var result = Guid.NewGuid().ToString();

            using (var folder = CreateTemporaryDirectory(false))
            {
                new FastZip().ExtractZip(document.Value, folder.Path, FastZip.Overwrite.Always, null, null, null, true, false);
            }

            return result;
        }
        #endregion // Public methods
        #region Private methods
        /// <summary>
        /// Converts all streams to be zipped to fileStreams in a single, dedicated, flat, *hierarchical* folder,
        /// thus converting the flat physical folder structure into a real, temporary, hierarchical, physical structure
        /// that the zip library can receive. 
        /// </summary>
        /// <remarks>
        /// It allows a simple way to include folder path information in the zip file. An alternative method would
        /// be to put them all into the zip flat and then manually set the zip descriptor for the path, but it's
        /// way, way easier to just let FastZip figure it out by setting up the temp folder with  a hierarchical
        /// structure.
        /// </remarks>
        /// <param name="documents">The input stream and a physical path, wrapped in a Document.</param>
        /// <param name="targetFolder">The destination root folder.</param>
        /// <returns></returns>
        private async Task WriteTempFilesAsync(IEnumerable<KeyValuePair<string, Stream>> documents, string targetFolder)
        {
            Ensure.NotNull(documents, $"{nameof(documents)} cannot be null.");
            Ensure.NotNullOrEmpty(targetFolder, $"{nameof(targetFolder)} cannot be null or empty.");
            Ensure.That(Directory.Exists(targetFolder), $"{nameof(targetFolder)} does not exist.");

            foreach (var document in documents)
            {
                if (document.Value == null) throw new InvalidOperationException($"Missing physical file for '{document.Key}'");

                using (var tempFileStream = CreateTempFile(document, targetFolder))
                {
                    await document.Value.CopyToAsync(tempFileStream);
                }
            }
        }

        private static TempDirectory CreateTemporaryDirectory(bool isDisposeIgnored)
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(path);

            return new TempDirectory() { Path = path, IsDisposeIgnored = isDisposeIgnored };
        }

        private static Stream CreateTempFile(KeyValuePair<string, Stream> document, string rootDirectory)
        {
            var path = Path.Combine(rootDirectory, document.Key.TrimStart('/'));

            var info = new FileInfo(path);

            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }

            return File.Create(path);
        }

        private static async Task ZipEasyAsync(Stream stream, TempDirectory tempDir)
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                using (var outputStream = File.Open(tempFile, FileMode.Append))
                {
                    new FastZip().CreateZip(outputStream, tempDir.Path, true, null, null);
                }
                using (var inputStream = File.Open(tempFile, FileMode.Open))
                {
                    await inputStream.CopyToAsync(stream);
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private static async Task ZipFastAsync(Stream stream, TempDirectory tempDir)
        {
            throw new NotImplementedException();
        }
        #endregion // Private methods
    }
}
