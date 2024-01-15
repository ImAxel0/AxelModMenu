using RedLoader;
using RedLoader.Utils;
using Sons.Atmosphere;
using Sons.Environment;
using Sons.Gameplay;
using SonsSdk;
using SUI;
using System.Collections;
using TheForest.Utils;
using UnityEngine;

namespace AxelModMenu;

public static class Config
{
    public static ConfigCategory Category { get; private set; }

    public static KeybindConfigEntry OpenKey { get; private set; }
    public static KeybindConfigEntry NoClipKey { get; private set; }
    public static KeybindConfigEntry MapKey { get; private set; }

    public static ConfigEntry<bool> ShouldSaveSettings { get; private set; }

    // PLAYER
    public static ConfigEntry<bool> IsGodMode { get; private set; }
    public static ConfigEntry<bool> IsInfStamina { get; private set; }
    public static ConfigEntry<bool> IsNoHungry { get; private set; }
    public static ConfigEntry<bool> IsNoDehydration { get; private set; }
    public static ConfigEntry<bool> IsNoSleep { get; private set; }
    public static ConfigEntry<bool> IsInfiniteAmmo { get; private set; }
    public static ConfigEntry<bool> IsNoFallDamage { get; private set; }
    public static ConfigEntry<bool> IsNoClip { get; private set; }
    public static ConfigEntry<float> NoClipSpeed { get; private set; }
    public static ConfigEntry<float> NoClipUpDownSpeed { get; private set; }
    public static ConfigEntry<float> WalkSpeed { get; private set; }
    public static ConfigEntry<float> RunSpeed { get; private set; }
    public static ConfigEntry<float> SwimSpeed { get; private set; }
    public static ConfigEntry<float> JumpMultiplier { get; private set; }

    // ENVIRONMENT
    public static ConfigEntry<bool> IsFreezeLakes { get; private set; }
    public static ConfigEntry<bool> IsNoLakes { get; private set; }
    public static ConfigEntry<bool> IsNoOcean { get; private set; }
    public static ConfigEntry<bool> IsNoWaterfalls { get; private set; }
    public static ConfigEntry<bool> IsNoGravity { get; private set; }
    public static ConfigEntry<int> WorldEditUpDownSpeed { get; private set; }
    public static ConfigEntry<int> WorldEditRotationSpeed { get; private set; }
    public static ConfigEntry<bool> IsLockTime { get; private set; }
    public static ConfigEntry<float> TreeRegrowRate { get; private set; }

    // MODDED ITEMS
    public static ConfigEntry<bool> IsPistolRapidFire { get; private set; }
    public static ConfigEntry<bool> IsShotgunRapidFire { get; private set; }
    public static ConfigEntry<bool> IsRevolverRapidFire { get; private set; }
    public static ConfigEntry<bool> IsRifleRapidFire { get; private set; }
    public static ConfigEntry<float> KnightSpeed { get; private set; }
    public static ConfigEntry<float> KnightJumpForce { get; private set; }
    public static ConfigEntry<float> HangGliderSpeed { get; private set; }
    public static ConfigEntry<bool> HangGliderNoDownforce { get; private set; }
    public static ConfigEntry<float> LighterIntensity { get; private set; }
    public static ConfigEntry<bool> LighterIncRange { get; private set; }
    public static ConfigEntry<float> FlashlightIntensity { get; private set; }
    public static ConfigEntry<bool> FlashlightNoDrain { get; private set; }
    public static ConfigEntry<float> RebreatherIntensity { get; private set; }
    public static ConfigEntry<bool> RebreatherNoOxigen { get; private set; }
    public static ConfigEntry<bool> IsRopegunInfinite { get; private set; }

    public static void Init()
    {
        Category = ConfigSystem.CreateFileCategory("AxelModMenu", "AxelModMenu", "AxelModMenu.cfg");

        OpenKey = Category.CreateKeybindEntry("OpenKey", "insert", "Open Key");
        OpenKey.Notify(AxelModMenu.ToggleMenu);

        ShouldSaveSettings = Category.CreateEntry("ShouldSaveSettings", false, "ShouldSaveSettings", "Save settings upon game restart", true);

        NoClipKey = Category.CreateKeybindEntry("NoClipKey", "v", "NoClip Key", "NoClip key");
        NoClipKey.Notify(PlayerSettings.NoClipToggleKey);

        MapKey = Category.CreateKeybindEntry("MapKey", "m", "Map Key", "Map key");
        MapKey.Notify(Teleport.ToggleInteractiveMap);
    }

