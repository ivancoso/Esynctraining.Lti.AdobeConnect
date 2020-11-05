using System;
using System.IO;
using System.Threading.Tasks;
using Esynctraining.Core.Logging;
using Esynctraining.Mail.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Esynctraining.Mail.TemplateTransform.AspNetCore
{
    public class ViewRenderService : ITemplateTransformer
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITemplateSettings _templateSettings;
        private readonly ILogger _logger;


        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            ITemplateSettings templateSettings,
            ILogger logger)
        {
            _razorViewEngine = razorViewEngine ?? throw new ArgumentNullException(nameof(razorViewEngine));
            _tempDataProvider = tempDataProvider ?? throw new ArgumentNullException(nameof(tempDataProvider));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _templateSettings = templateSettings ?? throw new ArgumentNullException(nameof(templateSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<string> TransformAsync(string templateName, object model)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Non-empty value expected", nameof(templateName));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var viewLocation = BuildViewLocation(templateName);
            _logger.Debug($"RenderToStringAsync before finding view {templateName}");
            var viewResult = _razorViewEngine.FindView(actionContext, viewLocation, false);
            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"{viewLocation} does not match any available view");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };
            
            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                _logger.Debug($"RenderToStringAsync before rendering view with context");
                try
                {
                    await viewResult.View.RenderAsync(viewContext);
                }
                catch (Exception e)
                {
                   _logger.Error("Error rendering razor view", e);
                }
                
                return sw.ToString();
            }
        }


        private string BuildViewLocation(string viewName)
        {
            return _templateSettings.TemplatesFolderPath + viewName;
        }

    }

}