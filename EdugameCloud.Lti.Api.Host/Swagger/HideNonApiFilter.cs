using System.Linq;
using EdugameCloud.Lti.Api.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EdugameCloud.Lti.Api.Host.Swagger
{
    public sealed class HideNonApiFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var itm in context.ApiDescriptionsGroups.Items)
            {
                foreach (var i in itm.Items)
                {
                    var lmsAttr = i.ActionAttributes().SingleOrDefault(x => x is IApiEnableAttribute) as IApiEnableAttribute;
                    if (lmsAttr == null || !lmsAttr.ApiCallEnabled)
                    {
                        var route = "/" + i.RelativePath.TrimEnd('/');
                        swaggerDoc.Paths.Remove(route);
                    }
                }
            }
        }

    }

}
