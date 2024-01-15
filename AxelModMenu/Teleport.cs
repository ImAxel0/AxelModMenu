using Sons.Items.Core;
using SonsSdk;
using SUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheForest;
using UnityEngine.SceneManagement;
using UnityEngine;
using TheForest.Utils;
using System.Collections;
using RedLoader;
using Sons.Input;
using SonsNetwork;
using TheForest.UI.Multiplayer;

namespace AxelModMenu;

public class Teleport
{
    public static Observable<string> GoToBuffer = new("");
    public static Observable<bool> ShowMap = new(false);

    public static string Com(string commandName)
    {
        return $"<color=yellow>{commandName}</color>";
    }

    public static void GoToItem()
    {
        ItemData data = ItemDatabaseManager.ItemByName(GoToBuffer.Value);
        if (!data)
        {
            if (int.TryParse(GoToBuffer.Value, out var value))
            {
                data = ItemDatabaseManager.ItemById(value);
            }
        }
        if (!data)
        {
            SonsTools.ShowMessage($"Invalid parameter, {Com($"{GoToBuffer.Value}")} is not a valid item name/id");
            return;
        }
        Transform itemPosition = null;

        GameObject go = GameObject.Find($"{data.Name}Pickup");
        if (go)
        {
            itemPosition = go.transform;
            Vector3 position = itemPosition.position;
            DebugConsole.Instance._goto($"{position.x} {position.y} {position.z}");
            SonsTools.ShowMessage($"Ran command {Com($"gotoitem {GoToBuffer.Value}")}");
            return;
        }

        Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<Scene> scenes = SceneManager.GetAllScenes();
        List<Scene> sceneList = scenes.ToList();

        foreach (Scene scene in sceneList)
        {
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameObject> gameObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in gameObjects)
            {
                for (int i = 0; i < gameObject.transform.GetChildCount(); i++)
                {
                    Transform tr = gameObject.transform.GetChild(i);
                    if (tr.gameObject.name == $"{data.Name}Pickup")
                    {
                        itemPosition = tr;
                        break;
                    }
                }
            }
        }

        if (!itemPosition)
        {
            SonsTools.ShowMessage($"Couldn't find {Com($"{data.Name}")} in world");
            return;
        }
        Vector3 pos = itemPosition.position;
        DebugConsole.Instance._goto($"{pos.x} {pos.y} {pos.z}");
    }
    public static void TeleportTo(SButtonOptions coords)
    {
        DebugConsole.Instance._goto(coords._id);
    }

    public static void SetRaycastMode(bool onoff)
    {
        if (onoff)
        {
            GlobalEvents.OnUpdate.Subscribe(RaycastTeleport);
            return;
        }
        GlobalEvents.OnUpdate.Unsubscribe(RaycastTeleport);
    }

    public static void RaycastTeleport()
    {
        if (Physics.Raycast(LocalPlayer.MainCamTr.position + Vector3.forward * 1.2f, LocalPlayer.MainCamTr.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !LocalPlayer.IsInMidAction)
            {
                LocalPlayer.Transform.position = hit.point + Vector3.up * 1f;
            }
        }
    }

    public static void ToggleInteractiveMap()
    {
        if (BoltNetwork.isRunning)
        {
            if (ChatBox.IsChatOpen)
            {
                ShowMap.Value = false;
                return;
            }
        }
        if (AxelModMenu._showMenu)
        {
            ShowMap.Value = false;
            return;
        }
        ShowMap.Value = !ShowMap.Value;
    }

    public static void InteractiveMap()
    {
        Vector3 mousePos = Input.mousePosition;

        float x = (mousePos.x - 360) * 5.5f;
        float z = (mousePos.y - 360) * 5.5f;

        if (Physics.Raycast(new Vector3(x, 1000, z), Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain"), QueryTriggerInteraction.Ignore))
        {
            LocalPlayer.Transform.position = hit.point + Vector3.up * 2f;
        }
    }

    public static void UpdateDotPosition()
    {
        Vector2 playerPos = new((LocalPlayer.Transform.position.x / 5.5f) + 360, (LocalPlayer.Transform.position.z / 5.5f) + 360);
        AxelModMenuUi.MapDot.ImageObject.transform.position = new Vector3(playerPos.x, playerPos.y);
    }
}
