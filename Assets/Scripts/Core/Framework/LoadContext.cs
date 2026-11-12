namespace Scrapwalker.Core.Framework
{
    internal sealed class LoadContext
    {
        private readonly System.Func<string, Definition> _byID;

        public LoadContext(System.Func<string, Definition> byID)
        {
            _byID = byID;
        }

        public T Find<T>(string id) where T : Definition
        {
            return (string.IsNullOrEmpty(id) ? null : _byID(id) as T) ?? throw new LoadRefused(new LoadError(LoadErrorKind.UnknownDefinition, id: id));
        }

        public T FindOrNone<T>(string id) where T : Definition
        {
            return id == null ? null : Find<T>(id);
        }

        public LoadRefused UnknownRecipe(string id, Definition facility)
        {
            return new LoadRefused(new LoadError(LoadErrorKind.UnknownRecipe, id: id, facility: facility));
        }

        public LoadRefused Malformed()
        {
            return new LoadRefused(new LoadError(LoadErrorKind.NotSaveFormat));
        }
    }
}
