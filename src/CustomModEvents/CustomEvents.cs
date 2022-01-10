using System.Collections.Generic;

namespace MSNTools
{
    public static class CustomModEvents
    {
        public static readonly CustomModEvent<EntityAlive, EntityAlive> AwardKill;

        static CustomModEvents()
        {
            CustomModEvent<EntityAlive, EntityAlive> modEvent1 = new CustomModEvent<EntityAlive, EntityAlive>();
            modEvent1.eventName = nameof(AwardKill);
            AwardKill = modEvent1;
        }
    }
}