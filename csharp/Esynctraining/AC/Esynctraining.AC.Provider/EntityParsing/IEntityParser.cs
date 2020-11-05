using System.Xml;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public interface IEntityParser<T>
    {
        T Parse(XmlNode xml);

    }

    internal sealed class ParserSingleton<T> where T : class, new()
    {
        ParserSingleton() { }

        private static readonly T _instance = new T();

        public static T Instance { get { return _instance; } }

    }

}
