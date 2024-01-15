using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TheForest;
using TheForest.Utils;
using SUI;
using Endnight.Types;
using SonsAxLib;
using Sons.Weapon;
using Sons.Items.Core;
using SonsSdk;
using Sons.Wearable.Armour;
using RedLoader;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Il2CppSystem;
using Sons.Input;
using static SUI.SUI;
using HarmonyLib;
using TheForest.UI.Multiplayer;

namespace AxelModMenu;

public class PlayerSettings
{
    public struct PlayerPos
    {
        public static Observable<string> X = new("");
        public static Observable<string> Y = new("");
        public static Observable<string> Z = new("");

        public static Observable<float> Xf = new(0);
        public static Observable<float> Yf = new(0);
        public static Observable<float> Zf = new(0);
    }

    public struct NoClipData
    {
        public static bool NoClipToggle;
        public static Observable<float> Speed = new(2.5f);
        public static Observable<float> UpDownSpeed = new(0.5f);
    }

    public static Observable<string> currArmour = new("BoneArmour");

    public static void GodMode(bool onoff)
    {
        Config.IsGodMode.Value = onoff;
        string b = onoff ? "on" : "off";
        DebugConsole.Instance._godmode(b);
    }

    public static void InfStamina(bool onoff)
    {
        Config.IsInfStamina.Value = onoff;
        string b = onoff ? "on" : "off";
        DebugConsole.Instance._energyhack(b);
    }

    public static void NoHungry(bool onoff)
    {
        Config.IsNoHungry.Value = onoff;
        if (onoff)
        {
            LocalPlayer.Vitals.Fullness.SetMin(100);
            return;
        }
        LocalPlayer.Vitals.Fullness.SetMin(0);        
    }

    public static void NoDehydration(bool onoff)
    {
        Config.IsNoDehydration.Value = onoff;
        if (onoff)
        {
            LocalPlayer.Vitals.Hydration.SetMin(100);
            return;
        }
        LocalPlayer.Vitals.Hydration.SetMin(0);
    }

    public static void NoSleep(bool onoff)
    {
        Config.IsNoSleep.Value = onoff;
        if (onoff)
        {
            LocalPlayer.Vitals.Rested.SetMin(100);
            return;
        }
        LocalPlayer.Vitals.Rested.SetMin(0);
    }

    public static void InfAmmo(bool onoff)
    {
        Config.IsInfiniteAmmo.Value = onoff;
        if (onoff)
        {
            HarmonyTools.CreatePrefix(AxelModMenu.Harmony, typeof(RangedWeapon.Ammo), nameof(RangedWeapon.Ammo.Remove), typeof(Patches), nameof(Patches.InfAmmoPatch));
            return;
        }
        HarmonyTools.RemovePatch(AxelModMenu.Harmony, typeof(RangedWeapon.Ammo), nameof(RangedWeapon.Ammo.Remove));
    }

    static float? fallDamage;
    public static void NoFallDamage(bool onoff)
    {
        fallDamage ??= LocalPlayer.FpCharacter._baseFallDamage;
        Config.IsNoFallDamage.Value = onoff;
        if (onoff)
        {
            LocalPlayer.FpCharacter._baseFallDamage = 0;
            return;
        }
        LocalPlayer.FpCharacter._baseFallDamage = (float)fallDamage;
    }

    public static void NoClip(bool onoff)
    {
        Config.IsNoClip.Value = onoff;
        NoClipData.NoClipToggle = onoff;
        if (onoff)
        {
            GlobalEvents.OnUpdate.Subscribe(_noClip);
            LocalPlayer.FpCharacter._rigidbody.isKinematic = true;
            LocalPlayer.Transform.Find("PlayerAnimator/CameraShakeDriver").GetComponent<Animator>().enabled = !onoff;
            return;
        }
        GlobalEvents.OnUpdate.Unsubscribe(_noClip);
        LocalPlayer.Transform.Find("PlayerAnimator/CameraShakeDriver").GetComponent<Animator>().enabled = !onoff;
        LocalPlayer.FpCharacter._rigidbody.isKinematic = false;
    }

    public static void SetNoClipSpeed(float value)
    {
        Config.NoClipSpeed.Value = value;
        NoClipData.Speed.Value = value;
    }

    public static void SetNoClipUpDownSpeed(float value)
    {
        Config.NoClipUpDownSpeed.Value = value;
        NoClipData.UpDownSpeed.Value = value;
    }

