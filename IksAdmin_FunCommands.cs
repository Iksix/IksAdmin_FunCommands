using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using IksAdminApi;

namespace IksAdmin_FunCommands;

public class IksAdmin_FunCommands : BasePlugin
{
    public override string ModuleName => "IksAdmin_FunCommands";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "iks__ && quiz__";

    private readonly PluginCapability<IIksAdminApi> _pluginCapability = new("iksadmin:core");
    public static IIksAdminApi? AdminApi;
    public override void OnAllPluginsLoaded(bool hotReload)
    {
        AdminApi = _pluginCapability.Get();
    }

    
}
