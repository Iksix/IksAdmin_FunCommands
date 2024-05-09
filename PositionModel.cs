using CounterStrikeSharp.API.Modules.Utils;

namespace IksAdmin_FunCommands;

public class PositionModel
{
    public string Index;
    public Vector Position;
    public ulong SteamId;
    public PositionModel(string index, Vector position, ulong steamId)
    {
        Index = index;
        Position = position;
        SteamId = steamId;
    }
}