using System.Collections.Generic;

namespace MSNTools
{
    public static class CustomModEvents
    {
        public static readonly CustomModEvent<EntityAlive, EntityAlive> AwardKill;
        public static readonly CustomModEvent<GameManager, PlatformUserIdentifierAbs, List<BlockChangeInfo>> BlockChange;
        public static readonly CustomModEvent StartBloodMoon;
        public static readonly CustomModEvent EndBloodMoon;
        public static readonly CustomModEvent<EntityAlive, EntityAlive> PlayerKillPlayer;

        static CustomModEvents()
        {
            CustomModEvent<EntityAlive, EntityAlive> modEvent1 = new CustomModEvent<EntityAlive, EntityAlive>();
            modEvent1.eventName = nameof(AwardKill);
            AwardKill = modEvent1;

            CustomModEvent<GameManager, PlatformUserIdentifierAbs, List<BlockChangeInfo>> modEvent2 = new CustomModEvent<GameManager, PlatformUserIdentifierAbs, List<BlockChangeInfo>>();
            modEvent2.eventName = nameof(BlockChange);
            BlockChange = modEvent2;

            CustomModEvent modEvent3 = new CustomModEvent();
            modEvent3.eventName = nameof(StartBloodMoon);
            StartBloodMoon = modEvent3;

            CustomModEvent modEvent4 = new CustomModEvent();
            modEvent4.eventName = nameof(EndBloodMoon);
            EndBloodMoon = modEvent4;

            CustomModEvent<EntityAlive, EntityAlive> modEvent5 = new CustomModEvent<EntityAlive, EntityAlive>();
            modEvent5.eventName = nameof(PlayerKillPlayer);
            PlayerKillPlayer = modEvent5;
        }
    }
}