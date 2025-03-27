

using Example.Blog.Models;
using FeintFramework.Core.Http;
using FeintFramework.Db;
using LinqToDB;
using LinqToDB.Data;
using FeintFramework.Forms.Widgets;
using FeintFramework.Forms;
using FeintFramework.Forms.Fields;

class AuthForm: Form
{
    public CharFormField Username = new CharFormField(){Label = "Username"};
    public CharFormField Password = new CharFormField(){Label = "Password"};
}

class ExampleView
{
    public FeintHttpResponse AsView(FeintHttpRequest request)
    {
        var input = new TextInput();
        var html = input.Render();
        Console.WriteLine(html);
        

        // Connections.Connection!.GetTable<Blog>();
        // var blog = new Blog{
        //     Url = "https://example.com"
        // };

        // var author = Author.Objects.ToArray().First();


        return new FeintHttpResponse
        {
            StatusCode = 200,
            Content = new AuthForm().AsP(),
            ContentType = "text/html"
        };
    }
}