namespace Esynctraining.Core.Caching
{
    public interface ICache
    {
        object Get(string key);

        void Add(string key, object value);

        void Add(string key, object value, string dependencyPath);
        
        //void Remove(string key);
    }

}
