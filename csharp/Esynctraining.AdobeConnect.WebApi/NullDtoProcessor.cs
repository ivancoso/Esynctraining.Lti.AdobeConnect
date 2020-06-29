namespace Esynctraining.AdobeConnect.WebApi
{
    public sealed class NullDtoProcessor<TDto> : IDtoProcessor<TDto>
    {
        public TDto Process(TDto dto)
        {
            return dto;
        }

    }

}
