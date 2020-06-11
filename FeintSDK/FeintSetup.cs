using System;
using System.Reflection;
using System.Linq;

namespace FeintSDK
{

    public abstract class FeintSetup
    {
        private static bool configured = false;
        protected static void Setup()
        {

        }

        public static void CallSetup()
        {
            if (configured)
            {
                return;
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(FeintSetup)));

                if (types.Count() != 0)
                {
                    types.First().GetMethod("Setup").Invoke(null, null);
                    configured = true;
                    return;
                }
            }
            throw new InvalidOperationException("You need to define Feint Setup class!");
        }
    }
}
