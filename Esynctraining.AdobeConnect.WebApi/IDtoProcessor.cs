namespace Esynctraining.AdobeConnect.WebApi
{
    public interface IDtoProcessor<TDto>
    {
        TDto Process(TDto dto);

    }

}
