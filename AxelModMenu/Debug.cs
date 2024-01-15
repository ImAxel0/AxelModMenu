using RedLoader.Utils;
using SonsAxLib;
using SonsSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AxelModMenu;

public class Debug
{
    public static void DumpItemsId()
    {
        if (ItemIdManager.DumpAll()) 
            SonsTools.ShowMessageBox("Info", $"An itemsid.txt file has been created at " +
                $"{LoaderEnvironment.GameExecutablePath}");
        else SonsTools.ShowMessageBox("Error", "Couldn't create itemsid.txt file");
    }
}
