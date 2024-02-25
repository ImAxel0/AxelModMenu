using SonsSdk;
using UnityEngine;
using SUI;
using static SUI.SUI;
using RedLoader.Utils;
using TheForest.Utils;
using System.Reflection;
using SonsNetwork;
using TheForest.UI.Multiplayer;

namespace AxelModMenu;

public class AxelModMenu : SonsMod
{
    public static HarmonyLib.Harmony Harmony;
    public static bool _showMenu;
    static bool _firstStart = true;

    public AxelModMenu()
    {
    }

    protected override void OnInitializeMod()
    {
        Config.Init();
    }

    protected override void OnSdkInitialized()
    {
        SettingsRegistry.CreateSettings(this, null, typeof(Config), false);
        SoundTools.RegisterSound("uierror", Path.Combine(LoaderEnvironment.ModsDirectory, @"AxelModMenu\uierror.flac"));
        SoundTools.RegisterSound("uisound", Path.Combine(LoaderEnvironment.ModsDirectory, @"AxelModMenu\uisound.wav"));
        Harmony = HarmonyInstance;
    }

    public static bool TryGetEmbeddedResourceBytes(string name, out byte[] bytes)
    {
        bytes = null;

        var executingAssembly = Assembly.GetExecutingAssembly();

        var desiredManifestResources = executingAssembly.GetManifestResourceNames().FirstOrDefault(resourceName => {
            var assemblyName = executingAssembly.GetName().Name;
            return !string.IsNullOrEmpty(assemblyName) && resourceName.StartsWith(assemblyName) && resourceName.Contains(name);
        });

        if (string.IsNullOrEmpty(desiredManifestResources))
            return false;

        using (var ms = new MemoryStream())
        {
            executingAssembly.GetManifestResourceStream(desiredManifestResources).CopyTo(ms);
            bytes =  ms.ToArray();
            return true;
        }
    }

    protected override void OnGameStart()
    {
        _firstStart = false;
        Config.GetSavedSettings();
        Patches.Init();
        WorldEdit.Init();
        AxelModMenuUi.Create();
    }

    protected override void OnSonsSceneInitialized(ESonsScene sonsScene)
    {
        if (sonsScene == ESonsScene.Title)
        {
            if (!_firstStart) 
                SonsTools.ShowMessageBox("Warning", 
                    "Axel's mod menu requires a game restart when switching savegame");

            HarmonyInstance.UnpatchSelf();
            SdkEvents.OnInWorldUpdate.UnsubscribeAll();
            Cleanup();
        }
    }

    public static void Cleanup()
    {
        AxelModMenuUi.RunCleanup();
        ModdedItems.RunCleanup();
    }

    public static void Update()
    {
        if (Teleport.ShowMap.Value) Teleport.UpdateDotPosition();   
        PlayerSettings.Update();
    }

    public static void ToggleMenu()
    {         
        if (BoltNetwork.isRunning)
        {
            if (ChatBox.IsChatOpen) return;
        }

        _showMenu = !_showMenu;
        TogglePanel(AxelModMenuUi.PanelSelector, _showMenu);
        TogglePanel(AxelModMenuUi.PlayerPanel, _showMenu);
        TogglePanel(AxelModMenuUi.SwitchablePanels[AxelModMenuUi.CurrentPanel], _showMenu);
        if (!_showMenu && Config.ShouldSaveSettings.Value) Config.SaveSettings();
    }
}