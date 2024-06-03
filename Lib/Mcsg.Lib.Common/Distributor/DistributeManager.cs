using System.Reflection;

namespace Mcsg.Lib.Common.Distributor
{
    public sealed class DistributeManager
    {
        private readonly IList<BaseDistributor> _distrbutors;
        public DistributeManager(Assembly assembly, IServiceProvider serviceProvider)
        {
            _distrbutors = LoadDistributor(assembly, serviceProvider);
        }

        public async Task Deliver(DistributedItem item)
        {
            foreach (var distributor in _distrbutors)
            {
                await distributor.Deliver(item).ConfigureAwait(false);
            }
        }

        private IList<BaseDistributor> LoadDistributor(Assembly assembly, IServiceProvider serviceProvider)
        {
            List<BaseDistributor> objects = new List<BaseDistributor>();

            Type baseType = typeof(BaseDistributor);

            var derivedTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type))
                .ToList();

            IList<BaseDistributor> distributors = new List<BaseDistributor>();

            foreach (var implementType in derivedTypes)
            {
                var instance = Activator.CreateInstance(implementType, serviceProvider) as BaseDistributor;
                distributors.Add(instance);
            }

            return distributors;
        }
    }
}
