namespace EdugameCloud.Core
{
    public interface IJsonSerializer
    {
        string JsonSerialize<T>(T obj);

        T JsonDeserialize<T>(string json);

    }

}
