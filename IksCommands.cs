using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public class IksCommands
{
    public IIksAdminApi AdminApi = IksAdmin_FunCommands.AdminApi!;
    public IStringLocalizer Localizer = IksAdmin_FunCommands.GlobalLocalizer!;
    public List<PositionModel> SavedPositions = new();
    public IksCommands()
    {
        AdminApi.AddNewCommand(
            "savepos",
            "Save player position",
            "css_savepos <position index>",
            1,
            "savepos",
            "d",
            CommandUsage.CLIENT_ONLY,
            OnSavePosCommand
        );
        AdminApi.AddNewCommand(
            "teleport",
            "teleport to saved location",
            "css_teleport <index>\ncss_teleport <#sid/#uid/name> <index>",
            1,
            "teleport",
            "d",
            CommandUsage.CLIENT_ONLY,
            OnTeleportCommand
        );
        AdminApi.AddNewCommand(
            "teleport",
            "teleport to saved location",
            "css_teleport <index>\ncss_teleport <#sid/#uid/name> <index>",
            1,
            "teleport",
            "d",
            CommandUsage.CLIENT_ONLY,
            OnTeleportCommand
        );
        AdminApi.AddNewCommand(
            "slap",
            "slap the player",
            "css_slap <#uid/#sid/name> <damage>",
            1,
            "slap",
            "s",
            CommandUsage.CLIENT_ONLY,
            OnSlapCommand
        );
    }

    private void OnSlapCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (args.Count == 1)
        {
            Slap(caller, target);
        }
        else if (args.Count == 2)
        {
            Slap(caller, target, int.Parse(args[1]));
        }
    }

    public void Slap(CCSPlayerController caller, CCSPlayerController? target, int damage = 0)
    {
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["ERROR_PlayerNotAlive"]);
            return;
        }
        if (target != caller)
        {
            if (AdminApi.HasMoreImmunity(target.GetSteamId(), caller.GetSteamId()))
            {
                AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerHaveBiggerImmunity"]);
                return;
            }
        }
        AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_Slap"].Value.Replace("{name}", target.PlayerName));
        target.Slap(damage);
    }

    private void OnTeleportCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        string index;
        var target = caller;
        if (args.Count == 1) {
            index = args[0]; 
        }
        else {
            index = args[1];
            target = Extensions.GetPlayerFromArg(args[0]);
        }
        Teleport(caller, target, index);
    }
    public void Teleport(CCSPlayerController caller, CCSPlayerController? target, string index)
    {
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (target != caller)
        {
            if (AdminApi.HasMoreImmunity(target.GetSteamId(), caller.GetSteamId()))
            {
                AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerHaveBiggerImmunity"]);
                return;
            }
        }
        var position = GetPosition(caller, index);
        if (position == null)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["ERROR_PositionWithIndexNotFound"]);
            return;
        }

        target.PlayerPawn.Value!.Teleport(position.Position, target.PlayerPawn.Value.AbsRotation);

        if (target == caller)
            AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_Teleported"].Value.Replace("{index}", index));
        else AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_TargetTeleported"].Value
        .Replace("{name}", target.PlayerName)
        .Replace("{index}", index)
        );
    }
    private void OnSavePosCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var index = args[0];
        SavePos(caller, index);
    }

    #region Functions
    public void SavePos(CCSPlayerController caller, string index)
    {
        var existingPosition = GetPosition(caller, index);
        var playerPos = caller.PlayerPawn.Value!.AbsOrigin!;
        var vector = new Vector(playerPos.X, playerPos.Y, playerPos.Z);
        var newPosition = new PositionModel(index, vector, caller.AuthorizedSteamID!.SteamId64);
        if (existingPosition == null)
        {
            SavedPositions.Add(newPosition);
            AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_PostionSaved"].Value.Replace("{index}", index));
            return;
        }
        SavedPositions.Remove(existingPosition);
        SavedPositions.Add(newPosition);
        AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_PostionReSaved"].Value.Replace("{index}", index));
    }
    public PositionModel? GetPosition(CCSPlayerController caller, string index)
    {
        var existingPosition = SavedPositions.FirstOrDefault(x => x.Index == index && x.SteamId == caller.AuthorizedSteamID!.SteamId64);
        return existingPosition;
    }
    public List<PositionModel> GetPositions(CCSPlayerController caller)
    {
        var existingPositions = SavedPositions.Where(x => x.SteamId == caller.AuthorizedSteamID!.SteamId64).ToList();
        return existingPositions;
    }

    #endregion
    
}
