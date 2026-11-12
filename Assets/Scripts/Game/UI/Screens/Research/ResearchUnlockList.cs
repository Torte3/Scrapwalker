using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Research
{
    internal sealed class ResearchUnlockList : IRecipeShelfView<(string Kind, string KindText)>
    {
        private readonly RowList<Line> _lines;
        private readonly Label _none;

        public ResearchUnlockList(VisualElement parent, VisualTreeAsset lineTemplate, Label none)
        {
            _lines = new RowList<Line>(parent, () => new Line(lineTemplate.Instantiate()), line => line.Element);
            _none = none;
        }

        public void Show(GameData data, GameDefinitions definitions, ResearchDefinition research)
        {
            _lines.Begin();
            foreach (FacilityDefinition facility in definitions.FacilitiesUnlockedBy(research))
            {
                Add("facility", ResearchWords.UnlockFacility, data.NameOf(facility));
            }

            foreach ((FacilityDefinition facility, RecipeDefinition recipe) in definitions.RecipesUnlockedBy(research))
            {
                (string kind, string kindText) = recipe.Result.ShowShelf(this);
                Add(kind, kindText, ResearchWords.UnlockIn(data.NameOf(recipe), data.NameOf(facility)));
            }

            _lines.End();
            _none.Display(_lines.Count == 0);
        }

        public (string Kind, string KindText) Task() => ("task", ResearchWords.UnlockTask);

        public (string Kind, string KindText) Upgrade() => ("upgrade", ResearchWords.UnlockUpgrade);

        public (string Kind, string KindText) Research() => Task();

        private void Add(string kind, string kindText, string text)
        {
            if (_lines.Count < ResearchPage.UnlockSlots)
            {
                _lines.Next().Show(kind, kindText, text);
            }
        }

        private sealed class Line
        {
            private readonly Label _kind;
            private readonly Label _name;

            public Line(VisualElement element)
            {
                Element = element;
                _kind = element.Need<Label>("unlock-kind");
                _name = element.Need<Label>("unlock-name");
            }

            public VisualElement Element { get; }

            public void Show(string kind, string kindText, string text)
            {
                _kind.SetText(kindText);
                _kind.SetVariant("research-unlock__kind--", kind);
                _name.SetText(text);
            }
        }
    }
}