    public static void GetSavedSettings()
    {
        // PLAYER       
        IsGodMode = Category.CreateEntry("GodMode", false, "GodMode", "", true);
        IsInfStamina = Category.CreateEntry("InfiniteStamina", false, "InfiniteStamina", "", true);
        IsNoHungry = Category.CreateEntry("NoHungry", false, "NoHungry", "", true);
        IsNoDehydration = Category.CreateEntry("NoDehydration", false, "NoDehydration", "", true);
        IsNoSleep = Category.CreateEntry("NoSleep", false, "NoSleep", "", true);
        IsInfiniteAmmo = Category.CreateEntry("InfiniteAmmo", false, "InfiniteAmmo", "", true);
        IsNoFallDamage = Category.CreateEntry("NoFallDamage", false, "NoFallDamage", "", true);
        IsNoClip = Category.CreateEntry("IsNoClip", false, "IsNoClip", "", true);
        NoClipSpeed = Category.CreateEntry("NoClipSpeed", PlayerSettings.NoClipData.Speed.Value, "NoClipSpeed", "", true);
        NoClipUpDownSpeed = Category.CreateEntry("NoClipUpDownSpeed", PlayerSettings.NoClipData.UpDownSpeed.Value, "NoClipUpDownSpeed", "", true);
        WalkSpeed = Category.CreateEntry("WalkSpeed", LocalPlayer.FpCharacter.WalkSpeed, "WalkSpeed", "", true);
        RunSpeed = Category.CreateEntry("RunSpeed", LocalPlayer.FpCharacter.RunSpeed, "RunSpeed", "", true);
        SwimSpeed = Category.CreateEntry("SwimSpeed", LocalPlayer.FpCharacter.SwimSpeed, "SwimSpeed", "", true);
        JumpMultiplier = Category.CreateEntry("JumpMultiplier", LocalPlayer.FpCharacter.JumpMultiplier, "JumpMultiplier", "", true);

        // ENVIRONMENT
        IsFreezeLakes = Category.CreateEntry("IsFreezeLakes", false, "IsFreezeLakes", "", true);
        IsNoLakes = Category.CreateEntry("IsNoLakes", false, "IsNoLakes", "", true);
        IsNoOcean = Category.CreateEntry("IsNoOcean", false, "IsNoOcean", "", true);
        IsNoWaterfalls = Category.CreateEntry("IsNoWaterfalls", false, "IsNoWaterfalls", "", true);
        IsNoGravity = Category.CreateEntry("IsNoGravity", false, "IsNoGravity", "", true);
        WorldEditUpDownSpeed = Category.CreateEntry("WorldEditUpDownSpeed", 2, "WorldEditUpDownSpeed", "", true);
        WorldEditRotationSpeed = Category.CreateEntry("WorldEditRotationSpeed", 10, "WorldEditRotationSpeed", "", true);
        IsLockTime = Category.CreateEntry("IsLockTime", false, "IsLockTime", "", true);
        TreeRegrowRate = Category.CreateEntry("TreeRegrowRate", UnityEngine.Object.FindObjectOfType<TreeRegrowChecker>()._regrowthFactor, "TreeRegrowRate", "", true);

        // MODDED ITEMS
        IsPistolRapidFire = Category.CreateEntry("IsPistolRapidFire", false, "IsPistolRapidFire", "", true);
        IsShotgunRapidFire = Category.CreateEntry("IsShotgunRapidFire", false, "IsShotgunRapidFire", "", true);
        IsRevolverRapidFire = Category.CreateEntry("IsRevolverRapidFire", false, "IsRevolverRapidFire", "", true);
        IsRifleRapidFire = Category.CreateEntry("IsRifleRapidFire", false, "IsRifleRapidFire", "", true);
        KnightSpeed = Category.CreateEntry("KnightSpeed", LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerKnightVAction>()._controlDefinition.MaxVelocity, "KnightSpeed", "", true);
        KnightJumpForce = Category.CreateEntry("KnightJumpForce", LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerKnightVAction>()._controlDefinition.JumpForce, "KnightJumpForce", "", true);
        HangGliderSpeed = Category.CreateEntry("HangGliderSpeed", LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._constantForwardForce, "HangGliderSpeed", "", true);
        HangGliderNoDownforce = Category.CreateEntry("HangGliderNoDownforce", false, "HangGliderNoDownforce", "", true);
        LighterIntensity = Category.CreateEntry("LighterIntensity", -1f, "LighterIntensity", "", true);
        LighterIncRange = Category.CreateEntry("LighterIncRange", false, "LighterIncRange", "", true);
        FlashlightIntensity = Category.CreateEntry("FlashlightIntensity", -1f, "FlashlightIntensity", "", true);
        FlashlightNoDrain = Category.CreateEntry("FlashlightNoDrain", false, "FlashlightNoDrain", "", true);
        RebreatherIntensity = Category.CreateEntry("RebreatherIntensity", -1f, "RebreatherIntensity", "", true);
        RebreatherNoOxigen = Category.CreateEntry("RebreatherNoOxigen", false, "RebreatherNoOxigen", "", true);
        IsRopegunInfinite = Category.CreateEntry("IsRopegunInfinite", false, "IsRopegunInfinite", "", true);

        if (ShouldSaveSettings.Value) LoadSavedSettigs();
        else
        {
            foreach (var entry in Category.Entries)
            {
                if (entry == OpenKey || entry == ShouldSaveSettings)
                    continue;

                entry.ResetToDefault();
            }
        }
    }

