using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.WebApi
{
    public interface IScoContentDtoMapper<TDto>
    {
        TDto Map(ScoContent sco);

    }

}
