using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;

namespace DarkRoles.Roles.Impostor
{
    public class Troll : RoleBase, IImpostor
    {
        public static OptionItem ShiftCooldown, ShiftDuration, VentCooldown;

        public static readonly SimpleRoleInfo RoleInfo =
            SimpleRoleInfo.Create(typeof(Troll),
                player => new Troll(player),
                CustomRoles.Troll,
                () => RoleTypes.Engineer,
                CustomRoleTypes.Neutral,
                22100, SetupOptionItem, "tro", "#d8eb31",
                countType: CountTypes.Crew);

        public Troll(PlayerControl player) : base(RoleInfo, player, () => HasTask.True) { }

        private static void SetupOptionItem()
        {
            ShiftCooldown = FloatOptionItem.Create(RoleInfo, 10, "TrollShiftCooldown", new(2.5f, 180f, 2.5f), 25f, false).SetValueFormat(OptionFormat.Seconds);
            ShiftDuration = FloatOptionItem.Create(RoleInfo, 11, "TrollShiftDuration", new(2.5f, 180f, 2.5f), 12f, false).SetValueFormat(OptionFormat.Seconds);
            VentCooldown = FloatOptionItem.Create(RoleInfo, 12, "TrollVentCooldown", new(2.5f, 180f, 2.5f), 17.5f, false).SetValueFormat(OptionFormat.Seconds);
        }

        public override void ApplyGameOptions(IGameOptions opt)
        {
            opt.SetFloat(FloatOptionNames.ShapeshifterCooldown, ShiftCooldown.GetFloat());
            opt.SetFloat(FloatOptionNames.ShapeshifterDuration, ShiftDuration.GetFloat());
            opt.SetFloat(FloatOptionNames.EngineerInVentMaxTime, 5);
            opt.SetFloat(FloatOptionNames.EngineerCooldown, VentCooldown.GetFloat());
        }

        public override bool OnEnterVent(PlayerPhysics physics, int ventId)
        {
            foreach (var pc in Main.AllAlivePlayerControls)
                if (pc.PlayerId != Player.PlayerId)
                {
                    pc.RpcRandomVentTeleport();
                    return true;
                }
            return false;
        }

        public override bool OnCompleteTask()
        {
            var random = IRandom.Instance;
            List<PlayerControl> targetPlayers = [.. Main.AllAlivePlayerControls.ToArray()];
            if (targetPlayers.Count >= 1)
                targetPlayers[random.Next(0, targetPlayers.Count)].RpcRandomVentTeleport();
            return true;
        }
    }
}
