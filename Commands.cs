using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public class Commands
{
    public IIksAdminApi AdminApi = IksAdmin_FunCommands.AdminApi!;
    public IStringLocalizer Localizer = IksAdmin_FunCommands.GlobalLocalizer!;
    public List<PositionModel> SavedPositions = new();
    public Commands()
    {
        AdminApi.AddNewCommand(
            "hp",
            "set player hp",
            "css_hp <#sid/#uid/name> <hp>",
            2,
            "set_hp",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnHPCommand
        );  
        AdminApi.AddNewCommand(
            "speed",
            "set player speed",
            "css_speed <#sid/#uid/name> <speed>",
            2,
            "set_speed",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnSpeedCommand
        );  
        AdminApi.AddNewCommand(
            "setmoney",
            "set player money",
            "css_setmoney <#sid/#uid/name> <money>",
            2,
            "set_money",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnSetMoneyCommand
        );  
        AdminApi.AddNewCommand(
            "gravity",
            "set player gravity",
            "css_gravity <#sid/#uid/name> <gravity>",
            2,
            "set_gravity",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnGravityCommand
        );  
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
            "slap",
            "slap the player",
            "css_slap <#uid/#sid/name> <damage> <force>",
            1,
            "slap",
            "s",
            CommandUsage.CLIENT_ONLY,
            OnSlapCommand
        );
        AdminApi.AddNewCommand(
            "isay",
            "send image to center for all players",
            "css_isay <\"img link\">",
            1,
            "isay",
            "s",
            CommandUsage.CLIENT_AND_SERVER,
            OnISayCommand
        );
        AdminApi.AddNewCommand(
            "invisible",
            "send image to center for all players",
            "css_invisible <#uid/#sid/name> <0 - off/1 - on>",
            2,
            "invisible",
            "d",
            CommandUsage.CLIENT_AND_SERVER,
            OnInvisibleCommand
        );
        AdminApi.AddNewCommand(
            "scale",
            "change player scale",
            "css_scale <#uid/#sid/name> <scale>",
            2,
            "scale",
            "d",
            CommandUsage.CLIENT_AND_SERVER,
            OnScaleCommand
        );
    }

    private void OnInvisibleCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        var status = int.Parse(args[1]);
        var identity = args[0];
        switch (identity)
        {
            case "@ct":
                Extensions.DoForCt(player => { Invisible(caller, player, status); });
                return;
            case "@t":
                Extensions.DoForT(player => { Invisible(caller, player, status); });
                return;
            case "@all":
                Extensions.DoForAll(player => { Invisible(caller, player, status); });
                return;
            case "@spec":
                Extensions.DoForSpec(player => { Invisible(caller, player, status); });
                return;
        }
        Invisible(caller, target, status);
    }

    private void OnISayCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var link = args[0];
        var lastFormat = link.Split(".").Last();
        if (lastFormat is not "png" or "jpg" or "jpeg" or "gif")
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["ERROR_FormatException"]);
        }
        var message = @$"<img src='{link}'>";
        IksAdmin_FunCommands.PrintHtmlToAll(message); 
    }

    private void OnScaleCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        var identity = args[0];
        var scale = float.Parse(args[1]);
        switch (identity)
        {
            case "@ct":
                Extensions.DoForCt(player => { SetScale(caller, player, scale); });
                return;
            case "@t":
                Extensions.DoForT(player => { SetScale(caller, player, scale); });
                return;
            case "@all":
                Extensions.DoForAll(player => { SetScale(caller, player, scale); });
                return;
            case "@spec":
                Extensions.DoForSpec(player => { SetScale(caller, player, scale); });
                return;
        }
        SetScale(caller, target, scale);
    }

    private void OnSlapCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        var damage = 0;
        var force = 1;
        if (args.Count == 1)
        {
            Slap(caller, target);
        }
        else if (args.Count == 2)
        {
            damage = int.Parse(args[1]);
        }
        else if (args.Count == 3)
        {
            damage = int.Parse(args[1]);
            force = int.Parse(args[2]);
        }

        var identity = args[0];
        switch (identity)
        {
            case "@ct":
                Extensions.DoForCt(player => { Slap(caller, player, damage, force); });
                return;
            case "@t":
                Extensions.DoForT(player => { Slap(caller, player, damage, force); });
                return;
            case "@all":
                Extensions.DoForAll(player => { Slap(caller, player, damage, force); });
                return;
            case "@spec":
                Extensions.DoForSpec(player => { Slap(caller, player, damage, force); });
                return;
        }
        Slap(caller, target, damage, force);
    }

    private void OnGravityCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var identity = args[0];
        switch (identity)
        {
            case "@ct":
                Extensions.DoForCt(player => { player.SetGravity(caller, float.Parse(args[1])); });
                return;
            case "@t":
                Extensions.DoForT(player => { player.SetGravity(caller, float.Parse(args[1])); });
                return;
            case "@all":
                Extensions.DoForAll(player => { player.SetGravity(caller, float.Parse(args[1])); });
                return;
            case "@spec":
                Extensions.DoForSpec(player => { player.SetGravity(caller, float.Parse(args[1])); });
                return;
        }
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
            return;
        }
        target.SetGravity(caller, float.Parse(args[1]));
    }

    private void OnSetMoneyCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        target.SetMoney(int.Parse(args[1]));
    }

    private void OnSpeedCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var identity = args[0];
        switch (identity)
        {
            case "@ct":
                Extensions.DoForCt(player => { Extensions.SetSpeed(caller, player, int.Parse(args[1])); });
                return;
            case "@t":
                Extensions.DoForT(player => { Extensions.SetSpeed(caller, player, int.Parse(args[1])); });
                return;
            case "@all":
                Extensions.DoForAll(player => { Extensions.SetSpeed(caller, player, int.Parse(args[1])); });
                return;
            case "@spec":
                Extensions.DoForSpec(player => { Extensions.SetSpeed(caller, player, int.Parse(args[1])); });
                return;
        }
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
            return;
        }
        Extensions.SetSpeed(caller, target, int.Parse(args[1]));
    }

    private void OnHPCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var identity = args[0];
        switch (identity)
        {
            case "@ct":
                Extensions.DoForCt(player => { Extensions.Hp(caller, player, int.Parse(args[1])); });
                return;
            case "@t":
                Extensions.DoForT(player => { Extensions.Hp(caller, player, int.Parse(args[1])); });
                return;
            case "@all":
                Extensions.DoForAll(player => { Extensions.Hp(caller, player, int.Parse(args[1])); });
                return;
            case "@spec":
                Extensions.DoForSpec(player => { Extensions.Hp(caller, player, int.Parse(args[1])); });
                return;
        }
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
            return;
        }
        Extensions.Hp(caller, target, int.Parse(args[1]));
    }
    
    private void OnTeleportCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        string index = args[0];
        var target = caller;
        if (args.Count == 1) {
            index = args[0]; 
        }
        else if (args.Count == 2) {
            index = args[1];
            var identity = args[0];
            switch (identity)
            {
                case "@ct":
                    Extensions.DoForCt(player => { Teleport(caller, player, index); });
                    return;
                case "@t":
                    Extensions.DoForT(player => { Teleport(caller, player, index); });
                    return;
                case "@all":
                    Extensions.DoForAll(player => { Teleport(caller, player, index); });
                    return;
                case "@spec":
                    Extensions.DoForSpec(player => { Teleport(caller, player, index); });
                    return;
            }
            target = Extensions.GetPlayerFromArg(identity);
        }
        Teleport(caller, target, index);
    }
    
    private void OnSavePosCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo _)
    {
        var index = args[0];
        SavePos(caller, index);
    }
    
    #region Functions
    public void Slap(CCSPlayerController caller, CCSPlayerController? target, int damage = 0, int force = 1)
    {
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
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
        target.Slap(damage, force);
    }

    public void Invisible(CCSPlayerController caller, CCSPlayerController? target, int status)
    {
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
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
        if (status == 0)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_InvisibilityOff"].Value.Replace("{name}", target.PlayerName));
            target.SetPlayerVisible();
        }
        else {
            AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_InvisibilityOn"].Value.Replace("{name}", target.PlayerName));
            target.SetPlayerInvisible();
        }
        
    }

    public void SetScale(CCSPlayerController caller, CCSPlayerController? target, float scale)
    {
        if (scale < 0.1f)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["ERROR_VerySmallArgument"]);
            return;
        }
        if (scale > 50)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["ERROR_VeryLargeArgument"]);
            return;
        }
        if (target == null)
        {
            AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            AdminApi.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
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
        target.SetScale(scale);
        AdminApi.SendMessageToPlayer(caller, AdminApi.Localizer["NOTIFY_Scale"].Value.Replace("{name}", target.PlayerName));
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
        var playerPos = caller.Pawn.Value!.AbsOrigin;
        var vector = new Vector(playerPos!.X, playerPos.Y, playerPos.Z);
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
