using System;

using Laan.ContentMatters.Models;
using Laan.ContentMatters.Models.Custom;
using Laan.Persistence.Interfaces;

namespace Laan.ContentMatters
{
    public class Build
    {
        private static User SaveUser( string name )
        {
            var repository = IoC.Container.Resolve<IRepository<User>>();
            User user = new User() { Name = name };
            repository.SaveOrUpdate( user );
            return user;
        }

        public static void TestData()
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
                Body = "John Howard was a fighter. He was never glamourous, but he was well regarded as a good politician, and a great Australian." +
                "<br/> Moreover, Howard was a man of the people, in the sense that he knew exactly what the silent majority were thinking, and he " +
                "exploited their fears, and utilized their zeal in what some would come to call nationalism, bordering on jingoism.",
                Author = author
            };
            blog.Posts.Add( post );

            post.Comments.Add( new Comment() { Body = "Nice work.. I like the art work!", Author = commenter1 } );
            post.Comments.Add( new Comment() { Body = "Could be longer.. sometimes these thoughts are hard to express succinctly", Author = commenter2 } );

            repository.SaveOrUpdate( blog );
        }
    }
}
