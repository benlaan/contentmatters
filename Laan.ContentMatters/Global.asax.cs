using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using MvcContrib.ViewEngines;

using Laan.ContentMatters.Models;
using Laan.ContentMatters.Controllers;
using Laan.ContentMatters.Engine;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.AutoMap;
using System.Reflection;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using NHibernate.Mapping;
using FluentNHibernate.Conventions.Helpers;
using Helpers = FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions;
using Castle.Windsor;
using FluentNHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using System.Configuration;
using log4net.Config;
using Laan.Persistence.Interfaces;

namespace Laan.ContentMatters
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication, IContainerAccessor
    {
        public static IWindsorContainer Container { get; private set; }

        IWindsorContainer IContainerAccessor.Container
        {
            get { return Container; }
        }

        public static void RegisterRoutes( RouteCollection routes )
        {
            routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

            routes.MapRoute(
                "ViewItem",                                             // Route name
                "{controller}/{id}/{name}",                             // URL with parameters
                new { controller = "Home", action = "View", id = "" }   // Parameter defaults
            );

            routes.MapRoute(
                "NamedDefault",                                         // Route name
                "{controller}/{action}/{id}/{name}",                    // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );
        }

        private void InitialiseWindsor()
        {
            if ( Container != null )
                return;

            IoC.ConfigPath = Server.MapPath( ConfigurationManager.AppSettings[ "castle.config" ] );
            Container = IoC.Container;
        }

        private User SaveUser( string name )
        {
            var repository = IoC.Container.Resolve<IRepository<User>>();
            User user = new User() { Name = name };
            repository.SaveOrUpdate( user );
            return user;
        }

        private void BuildTestData()
        {
            var repository = IoC.Container.Resolve<IRepository<Blog>>();
            var author = SaveUser( "Ben Laan" );
            var commenter1 = SaveUser( "Jo Blogs" );
            var commenter2 = SaveUser( "Mary Jane Smith" );

            Blog blog = new Blog()
            {
                Title = "Lounging with Laany",
                Description = "All things Laan - philosophy, politics, history, economics, etc..",
                Author = author
            };

            Post post = new Post()
            {
                Title = "Was Howard good for Australia",
                Description = "A look at the history of Howardism in Australia, for good or bad",
                Body = "John Howard was a fighter. He was never glamourous, but he was well regarded as a great politician.",
                Author = author
            };
            blog.Posts.Add( post );

            post.Comments.Add( new Comment() { Body = "Nice work.. I like the art work!", Author = commenter1 } );
            post.Comments.Add( new Comment() { Body = "Could be longer.. sometimes these thoughts are hard to express succinctly", Author = commenter2 } );

            repository.SaveOrUpdate( blog );
        }

        protected void Application_Start()
        {
            XmlConfigurator.Configure();
            InitialiseWindsor();

            BuildTestData();            
            ViewEngines.Engines.Add( new TemplatingViewEngine() );

            ControllerBuilder.Current.SetControllerFactory( new CustomControllerFactory() );
            RegisterRoutes( RouteTable.Routes );
        }
    }
}