using System;
using Scrapwalker.Core.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public static class UIKit
    {
        private const string Unusable = "btn--unusable";

        public static T Need<T>(this VisualElement parent, string name) where T : VisualElement
        {
            return parent.Q<T>(name) ?? throw new InvalidOperationException(parent.name + " に " + typeof(T).Name + " 「" + name + "」が無い（UXML を見る）");
        }

        public static VisualElement NeedClass(this VisualElement parent, string className)
        {
            return parent.Q(className: className) ?? throw new InvalidOperationException(parent.name + " に「." + className + "」が無い（UXML を見る）");
        }

        public static void WriteLabel(this VisualElement parent, string name, string text)
        {
            parent.Need<Label>(name).text = text;
        }

        public static void SetText(this TextElement label, string text)
        {
            if (label.text != text)
            {
                label.text = text;
            }
        }

        public static void Show(this VisualElement element, bool on)
        {
            element.EnableInClassList("invisible", !on);
        }

        public static void Display(this VisualElement element, bool on)
        {
            element.EnableInClassList("hidden", !on);
        }

        public static bool IsLaidOut(this VisualElement element)
        {
            return element != null && element.panel != null && element.IsShown() && element.worldBound.width > 0f && element.worldBound.height > 0f;
        }

        public static bool IsShown(this VisualElement element)
        {
            return !element.ClassListContains("invisible") && !element.ClassListContains("hidden");
        }

        public static T Ignored<T>(this T element) where T : VisualElement
        {
            element.pickingMode = PickingMode.Ignore;
            return element;
        }

        public static void SetSeverity(this VisualElement element, Severity severity)
        {
            element.EnableInClassList("warn", severity == Severity.Warning);
            element.EnableInClassList("caution", severity == Severity.Caution);
        }

        public static void SetFill(this VisualElement fill, float from, float to)
        {
            from = Mathf.Clamp01(from);
            to = Mathf.Clamp01(to);
            fill.style.left = Length.Percent(from * 100f);
            fill.style.width = Length.Percent(Mathf.Max(0f, to - from) * 100f);
            fill.style.display = to > from ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void OnClick(this Button button, Action action)
        {
            if (ReferenceEquals(button.userData, action))
            {
                return;
            }

            if (button.userData is Action previous)
            {
                button.clicked -= previous;
            }

            button.userData = action;
            if (action != null)
            {
                button.clicked += action;
            }
        }

        public static void SetUsable(this Button button, bool usable, Action action)
        {
            button.EnableInClassList(Unusable, !usable);
            button.OnClick(usable ? action : null);
        }

        public static bool IsUsable(this Button button)
        {
            return button.enabledInHierarchy && !button.ClassListContains(Unusable);
        }

        public static VisualTreeAsset TakeTemplate(this VisualElement root, string name)
        {
            TemplateContainer sample = root.Q<TemplateContainer>(name);
            if (sample == null || sample.templateSource == null)
            {
                throw new InvalidOperationException(root.name + " に行の見本「" + name + "」が無い（UXML を見る）");
            }

            sample.RemoveFromHierarchy();
            return sample.templateSource;
        }

        public static void SetVariant(this VisualElement element, string prefix, string variant)
        {
            foreach (string name in new System.Collections.Generic.List<string>(element.GetClasses()))
            {
                if (name.StartsWith(prefix) && name != prefix + variant)
                {
                    element.RemoveFromClassList(name);
                }
            }

            element.AddToClassList(prefix + variant);
        }

        public static void SetVariant(this VisualElement element, string prefix, Enum variant)
        {
            string name = variant.ToString();
            var builder = new System.Text.StringBuilder(name.Length + 4);
            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]))
                {
                    builder.Append('-');
                }

                builder.Append(char.ToLowerInvariant(name[i]));
            }

            element.SetVariant(prefix, builder.ToString());
        }

        public static Vector2 ScreenToPanel(this IPanel panel, Vector2 screen)
        {
            return RuntimePanelUtils.ScreenToPanel(panel, new Vector2(screen.x, Screen.height - screen.y));
        }

        public static void SetQuality(this VisualElement element, Quality? quality)
        {
            foreach (Quality q in QualityRules.All)
            {
                element.EnableInClassList("quality-" + (int)q, quality == q);
            }
        }
    }
}
