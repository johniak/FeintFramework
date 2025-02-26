

using FeintFramework.Core.Http;
using FeintFramework.Db;
using LinqToDB;
using LinqToDB.Data;

class ExampleView{
    public FeintHttpResponse AsView(FeintHttpRequest request){

        Connections.Connection!.GetTable<Blog>();
        var blog = new Blog{
            Url = "https://example.com"
        };
        

        return new FeintHttpResponse{
            StatusCode = 200,
            Content = "OK"
        };
    }
}