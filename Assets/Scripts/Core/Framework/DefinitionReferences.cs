using System;
using System.Collections;
using System.Reflection;

namespace Scrapwalker.Core.Framework
{
    public static class DefinitionReferences
    {
        public static void ForEach(object root, Action<DefinitionReference> visit)
        {
            Visit(root, visit);
        }

        private static void Visit(object value, Action<DefinitionReference> visit)
        {
            switch (value)
            {
                case null:
                case string _:
                    return;
                case DefinitionReference reference:
                    visit(reference);
                    return;
                case IList list:
                    foreach (object item in list)
                    {
                        Visit(item, visit);
                    }

                    return;
            }

            Type type = value.GetType();
            if (type.IsPrimitive || type.IsEnum || type.Assembly != typeof(DefinitionReferences).Assembly)
            {
                return;
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!field.IsDefined(typeof(NonSerializedAttribute)))
                {
                    Visit(field.GetValue(value), visit);
                }
            }
        }
    }
}
