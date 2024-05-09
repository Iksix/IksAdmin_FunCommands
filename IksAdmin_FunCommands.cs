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
    public override void OnAllPluginsLoaded(bool hotReload)
    {
        AdminApi = _pluginCapability.Get();
        GlobalLocalizer = Localizer;
        var iksCommands = new IksCommands(hotReload);
        var iksMenus = new IksMenus(iksCommands);
        var quizCommands = new QuizCommands(hotReload);
        var quizMenus = new QuizMenus(quizCommands);

    }

    
}
