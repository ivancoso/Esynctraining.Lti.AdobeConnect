using System.Threading.Tasks;

namespace Esynctraining.Mail
{
    public interface ITemplateTransformer
    {
        Task<string> TransformAsync(string templateName, object model);

    }

}
