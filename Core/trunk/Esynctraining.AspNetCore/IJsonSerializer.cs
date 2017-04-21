namespace Esynctraining.AspNetCore
{
    public interface IJsonSerializer
    {
        string JsonSerialize<T>(T obj);

    }

}
