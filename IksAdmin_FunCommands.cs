using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using IksAdminApi;
using Microsoft.Extensions.Localization;

namespace IksAdmin_FunCommands;

public class IksAdmin_FunCommands : BasePlugin
{
    public override string ModuleName => "IksAdmin_FunCommands";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks__ && quiz__";

    private readonly PluginCapability<IIksAdminApi> _pluginCapability = new("iksadmin:core");
    public static IIksAdminApi? AdminApi;
    public static IStringLocalizer? GlobalLocalizer;
    public static string HTMLMessageForAll = "";
    public static int HTMLMessageTime = 5;

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(() => {
            if (HTMLMessageTime > 0)
            {
                var players = Extensions.GetOnlinePlayers();
                foreach (var player in players)
                {
                    player.PrintToCenterHtml(HTMLMessageForAll);
                }
            }
        });
        AddTimer(1, () => {
            if (HTMLMessageTime > 0)
                HTMLMessageTime--;
        }, CounterStrikeSharp.API.Modules.Timers.TimerFlags.REPEAT);
    }

    public static void PrintHtmlToAll(string message, int time = 5)
    {
        HTMLMessageForAll = message;
        HTMLMessageTime = time;
    }
    public override void OnAllPluginsLoaded(bool hotReload)
    {
        AdminApi = _pluginCapability.Get();
        GlobalLocalizer = Localizer;
        Extensions.Localizer = Localizer;
        var Commands = new Commands();
        var Menus = new Menus(Commands);
    }

    
}
