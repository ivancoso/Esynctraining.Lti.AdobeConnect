namespace Esynctraining.Core.Json
{
    public interface IJsonSerializer
    {
        string JsonSerialize<T>(T obj);

    }

}