    public static void NoClipToggleKey()
    {
        if (!GlobalEvents.OnUpdate.CheckIfSubscribed(AccessTools.Method(typeof(PlayerSettings), nameof(_noClip))) || !LocalPlayer._instance) 
            return;

        if (BoltNetwork.isRunning)
        {
            if (ChatBox.IsChatOpen) return;
        }

        if (AxelModMenu._showMenu || Teleport.ShowMap.Value) return;

        NoClipData.NoClipToggle = !NoClipData.NoClipToggle;
    }

    private static void _noClip()
    {
        if (!LocalPlayer._instance) return;

        if (!NoClipData.NoClipToggle)
        {
            LocalPlayer.FpCharacter._rigidbody.isKinematic = false;
            return;
        }

        LocalPlayer.FpCharacter._rigidbody.isKinematic = true;
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        float updown = 0;
        if (InputSystem.InputMapping.@default.Jump.IsPressed())
        {
            updown = NoClipData.UpDownSpeed.Value;
        }
        else if (InputSystem.InputMapping.@default.Crouch.IsPressed())
        {
            updown = -NoClipData.UpDownSpeed.Value;
        }

        if (!Camera.main) return;

        Vector3 vector = Camera.main.transform.forward;
        Vector3 vector2 = Camera.main.transform.right;

        Vector3 vector3 = vector * vertical + vector2 * horizontal;

        if (vector3.magnitude > 1f) vector3.Normalize();
        Vector3 vector4 = LocalPlayer.Transform.position;

        if (InputSystem.InputMapping.@default.Run.IsPressed())
        {
            vector4 += vector3 * NoClipData.Speed.Value * 0.1f * 10f + LocalPlayer.Transform.up * updown;
        }
        else vector4 += vector3 * (NoClipData.Speed.Value + NoClipData.Speed.Value) * 0.1f + LocalPlayer.Transform.up * updown;
        LocalPlayer.Transform.position = Vector3.Lerp(LocalPlayer.Transform.position, vector4, 0.1f);
    }

    public static void WalkSpeed(float value)
    {
        Config.WalkSpeed.Value = value;
        LocalPlayer.FpCharacter.SetWalkSpeed(value);
    }

    public static void RunSpeed(float value)
    {
        Config.RunSpeed.Value = value;
        LocalPlayer.FpCharacter.SetRunSpeed(value);
    }

    public static void SwimSpeed(float value)
    {
        Config.SwimSpeed.Value = value;
        LocalPlayer.FpCharacter.SetSwimSpeed(value);
    }

    public static void JumpMultiplier(float value)
    {
        Config.JumpMultiplier.Value = value;
        LocalPlayer.FpCharacter.SetSuperJump(value);
    }

    public static void MovePlayerX(float x)
    {
        Vector3 pos = LocalPlayer.Transform.position;
        LocalPlayer.Transform.position = new Vector3(x, pos.y, pos.z);
    }

    public static void MovePlayerY(float y)
    {
        Vector3 pos = LocalPlayer.Transform.position;
        LocalPlayer.Transform.position = new Vector3(pos.x, y, pos.z);
    }

    public static void MovePlayerZ(float z)
    {
        Vector3 pos = LocalPlayer.Transform.position;
        LocalPlayer.Transform.position = new Vector3(pos.x, pos.y, z);
    }

    public static void MaxArmorOfType()
    {
        int armourID = ItemDatabaseManager.ItemIdByName(currArmour.Value);
        ArmourPiece armour = LocalPlayer.Stats.ArmourSystem.GetArmourPieceById(armourID);
        if (!armour)
        {
            SonsTools.ShowMessage($"Invalid parameter, {currArmour.Value} is not a valid armour name");
            return;
        }
        foreach (var slot in (Sons.Wearable.WearableSlots[])System.Enum.GetValues(typeof(Sons.Wearable.WearableSlots)))
        {
            LocalPlayer.Stats.ArmourSystem.TryUnequipSlot(slot, true);
        }
        for (int i = 0; i < 10; i++)
        {
            if (!LocalPlayer.Stats.ArmourSystem.EquipToNextAvailableSlot(armour))
                break;
        }
    }

    public static void Update()
    {
        PlayerPos.X.Value = LocalPlayer.Transform.position.x.ToString("0");
        PlayerPos.Y.Value = LocalPlayer.Transform.position.y.ToString("0");
        PlayerPos.Z.Value = LocalPlayer.Transform.position.z.ToString("0");

        PlayerPos.Xf.Value = LocalPlayer.Transform.position.x;
        PlayerPos.Yf.Value = LocalPlayer.Transform.position.y;
        PlayerPos.Zf.Value = LocalPlayer.Transform.position.z;
    }
}
