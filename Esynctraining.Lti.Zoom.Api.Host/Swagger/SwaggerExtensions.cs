using System;
using Esynctraining.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Esynctraining.Lti.Zoom.Api.Host.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddLtiSwagger(this IServiceCollection services, 
            IHostingEnvironment env)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "eSyncTraining LTI Zoom API", Version = "v1" });
                //c.DescribeAllParametersInCamelCase();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();

                c.MapType<DateTime?>(() =>
                    new Schema
                    {
                        Type = "integer",
                        Format = "int64",
                        Example = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        Description = "MillisecondsSinceUnixEpoch",
                        //Default = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        //Pattern = "pattern",
                    }
                );

                c.MapType<DateTime>(() =>
                    new Schema
                    {
                        Type = "integer",
                        Format = "int64",
                        Example = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        Description = "MillisecondsSinceUnixEpoch",
                        //Default = (long)DateTime.Now.ConvertToUnixTimestamp(),
                        //Pattern = "pattern",
                    }
                );

                //c.IncludeXmlComments(string.Format(@"{0}\EdugameCloud.Lti.Api.xml", env.WebRootPath));
                //c.IncludeXmlComments(string.Format(@"{0}\EdugameCloud.Lti.Core.xml", env.WebRootPath));
                //c.IncludeXmlComments(string.Format(@"{0}\Esynctraining.AdobeConnect.Api.xml", env.WebRootPath));
                //c.IncludeXmlComments(string.Format(@"{0}\EdugameCloud.Lti.xml", env.WebRootPath));

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization header using the Bearer scheme. " +
                    "<br/> Example: \"Authorization: ltiapi {consumerKey};{courseId}\""
                    + "<br/> {courseId} - LMS course Id."
                    + "For Sakai {courseId} - is GetHashCode() result for course context_id value.",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

            });

            return services;
        }

    }

}
