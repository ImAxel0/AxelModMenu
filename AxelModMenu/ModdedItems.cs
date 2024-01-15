using HarmonyLib;
using RedLoader;
using Sons.Gameplay;
using Sons.Input;
using Sons.Item;
using Sons.Weapon;
using SonsAxLib;
using SonsSdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheForest.Utils;
using UnityEngine;

namespace AxelModMenu;

public class ModdedItems
{
    public struct Lighter
    {
        public static PlasmaLighterController lighterController;
        public static float Intensity = -1;
        public static float? Def_Intensity = null;
        public static float? Def_Range = null;
        public static bool IncreaseRange;
    }

    public struct Flashlight
    {
        public static FlashlightController flashlightController;
        public static float Intensity = -1;
        public static float? Def_Intensity = null;
        public static float? Def_Drain = null;
        public static bool NoDrain;
    }

    public struct Rebreather
    {
        public static RebreatherController rebreatherController;
        public static float Intensity = -1;
        public static float? Def_Intensity = null;
        public static float? Def_Drain = null;
        public static bool NoDrain;
    }

    public struct Ropegun
    {
        public static RopeGunController ropegunController;
        public static float? Def_Length = null;
        public static float? Def_FiringRange = null;
        public static bool InfiniteLength;
    }

    public static void KnightSpeed(float value)
    {
        Config.KnightSpeed.Value = value;
        LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerKnightVAction>()._controlDefinition.MaxVelocity = value;
    }

    public static void KnightJumpForce(float value)
    {
        Config.KnightJumpForce.Value = value;
        LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerKnightVAction>()._controlDefinition.JumpForce = value;
    }

    public static void GliderSpeed(float value)
    {
        Config.HangGliderSpeed.Value = value;
        LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._constantForwardForce = value;
    }

    static float? _baseDownPitchForceBk;
    static float? _upPitchDownForceBk;
    public static void GliderNoDownforce(bool onoff)
    {
        _baseDownPitchForceBk ??= LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._baseDownPitchForce;
        _upPitchDownForceBk ??= LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._upPitchDownForce;
        Config.HangGliderNoDownforce.Value = onoff;

        if (onoff)
        {
            LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._baseDownPitchForce = 0;
            LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._upPitchDownForce = 0;
            return;
        }
        LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._baseDownPitchForce = (float)_baseDownPitchForceBk;
        LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._upPitchDownForce = (float)_upPitchDownForceBk;
    }

    public static void LighterIntensity(float value)
    {
        Config.LighterIntensity.Value = value;
        Lighter.Intensity = value;
    }

    public static void IncreaseLighterRange(bool onoff)
    {
        Config.LighterIncRange.Value = onoff;
        Lighter.IncreaseRange = onoff;
    }

    public static void _lighterMods()
    {
        if (LocalPlayer.Inventory.LeftHandItem?._itemID == (int)ItemIdManager.ItemsId.PlasmaLighter)
        {
            Lighter.lighterController ??= LocalPlayer.Transform.GetComponentInChildren<PlasmaLighterController>();
            Lighter.Def_Range ??= Lighter.lighterController?._localPlayerLight.range;
            Lighter.Def_Intensity ??= Lighter.lighterController?._localPlayerLight.intensity;

            if (Lighter.Intensity >= 0 && Lighter.lighterController)
            {
                Lighter.lighterController._localPlayerLight.intensity = Lighter.Intensity;
            }
            else if (Lighter.lighterController && Lighter.Def_Intensity != null) Lighter.lighterController._localPlayerLight.intensity = (float)Lighter.Def_Intensity;

            if (Lighter.IncreaseRange && Lighter.lighterController)
            {
                Lighter.lighterController._localPlayerLight.range = 100;
            }
            else if (Lighter.lighterController && Lighter.Def_Range != null) Lighter.lighterController._localPlayerLight.range = (float)Lighter.Def_Range;
        }
        else Lighter.lighterController = null;
    }

    public static void FlashlightIntensity(float value)
    {
        Config.FlashlightIntensity.Value = value;
        Flashlight.Intensity = value;
    }

    public static void FlashlightNoDrain(bool onoff)
    {
        Config.FlashlightNoDrain.Value = onoff;
        Flashlight.NoDrain = onoff;
    }

