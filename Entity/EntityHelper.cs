using System;
using System.Linq;
using System.Reflection;

namespace MtdKey.OrderMaker.Entity
{
    public static partial class EntityHelper
    {
        public static Type GetEntityType(string typeName)
        {
            string nameSpace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            string fullName = $"{nameSpace}.{typeName}";
            return Type.GetType(fullName);
        }

        public static Type[] GetEntityTypes()
        {
            string nameSpace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;

            return Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .Where(entity =>
                     entity.Namespace.Equals(nameSpace) &&
                     entity.Name[..3].Equals("Mtd") &&
                     entity.IsClass)
                 .ToArray();
        }

    }
}
