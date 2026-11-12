using System;
using Scrapwalker.Core.Research;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Research
{
    public sealed class ResearchNode
    {
        public static readonly Vector2 PictureSize = new Vector2(ResearchTreeLayout.NodeWidth - 16f, ResearchTreeLayout.NodeHeight - 34f);

        private readonly VisualElement _fill;
        private readonly ResearchPicture _picture;
        private ResearchStatus? _status;
        private bool _selected;
        private float _progress = -1f;

        public ResearchNode(TemplateContainer instance, GameData data, ResearchDefinition definition, Vector2 at, Action<ResearchDefinition> onClick)
        {
            Definition = definition;
            Root = instance.Need<VisualElement>("node");
            Frame = instance.Need<VisualElement>("frame");
            _fill = instance.Need<VisualElement>("bar-fill");
            instance.pickingMode = PickingMode.Ignore;
            instance.AddToClassList("research-node-slot");
            Root.style.left = at.x;
            Root.style.top = at.y;
            Root.style.width = ResearchTreeLayout.NodeWidth;
            Root.style.height = ResearchTreeLayout.NodeHeight;
            instance.Need<Label>("name").text = data.NameOf(definition);
            _picture = new ResearchPicture(instance.Need<VisualElement>("picture"), PictureSize);
            _picture.Show(data, definition);
            Frame.RegisterCallback<ClickEvent>(_ => onClick(Definition));
            _fill.SetFill(0f, 0f);
        }

        public ResearchDefinition Definition { get; }
        public VisualElement Root { get; }
        public VisualElement Frame { get; }

        public void Show(ResearchStatus status, bool selected, float progress)
        {
            if (status != _status)
            {
                _status = status;
                Root.SetVariant("research-node--status-", status);
                _picture.SetDim(status == ResearchStatus.Locked);
            }

            if (selected != _selected)
            {
                _selected = selected;
                Root.EnableInClassList("research-node--selected", selected);
            }

            if (!Mathf.Approximately(progress, _progress))
            {
                _progress = progress;
                _fill.SetFill(0f, progress);
            }
        }
    }
}
