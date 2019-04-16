using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class FolderFactory
    {
        public IEnumerable<TDocument> GetDocuments<TFolder, TDocument>(string path)
        {
            throw new NotImplementedException();
        }


        // To be used by the Startup/extension method

        public void SetFolderManager<TFolder, TDocument>(IFolderManager folderManager)
        {
            throw new NotImplementedException();
        }

        public IFolderManager GetFolderManager<TFolder, TDocument>()
        {
            throw new NotImplementedException();
        }
    }
}
