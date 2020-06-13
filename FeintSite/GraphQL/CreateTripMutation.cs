

using System;
using Feint.Graphql;
using FeintSite.Models;
using GraphQL.Types;

namespace FeintSite.GraphQL
{

    public class CreateTripMutation : FeintMutation<TripModelType>
    {
        public CreateTripMutation()
        {
            Name = "createTrip";
            Arguments = new QueryArguments(
            new QueryArgument<NonNullGraphType<TripInputType>> { Name = "input" }
          );
        } 
        public override object Mutate(ResolveFieldContext<object> ctx)
        {
            var trip = ctx.GetArgument<TripModel>("input");
            trip.StartTime = DateTime.Now;
            trip.Save();
            return trip;
        }
    }

}