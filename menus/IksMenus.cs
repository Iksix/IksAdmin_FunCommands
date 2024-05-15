using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;
public class IksMenus
{
    public IIksAdminApi AdminApi = IksAdmin_FunCommands.AdminApi!;
    public IStringLocalizer Localizer = IksAdmin_FunCommands.GlobalLocalizer!;
    public IksCommands Commands;
    public IksMenus(IksCommands commands)
    {
        Commands = commands;

        AdminApi.OnMenuOpen += OnManagePlayersMenuOpen;
    }

    private void OnManagePlayersMenuOpen(string key, IMenu menu, CCSPlayerController caller)
    {
        if (key != "ManagePlayers") return;
        // SavePos
        if (AdminApi.HasPermisions(caller.GetSteamId(), "savepos", "d"))
            menu.AddMenuOption(Localizer["MENUOPTION_SavePos"], (_, _) => {
                MenuManager.CloseActiveMenu(caller);
                AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_WritePositionIndex"]);
                AdminApi.NextCommandAction.Add(caller, msg => {
                    Commands.SavePos(caller, msg);
                });
            });
        // Teleport
        if (AdminApi.HasPermisions(caller.GetSteamId(), "teleport", "d"))
            menu.AddMenuOption(Localizer["MENUOPTION_Teleport"], (_, _) => {
                OpenTeleportMenu(caller, menu);
            });
        // Slap
        if (AdminApi.HasPermisions(caller.GetSteamId(), "slap", "s"))
            menu.AddMenuOption(Localizer["MENUOPTION_Slap"], (_, _) => {
                OpenSlapMenu(caller, menu);
            });
    }

    private void OpenSlapMenu(CCSPlayerController caller, IMenu backmenu)
    {
        OpenSelectPlayerMenu(caller, (target, playerMenu) => {
            var dmgMenu = AdminApi.CreateMenu((_, _, newMenu) =>
            {
                newMenu.AddMenuOption("0" + Localizer["OTHER_Hp"], (_, _) =>
                {
                    Commands.Slap(caller, target);
                });
                newMenu.AddMenuOption("5" + Localizer["OTHER_Hp"], (_, _) =>
                {
                    Commands.Slap(caller, target, 5);
                });
                newMenu.AddMenuOption("10" + Localizer["OTHER_Hp"], (_, _) =>
                {
                    Commands.Slap(caller, target, 10);
                });
                newMenu.AddMenuOption("20" + Localizer["OTHER_Hp"], (_, _) =>
                {
                    Commands.Slap(caller, target, 20);
                });
                newMenu.AddMenuOption("30" + Localizer["OTHER_Hp"], (_, _) =>
                {
                    Commands.Slap(caller, target, 30);
                });
                newMenu.AddMenuOption("40" + Localizer["OTHER_Hp"], (_, _) =>
                {
                    Commands.Slap(caller, target, 40);
                });
                newMenu.AddMenuOption("50" + Localizer["OTHER_Hp"], (_, _) =>
                {
                    Commands.Slap(caller, target, 50);
                });
            });
            dmgMenu.Open(caller, Localizer["MENUTITLE_SelectDamage"], playerMenu);
        }, onlyAlive: true, backmenu: backmenu);
    }

    private void OpenTeleportMenu(CCSPlayerController caller, IMenu backmenu)
    {
        OpenSelectPlayerMenu(caller, (target, playerMenu) => {
            var posMenu = AdminApi.CreateMenu((_, _, newMenu) => {
                    PosMenu(caller, target, newMenu);
                });
            posMenu.Open(caller, AdminApi.Localizer["MENUTITLE_SelectPlayer"], playerMenu);
        }, onlyAlive: true, backmenu: backmenu);
    }


    private void OpenSelectPlayerMenu(CCSPlayerController caller, Action<CCSPlayerController, IMenu> onSelect, bool onlyAlive = false, bool withBots = false, IMenu? backmenu = null)
    {
        var menu = AdminApi.CreateMenu((_, _, menu) => {
            var players = Utilities.GetPlayers().Where(x => x.IsValid && x.Connected == PlayerConnectedState.PlayerConnected).ToList();
            foreach (var player in players.ToList())
            {
                if (onlyAlive && !player.PawnIsAlive)
                {
                    players.Remove(player);
                    return;
                }
                if (!withBots && player.IsBot)
                {
                    players.Remove(player);
                }
                    
            }
            foreach (var player in players)
            {
                if (player != caller)
                {
                    if (!player.IsBot)
                    {
                        if (AdminApi.HasMoreImmunity(player.GetSteamId(), caller.GetSteamId()))
                            continue;
                    }
                }
                menu.AddMenuOption(player.PlayerName, (_, _) => {
                    onSelect.Invoke(player, menu);
                });
            }
        });
        menu.Open(caller, AdminApi.Localizer["MENUTITLE_SelectPlayer"], backmenu);
    }

    private void PosMenu(CCSPlayerController caller, CCSPlayerController target, IMenu menu)
    {
        var savedPositions = Commands.GetPositions(caller);
        foreach (var position in savedPositions)
        {
            menu.AddMenuOption(position.Index, (_, _) => {
                Commands.Teleport(caller, target, position.Index);
            });
        }
    }
}
