using System;
using System.Collections.Generic;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI
{
    public abstract class UIScreen
    {
        private readonly List<UIScreen> _parts = new List<UIScreen>();

        protected UIScreen(VisualElement root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
        }

        public VisualElement Root { get; }
        protected UIContext Context { get; private set; }
        protected Session Session => Context.Session;
        protected IAppHost App => Context.App;
        protected UIState UIState => Context.UI;
        protected World World => Context.World;
        protected GameData Data => Context.Data;

        public void Bind(UIContext context)
        {
            Context = context;
            foreach (UIScreen part in _parts)
            {
                part.Bind(context);
            }

            OnBind();
        }

        protected virtual void OnBind()
        {
        }

        protected T AddPart<T>(T part) where T : UIScreen
        {
            _parts.Add(part);
            return part;
        }

        protected T Q<T>(string name) where T : VisualElement
        {
            return Q<T>(Root, name);
        }

        protected static T Q<T>(VisualElement parent, string name) where T : VisualElement
        {
            return parent.Need<T>(name);
        }

        protected VisualElement Q(string name)
        {
            return Q<VisualElement>(name);
        }
    }
}
