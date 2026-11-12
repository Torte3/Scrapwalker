using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Items
{
    [System.Serializable]
    public struct MaterialAmount
    {
        public DefinitionReference<MaterialDefinition> TypeReference;
        public int Count;

        public MaterialAmount(MaterialDefinition type, int count)
        {
            TypeReference = new DefinitionReference<MaterialDefinition>(type);
            Count = count;
        }

        public MaterialDefinition Type
        {
            get => TypeReference?.Definition;
            set => TypeReference = new DefinitionReference<MaterialDefinition>(value);
        }

        public static int CountIn(IReadOnlyList<MaterialAmount> amounts, MaterialDefinition type)
        {
            return amounts.Where(a => a.Type == type).Sum(a => a.Count);
        }

        public static List<MaterialAmount> PerMaterial(IReadOnlyList<MaterialAmount> amounts)
        {
            var merged = new List<MaterialAmount>();
            for (int i = 0; i < amounts.Count; i++)
            {
                MaterialDefinition type = amounts[i].Type;
                if (IndexOf(amounts, type) == i)
                {
                    merged.Add(new MaterialAmount(type, CountIn(amounts, type)));
                }
            }

            return merged;
        }

        public static int Total(IReadOnlyList<MaterialAmount> amounts)
        {
            return amounts.Sum(a => a.Count);
        }

        public static int IndexOf(IReadOnlyList<MaterialAmount> amounts, MaterialDefinition type)
        {
            for (int i = 0; i < amounts.Count; i++)
            {
                if (amounts[i].Type == type)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
