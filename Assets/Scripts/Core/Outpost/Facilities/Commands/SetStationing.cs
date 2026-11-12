using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Commands
{
    public sealed class SetStationing : FacilityCommand
    {
        public SetStationing(int facilityID, Stationing stationing)
            : base(facilityID)
        {
            Stationing = stationing;
        }

        public Stationing Stationing { get; }

        internal override string LogText => base.LogText + " 平常時" + Stationing.InPeace + "・戦闘中" + Stationing.InCombat;

        protected override CommandResult Check(World world, Facility facility)
        {
            return facility.Definition.HasStation ? CommandResult.Success() : CommandResult.Fail(CommandError.NoStation);
        }

        protected override void Run(World world, Facility facility)
        {
            facility.Stationing = Stationing;
        }
    }
}
