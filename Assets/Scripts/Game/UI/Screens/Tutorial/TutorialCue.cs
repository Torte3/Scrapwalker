using Scrapwalker.Core.Items;
using Scrapwalker.Core.Sessions.Tutorial;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    public readonly struct TutorialCue
    {
        public TutorialCue(TutorialStep step, TutorialTarget target)
            : this(step, target, null, null, null, null)
        {
        }

        private TutorialCue(TutorialStep step, TutorialTarget target, VisualElement context, VisualElement reveal, MaterialDefinition material, string explanation)
        {
            Step = step;
            Target = target;
            Context = context;
            Reveal = reveal;
            Material = material;
            Explanation = explanation;
        }

        public TutorialStep Step { get; }
        public TutorialTarget Target { get; }
        public VisualElement Context { get; }
        public VisualElement Reveal { get; }
        public MaterialDefinition Material { get; }
        public string Explanation { get; }

        public static TutorialCue Explaining(TutorialTarget area, string text)
        {
            return new TutorialCue(default, area, null, null, null, text);
        }

        public TutorialCue Within(VisualElement context)
        {
            return new TutorialCue(Step, Target, context, Reveal, Material, Explanation);
        }

        public TutorialCue Revealing(VisualElement reveal)
        {
            return new TutorialCue(Step, Target, Context, reveal, Material, Explanation);
        }

        public TutorialCue Making(MaterialDefinition material)
        {
            return new TutorialCue(Step, Target, Context, Reveal, material, Explanation);
        }
    }
}
