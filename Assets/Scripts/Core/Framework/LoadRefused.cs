namespace Scrapwalker.Core.Framework
{
    internal sealed class LoadRefused : System.Exception
    {
        public LoadRefused(LoadError error)
        {
            Error = error;
        }

        public LoadError Error { get; }
    }
}
