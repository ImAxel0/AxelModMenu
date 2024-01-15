using SonsSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using Sons.Atmosphere;
using Sons.Gameplay;
using Sons.Environment;
using TheForest;
using TheForest.Utils;
using SUI;
using SonsAxLib;

namespace AxelModMenu;

public class Environment
{
    public static Observable<string> SetDayBuffer = new("");

    public static void FreezeLakes(bool onoff)
    {
        Config.IsFreezeLakes.Value = onoff;
        GameObject lakes = SceneManager.GetSceneByName("Site02StreamsAndLakes").GetRootGameObjects().FirstWithName("Lakes");
        if (lakes)
        {
            for (int i = 0; i < lakes.transform.GetChildCount(); i++)
            {
                Transform lake = lakes.transform.GetChild(i);
                FreezeWater freeze = lake.GetComponentInChildren<FreezeWater>();
                if (!freeze) { continue; }
                if (!freeze._forceFrozen && !freeze._neverFreeze)
                {
                    freeze.Freeze(onoff);
                }
            }
        }
    }

    public static void NoLakes(bool onoff)
    {
        Config.IsNoLakes.Value = onoff;
        Scene sonsMain = SceneManager.GetSceneByName("Site02StreamsAndLakes");
        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameObject> gameObjects = sonsMain.GetRootGameObjects();
        gameObjects.FirstWithName("Lakes").SetActive(!onoff);
    }

    public static void NoOcean(bool onoff)
    {
        Config.IsNoOcean.Value = onoff;
        Scene sonsMain = SceneManager.GetSceneByName("SonsMain");
        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameObject> gameObjects = sonsMain.GetRootGameObjects();
        gameObjects.FirstWithName("OceanZone").SetActive(!onoff);
        gameObjects.FirstWithName("Atmosphere").transform.Find("CrestOcean").gameObject.SetActive(!onoff);
    }

    public static void NoWaterfalls(bool onoff)
    {
        Config.IsNoWaterfalls.Value = onoff;
        Scene sonsMain = SceneManager.GetSceneByName("Site02StreamsAndLakes");
        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameObject> gameObjects = sonsMain.GetRootGameObjects();
        gameObjects.FirstWithName("Waterfalls").SetActive(!onoff);
    }

    public static void NoGrass(bool onoff)
    {
        DebugConsole.Instance.SendCommand("togglegrass");
    }

    public static void NoForest(bool onoff)
    {
        DebugConsole.Instance.SendCommand("noforest");
    }

    public static void NoGravity(bool onoff)
    {
        Config.IsNoGravity.Value = onoff;
        LocalPlayer.FpCharacter.SetDisabledGravity(onoff);
    }

    public static void SetSpring()
    {
        DebugConsole.Instance._season("spring");
    }

    public static void SetSummer()
    {
        DebugConsole.Instance._season("summer");
    }

    public static void SetAutumn()
    {
        DebugConsole.Instance._season("autumn");
    }

    public static void SetWinter()
    {
        DebugConsole.Instance._season("winter");
    }

    public static void SunColorRed(float red)
    {
        Color col = SunLightManager._instance._sunLight.color;
        SunLightManager._instance._sunLight.color = new Color(red, col.g, col.b);
    }

    public static void SunColorGreen(float green)
    {
        Color col = SunLightManager._instance._sunLight.color;
        SunLightManager._instance._sunLight.color = new Color(col.r, green, col.b);
    }

    public static void SunColorBlue(float blue)
    {
        Color col = SunLightManager._instance._sunLight.color;
        SunLightManager._instance._sunLight.color = new Color(col.r, col.g, blue);
    }

    public static void SunIntensity(float value)
    {
        SunLightManager._instance._sunLight.intensity = value;
    }

    public static void TreeRegrowFactor(float value)
    {
        Config.TreeRegrowRate.Value = value;
        UnityEngine.Object.FindObjectOfType<TreeRegrowChecker>()._regrowthFactor = value;
    }

    public static void WindIntensity(float value)
    {
        if (value < 0)
        {
            WindManager.Unlock();
            return;
        }
        WindManager.SetAndLockIntensity(value);
    }

    public static void SetDay()
    {
        if (int.TryParse(SetDayBuffer.Value.Replace(" ", ""), out int day))
        {
            TimeOfDayHolder.SetDay(day);
            return;
        }
        SonsTools.ShowMessage("Invalid day parameter");
    }

    public static void DaytimeSpeed(float value)
    {
        TimeOfDayHolder.SetBaseTimeSpeed(value);
    }

    public static void LockDaytime(bool onoff)
    {
        Config.IsLockTime.Value = onoff;
        if (onoff)
        {
            int hour = TimeOfDayHolder.GetTimeOfDay().Hours;
            int minutes = TimeOfDayHolder.GetTimeOfDay().Minutes;
            DebugConsole.Instance._lockTimeOfDay($"{hour}:{minutes}");
            return;
        }
        int hour1 = TimeOfDayHolder.GetTimeOfDay().Hours;
        int minutes1 = TimeOfDayHolder.GetTimeOfDay().Minutes;
        DebugConsole.Instance._setTimeOfDay($"{hour1}:{minutes1}");
    }
}
