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
using Feint.Graphql;
using System.Linq.Expressions;
using FeintSite.Models;
using FeintSite.GraphQL;

namespace FeintSite
{

    public class ExampleModelType : FeintObjectType<ExampleModel>
    {
        static Expression<Func<ExampleModel, dynamic>>[] Excluded = { em => em.ExamplePropertyInteger };
        public ExampleModelType() : base(Excluded)
        {
            Field<StringGraphType>("TestField", resolve: ctx => ctx.Source.ExamplePropertyInteger + "Test");
        }
    }

    public class TripModelType : FeintObjectType<TripModel>
    {

    }
    public class TripInputType : InputObjectGraphType
    {
        public TripInputType()
        {
            Field<NonNullGraphType<StringGraphType>>("name");
        }
    }

    public class Mutation : ObjectGraphType
    {
        public Mutation()
        {
            AddField(new CreateTripMutation().FieldType);

        }

    }

    public class Query : ObjectGraphType
    {
        public Query()
        {
            Name = "Query";
            Field<ExampleModelType>(
                "exampleModel",
                resolve: context => Db.DbSet<ExampleModel>().FirstOrDefault()
            );
            Field<ListGraphType<ExampleModelType>>("allExamples", resolve: ctx => Db.DbSet<ExampleModel>().ToList());
            this.FeintConnectionField<TripModel, TripModelType>("allTrips");
            // Field<ListGraphType<TripModelType>>("allTrips", resolve: ctx => Db.DbSet<TripModel>().ToList());
        }
    }
    public class StarWarsSchema : Schema
    {
        public StarWarsSchema()
            : base()
        {
            Query = new Query();
            Mutation = new Mutation();
        }
    }
    class Feint : FeintSetup
    {
        public static Response Index(Request request)
        {
            var tripModel = new TripModel()
            {
                StartTime = DateTime.Now
            };
            tripModel.Save();
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
            Console.WriteLine(request.Data.GetType());
            Schema schema = new StarWarsSchema();
            var data = (Dictionary<string, object>)request.Data;
            Dictionary<string, object> variables = null;
            if (data.ContainsKey("variables"))
            {
                variables = (Dictionary<string, object>)data["variables"];
            }
            var xD = schema.Execute(_ =>
            {
                _.Schema = schema;
                _.Query = data["query"].ToString();
                if (variables != null)
                {
                    _.Inputs = new Inputs(variables);
                }
            });

            var response = new ApiResponse(xD);
            return response;
        }
        public static new void Setup()
        {
            Settings.Urls.Add(new Url(@"^/$", Feint.Index));
            Settings.Urls.Add(new Url(@"^/graphql/$", Feint.Graphql));
            Settings.DatabaseSettings = new DbSettings() { ConnectionString = "Host=localhost;Database=postgres;Username=postgres;Password=pass", Type = DbTypes.PostGIS };
        }

        static void Main(string[] args)
        {
            Schema schema = new StarWarsSchema();
            FeintSetup.CallSetup();
            Server s = new Server("0.0.0.0", 5000);
            s.Start();
        }
    }
}
