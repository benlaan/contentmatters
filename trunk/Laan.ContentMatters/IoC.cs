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
            set { _configPath = value; }
        }

        public static IWindsorContainer Container
        {
            get
            {
                if ( _container == null )
                    _container = new WindsorContainer( ConfigPath );

                return _container;
            }
        }
    }
}
