using System.Linq;
using AutoMapper;

namespace ImpulseReSTCore.Mapping
{
    /// <summary>
    /// See http://consultingblogs.emc.com/owainwragg/archive/2010/12/22/automapper-mapping-from-multiple-objects.aspx
    /// </summary>
    public static class EntityMapper
    {
        public static T Map<T>(params object[] sources) where T : class
        {
            if (!sources.Any())
            {
                return default(T);
            }

            int i = 0;
            for (; i < sources.Length; i++)
            {
                if (sources[i] != null)
                    break;
                if (i == sources.Length - 1)
                    return null;
            }

            var initialSource = sources[i];

            var mappingResult = Map<T>(initialSource);

            // Now map the remaining source objects
            if (sources.Count() > i)
            {
                Map(mappingResult, sources.Skip(i).ToArray());
            }

            return mappingResult;
        }

        private static void Map(object destination, params object[] sources)
        {
            if (!sources.Any())
            {
                return;
            }

            var destinationType = destination.GetType();

            foreach (var source in sources)
            {
                if (source == null)
                    continue;
                var sourceType = source.GetType();
                Mapper.Map(source, destination, sourceType, destinationType);
            }
        }

        private static T Map<T>(object source) where T : class
        {
            var destinationType = typeof(T);
            var sourceType = source.GetType();

            var mappingResult = Mapper.Map(source, sourceType, destinationType);

            return mappingResult as T;
        }
    }
}
