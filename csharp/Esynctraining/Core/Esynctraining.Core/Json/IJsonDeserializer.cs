namespace Esynctraining.Core.Json
{
    public interface IJsonDeserializer
    {
        T JsonDeserialize<T>(string json);

    }

}
