using System.Collections.Generic;

namespace Scrapwalker.Core.Items
{
    public static class QualityRules
    {
        public static readonly int Count = System.Enum.GetValues(typeof(Quality)).Length;
        public static readonly IReadOnlyList<Quality> All = (Quality[])System.Enum.GetValues(typeof(Quality));

        public static Quality Average(this IEnumerable<ItemStack> stacks)
        {
            int sum = 0;
            int count = 0;
            foreach (ItemStack stack in stacks)
            {
                sum += (int)stack.Quality * stack.Count;
                count += stack.Count;
            }

            return count > 0 ? (Quality)(sum / count) : Quality.Plain;
        }

        public static Quality Clamp(int index)
        {
            return (Quality)System.Math.Clamp(index, 0, Count - 1);
        }
    }
}
