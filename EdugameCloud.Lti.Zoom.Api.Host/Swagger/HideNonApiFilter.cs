using System.Linq;
//using EdugameCloud.Lti.Api.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using IApiEnableAttribute = EdugameCloud.Lti.Zoom.Api.Host.FIlters.IApiEnableAttribute;

namespace EdugameCloud.Lti.Zoom.Api.Host.Swagger
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

// TODO: move
    //public class ApplyCustomSchemaFilters : ISchemaFilter
    //{
    //    public void Apply(Schema model, SchemaFilterContext context)
    //    {
    //        if (context.SystemType == typeof(DTO.MeetingDTO))
    //        {
    //            var excludeProperties = new[] { "accessLevel", "canJoin", "isEditable", "officeHours", "isDisabledForThisCourse",
    //                "telephonyProfileFields", "sessions"
    //            };

    //            foreach (var prop in excludeProperties)
    //                if (model.Properties.ContainsKey(prop))
    //                    model.Properties.Remove(prop);
    //        }
    //    }

    //}


}
