using Sons.Characters;
using SUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sons.Ai;
using Sons.Ai.Vail;
using Sons;
using TheForest;
using TheForest.Utils;
using UnityEngine;

namespace AxelModMenu;

public class Actors
{
    public static void SpawnActor(SButtonOptions name)
    {
        CharacterManager.Instance.DebugAddCharacter(name._id, true);
    }

    public static void KillEnemies()
    {
        VailActorManager.GetActiveActors().ToList().ForEach(actor =>
        {
            if (actor.ClassId == VailActorClassId.Cannibal || actor.ClassId == VailActorClassId.Creepy)
            {
                actor.ForceDeath();
            }
        });
    }

    public static void BurnEnemies()
    {
        VailActorManager.GetActiveActors().ToList().ForEach(actor =>
        {
            if (actor.ClassId == VailActorClassId.Cannibal || actor.ClassId == VailActorClassId.Creepy)
            {
                actor.IgniteSelf();
            }
        });
    }

    public static void KillAnimals()
    {
        VailActorManager.GetActiveActors().ToList().ForEach(actor =>
        {
            if (actor.ClassId == VailActorClassId.Animal)
            {
                actor.ForceDeath();
            }
        });
    }

    public static void FreezeActors(bool onoff)
    {
        VailWorldSimulation.SetPaused(onoff);
    }
}
