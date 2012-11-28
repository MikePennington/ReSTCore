using System;
using System.Collections.Generic;
using System.Linq;
using ReSTCore.Controllers;
using ReSTCore.DTO;

namespace ReSTCore.Util
{
    internal static class ObjectFinder
    {
        public static IEnumerable<Type> FindDtoTypes()
        {
            if (RestCore.Configuration.DtoTypes != null)
                return RestCore.Configuration.DtoTypes;

            var dtoTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => IsSubclassOfRawGeneric(typeof(RestDTO<>), type));
            return dtoTypes;
        }

        public static IEnumerable<Type> FindServiceTypes()
        {
            if (RestCore.Configuration.ControllerTypes != null)
                return RestCore.Configuration.ControllerTypes;

            var serviceTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(RestController)));
            return serviceTypes;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            if (generic == null || toCheck == null)
                return false;
            if (generic == toCheck)
                return false;

            while (toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
                if (toCheck == null)
                    return false;
            }
            return false;
        }
    }
}
