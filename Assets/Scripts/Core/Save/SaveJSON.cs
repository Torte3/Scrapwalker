using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;

namespace Scrapwalker.Core.Save
{
    internal static class SaveJSON
    {
        internal static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = { new StringEnumConverter() },
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new MementoKinds(),
        };

        private sealed class MementoKinds : ISerializationBinder
        {
            private static readonly Type[] Kinds = { typeof(BotActionMemento) };
            private readonly Dictionary<string, Type> _byName = typeof(BotActionMemento).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && Kinds.Any(kind => kind.IsAssignableFrom(type)))
                .ToDictionary(type => type.Name);

            public Type BindToType(string assemblyName, string typeName)
            {
                return _byName.TryGetValue(typeName, out Type type) ? type : throw new JsonSerializationException("写しの型ではない: " + typeName);
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = _byName.ContainsKey(serializedType.Name) ? serializedType.Name : throw new JsonSerializationException("写しの型ではない: " + serializedType);
            }
        }
    }
}
