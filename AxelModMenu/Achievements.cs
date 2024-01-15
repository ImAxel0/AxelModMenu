using Sons.Gameplay.Achievements;
using SonsSdk;
using SUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AxelModMenu;

public class Achievements
{
    public static void UnlockAllAchievements()
    {
        AchievementManager achievementManager = GameObject.Find("SteamManager/AchievementManager")?.GetComponent<AchievementManager>();
        if (!achievementManager)
        {
            SonsTools.ShowMessage("<color=red>Couldn't</color> unlock achievements");
            return;
        }

        foreach (var achievement in achievementManager._achievements)
        {
            achievement.Unlock();
        }
        SonsTools.ShowMessage("All achievements <color=green>unlocked</color>");
    }

    public static void LockAllAchievements()
    {
        AchievementManager achievementManager = GameObject.Find("SteamManager/AchievementManager")?.GetComponent<AchievementManager>();
        if (!achievementManager)
        {
            SonsTools.ShowMessage("<color=red>Couldn't</color> lock achievements");
            return;
        }

        foreach (var achievement in achievementManager._achievements)
        {
            achievement.Reset();
        }
        SonsTools.ShowMessage("All achievements <color=green>locked</color>");
    }
}
