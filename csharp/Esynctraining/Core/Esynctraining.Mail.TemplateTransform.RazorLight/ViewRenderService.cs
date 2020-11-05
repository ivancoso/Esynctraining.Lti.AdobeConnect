using System;
using System.Threading.Tasks;
using Esynctraining.Mail.Configuration;
using RazorLight;

namespace Esynctraining.Mail.TemplateTransform.RazorLight
{
    public class ViewRenderService : ITemplateTransformer
    {
        private readonly IRazorLightEngine _engine;


        public ViewRenderService(ITemplateSettings templateSettings)
        {
            if (templateSettings == null)
                throw new ArgumentNullException(nameof(templateSettings));

            // TODO:
            _engine = EngineFactory.CreatePhysical(templateSettings.TemplatesFolderPath);
        }


        public Task<string> TransformAsync(string viewName, object model)
        {
            return Task.FromResult(_engine.Parse(viewName + ".cshtml", model));
        }

    }

}
