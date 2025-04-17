

using Example.Blog.Models;
using FeintFramework.Http;
using FeintFramework.Db;
using LinqToDB;
using LinqToDB.Data;
using FeintFramework.Forms.Widgets;
using FeintFramework.Forms;
using FeintFramework.Forms.Fields;
using FeintFramework.Contrib.Auth;
using FeintFramework.Routing;

class AuthForm : Form
{
    public CharFormField Username = new CharFormField() { Label = "Username" };
    public CharFormField Password = new CharFormField() { Label = "Password" };
}

class BlogPostForm : ModelForm<BlogPost>
{
    public static class Meta
    {
        public static string[] Fields = ["__all__"];
    }
}

class ExampleView
{
    public FeintHttpResponse AsView(FeintHttpRequest request)
    {
        var input = new TextInput();
        var html = input.Render();
        var parameters = new Dictionary<string, object>
        {
            { "test", "21" }
        };
        var url = Router.Reverse("example2", parameters);
        Console.WriteLine(html);
        // var lo2 = 0;
        // var lol = 200 / lo2;
        User.Objects.Where(x => x.Username == "root").ToArray();
        // var user = new User(){
        //     Username="root",
        //     IsStaff=true,
        //     IsSuperuser=true,
        //     Email="root@root.com"
        // };
        // user.SetPassword("root1234");
        // Console.WriteLine("Pass");
        // Console.WriteLine(user.Password);
        // user.Save();

        // Connections.Connection!.GetTable<Blog>();
        // var blog = new Blog{
        //     Url = "https://example.com"
        // };

        // var author = Author.Objects.ToArray().First();


        return new FeintHttpResponse
        {
            StatusCode = 200,
            Content = new BlogPostForm().AsP,
            ContentType = "text/html"
        };
    }
}