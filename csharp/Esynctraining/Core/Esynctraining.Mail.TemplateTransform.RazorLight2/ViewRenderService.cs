using System;
using System.Threading.Tasks;
using Esynctraining.Mail.Configuration;
using RazorLight;

namespace Esynctraining.Mail.TemplateTransform.RazorLight2
{
    public class ViewRenderService : ITemplateTransformer
    {
        private readonly IRazorLightEngine _engine;


        public ViewRenderService(ITemplateSettings templateSettings)
        {
            if (templateSettings == null)
                throw new ArgumentNullException(nameof(templateSettings));

            _engine = new EngineFactory().ForFileSystem(templateSettings.TemplatesFolderPath);
        }


        public Task<string> TransformAsync(string viewName, object model)
        {
            return _engine.CompileRenderAsync(viewName + ".cshtml", model);
        }

    }
}
