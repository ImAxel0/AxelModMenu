using Sons.Multiplayer;
using SonsSdk;
using SUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxelModMenu;

public class MultiplayerSettings
{
    public static Observable<string> PlayerLimitBuffer = new("");

    public static void SetPlayerLimit()
    {
        if (BoltNetwork.isClient || !BoltNetwork.isRunning)
        {
            SonsTools.ShowMessage("<color=red>Error</color>, gamemode is Singleplayer or you are not the host");
            return;
        }

        if (!int.TryParse(PlayerLimitBuffer.Value.Replace(" ", ""), out var value))
        {
            SonsTools.ShowMessage("Invalid player limit parameter");
            return;
        }
        CoopLobbyManager.GetActiveInstance().SetMemberLimit(value);
    }
}
