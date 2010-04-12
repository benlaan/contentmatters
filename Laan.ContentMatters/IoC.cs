using System;

using Castle.Windsor;

namespace Laan.ContentMatters
{
    public class IoC
    {
        private static IWindsorContainer _container;
        private static string _configPath = "castle.config";

        public static string ConfigPath
        {
            get { return _configPath; }
            set
            {
                if ( _configPath == value )
                    return;
                _configPath = value;
                Rebuild();
            }
        }

        public static IWindsorContainer Load()
        {
            _container = new WindsorContainer( ConfigPath );
            return _container;
        }

        public static IWindsorContainer Rebuild()
        {
            Container.Dispose();
            Load();
            return _container;
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static IWindsorContainer Container
        {
            get
            {
                if ( _container == null )
                    Load();

                return _container;
            }
        }
    }
}
