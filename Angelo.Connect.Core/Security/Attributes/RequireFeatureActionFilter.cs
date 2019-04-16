using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Abstractions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class RequireFeatureAttribute : TypeFilterAttribute
    {
         public RequireFeatureAttribute(string id) : base(typeof(RequireFeatureActionFilter))
        {
            Arguments = new object[] { id };
        }

        private class RequireFeatureActionFilter : IAsyncActionFilter
        {
            private readonly string _featureId;
            private readonly ProductContext _product;

            public RequireFeatureActionFilter(IContextAccessor<ClientAdminContext> clientAccessor, string id)
            {
                Ensure.NotNull(clientAccessor, "Null ClientAdminContextAccessor passed into constructor of " + GetType().Name);

                _product = clientAccessor.GetContext()?.Product;
                Ensure.NotNull(_product, "Null ProductContext derived from ClientAdminContext passed into constructor of " + GetType().Name);

                _featureId = id;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (_product.Features.HasFeature(_featureId))
                {
                    await next();
                }
                else
                {
                    // TO DO: Add logging of errors here
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }
        }
    }
}
