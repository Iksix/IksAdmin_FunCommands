using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public class QuizCommands
{
    public IStringLocalizer Localizer = IksAdmin_FunCommands.GlobalLocalizer!;
    public IIksAdminApi _api = IksAdmin_FunCommands.AdminApi!;
    public QuizCommands(bool hotReload)
    {
        _api!.AddNewCommand(
            "hp",
            "set player hp",
            "css_hp <#sid/#uid/name> <hp>",
            2,
            "set_hp",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnHPCommand
        );  
        _api!.AddNewCommand(
            "speed",
            "set player speed",
            "css_speed <#sid/#uid/name> <speed>",
            2,
            "set_speed",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnSpeedCommand
        );  
        _api!.AddNewCommand(
            "setmoney",
            "set player money",
            "css_setmoney <#sid/#uid/name> <money>",
            2,
            "set_money",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnSetMoneyCommand
        );  
        _api!.AddNewCommand(
            "gravity",
            "set player gravity",
            "css_hp <#sid/#uid/name> <gravity>",
            2,
            "set_gravity",
            "b",
            CommandUsage.CLIENT_ONLY,
            OnGravityCommand
        );  
    }

    private void OnGravityCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            _api.SendMessageToPlayer(caller, _api.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            _api.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
            return;
        }
        target.SetGravity(int.Parse(args[1]));
    }

    private void OnSetMoneyCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            _api.SendMessageToPlayer(caller, _api.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        target.SetMoney(int.Parse(args[1]));
    }

    private void OnSpeedCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            _api.SendMessageToPlayer(caller, _api.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            _api.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
            return;
        }
        target.SetSpeed(int.Parse(args[1]));
    }

    private void OnHPCommand(CCSPlayerController caller, Admin? admin, List<string> args, CommandInfo info)
    {
        var target = Extensions.GetPlayerFromArg(args[0]);
        if (target == null)
        {
            _api.SendMessageToPlayer(caller, _api.Localizer["NOTIFY_PlayerNotFound"]);
            return;
        }
        if (!target.PawnIsAlive)
        {
            _api.SendMessageToPlayer(caller, Localizer["ERROR_PlayerNotAlive"]);
            return;
        }
        target.Hp(int.Parse(args[1]));
    }
}
