namespace Esynctraining.AdobeConnect.Api
{
    public interface IDtoProcessor<TDto>
    {
        TDto Process(TDto dto);

    }

}
