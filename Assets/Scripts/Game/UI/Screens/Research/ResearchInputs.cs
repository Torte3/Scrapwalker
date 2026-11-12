using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets.Materials;
using UnityEngine;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Screens.Research
{
    internal sealed class ResearchInputs
    {
        private readonly MaterialList _lines;

        public ResearchInputs(VisualElement parent, VisualTreeAsset materialLine)
        {
            _lines = new MaterialList(parent, materialLine);
        }

        public void Show(GameData data, World world, ResearchDefinition definition)
        {
            IReadOnlyList<MaterialAmount> inputs = definition != null ? world.Definitions.RecipeFor(definition).Inputs : Array.Empty<MaterialAmount>();
            _lines.Show(data, inputs, Mathf.Min(inputs.Count, ResearchPage.InputSlots));
            ResearchWork work = world.Research.FindWork();
            _lines.SetCounts(world.Ledger, QualityFilter.Any, 1, work != null && work.Research == definition ? work.DeliveredOf : (Func<MaterialDefinition, int>)null);
        }
    }
}
