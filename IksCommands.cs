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
    public IksCommands(bool hotReload)
    {
        AdminApi!.AddNewCommand(
            "savepos",
            "Save player position",
            "css_savepos <position index>",
            1,
            "savepos",
            "d",
            CommandUsage.CLIENT_ONLY,
            OnSavePosCommand
        );
        AdminApi!.AddNewCommand(
            "teleport",
            "teleport to saved location",
            "css_teleport <index>\ncss_teleport <#sid/#uid/name> <index>",
            1,
            "teleport",
            "d",
            CommandUsage.CLIENT_ONLY,
            OnTeleportCommand
        );
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
    private void OnSavePosCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var index = args[0];
        SavePos(caller, index);
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
}