    private static void LoadSavedSettigs()
    {
        // PLAYER
        PlayerSettings.GodMode(IsGodMode.Value);
        PlayerSettings.InfStamina(IsInfStamina.Value);
        PlayerSettings.NoHungry(IsNoHungry.Value);
        PlayerSettings.NoDehydration(IsNoDehydration.Value);
        PlayerSettings.NoSleep(IsNoSleep.Value);
        PlayerSettings.InfAmmo(IsInfiniteAmmo.Value);
        PlayerSettings.NoFallDamage(IsNoFallDamage.Value);
        PlayerSettings.NoClip(IsNoClip.Value);
        PlayerSettings.SetNoClipSpeed(NoClipSpeed.Value);
        PlayerSettings.SetNoClipUpDownSpeed(NoClipUpDownSpeed.Value);
        PlayerSettings.WalkSpeed(WalkSpeed.Value);
        PlayerSettings.RunSpeed(RunSpeed.Value);
        PlayerSettings.SwimSpeed(SwimSpeed.Value);
        PlayerSettings.JumpMultiplier(JumpMultiplier.Value);

        // ENVIRONMENT
        if (IsFreezeLakes.Value) Environment.FreezeLakes(true);
        Environment.NoLakes(IsNoLakes.Value);
        Environment.NoOcean(IsNoOcean.Value);
        Environment.NoWaterfalls(IsNoWaterfalls.Value);
        Environment.NoGravity(IsNoGravity.Value);
        WorldEdit.SetUpDownSpeed(WorldEditUpDownSpeed.Value);
        WorldEdit.SetRotationSpeed(WorldEditRotationSpeed.Value);
        Environment.LockDaytime(IsLockTime.Value);
        Environment.TreeRegrowFactor(TreeRegrowRate.Value);

        // MODDED ITEMS
        ModdedItems.PistolRapidFire(IsPistolRapidFire.Value);
        ModdedItems.ShotgunRapidFire(IsShotgunRapidFire.Value);
        ModdedItems.RevolverRapidFire(IsRevolverRapidFire.Value);
        ModdedItems.RifleRapidFire(IsRifleRapidFire.Value);
        ModdedItems.KnightSpeed(KnightSpeed.Value);
        ModdedItems.KnightJumpForce(KnightJumpForce.Value);
        ModdedItems.GliderSpeed(HangGliderSpeed.Value);
        ModdedItems.GliderNoDownforce(HangGliderNoDownforce.Value);
        ModdedItems.LighterIntensity(LighterIntensity.Value);
        ModdedItems.IncreaseLighterRange(LighterIncRange.Value);
        ModdedItems.FlashlightIntensity(FlashlightIntensity.Value);
        ModdedItems.FlashlightNoDrain(FlashlightNoDrain.Value);
        ModdedItems.RebreatherLight(RebreatherIntensity.Value);
        ModdedItems.RebreatherNoOxigen(RebreatherNoOxigen.Value);
        ModdedItems.RopegunInfinite(IsRopegunInfinite.Value);
    }

    public static void SaveSettings()
    {
        Category.SaveToFile(false);
    }

    public static void _ShouldSaveSettings(bool onoff)
    {
        ShouldSaveSettings.Value = onoff;
        SaveSettings();
    }
}