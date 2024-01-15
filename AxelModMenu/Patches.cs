using SonsSdk;

namespace AxelModMenu;

public class Patches
{
    public static void Init()
    {      
        SdkEvents.OnInWorldUpdate.Subscribe(AxelModMenu.Update);
        SdkEvents.OnInWorldUpdate.Subscribe(WorldEdit.WorldEditUpdate);
        SdkEvents.OnInWorldUpdate.Subscribe(ModdedItems._lighterMods);
        SdkEvents.OnInWorldUpdate.Subscribe(ModdedItems._flashlightMods);
        SdkEvents.OnInWorldUpdate.Subscribe(ModdedItems._rebreatherMods);
        SdkEvents.OnInWorldUpdate.Subscribe(ModdedItems._ropegunMods);
    }

    public static bool InfAmmoPatch()
    {
        return false;
    }
}
