

using Example.Blog.Models;
using FeintFramework.Core.Http;
using FeintFramework.Db;
using LinqToDB;
using LinqToDB.Data;

class ExampleView
{
    public FeintHttpResponse AsView(FeintHttpRequest request)
    {

        // Connections.Connection!.GetTable<Blog>();
        // var blog = new Blog{
        //     Url = "https://example.com"
        // };

        var author = Author.Objects.ToArray().First();


        return new FeintHttpResponse
        {
            StatusCode = 200,
            Content = $"{author?.FullName} authors found"
        };
    }
}