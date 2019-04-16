using System.Collections.Generic;

using Angelo.Common.Models;
using Angelo.Connect.Models;
using Angelo.Common.Extensions;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientsAdminViewModel
    {
        public PagedResult<ClientViewModel> ClientPages { get; set; }
        public string SelectedClientId { get; set; }
        public ClientViewModel SelectedClient
        {
            get
            {
                return GetSelectedClient();
            }
        }

        public ClientsAdminViewModel()
        {
            ClientPages = new PagedResult<ClientViewModel>();
            SelectedClientId = string.Empty;
        }

        protected ClientViewModel GetSelectedClient()
        {
            foreach (var item in ClientPages.Data)
                if (!string.IsNullOrEmpty(item.Id) && item.Id.Equals(SelectedClientId, System.StringComparison.OrdinalIgnoreCase) == true)
                    return item;
            return null;
        }
    }
}
