using MSNTools.Discord;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSNTools
{
    public class AntiCollapse
    {
        public static bool IsEnabled = false;
        public static int MinEntitiesDetected = 50;
        private static bool CollapseStart = false;
        private static bool FirstPass = false;
        private static int fallingBlocksCount = 0;
        private static PlayerSorter sorter;

        public static void Exec()
        {
            try
            {
                if (GameManager.Instance.World != null && GameManager.Instance.World.Players.Count > 0 && GameManager.Instance.World.Players.dict.Count > 0)
                {
                    List<Entity> entities = GameManager.Instance.World.Entities.list;
                    int fallingBlocksCount1 = 0;
                    Vector3 fallingBlockPos = new Vector3();

                    for (int i = 0; i < entities.Count; i++)
                    {
                        if (entities[i] is EntityFallingBlock)
                        {
                            if (!CollapseStart)
                                CollapseStart = true;
                            EntityFallingBlock entityFallingBlock = (EntityFallingBlock)entities[i];
                            IChunk chunk = GameManager.Instance.World.GetChunkFromWorldPos(Vector3i.FromVector3Rounded(entityFallingBlock.position));
                            GameManager.Instance.World.RemoveEntity(entityFallingBlock.entityId, EnumRemoveEntityReason.Despawned);
                            fallingBlockPos = entityFallingBlock.position;
                            ++fallingBlocksCount1;
                            ++fallingBlocksCount;
                        }
                        if (fallingBlocksCount1 == 0)
                            CollapseStart = false;
                    }
                    if (fallingBlocksCount > MinEntitiesDetected && CollapseStart && !FirstPass)
                    {
                        List<EntityPlayer> playersNearCollapse = GetPlayersNearCollapsePosition(fallingBlockPos);

                        DiscordWebhookSender.SendAlertStartCollapseEmbedToWebHook(fallingBlockPos, playersNearCollapse);
                        FirstPass = true;
                        return;
                    }
                    if (fallingBlocksCount > MinEntitiesDetected && !CollapseStart)
                    {
                        DiscordWebhookSender.SendAlertFinishCollapseEmbedToWebHook(fallingBlocksCount);
                        fallingBlocksCount = 0;
                        FirstPass = false;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in AntiCollapse.Exec: {e.Message}");
            }
        }

        static List<EntityPlayer> GetPlayersNearCollapsePosition(Vector3 collapsePosition)
        {
            try
            {
                List<EntityPlayer> playersList = GameManager.Instance.World.Players.list;
                List<EntityPlayer> playersNearCollapse = new List<EntityPlayer>();

                for (int i = 0; i < playersList.Count; i++)
                {
                    EntityPlayer player = playersList[i];
                    if (Vector3.Distance(player.position, collapsePosition) <= 50)
                    {
                        playersNearCollapse.Add(player);
                    }
                }

                sorter = new PlayerSorter(collapsePosition);
                playersNearCollapse.Sort(sorter);
                return playersNearCollapse;
            }
            catch (Exception e)
            {
                MSNUtils.LogError($"Error in AntiCollapse.GetPlayersNearCollapsePosition: {e.Message}");
            }
            return null;
        }
    }

    public class PlayerSorter : IComparer<Entity>
    {
        private Vector3 self;

        public PlayerSorter(Vector3 _self)
        {
            self = _self;
        }

        private int isNearer(Entity _e, Entity _other)
        {
            float num1 = DistanceSqr(self, _e.position);
            float num2 = DistanceSqr(self, _other.position);
            if (num1 < num2)
                return -1;
            return num1 > num2 ? 1 : 0;
        }

        public int Compare(Entity _obj1, Entity _obj2)
        {
            return isNearer(_obj1, _obj2);
        }

        public float DistanceSqr(Vector3 pointA, Vector3 pointB)
        {
            Vector3 vector3 = pointA - pointB;
            return (float)(vector3.x * vector3.x + vector3.y * vector3.y + vector3.z * vector3.z);
        }
    }
}