    public static void _flashlightMods()
    {
        if (LocalPlayer.Inventory.LeftHandItem?._itemID == (int)ItemIdManager.ItemsId.Flashlight)
        {
            Flashlight.flashlightController ??= LocalPlayer.Transform.GetComponentInChildren<FlashlightController>();
            if (Flashlight.flashlightController?._powerDrainRate != 0)
            {
                Flashlight.Def_Drain ??= Flashlight.flashlightController?._powerDrainRate;
            }
            if (Flashlight.flashlightController?._maxLightIntensity != 0)
            {
                Flashlight.Def_Intensity ??= Flashlight.flashlightController?._maxLightIntensity;
            }

            if (Flashlight.Intensity >= 0 && Flashlight.flashlightController)
            {
                Flashlight.flashlightController._maxLightIntensity = Flashlight.Intensity;
            }
            else if (Flashlight.flashlightController && Flashlight.Def_Intensity != null) Flashlight.flashlightController._maxLightIntensity = Flashlight.Def_Intensity.Value;

            if (Flashlight.NoDrain && Flashlight.flashlightController)
            {
                Flashlight.flashlightController._powerDrainRate = 0;
            }
            else if (Flashlight.flashlightController && Flashlight.Def_Drain != null) Flashlight.flashlightController._powerDrainRate = Flashlight.Def_Drain.Value;
        }
        else Flashlight.flashlightController = null;
    }

    public static void RebreatherLight(float value)
    {
        Config.RebreatherIntensity.Value = value;
        Rebreather.Intensity = value;
    }

    public static void RebreatherNoOxigen(bool onoff)
    {
        Config.RebreatherNoOxigen.Value = onoff;
        Rebreather.NoDrain = onoff;
    }

    public static void _rebreatherMods()
    {
        if (LocalPlayer.Inventory.EquippedChestItem?._itemID == (int)ItemIdManager.ItemsId.Rebreather)
        {
            Rebreather.rebreatherController ??= LocalPlayer.Transform.GetComponentInChildren<RebreatherController>();
            if (Rebreather.rebreatherController?._airConsumptionRate != 0)
            {
                Rebreather.Def_Drain ??= Rebreather.rebreatherController?._airConsumptionRate;
            }
            if (Rebreather.rebreatherController?._maxLightIntensity != 0)
            {
                Rebreather.Def_Intensity ??= Rebreather.rebreatherController?._maxLightIntensity;
            }

            if (Rebreather.Intensity >= 0 && Rebreather.rebreatherController)
            {
                Rebreather.rebreatherController._maxLightIntensity = Rebreather.Intensity;
            }
            else if (Rebreather.rebreatherController && Rebreather.Def_Intensity != null) Rebreather.rebreatherController._maxLightIntensity = Rebreather.Def_Intensity.Value;

            if (Rebreather.NoDrain && Rebreather.rebreatherController)
            {
                Rebreather.rebreatherController._airConsumptionRate = 0;
            }
            else if (Rebreather.rebreatherController && Rebreather.Def_Drain != null) Rebreather.rebreatherController._airConsumptionRate = Rebreather.Def_Drain.Value;
        }
        else Rebreather.rebreatherController = null;
    }

    public static void RopegunInfinite(bool onoff)
    {
        Config.IsRopegunInfinite.Value = onoff;
        Ropegun.InfiniteLength = onoff;
    }

    public static void _ropegunMods()
    {
        if (LocalPlayer.Inventory.RightHandItem?._itemID == (int)ItemIdManager.ItemsId.RopeGun)
        {
            Ropegun.ropegunController ??= LocalPlayer.Transform.GetComponentInChildren<RopeGunController>();
            if (Ropegun.ropegunController?._maxRopeLength != 0)
            {
                Ropegun.Def_Length ??= Ropegun.ropegunController?._maxRopeLength;
            }
            if (Ropegun.ropegunController?._maxFiringRange != 0)
            {
                Ropegun.Def_FiringRange ??= Ropegun.ropegunController?._maxFiringRange;
            }

            if (Ropegun.InfiniteLength && Ropegun.ropegunController)
            {
                Ropegun.ropegunController._maxRopeLength = 10000000;
                Ropegun.ropegunController._maxFiringRange = 10000000;
            }
            else if (Ropegun.ropegunController && Ropegun.Def_Length != null && Ropegun.Def_FiringRange != null)
            {
                Ropegun.ropegunController._maxRopeLength = Ropegun.Def_Length.Value;
                Ropegun.ropegunController._maxFiringRange = Ropegun.Def_FiringRange.Value;
            }
        }
        else Ropegun.ropegunController = null;
    }

