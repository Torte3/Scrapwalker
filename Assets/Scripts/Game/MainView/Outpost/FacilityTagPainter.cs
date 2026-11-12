using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using TMPro;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Outpost
{
    public sealed class FacilityTagPainter : WorldPainter
    {
        private const float QualityTagWidth = 52f;
        private const float LabelHeight = 14f;
        private const float LabelPixels = 10.5f;
        private const float LabelPadding = 6f;
        private const float JobHeight = 8.5f;

        private readonly WorldUI _ui;
        private readonly GameData _data;
        private readonly Dictionary<string, float> _labelWidths = new Dictionary<string, float>();

        public FacilityTagPainter(WorldScene scene, WorldUI ui, GameData data)
            : base(scene)
        {
            _ui = ui;
            _data = data;
        }

        private string NameOf(in FacilityVisual facility)
        {
            return _data.NameOf(facility.Facility.Definition);
        }

        private static BarColors BarOf(bool isActive)
        {
            return isActive ? Palette.ActiveBar : Palette.IdleBar;
        }

        public void DrawRelocateTargets(List<FacilityVisual> facilities)
        {
            _ui.Layer(DrawBand.Facilities);
            foreach (FacilityVisual facility in facilities)
            {
                CellRect? relocate = facility.Facility.RelocateRect;
                if (!relocate.HasValue)
                {
                    continue;
                }

                Rect target = new FacilityRect(relocate.Value).Inset(3f);
                _ui.FillRect(target.x, target.y, target.width, target.height, Palette.RelocateFill);
                _ui.DashedRect(target.x, target.y, target.width, target.height, 1.5f, Palette.Cyan, 5f, 4f, -Scene.ElapsedFrames * 0.4f);
                FacilityLabel(WorldWords.RelocateTo(NameOf(facility)), target.x + 2f, target.y + 2f, Palette.Cyan, target.width - 4f, LabelHeight);
            }
        }

        public void Draw(List<FacilityVisual> facilities, bool showsQuality)
        {
            _ui.Layer(DrawBand.FacilityUI);
            foreach (FacilityVisual facility in facilities)
            {
                Rect inner = facility.Rect.Inner;
                bool isRunningTask = facility.Task != null && facility.IsWorking;
                DrawBar(facility, inner, isRunningTask);
                if (!facility.Look.Unlabeled || facility.Job.IsUnderWay)
                {
                    DrawLabels(facility, inner, isRunningTask);
                }

                Quality? quality = facility.Facility.BuiltQuality;
                if (showsQuality && facility.Facility.IsOperational && !facility.Job.IsUnderWay && quality.HasValue && quality.Value != Quality.Plain)
                {
                    float x = inner.xMax - 2f - QualityTagWidth;
                    _ui.FillRect(x, inner.y + 4f, QualityTagWidth, 14f, Palette.Quality(quality.Value));
                    _ui.Text(x + 4f, inner.y + 15f, quality.Value.WithStars(), 10f, Palette.QualityText, FontStyles.Bold);
                }
            }
        }

        private void DrawBar(in FacilityVisual facility, Rect inner, bool isRunningTask)
        {
            FacilityJob job = facility.Job;
            if (job.IsUnderWay)
            {
                BarColors colors = BarOf(job.IsManned(facility));
                ProgressBar(inner, facility.Facility.SupplyShare, job.ProgressOf(facility.Facility), colors.Ready, job.BarColor ?? colors.Front);
            }
            else if (facility.Task != null)
            {
                BarColors colors = BarOf(isRunningTask);
                Workshop work = facility.Facility.Work;
                ProgressBar(inner, work.ReadyShare(facility.Task), work.WorkProgress, colors.Ready, colors.Front);
            }
        }

        private void DrawLabels(in FacilityVisual facility, Rect inner, bool isRunningTask)
        {
            float x = inner.x + 3f;
            float width = inner.width - 6f;
            if (isRunningTask)
            {
                FacilityLabel(_data.NameOf(facility.Task.Recipe), x, inner.y + 3f, Palette.TaskName, width, LabelHeight);
            }
            else
            {
                FacilityLabel(NameOf(facility), x, inner.y + 3f, facility.Job.NameColor, width, LabelHeight);
            }

            FacilityJob job = facility.Job;
            if (!job.IsUnderWay)
            {
                return;
            }

            if (facility.IsOverLimit)
            {
                FacilityLabel(WorldWords.OverLimit, x, inner.y + 19f, Palette.Red, width, JobHeight);
                return;
            }

            string text = job.Word + " " + Mathf.FloorToInt(job.ProgressOf(facility.Facility) * 100f) + "%";
            FacilityLabel(text, x, inner.y + 19f, job.TextColor, width, JobHeight);
        }

        private void FacilityLabel(string text, float x, float y, Color color, float width, float height)
        {
            float textWidth = Mathf.Max(1f, LabelWidth(text));
            float pixels = Mathf.Min(LabelPixels * height / LabelHeight, LabelPixels * (width - LabelPadding) / textWidth);
            _ui.FillRoundRect(x, y, Mathf.Min(width, textWidth * pixels / LabelPixels + LabelPadding), height, 2f, Palette.LabelBack);
            _ui.Text(x + 4f, y + height * 0.5f + pixels * 0.4f, text, pixels, color, FontStyles.Bold);
        }

        private float LabelWidth(string text)
        {
            if (!_labelWidths.TryGetValue(text, out float width))
            {
                width = _ui.TextWidth(text, LabelPixels, FontStyles.Bold);
                _labelWidths[text] = width;
            }

            return width;
        }

        private void ProgressBar(Rect inner, float fed, float progress, Color ready, Color front)
        {
            var bar = new WorldBar(_ui, new Rect(inner.x + 1f, inner.yMax - 3f, inner.width - 2f, 2f));
            bar.Back(Palette.BarBack, 0f);
            if (fed > 0f)
            {
                bar.Fill(fed, ready);
            }

            bar.Fill(progress, front);
        }
    }
}
