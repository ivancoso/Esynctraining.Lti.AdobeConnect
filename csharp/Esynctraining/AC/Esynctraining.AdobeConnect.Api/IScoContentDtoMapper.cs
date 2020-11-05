using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Api
{
    public interface IScoContentDtoMapper<TDto>
    {
        TDto Map(ScoContent sco);

    }

}