    public static void ShotgunRapidFire(bool onoff)
    {
        Config.IsShotgunRapidFire.Value = onoff;
        if (onoff) SdkEvents.OnInWorldUpdate.Subscribe(_shotgunRapidFire);
        else SdkEvents.OnInWorldUpdate.Unsubscribe(_shotgunRapidFire);
    }

    static ShotgunWeaponController shotgunWeaponController;
    internal static void _shotgunRapidFire()
    {
        if (LocalPlayer.Inventory.RightHandItem?._itemID == (int)ItemIdManager.ItemsId.ShotgunPumpAction)
        {
            shotgunWeaponController ??= LocalPlayer.Transform.GetComponentInChildren<ShotgunWeaponController>();
            if (InputSystem.InputMapping.@default.PrimaryAction.currentState.isPressed)
            {
                shotgunWeaponController?.FireWeapon();
            }
        }
        else shotgunWeaponController = null;
    }

    public static void PistolRapidFire(bool onoff)
    {
        Config.IsPistolRapidFire.Value = onoff;
        if (onoff) SdkEvents.OnInWorldUpdate.Subscribe(_pistolRapidFire);
        else SdkEvents.OnInWorldUpdate.Unsubscribe(_pistolRapidFire);
    }

    static CompactPistolWeaponController pistolWeaponController;
    internal static void _pistolRapidFire()
    {
        if (LocalPlayer.Inventory.RightHandItem?._itemID == (int)ItemIdManager.ItemsId.CompactPistol)
        {
            pistolWeaponController ??= LocalPlayer.Transform.GetComponentInChildren<CompactPistolWeaponController>();
            if (InputSystem.InputMapping.@default.PrimaryAction.currentState.isPressed)
            {
                pistolWeaponController?.FireWeapon();
            }
        }
        else pistolWeaponController = null;
    }

    public static void RevolverRapidFire(bool onoff)
    {
        Config.IsRevolverRapidFire.Value = onoff;
        if (onoff) SdkEvents.OnInWorldUpdate.Subscribe(_revolverRapidFire);
        else SdkEvents.OnInWorldUpdate.Unsubscribe(_revolverRapidFire);
    }

    static RevolverWeaponController revolverWeaponController;
    internal static void _revolverRapidFire()
    {
        if (LocalPlayer.Inventory.RightHandItem?._itemID == (int)ItemIdManager.ItemsId.Revolver)
        {
            revolverWeaponController ??= LocalPlayer.Transform.GetComponentInChildren<RevolverWeaponController>();
            if (InputSystem.InputMapping.@default.PrimaryAction.currentState.isPressed)
            {
                revolverWeaponController?.FireWeapon();
            }
        }
        else revolverWeaponController = null;
    }

    public static void RifleRapidFire(bool onoff)
    {
        Config.IsRifleRapidFire.Value = onoff;
        if (onoff) SdkEvents.OnInWorldUpdate.Subscribe(_rifleRapidFire);
        else SdkEvents.OnInWorldUpdate.Unsubscribe(_rifleRapidFire);
    }

    static RifleAnimatorController rifleWeaponController;
    internal static void _rifleRapidFire()
    {
        if (LocalPlayer.Inventory.RightHandItem?._itemID == (int)ItemIdManager.ItemsId.Rifle)
        {
            rifleWeaponController ??= LocalPlayer.Transform.GetComponentInChildren<RifleAnimatorController>();
            if (InputSystem.InputMapping.@default.PrimaryAction.currentState.isPressed)
            {
                rifleWeaponController?.FireWeapon();
            }
        }
        else rifleWeaponController = null;
    }

    public static void RunCleanup()
    {
        Lighter.Def_Range = null;
        Lighter.Def_Intensity = null;
        Lighter.Intensity = -1;
        Flashlight.Def_Intensity = null;
        Flashlight.Def_Drain = null;
        Flashlight.Intensity = -1;
        Rebreather.Def_Intensity = null;
        Rebreather.Def_Drain = null;
        Rebreather.Intensity = -1;
        Ropegun.Def_Length = null;
        Ropegun.Def_FiringRange = null;
    }
}
