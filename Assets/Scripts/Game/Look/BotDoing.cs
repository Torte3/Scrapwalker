using System;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Game.Look
{
    public readonly struct BotDoing : IEquatable<BotDoing>
    {
        public BotDoing(BotVerb verb, Place place, Place serving, int? task, MaterialDefinition material, BotProgress progress)
        {
            Verb = verb;
            Place = place;
            Serving = serving;
            Task = task;
            Material = material;
            Progress = progress;
        }

        public BotVerb Verb { get; }
        public Place Place { get; }
        public Place Serving { get; }
        public int? Task { get; }
        public MaterialDefinition Material { get; }
        public BotProgress Progress { get; }

        public bool Serves(Facility facility, int taskID)
        {
            return Serving.IsAt(facility) && Task == taskID;
        }

        public bool Equals(BotDoing other) => Verb == other.Verb && Place == other.Place;
        public override bool Equals(object value) => value is BotDoing other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Verb, Place);
        public static bool operator ==(BotDoing a, BotDoing b) => a.Equals(b);
        public static bool operator !=(BotDoing a, BotDoing b) => !a.Equals(b);
    }
}
