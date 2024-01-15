using SonsSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Sons;
using TheForest.Utils;
using SonsAxLib;
using static Il2CppSystem.Array;
using Sons.Input;
using SUI;
using Il2CppSystem.Runtime.Remoting.Messaging;
using static AxelModMenu.PlayerSettings;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Endnight.Extensions;
using UnityEngine.Rendering;
using Sons.Crafting.Structures;
using UnityEngine.Playables;

namespace AxelModMenu;

public class WorldEdit
{
    public struct WorldEditInfo
    {
        public static Observable<string> PointedObject = new("Pointed object:");
        public static Observable<string> ActiveObject = new("Selected object:");
        public static Observable<string> SnapToPlayer = new("Snap to player:");

        public static int UpDownSpeed = 2;
        public static int RotationSpeed = 10;
    }

    public static int CurrRotationAxis = 1;
    public static Observable<string> CurrRotationAxisString = new("Y");
    public enum RotationAxis
    {
        X, Y, Z
    }

    public static Observable<bool> IsWorldEditMode = new(false);
    static GameObject AxelWorldEdit;
    static Material OutlineMat;
    static LineRenderer lineRenderer;

    static GameObject PointedGameobject;
    static GameObject LastPointedGameobject;
    static GameObject ActiveGameobject;
    static Material OriginalMat;

    [System.Runtime.InteropServices.DllImport("USER32.dll")] public static extern short GetKeyState(int nVirtKey);
    static bool IsCapsLockOn => (GetKeyState(0x14) & 1) > 0;

    public static void Init()
    {
        AxelWorldEdit = new("AxelWorldEdit")
        {
            name = "AxelWorldEdit",
            tag = "AxelWorldEdit"
        };

        OutlineMat = new Material(Shader.Find("HDRP/Unlit"))
        {
            color = Color.cyan        
        };
        OutlineMat.SetFloat("_Cull", (float)CullMode.Back);

        AxelWorldEdit.SetParent(LocalPlayer.GameObject.transform, true);
        lineRenderer = AxelWorldEdit.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.05f;
    }

    public static void SetWorldEditMode(bool onoff)
    {
        IsWorldEditMode.Value = onoff;
    }

    public static void SetUpDownSpeed(float value)
    {
        WorldEditInfo.UpDownSpeed = (int)value;
    }

    public static void SetRotationSpeed(float value)
    {
        WorldEditInfo.RotationSpeed = (int)value;
    }

