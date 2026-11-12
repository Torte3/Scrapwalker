using System.Collections.Generic;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Root
{
    public readonly struct CommandResult
    {
        public bool IsOK => Error == CommandError.None;
        public CommandError Error { get; }
        public string SubjectID { get; }
        public IReadOnlyList<MaterialAmount> Shortage { get; }

        private CommandResult(CommandError error, string subjectID, IReadOnlyList<MaterialAmount> shortage)
        {
            Error = error;
            SubjectID = subjectID;
            Shortage = shortage;
        }

        public static CommandResult Success()
        {
            return new CommandResult(CommandError.None, null, null);
        }

        public static CommandResult Fail(CommandError error, string subjectID = null, IReadOnlyList<MaterialAmount> shortage = null)
        {
            return new CommandResult(error, subjectID, shortage);
        }

        public static CommandResult From(CommandError error)
        {
            return error == CommandError.None ? Success() : Fail(error);
        }
    }
}
