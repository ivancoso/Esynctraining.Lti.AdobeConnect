namespace Esynctraining.AspNetCore
{
    public interface IJsonDeserializer
    {
        T JsonDeserialize<T>(string json);

    }

}