    private static bool TryRaycast(out GameObject hittedObj)
    {
        var pistolTr = LocalPlayer.Inventory.RightHandItem.ItemObject.transform;
        if (Physics.Raycast(pistolTr.position + pistolTr.right * 1.25f, pistolTr.right, out RaycastHit hit, 25f, ~LayerMask.GetMask("Terrain"), QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject == LocalPlayer.GameObject)
            {
                lineRenderer.enabled = false;
                hittedObj = null;
                return false;
            }

            if (TryHighlightGameobject(hit.transform.gameObject))
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, pistolTr.position + pistolTr.right);
                lineRenderer.SetPosition(1, hit.point);
                hittedObj = hit.transform.gameObject;
                return true;
            }
        }
        lineRenderer.enabled = false;
        hittedObj = null;
        return false;
    }

    private static bool TryHighlightGameobject(GameObject pointedGo)
    {
        var hasMash = pointedGo.GetComponentInChildren<MeshRenderer>();
        if (hasMash) return true;    
        else return false;
    }

    public static void WorldEditUpdate()
    {
        if (!IsWorldEditMode.Value) return;

        if (LocalPlayer.Inventory?.RightHandItem?._itemID == (int)ItemIdManager.ItemsId.CompactPistol)
        {
            if (!ActiveGameobject)
            {
                if (TryRaycast(out var hittedObj))
                {
                    PointedGameobject = hittedObj;
                }
                else PointedGameobject = null;

                if (InputSystem.InputMapping.@default.Interact.triggered && PointedGameobject)
                {
                    ActiveGameobject = PointedGameobject;
                }
            }
            if (ActiveGameobject) ObjectControl(ActiveGameobject);
        }
        else
        {
            lineRenderer.enabled = false;
            PointedGameobject = null;
            ReleaseObject(ActiveGameobject);
        }
        UpdateGUI();
    }

    private static void UpdateGUI()
    {
        Color bgCol = ActiveGameobject ? new Color32(54, 54, 54, 229) : Color.black.WithAlpha(0.9f);
        AxelModMenuUi.WorldEditPanel.Background(bgCol);

        string pointedName = PointedGameobject ? $"<color=#3366ff>{PointedGameobject.name}</color>" : "<color=#6600ff>null</color>";
        WorldEditInfo.PointedObject.Value =  $"Pointed object: {pointedName}";
        string activeName = ActiveGameobject ? $"<color=#66ff66>{ActiveGameobject.name}</color>" : "<color=#6600ff>null</color>";
        WorldEditInfo.ActiveObject.Value =  $"Selected object: {activeName}";

        string IsSnapToPlayer = IsCapsLockOn ? "<color=#66ff66>ON</color>" : "<color=#ff3300>OFF</color>";
        WorldEditInfo.SnapToPlayer.Value = $"Snap to player: {IsSnapToPlayer}";

        switch (CurrRotationAxis)
        {
            case (int)RotationAxis.X:
                CurrRotationAxisString.Value = "Rotation axis: <color=#A77ADD>X</color>";
                break;
            case (int)RotationAxis.Y:
                CurrRotationAxisString.Value = "Rotation axis: <color=#A77ADD>Y</color>";
                break;
            case (int)RotationAxis.Z:
                CurrRotationAxisString.Value = "Rotation axis: <color=#A77ADD>Z</color>";
                break;
        }
    }

    private static void ObjectControl(GameObject activeObject)
    {
        lineRenderer.enabled = false;

        var pos = activeObject.transform.localPosition;
        var rot = activeObject.transform.localRotation;
        var scale = activeObject.transform.localScale;

        if (!IsCapsLockOn)
        {
            if (Input.GetKey(KeyCode.DownArrow)) activeObject.transform.position = new(pos.x, 
                pos.y - WorldEditInfo.UpDownSpeed * Time.deltaTime, 
                pos.z);
            else if (Input.GetKey(KeyCode.UpArrow)) activeObject.transform.position = new(pos.x, 
                pos.y + WorldEditInfo.UpDownSpeed * Time.deltaTime, 
                pos.z);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) CurrRotationAxis--;
        else if ( Input.GetKeyDown(KeyCode.RightArrow)) CurrRotationAxis++;
        CurrRotationAxis = Mathf.Clamp(CurrRotationAxis, 0, 2);

        switch (CurrRotationAxis)
        {
            case (int)RotationAxis.X:
                if (Input.GetKey(KeyCode.Q)) activeObject.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x -WorldEditInfo.RotationSpeed * 10 * Time.deltaTime,
                    rot.eulerAngles.y,
                    rot.eulerAngles.z);
                else if (Input.GetKey(KeyCode.R)) activeObject.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x +WorldEditInfo.RotationSpeed * 10 * Time.deltaTime,
                    rot.eulerAngles.y,
                    rot.eulerAngles.z);
                break;

            case (int)RotationAxis.Y:
                if (Input.GetKey(KeyCode.Q)) activeObject.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x,
                    rot.eulerAngles.y -WorldEditInfo.RotationSpeed * 10 * Time.deltaTime,
                    rot.eulerAngles.z);
                else if (Input.GetKey(KeyCode.R)) activeObject.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x,
                    rot.eulerAngles.y +WorldEditInfo.RotationSpeed * 10 * Time.deltaTime,
                    rot.eulerAngles.z);
                break;

            case (int)RotationAxis.Z:
                if (Input.GetKey(KeyCode.Q)) activeObject.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x,
                    rot.eulerAngles.y,
                    rot.eulerAngles.z -WorldEditInfo.RotationSpeed * 10 * Time.deltaTime);
                else if (Input.GetKey(KeyCode.R)) activeObject.transform.localRotation = Quaternion.Euler(rot.eulerAngles.x,
                    rot.eulerAngles.y,
                    rot.eulerAngles.z +WorldEditInfo.RotationSpeed * 10 * Time.deltaTime);
                break;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) activeObject.transform.localScale = new(scale.x + 1f, scale.y + 1f, scale.z + 1f);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) activeObject.transform.localScale = new(scale.x - 1f, scale.y - 1f, scale.z - 1f);

        if (IsCapsLockOn)
        {
            if (activeObject.transform.parent != LocalPlayer.Transform)
                activeObject.SetParent(LocalPlayer.Transform, true);
        }
        else
        {
            if (activeObject.transform.parent == LocalPlayer.Transform)
                activeObject.transform.parent = null;
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            UnityEngine.Object.Destroy(activeObject);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) ReleaseObject(activeObject);
    }

    private static void ReleaseObject(GameObject activeObject)
    {
        if (!activeObject) return;
        if (activeObject.transform.parent == LocalPlayer.Transform) activeObject.transform.parent = null;
        ActiveGameobject = null;
    }
}
