using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;
using MS.Internal.Xml.XPath;
using static UnityEngine.GraphicsBuffer;

namespace DarkRoles.Roles.Neutral
{
    public class Accelerator : RoleBase, IKiller
    {
        public static bool HasUsed, HasTarget, Shifted, HasVented;
        private static PlayerControl ShiftTarget;
        private static OptionItem KillCooldown;
        public float originalSpeed;

        public static readonly SimpleRoleInfo RoleInfo =
           SimpleRoleInfo.Create(
               typeof(Accelerator),
               player => new Accelerator(player),
               CustomRoles.Accelerator,
               () => RoleTypes.Impostor,
               CustomRoleTypes.Neutral,
               11000,
               SetupOptionItem,
               "ac",
               "#4f5e6e",
               true,
               countType: CountTypes.Accelerator,
               assignInfo: new RoleAssignInfo(CustomRoles.Accelerator, CustomRoleTypes.Neutral) { AssignCountRule = new(1, 1, 1) });

        public Accelerator(PlayerControl player) : base(RoleInfo, player, () => HasTask.False)
        {
            HasTarget = false;
            Shifted = false;
            HasUsed = false;
            HasVented = false;
            originalSpeed = AURoleOptions.PlayerSpeedMod;
        }

        public static void SetupOptionItem()
        {
            KillCooldown = FloatOptionItem.Create(RoleInfo, 10, "AccKillCooldown", new(0f, 100f, 2.5f), 22.5f, false).SetValueFormat(OptionFormat.Seconds);
        }

        public void OnCheckMurderAsKiller(MurderInfo info)
        {
            var (killer, target) = info.AttemptTuple;
            if (!HasTarget)
            {
                info.DoKill = false;
                ShiftTarget = target;
                HasTarget = true;
                killer.SetKillCooldown(2f);
            }
            if (Shifted && HasTarget)
            {
                info.DoKill = true;
                killer.RpcShapeshift(killer, false);
                killer.SetKillCooldown(KillCooldown.GetFloat());
                HasTarget = false;
            }
            return;
        }

        /* public override bool OnEnterVent(PlayerPhysics physics, int ventId)
         {
             if (!HasVented)
                 HasVented = true;
             return true;
         }*/

        public override void IsInVent()
        {
            if (!HasVented)
                HasVented = true;
        }

        public static void OnExitVent(PlayerControl pc)
        {
            if (HasTarget)
            {
                Shifted = true;
                pc.RpcShapeshift(ShiftTarget, false);
            }
        }

        public bool CanUseSabotageButton() => false;
        public bool CanUseImpostorVentButton() => true;
        public float CalculateKillCooldown() => KillCooldown.GetFloat();
    }
}
