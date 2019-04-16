using Angelo.Drive.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Angelo.Drive.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]")]
    public class RemoveController : ControllerBase
    {
        private LibraryManager _libraryManager;

        public RemoveController(LibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        // NOTE: IFormFile is the .Net Core WebAPI-level file format, as foud in Request.Form.Files
        [HttpPost]
        public async Task Remove(string documentId, string ownerId)
        {
            await _libraryManager.DeleteFileDocument(documentId, ownerId);

        }

       
    }
}

