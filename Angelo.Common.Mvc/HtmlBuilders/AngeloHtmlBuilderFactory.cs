using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc
{

    public class AngeloHtmlBuilderFactory
    {
        public class EmptyClass { }

        private IHtmlHelper _htmlHelper;

        public AngeloHtmlBuilderFactory(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public AngeloCheckboxListBuilder<EmptyClass> CheckboxList()
        {
            return new AngeloCheckboxListBuilder<EmptyClass>(_htmlHelper, new EmptyClass());
        }

        public AngeloCheckboxListBuilder<TModel> CheckboxList<TModel>(TModel model) where TModel : class
        {
            return new AngeloCheckboxListBuilder<TModel>(_htmlHelper, model);
        }

        public AngeloNavMenuBuilder<EmptyClass> NavMenu()
        {
            return new AngeloNavMenuBuilder<EmptyClass>(_htmlHelper, new EmptyClass());
        }

        public AngeloNavMenuBuilder<TModel> NavMenu<TModel>(TModel model) where TModel : class
        {
            return new AngeloNavMenuBuilder<TModel>(_htmlHelper, model);
        }



    }
}
