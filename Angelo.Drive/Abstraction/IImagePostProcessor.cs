using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Drive.Abstraction
{
    public interface IImagePostProcessor
    {
        Task Invoke(string librarypath, FileDocument document);
        Task Delete(string librarypath, string documentId);

        Stream GetFileStream(string librarypath, string documentId);
    }
}
