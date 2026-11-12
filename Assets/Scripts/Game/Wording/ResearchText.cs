using System;
using Scrapwalker.Core.Research;

namespace Scrapwalker.Game.Wording
{
    public static class ResearchText
    {
        public static string Text(this ResearchStatus status)
        {
            return status switch
            {
                ResearchStatus.Done => "済み",
                ResearchStatus.Current => "研究中",
                ResearchStatus.Available => "研究できる",
                ResearchStatus.Locked => "前提が足りない",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, "研究の状態の文が無い"),
            };
        }

        public static string ActText(this ResearchState state, ResearchDefinition research)
        {
            switch (state.StatusOf(research))
            {
                case ResearchStatus.Done:
                    return "済み";
                case ResearchStatus.Current:
                    return state.IsWaitingForMaterials() ? "研究中（資材待ち）" : "研究中";
                case ResearchStatus.Locked:
                    return "まだ研究できない";
                default:
                    if (state.Current != null)
                    {
                        return "研究は1つずつ";
                    }

                    return state.IsShort(research) ? "研究する（資材が足りない）" : "研究する";
            }
        }
    }
}
