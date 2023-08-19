using System.Collections.Generic;
using System.Linq;
using Hazel;
using UnityEngine;
using AmongUs.GameOptions;

using DarkRoles.Roles.Core;
using DarkRoles.Roles.Core.Interfaces;
using static DarkRoles.Translator;

namespace DarkRoles.Roles.Crewmate;
public sealed class OneHitWonder : RoleBase, IKiller
{

    public bool hasShot;
    public static OptionItem KillCooldown;
    public float CurrentKillCooldown = 20;

    public static readonly SimpleRoleInfo RoleInfo =
        SimpleRoleInfo.Create(
            typeof(OneHitWonder),
            player => new OneHitWonder(player),
            CustomRoles.OneHitWonder,
            () => RoleTypes.Impostor,
            CustomRoleTypes.Crewmate,
            20400,
            SetupOptionItem,
            "oh",
            "#7b9ead",
            true,
            introSound: () => GetIntroSound(RoleTypes.Crewmate)
        );
    public OneHitWonder(PlayerControl player)
    : base(
        RoleInfo,
        player,
        () => HasTask.False
    )
    {
        CurrentKillCooldown = KillCooldown.GetFloat();
        hasShot = false;
    }

    private static void SetupOptionItem()
    {
        KillCooldown = FloatOptionItem.Create(RoleInfo, 10, GeneralOption.KillCooldown, new(0f, 990f, 1f), 30f, false)
           .SetValueFormat(OptionFormat.Seconds);
    }

    public float CalculateKillCooldown() => CanUseKillButton() ? CurrentKillCooldown : 0f;
    public bool CanUseKillButton() => Player.IsAlive() && !hasShot;

    public void OnCheckMurderAsKiller(MurderInfo info)
    {
        hasShot = true;
    }
}