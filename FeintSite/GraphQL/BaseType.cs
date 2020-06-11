
namespace Feint.GraphQL
{

    abstract public class BaseType
    {
        public string Name { get => this.DefaultName; }
        public string Description { get; }

        private string DefaultName
        {
            get
            {
                return this.GetType().Name;
            }
        }
    }

}
