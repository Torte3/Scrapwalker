namespace Scrapwalker.Core.Framework
{
    public readonly struct LoadError
    {
        public LoadErrorKind Kind { get; }
        public string ID { get; }
        public Definition Facility { get; }

        public LoadError(LoadErrorKind kind, string id = null, Definition facility = null)
        {
            Kind = kind;
            ID = id;
            Facility = facility;
        }
    }
}
