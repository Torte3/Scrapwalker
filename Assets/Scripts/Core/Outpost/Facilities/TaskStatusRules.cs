namespace Scrapwalker.Core.Outpost.Facilities
{
    public static class TaskStatusRules
    {
        public static bool IsStop(this TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Advancing:
                case TaskStatus.Paused:
                case TaskStatus.Ready:
                case TaskStatus.AwaitingBot:
                case TaskStatus.Upgrading:
                case TaskStatus.Operating:
                case TaskStatus.NoTasks:
                    return false;
                default:
                    return true;
            }
        }
    }
}
