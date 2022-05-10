using HarmonyLib;
using System;
using U = Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XMLData.Item;

namespace MSNTools.Harmony
{
    public class H_NetPackageDamageEntity
    {
        [HarmonyPatch(typeof(NetPackageDamageEntity), "ProcessPackage", new Type[] { typeof(World), typeof(GameManager) })]
        public class H_NetPackageDamageEntity_ProcessPackage
        {
            static bool Prefix(NetPackageDamageEntity __instance, World _world, GameManager _callbacks, ref EnumDamageTypes ___damageTyp, ref EnumDamageSource ___damageSrc, ref int ___entityId, ref int ___attackerEntityId, ref Vector3 ___dirV, ref string ___hitTransformName, ref Vector3 ___hitTransformPosition, ref Vector2 ___uvHit, ref bool ___bIgnoreConsecutiveDamages, ref float ___damageMultiplier, ref bool ___bIsDamageTransfer, ref byte ___bonusDamageType, ref ItemValue ___attackingItem, ref ushort ___strength, ref int ___movementState, ref int ___hitDirection, ref int ___hitBodyPart, ref bool ___bPainHit, ref bool ___bFatal, ref bool ___bCritical, ref float ___random, ref bool ___bCrippleLegs, ref bool ___bDismember, ref bool ___bTurnIntoCrawler, ref byte ___StunType, ref float ___StunDuration, ref EnumEquipmentSlot ___ArmorSlot, ref EnumEquipmentSlotGroup ___ArmorSlotGroup, ref int ___ArmorDamage)
            {
                if (PlayerInvulnerabilityAtTrader.IsEnabled)
                {
                    if (_world == null || ___damageTyp == EnumDamageTypes.Falling && _world.GetPrimaryPlayer() != null && _world.GetPrimaryPlayer().entityId == ___entityId)
                        return false;
                    Entity entity = _world.GetEntity(___entityId);
                    if (!(entity != null))
                        return false;
                    DamageSource damageSource = new DamageSourceEntity(___damageSrc, ___damageTyp, ___attackerEntityId, ___dirV, ___hitTransformName, ___hitTransformPosition, ___uvHit);

                    damageSource.SetIgnoreConsecutiveDamages(___bIgnoreConsecutiveDamages);
                    damageSource.DamageMultiplier = ___damageMultiplier;
                    damageSource.bIsDamageTransfer = ___bIsDamageTransfer;
                    damageSource.BonusDamageType = (EnumDamageBonusType)___bonusDamageType;
                    damageSource.AttackingItem = ___attackingItem;
                    entity.ProcessDamageResponse(new DamageResponse()
                    {
                        Strength = IsInTrader(entity) && entity is EntityPlayer ? 0 : ___strength,
                        ModStrength = 0,
                        MovementState = ___movementState,
                        HitDirection = (U.EnumHitDirection)___hitDirection,
                        HitBodyPart = (EnumBodyPartHit)___hitBodyPart,
                        PainHit = ___bPainHit,
                        Fatal = ___bFatal,
                        Critical = ___bCritical,
                        Random = ___random,
                        Source = damageSource,
                        CrippleLegs = ___bCrippleLegs,
                        Dismember = ___bDismember,
                        TurnIntoCrawler = ___bTurnIntoCrawler,
                        Stun = (EnumEntityStunType)___StunType,
                        StunDuration = ___StunDuration,
                        ArmorSlot = ___ArmorSlot,
                        ArmorSlotGroup = ___ArmorSlotGroup,
                        ArmorDamage = ___ArmorDamage
                    });
                    return false;
                }
                return true;
            }

            static bool IsInTrader(Entity player)
            {
                PrefabInstance prefab = player.world.GetPOIAtPosition(player.position);
                if (prefab != null && player is EntityPlayer)
                {
                    return prefab.prefab.bTraderArea;
                }
                return false;
            }
        }
    }
}
