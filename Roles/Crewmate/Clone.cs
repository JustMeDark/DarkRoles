using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Il2CppInterop.Generator.Passes;
using DarkRoles.Attributes;
using DarkRoles.Roles.Core;

namespace DarkRoles.Roles.Crewmate;
public sealed class Clone : RoleBase
{
    public static Dictionary<byte, float> CurrentKillCooldown = new();
    public static List<byte> playerIdList = new();
    public static OptionItem OptionKillCooldown;
    public static float KillCooldown;


    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(Clone),
            player => new Clone(player),
            CustomRoles.Clone,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Crewmate,
            20100,
            SetupOptionItem,
            "cl",
            "#43735b"
        );
    public Clone(PlayerControl player): base(RoleInfo,player)
    {
    }

    public override void Add(byte playerId)
    {
        playerIdList.Add(playerId);
        CurrentKillCooldown.Add(playerId, OptionKillCooldown.GetFloat());

        if (!AmongUsClient.Instance.AmHost) return;
        if (!Main.ResetCamPlayerList.Contains(playerId))
            Main.ResetCamPlayerList.Add(playerId);
    }

    public override void Add()
    {
        foreach (var ar in Main.AllPlayerControls)
            Add(ar.PlayerId);
    }

    [GameModuleInitializer]
    public static void Init()
    {
        playerIdList = new();
        CurrentKillCooldown = new();
    }

    public enum Options
    {
        CloneCooldown
    }

    public static bool IsEnable => playerIdList.Any();

    public static void SetKillCooldown(byte id) => Main.AllPlayerKillCooldown[id] = Utils.GetPlayerById(id).IsAlive() ? CurrentKillCooldown[id] : 0f;

    private static void SetupOptionItem()
    {
        OptionKillCooldown = FloatOptionItem.Create(RoleInfo, 10, Options.CloneCooldown, new(0f, 45f, 2.5f), 15f, false)
            .SetValueFormat(OptionFormat.Seconds);
    }

    public override bool OnCheckMurder(PlayerControl pc, PlayerControl tpc)
    {
        CustomRoles role = tpc.GetCustomRole();
        if (tpc.Is(CustomRoles.Madmate)) pc.RpcSetCustomRole(CustomRoles.Madmate); else pc.RpcSetCustomRole(role);
        pc.RpcGuardAndKill(pc);
        return false;
    }
}