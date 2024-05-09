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
public class QuizMenus
{
    public IIksAdminApi AdminApi = IksAdmin_FunCommands.AdminApi!;
    public IStringLocalizer Localizer = IksAdmin_FunCommands.GlobalLocalizer!;
    public QuizCommands Commands;
    public QuizMenus(QuizCommands commands)
    {
        Commands = commands;

        AdminApi.OnMenuOpen += OnManagePlayersMenuOpen;
    }

    private void OnManagePlayersMenuOpen(string key, IMenu menu, CCSPlayerController caller)
    {
        if (key != "ManagePlayers") return;
        if (AdminApi.HasPermisions(caller.GetSteamId(), "set_hp", "b"))
            menu.AddMenuOption(Localizer["MENUOPTION_Hp"], (_, _) => {
                OpenSelectPlayerMenu(caller, target => {
                    MenuManager.CloseActiveMenu(caller);
                    AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_PrintHp"]);
                    AdminApi.NextCommandAction.Add(caller, msg => {
                        target.Hp(int.Parse(msg));
                        AdminApi.SendMessageToPlayer(caller, Localizer["NOTIFY_HpSetted"]);
                    }
                    );
                }
                );
            });
    }


    private void OpenSelectPlayerMenu(CCSPlayerController caller, Action<CCSPlayerController> OnSelect, bool onlyAlive = false, bool withBots = false)
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
                    OnSelect.Invoke(player);
                });
            }
        });
        menu.Open(caller, AdminApi.Localizer["MENUTITLE_SelectPlayer"]);
    }

    
            
        
    
}
