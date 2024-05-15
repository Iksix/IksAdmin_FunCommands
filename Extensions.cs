using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public static class Extensions
{
	public static IStringLocalizer Localizer;
    public static void ToggleNoclip(this CCSPlayerController player)
    {
        var pawn = player.PlayerPawn.Value;
        if (pawn == null) return;
        if (pawn!.MoveType == MoveType_t.MOVETYPE_NOCLIP)
		{
			pawn.MoveType = MoveType_t.MOVETYPE_WALK;
			Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 2); // walk
			Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
		}
		else
		{
			pawn.MoveType = MoveType_t.MOVETYPE_NOCLIP;
			Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 8); // noclip
			Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
		}
    }
    public static void Freeze(this CBasePlayerPawn pawn)
	{
		pawn.MoveType = MoveType_t.MOVETYPE_OBSOLETE;
		Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 1); // obsolete
		Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
	}
	public static void Unfreeze(this CBasePlayerPawn pawn)
	{
		pawn.MoveType = MoveType_t.MOVETYPE_WALK;
		Schema.SetSchemaValue(pawn.Handle, "CBaseEntity", "m_nActualMoveType", 2); // walk
		Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
	}
    public static void TeleportPlayer(this CCSPlayerController? controller, CCSPlayerController? target)
	{
		if (controller?.PlayerPawn?.Value == null && target!.PlayerPawn?.Value == null)
			return;

		if (
			controller?.PlayerPawn?.Value?.AbsOrigin != null &&
			controller?.PlayerPawn?.Value?.AbsRotation != null &&
			target?.PlayerPawn?.Value?.AbsOrigin != null &&
			target?.PlayerPawn?.Value?.AbsRotation != null
		)
		{
			controller.PlayerPawn.Value.Teleport(
				target.PlayerPawn.Value.AbsOrigin,
				target.PlayerPawn.Value.AbsRotation,
				target.PlayerPawn.Value.AbsVelocity
			);
		}
	}

    public static List<CCSPlayerController> GetOnlinePlayers(bool getBots = false)
    {
        var players = Utilities.GetPlayers();

        List<CCSPlayerController> validPlayers = new List<CCSPlayerController>();

        foreach (var p in players)
        {
            if (!p.IsValid) continue;
            if (p.AuthorizedSteamID == null && !getBots) continue;
            if (p.IsBot && !getBots) continue;
            if (p.Connected != PlayerConnectedState.PlayerConnected) continue;
            validPlayers.Add(p);
        }

        return validPlayers;
    }
    /// <summary>
    /// Getting player by name, #uid, #sid64. For Uid and Sid You need add # at the start Like: #20
    /// </summary>
    public static CCSPlayerController? GetPlayerFromArg(string identity)
    {
        var players = GetOnlinePlayers();
        CCSPlayerController? player;
        if (identity.StartsWith("#"))
        {
            player = players.FirstOrDefault(u => u.SteamID.ToString() == identity.Replace("#", ""));

            if (player != null) return player;

            player = players.FirstOrDefault(u => u.UserId.ToString() == identity.Replace("#", ""));

            if (player != null) return player;
        }
        if (!identity.StartsWith("#"))
            return GetOnlinePlayers().FirstOrDefault(u => u.PlayerName.Contains(identity));
        return null;
    }

    public static void Slap(this CCSPlayerController player, int damage = 0)
	{
        var pawn = player.PlayerPawn.Value;
		if (pawn!.LifeState != (int)LifeState_t.LIFE_ALIVE)
			return;

		/* Teleport in a random direction - thank you, Mani!*/
		/* Thank you AM & al!*/
		var random = new Random();
		var vel = new Vector(pawn.AbsVelocity.X, pawn.AbsVelocity.Y, pawn.AbsVelocity.Z);

		vel.X += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
		vel.Y += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
		vel.Z += random.Next(200) + 100;

		pawn.AbsVelocity.X = vel.X;
		pawn.AbsVelocity.Y = vel.Y;
		pawn.AbsVelocity.Z = vel.Z;

		if (damage <= 0)
			return;

		pawn.Health -= damage;
		Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");

		if (pawn.Health <= 0)
			pawn.CommitSuicide(true, true);
	}
	
	public static void Hp(CCSPlayerController caller, CCSPlayerController target, int health = 100)
	{
		if (health <= 0 || !target.PawnIsAlive || target.PlayerPawn.Value == null) return;

		target.PlayerPawn.Value.Health = health;

		if (health > 100)
		{
			target.PlayerPawn.Value.MaxHealth = health;
		}
		
		Utilities.SetStateChanged(target.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
		IksAdmin_FunCommands.AdminApi!.SendMessageToPlayer(caller, Localizer["NOTIFY_HpSetted"]);
	}

	public static void SetSpeed(CCSPlayerController caller, CCSPlayerController target, float speed)
	{
		CCSPlayerPawn? playerPawnValue = target.PlayerPawn.Value;
		if (playerPawnValue == null) return;
		playerPawnValue.VelocityModifier = speed;
		IksAdmin_FunCommands.AdminApi!.SendMessageToPlayer(caller, Localizer["NOTIFY_SpeedSetted"]);
	}
	public static void SetGravity(this CCSPlayerController controller, float gravity)
	{
		CCSPlayerPawn? playerPawnValue = controller.PlayerPawn.Value;
		if (playerPawnValue == null) return;

		playerPawnValue.GravityScale = gravity;
	}

	public static void SetMoney(this CCSPlayerController controller, int money)
	{
		var moneyServices = controller.InGameMoneyServices;
		if (moneyServices == null) return;

		moneyServices.Account = money;

		Utilities.SetStateChanged(controller, "CCSPlayerController", "m_pInGameMoneyServices");
	}

	public static void DoForCt(Action<CCSPlayerController> action)
	{
		var players = GetOnlinePlayers().Where(x => x.TeamNum == 3).ToList();
		foreach (var player in players)
		{
			action.Invoke(player);
		}
	}
	public static void DoForT(Action<CCSPlayerController> action)
	{
		var players = GetOnlinePlayers().Where(x => x.TeamNum == 2).ToList();
		foreach (var player in players)
		{
			action.Invoke(player);
		}
	}
	public static void DoForAll(Action<CCSPlayerController> action)
	{
		var players = GetOnlinePlayers();
		foreach (var player in players)
		{
			action.Invoke(player);
		}
	}
	public static void DoForSpec(Action<CCSPlayerController> action)
	{
		var players = GetOnlinePlayers().Where(x => x.TeamNum == 1).ToList();
		foreach (var player in players)
		{
			action.Invoke(player);
		}
	}
}