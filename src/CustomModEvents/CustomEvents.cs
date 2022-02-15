using System.Collections.Generic;

namespace MSNTools
{
    public static class CustomModEvents
    {
        public static readonly CustomModEvent<EntityAlive, EntityAlive> AwardKill;
        public static readonly CustomModEvent<GameManager, PlatformUserIdentifierAbs, List<BlockChangeInfo>> BlockChange;

        static CustomModEvents()
        {
            CustomModEvent<EntityAlive, EntityAlive> modEvent1 = new CustomModEvent<EntityAlive, EntityAlive>();
            modEvent1.eventName = nameof(AwardKill);
            AwardKill = modEvent1;

            CustomModEvent<GameManager, PlatformUserIdentifierAbs, List<BlockChangeInfo>> modEvent2 = new CustomModEvent<GameManager, PlatformUserIdentifierAbs, List<BlockChangeInfo>>();
            modEvent2.eventName = nameof(BlockChange);
            BlockChange = modEvent2;
        }
    }
}