using System.Linq;
using FeintSDK.Exceptions;

namespace FeintSDK
{
    public static class Helpers
    {
        public static T GetObjectOr404<T>(IQueryable<T> queryable)
        {
            try
            {
                return queryable.Single();
            }
            catch
            {
                throw new Http404Exception();
            }
        }
    }
}