using Angelo.Connect.NavMenu.Models;
using Angelo.Connect.NavMenu.ViewModels;

namespace Angelo.Connect.NavMenu.Data
{
    public static class ModelMapper
    {
        public static NavMenuWidgetViewModel ProjectToViewModel(this NavMenuWidget model)
        {
            var viewModel = new NavMenuWidgetViewModel()
            {
                Id = model.Id,
                Title = model.Title,
                NavMenuId = model.NavMenuId
            };
            return viewModel;
        }

        public static NavMenuWidget ProjectToModel(this NavMenuWidgetViewModel viewModel)
        {
            var model = new NavMenuWidget()
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                NavMenuId = viewModel.NavMenuId
            };
            return model;
        }
    }
}
