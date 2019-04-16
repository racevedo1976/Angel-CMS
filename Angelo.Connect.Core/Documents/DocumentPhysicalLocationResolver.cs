using System;

namespace Angelo.Connect.Documents
{
    public class DocumentPhysicalLocationResolver
    {
        private string _clientId;
        private string _libraryType;
        private string _siteId;
        private string _userId;

        public DocumentPhysicalLocationResolver(string libraryType, string clientId, string siteId, string userId)
        {
            _libraryType = libraryType;
            _clientId = clientId;
            _siteId = siteId;
            _userId = userId;
        }

        public string Resolve()
        {
            switch (_libraryType)
            {
                case "User":
                    return $"clients/{_clientId}/users/{_userId}/";
                case "Site":
                    return $"clients/{_clientId}/sites/{_siteId}/";
                case "Client":
                    return $"clients/{_clientId}/";
                default:
                    throw new NotSupportedException("Library type is unknown.");
            }
        }
    }
}
