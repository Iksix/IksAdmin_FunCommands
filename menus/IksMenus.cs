using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }

    private void OpenTeleportMenu(CCSPlayerController caller, IMenu backmenu)
    {
        OpenSelectPlayerMenu(caller, (target, playerMenu) => {
            var posMenu = AdminApi.CreateMenu((caller, admin, newMenu) => {
                    PosMenu(caller, target, newMenu);
                });
            posMenu.Open(caller, AdminApi.Localizer["MENUTITLE_SelectPlayer"], playerMenu);
        }, onlyAlive: true, backmenu: backmenu);
    }


    private void OpenSelectPlayerMenu(CCSPlayerController caller, Action<CCSPlayerController, IMenu> OnSelect, bool onlyAlive = false, bool withBots = false, IMenu? backmenu = null)
    {
        var menu = AdminApi.CreateMenu((_, _, menu) => {
            var players = Utilities.GetPlayers().Where(x => x.IsValid && x.Connected == PlayerConnectedState.PlayerConnected).ToList();
            foreach (var player in players)
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
                    OnSelect.Invoke(player, menu);
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
