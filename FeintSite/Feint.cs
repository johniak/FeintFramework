using System;
using System.Collections.Generic;
using FeintApi;
using FeintSDK;
using FeintSDK.Server;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using GraphQL.Types;
using GraphQL;

namespace FeintSite
{

    public class ExampleModelType : ObjectGraphType<ExampleModel>
    {
        public ExampleModelType()
        {
            Name = "ExampleModel";

            // Field(h => h.Id).Description("The id of the human.");
            Field<Int32>("Id", f => f.Id.Value, nullable: true);
            Field(e=>e.ExamplePropertyInteger);
            // Field<String>("Weird")
        }
    }

    public class Query : ObjectGraphType<object>
    {
        public Query()
        {
            Name = "Query";
            Field<ExampleModelType>(
                "exampleModel",
                resolve: context => Db.DbSet<ExampleModel>().FirstOrDefault()
            );
        }
    }
    public class StarWarsSchema : Schema
    {
        public StarWarsSchema()
            : base()
        {
            Query = new Query();
        }
    }
    // class ExampleModelType : ObjectType
    // {
    //     public ExampleModelType()
    //     {
    //         this.Field(new StringType(), "ExamplePropertyString", null, true, null, null);
    //         this.Field(new IntType(), "ExamplePropertyInteger", null, true, null, null);
    //         this.Field(new IntType(), "ExamplePropertyInteger1", null, true, null, null);
    //     }
    // }
    // class Query : ObjectType
    // {
    //     public Query()
    //     {
    //         this.Field(new ExampleModelType(), "ExampleModel", null, true, null, null);
    //     }

    //     dynamic ResolveExampleModel(dynamic parent)
    //     {
    //         return null;
    //     }
    // }
    // class FeintSchema : Schema
    // {
    //     public FeintSchema()
    //         : base()
    //     {
    //         this.Query = new Query();
    //     }
    // }

    class Feint : FeintSetup
    {
        public static Response Index(Request request)
        {
            var exmpModel = Db.DbSet<ExampleModel>().FirstOrDefault();
            if (exmpModel == null)
            {
                exmpModel = new ExampleModel() { ExamplePropertyInteger = 0 };
            }
            else
            {
                exmpModel.ExamplePropertyInteger += 11;
            }
            exmpModel.Save();

            return new ApiResponse($"{exmpModel.ExamplePropertyInteger}");
        }

        public static Response Graphql(Request request)
        {
            Schema schema = new StarWarsSchema();
            Console.WriteLine(request.FormData["query"]);
            var xD = schema.Execute(_ =>
            {
                _.Schema = schema;
                _.Query = request.FormData["query"];
            });

            Console.WriteLine(xD);
            var response = new ApiResponse(xD);
            return response;
        }
        public static new void Setup()
        {
            Settings.Urls.Add(new Url(@"^/$", Feint.Index));
            Settings.Urls.Add(new Url(@"^/graphql/$", Feint.Graphql));
            Settings.DatabaseSettings = new DbSettings() { ConnectionString = "Host=db;Database=postgres;Username=postgres;Password=pass", Type = DbTypes.PosgreSQL };
        }

        static void Main(string[] args)
        {

            FeintSetup.CallSetup();
            Server s = new Server("0.0.0.0", 5000);
            s.Start();
        }
    }
}
