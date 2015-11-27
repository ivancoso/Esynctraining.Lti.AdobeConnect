namespace Esynctraining.Core.Utils
{
    using System;
    
    public static class IoC
    {
        private const string ContainerNotInitializedMessage =
            "The container has not been initialized! Please call IoC.Initialize(container) before using it.";


        private static IServiceLocator _container;


        private static IServiceLocator GlobalContainer
        {
            get
            {
                if (_container == null)
                {
                    throw new InvalidOperationException(ContainerNotInitializedMessage);
                }
                return _container;
            }
            set
            {
                _container = value;
            }
        }
        

        public static void Initialize(IServiceLocator container)
        {
            GlobalContainer = container;
        }


        public static T Resolve<T>()
        {
            return GlobalContainer.GetInstance<T>();
        }

        public static T Resolve<T>(string key)
        {
            return GlobalContainer.GetInstance<T>(key);
        }
        
        public static object Resolve(Type type)
        {
            return GlobalContainer.GetInstance(type);
        }

        public static object Resolve(Type type, string key)
        {
            return GlobalContainer.GetInstance(type, key);
        }

    }

}