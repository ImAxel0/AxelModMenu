using SUI;
using SonsAxLib;
using TheForest.Utils;
using Sons.Atmosphere;
using UnityEngine;
using SonsSdk;
using Sons.Gameplay;
using Sons.Environment;
using static SonsAxLib.AXSUI;
using static SUI.SUI;
using Il2CppSystem.Runtime.Remoting.Messaging;
using Sons.Weapon;
using UnityEngine.Rendering.HighDefinition;
using Sons.Gameplay.Achievements;
using TMPro;
using RedLoader;
using UnityEngine.UI;

namespace AxelModMenu;


public class AxelModMenuUi
{
    public static List<string> SwitchablePanels = new();
    public static int CurrentPanel = 0;
    public static SImageOptions MapDot;
    public static SPanelOptions WorldEditPanel;

    public static readonly string PanelSelector = "Panel selector";
    public static readonly string PlayerPanel = "Player settings";
    public static readonly string EnvironmentPanel = "Environment settings";
    public static readonly string TeleportPanel = "Teleport";
    public static readonly string ModdedItemsPanel = "Modded items";
    public static readonly string MiscPanel = "Misc";
    public static readonly string MultiplayerPanel = "Multiplayer";
    public static readonly string EnemiesPanel = "Enemies";

    static float _panelAlpha = 0.95f;
    static float _bgBlurAmount = 0.4f;
    static Color _greenButtonCol = new(0.04f, 0.63f, 0.04f, 0.75f);

    public static void RunCleanup()
    {
        SwitchablePanels.Clear();
        CurrentPanel = 0;
    }

    private static void NextPanel()
    {
        if (CurrentPanel == SwitchablePanels.Count - 1)
        {
            AudioController.PlaySound("uierror", AudioController.SoundType.Sfx);
            return;
        }

        CurrentPanel++;
        TogglePanel(SwitchablePanels[CurrentPanel - 1], false);
        TogglePanel(SwitchablePanels[CurrentPanel], true);
        AudioController.PlaySound("uisound", AudioController.SoundType.Sfx);
    }

    private static void PrevPanel()
    {
        if (CurrentPanel == 0)
        {
            AudioController.PlaySound("uierror", AudioController.SoundType.Sfx);
            return;
        }

        CurrentPanel--;
        TogglePanel(SwitchablePanels[CurrentPanel + 1], false);
        TogglePanel(SwitchablePanels[CurrentPanel], true);
        AudioController.PlaySound("uisound", AudioController.SoundType.Sfx);
    }

    public static void Create()
    {
        SwitchablePanels.Add(EnvironmentPanel);
        SwitchablePanels.Add(ModdedItemsPanel);
        SwitchablePanels.Add(TeleportPanel);
        SwitchablePanels.Add(EnemiesPanel);
        SwitchablePanels.Add(MiscPanel);

        if (BoltNetwork.isRunning)
        {
            SwitchablePanels.Add(MultiplayerPanel);
        }

        AxCreatePanel(PanelSelector, false, new Vector2(300, 60), AnchorType.TopCenter, Color.black.WithAlpha(_panelAlpha), EBackground.RoundedStandard, true).Horizontal(0, "CE").Active(false)
            .Add(AxButtonText("←", PrevPanel, 60).Dock(EDockType.Fill)
            .Add(AxButtonText("→", NextPanel, 60).Dock(EDockType.Fill)));

        AxGetMainContainer((SPanelOptions)AxCreateSidePanel(PlayerPanel, true, "Player", Side.Left, 525, Color.black.WithAlpha(_panelAlpha), EBackground.None, false).Material(PanelBlur.GetForShade(_bgBlurAmount)).OverrideSorting(999).Active(false))
            .Add(AxMenuCheckBox("God mode", PlayerSettings.GodMode, Config.IsGodMode.Value))
            .Add(AxMenuCheckBox("Inf. stamina", PlayerSettings.InfStamina, Config.IsInfStamina.Value))
            .Add(AxMenuCheckBox("No hungry", PlayerSettings.NoHungry, Config.IsNoHungry.Value))
            .Add(AxMenuCheckBox("No dehydration", PlayerSettings.NoDehydration, Config.IsNoDehydration.Value))
            .Add(AxMenuCheckBox("No sleep", PlayerSettings.NoSleep, Config.IsNoSleep.Value))
            .Add(AxMenuCheckBox("Infinite ammo", PlayerSettings.InfAmmo, Config.IsInfiniteAmmo.Value))
            .Add(AxMenuCheckBox("No fall damage", PlayerSettings.NoFallDamage, Config.IsNoFallDamage.Value))
            .Add(AxDivider("<color=yellow>No Clip</color>"))
            .Add(AxMenuCheckBox("<color=#3366ff>No clip/fly hack</color>", PlayerSettings.NoClip, Config.IsNoClip.Value).Id("NoClipContainer"))
            .Add(AxMenuSliderFloat("Speed", LabelPosition.Top, 1, 100, PlayerSettings.NoClipData.Speed.Value, 1, PlayerSettings.SetNoClipSpeed))
            .Add(AxMenuSliderFloat("UpDown speed", LabelPosition.Top, 0.1f, 5, PlayerSettings.NoClipData.UpDownSpeed.Value, 0.1f, PlayerSettings.SetNoClipUpDownSpeed))
            .Add(AxDivider("<color=yellow>Movement</color>"))
            .Add(AxMenuSliderFloat("Walk speed", LabelPosition.Top, 1, 50, LocalPlayer.FpCharacter.WalkSpeed, 0.1f, PlayerSettings.WalkSpeed))
            .Add(AxMenuSliderFloat("Run speed", LabelPosition.Top, 1, 50, LocalPlayer.FpCharacter.RunSpeed, 0.1f, PlayerSettings.RunSpeed))
            .Add(AxMenuSliderFloat("Swim speed", LabelPosition.Top, 1, 50, LocalPlayer.FpCharacter.SwimSpeed, 0.1f, PlayerSettings.SwimSpeed))
            .Add(AxMenuSliderFloat("Jump multiplier", LabelPosition.Top, 1, 100, LocalPlayer.FpCharacter.JumpMultiplier, 0.1f, PlayerSettings.JumpMultiplier))
            .Add(AxDivider("<color=yellow>Armour</color>")) 
            .Add(AxMenuOptions("Equip armour", LabelPosition.Top, PlayerSettings.currArmour, new[] { "BoneArmour", "CreepyArmour", "DeerHideArmour", "LeafArmour", "TechArmour" }))
            .Add(AxMenuButton("Equip", PlayerSettings.MaxArmorOfType, null, EBackground.None).Margin(0, 10))
            .Add(AxDivider("<color=yellow>Player pos.</color>"))
            .Add(AxMenuSliderFloat3(new[] { "<color=red>X</color>", "<color=red>Y</color>", "<color=red>Z</color>" },
                LayoutMode.Vertical, -2000, 2000, new[] { PlayerSettings.PlayerPos.Xf, PlayerSettings.PlayerPos.Yf, PlayerSettings.PlayerPos.Zf }, 1f, 
                new[] { PlayerSettings.MovePlayerX, PlayerSettings.MovePlayerY, PlayerSettings.MovePlayerZ }));

        AxGetMainContainer((SPanelOptions)AxCreateSidePanel(EnvironmentPanel, false, "Environment", Side.Right, 700, Color.black.WithAlpha(_panelAlpha), EBackground.None, false).Material(PanelBlur.GetForShade(_bgBlurAmount)).OverrideSorting(999).Active(false))
            .Add(AxMenuCheckBox("Freeze lakes", Environment.FreezeLakes, Config.IsFreezeLakes.Value))
            .Add(AxMenuCheckBox("No lakes", Environment.NoLakes, Config.IsNoLakes.Value))
            .Add(AxMenuCheckBox("No ocean", Environment.NoOcean, Config.IsNoOcean.Value))
            .Add(AxMenuCheckBox("No waterfalls", Environment.NoWaterfalls, Config.IsNoWaterfalls.Value))
            .Add(AxMenuCheckBox("No grass (by Toni Macaroni)", Environment.NoGrass))
            .Add(AxMenuCheckBox("No forest (by Toni Macaroni)", Environment.NoForest))
            .Add(AxMenuCheckBox("No gravity", Environment.NoGravity, Config.IsNoGravity.Value))
            .Add(AxDivider("<color=yellow>World edit</color>"))
            .Add(AxMenuCheckBox("<color=#3366ff>World edit mode</color> (equip pistol)", WorldEdit.SetWorldEditMode))
            .Add(AxMenuSliderInt("UpDown speed", LabelPosition.Top, 1, 15, Config.WorldEditUpDownSpeed.Value, WorldEdit.SetUpDownSpeed))
            .Add(AxMenuSliderInt("Rotation speed", LabelPosition.Top, 1, 15, Config.WorldEditRotationSpeed.Value, WorldEdit.SetRotationSpeed))
            .Add(AxDivider("<color=yellow>Seasons</color>"))
            .Add(AxMenuButton("Spring", Environment.SetSpring, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuButton("Summer", Environment.SetSummer, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuButton("Autumn", Environment.SetAutumn, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuButton("Winter", Environment.SetWinter, null, EBackground.None).Margin(0, 10))
            .Add(AxDivider("<color=yellow>Sun</color>"))
            .Add(AxText("<color=yellow>Sun color</color>", 25))
            .Add(AxMenuSliderFloat3(new[] { "<color=red>Red</color>", "<color=green>Green</color>", "<color=blue>Blue</color>" },
                LayoutMode.Vertical, 0, 1, new[] { SunLightManager._instance._sunLight.color.r, SunLightManager._instance._sunLight.color.g, SunLightManager._instance._sunLight.color.b }, 1f,
                new[] { Environment.SunColorRed, Environment.SunColorGreen, Environment.SunColorBlue }))
            .Add(AxMenuSliderFloat("Sun intensity", LabelPosition.Top, 0, 1000000, SunLightManager._instance._sunLight.intensity, 10f, Environment.SunIntensity))
            .Add(AxDivider("<color=yellow>Wind</color>"))
            .Add(AxMenuSliderFloat("Wind force (negative to default)", LabelPosition.Top, -0.1f, 1, -0.1f, 0.1f, Environment.WindIntensity))
            .Add(AxDivider("<color=yellow>Time</color>"))
            .Add(AxMenuInputText("Set day", LabelPosition.Left, "e.g 15", Environment.SetDayBuffer))
            .Add(AxMenuButton("Set", Environment.SetDay, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuCheckBox("Lock time", Environment.LockDaytime, Config.IsLockTime.Value))
            .Add(AxMenuSliderFloat("Daytime speed mul.", LabelPosition.Top, 0.5f, 100, TimeOfDayHolder.GetBaseSpeedMultiplier(), 0.5f, Environment.DaytimeSpeed))
            .Add(AxMenuSliderFloat("Tree regrow rate", LabelPosition.Top, 0, 1, UnityEngine.Object.FindObjectOfType<TreeRegrowChecker>()._regrowthFactor, 0.1f, Environment.TreeRegrowFactor));

        AxGetMainContainer((SPanelOptions)AxCreateSidePanel(TeleportPanel, false, "Teleport", Side.Right, 700, Color.black.WithAlpha(_panelAlpha), EBackground.None, false).Material(PanelBlur.GetForShade(_bgBlurAmount)).OverrideSorting(999).Active(false))
            .Add(AxMenuInputText("Go to item", LabelPosition.Left, "e.g shovel or 485", Teleport.GoToBuffer))
            .Add(AxMenuButton("Try go to", Teleport.GoToItem, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuCheckBox("Raycast teleport (RMB)", Teleport.SetRaycastMode))
            .Add(AxDivider("<color=yellow>Locations</color>"))
            .Add(AxMenuButtonText("Shotgun Grave", Teleport.TeleportTo, "-1340, 102, 1412", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Pistol loc.", Teleport.TeleportTo, "-1797, 16, 578", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Hang Glider 1st Loc.", Teleport.TeleportTo, "-1307, 87, 1732", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Knight V 1st Loc.", Teleport.TeleportTo, "-1026, 231, -625", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Top Of The Mountain", Teleport.TeleportTo, "4, 716, -459", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Rebreather cave", Teleport.TeleportTo, "-418, 19, 1532", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Flashlight Loc.", Teleport.TeleportTo, "-630, 142, 391", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Modern Axe Loc.", Teleport.TeleportTo, "-704, 108, 450", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Machete Loc.", Teleport.TeleportTo, "-65, 20, 1458", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Stun Baton Loc.", Teleport.TeleportTo, "-1142, 134, -157", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Putter Loc.", Teleport.TeleportTo, "1024, 145, 1212", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Revolver Loc.", Teleport.TeleportTo, "1111, 132, 1003", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Rope Gun Cave", Teleport.TeleportTo, "-1113, 132, -171", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Shovel Cave", Teleport.TeleportTo, "-531, 200, 124", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Compound Bow entrance", Teleport.TeleportTo, "-1136, 284, -1095", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Bunker Entertainment", Teleport.TeleportTo, "-1188, 70, 133", 30).Margin(0, 10))
            .Add(AxMenuButtonText("FireAxe bunker", Teleport.TeleportTo, "-474, 90, 710", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Crossbow Bunker", Teleport.TeleportTo, "-1014, 102, 1024", 30).Margin(0, 10))
            .Add(AxMenuButtonText("End Game Bunker", Teleport.TeleportTo, "1756, 45, 553", 30).Margin(0, 10));

        AxGetMainContainer((SPanelOptions)AxCreateSidePanel(ModdedItemsPanel, false, "Modded Items", Side.Right, 700, Color.black.WithAlpha(_panelAlpha), EBackground.None, false).Material(PanelBlur.GetForShade(_bgBlurAmount)).OverrideSorting(999).Active(false))
            // pistol
            .Add(AxDivider("<color=yellow>Pistol</color>"))
            .Add(AxMenuCheckBox("Rapid fire", ModdedItems.PistolRapidFire, Config.IsPistolRapidFire.Value))
            // shotgun
            .Add(AxDivider("<color=yellow>Shotgun</color>"))
            .Add(AxMenuCheckBox("Rapid fire", ModdedItems.ShotgunRapidFire, Config.IsShotgunRapidFire.Value))
            // revolver
            .Add(AxDivider("<color=yellow>Revolver</color>"))
            .Add(AxMenuCheckBox("Rapid fire", ModdedItems.RevolverRapidFire, Config.IsRevolverRapidFire.Value))
            // Rifle
            .Add(AxDivider("<color=yellow>Rifle</color>"))
            .Add(AxMenuCheckBox("Rapid fire", ModdedItems.RifleRapidFire, Config.IsRifleRapidFire.Value))
            // knightV
            .Add(AxDivider("<color=yellow>KnightV</color>"))
            .Add(AxMenuSliderFloat("Speed", LabelPosition.Top, 0, 100, LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerKnightVAction>()._controlDefinition.MaxVelocity, 1f, ModdedItems.KnightSpeed))
            .Add(AxMenuSliderFloat("Jump force", LabelPosition.Top, 0, 100, LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerKnightVAction>()._controlDefinition.JumpForce, 1f, ModdedItems.KnightJumpForce))
            // HangGlider
            .Add(AxDivider("<color=yellow>HangGlider</color>"))
            .Add(AxMenuSliderFloat("Speed", LabelPosition.Top, 0, 1000, LocalPlayer.SpecialActions.transform.GetComponentInChildren<PlayerHangGliderAction>()._constantForwardForce, 1f, ModdedItems.GliderSpeed))
            .Add(AxMenuCheckBox("No downforce", ModdedItems.GliderNoDownforce, Config.HangGliderNoDownforce.Value))
            // lighter
            .Add(AxDivider("<color=yellow>Lighter</color>"))
            .Add(AxMenuSliderFloat("Intensity (negative to default)", LabelPosition.Top, -1, 1000000, -1, 1000f, ModdedItems.LighterIntensity))
            .Add(AxMenuCheckBox("Increase range", ModdedItems.IncreaseLighterRange, Config.LighterIncRange.Value))
            // flashlight
            .Add(AxDivider("<color=yellow>Flashlight</color>"))
            .Add(AxMenuSliderFloat("Intensity (negative to default)", LabelPosition.Top, -1, 50, -1, 1, ModdedItems.FlashlightIntensity))
            .Add(AxMenuCheckBox("No drain", ModdedItems.FlashlightNoDrain, Config.FlashlightNoDrain.Value))
            // rebreather
            .Add(AxDivider("<color=yellow>Rebreather</color>"))
            .Add(AxMenuSliderFloat("Light intensity (negative to default)", LabelPosition.Top, -1, 50, -1, 1, ModdedItems.RebreatherLight))
            .Add(AxMenuCheckBox("Unlimited oxigen", ModdedItems.RebreatherNoOxigen, Config.RebreatherNoOxigen.Value))
            // RopeGun
            .Add(AxDivider("<color=yellow>Ropegun</color>"))
            .Add(AxMenuCheckBox("Infinite length", ModdedItems.RopegunInfinite, Config.IsRopegunInfinite.Value));

        AxGetMainContainer((SPanelOptions)AxCreateSidePanel(MiscPanel, false, "Miscellaneous", Side.Right, 700, Color.black.WithAlpha(_panelAlpha), EBackground.None, false).Material(PanelBlur.GetForShade(_bgBlurAmount)).OverrideSorting(999).Active(false))
            // Custom recipes
            .Add(AxDivider("<color=yellow>Custom recipes</color>"))
            .Add(AxMenuInputText("Ingredients id and count", LabelPosition.Left, "e.g 392x5 + 476x2 + ...", RecipeMaker.Ingredients))
            .Add(AxMenuInputText("Obtained item id", LabelPosition.Left, "e.g 367", RecipeMaker.ObtainedItemId))
            .Add(AxMenuButton("Try add recipe", RecipeMaker.AddCustomRecipe, null, EBackground.None).Margin(0, 10))
            .Add(AxDivider("<color=yellow>Debug</color>"))
            .Add(AxMenuButton("Dump items id in game folder", Debug.DumpItemsId, null, EBackground.None).Margin(0, 10))
            // Graphics settings
            .Add(AxDivider("<color=yellow>Graphics settings</color>"))
            .Add(AxText("<color=yellow>TAA Settings</color>", 25))
            .Add(AxMenuSliderFloat("TAA Sharpness", LabelPosition.Top, 0, 4, LocalPlayer.MainCam.GetComponent<HDAdditionalCameraData>().taaSharpenStrength, 0.1f, GraphicsSettings.SetTaaSharpness))
            .Add(AxMenuSliderFloat("TAA AntiFlicker strength", LabelPosition.Top, 0, 1, LocalPlayer.MainCam.GetComponent<HDAdditionalCameraData>().taaAntiFlicker, 0.05f, GraphicsSettings.SetTaaAntiFlicker))
            // Achievements
            .Add(AxDivider("<color=yellow>Achievements</color>"))
            .Add(AxMenuButton("Unlock all Achievements", Achievements.UnlockAllAchievements, _greenButtonCol, EBackground.None).Margin(0, 10))
            .Add(AxMenuButton("Lock all Achievements", Achievements.LockAllAchievements, null, EBackground.None).Margin(0, 10))
            // Mod menu settings
            .Add(AxDivider("<color=yellow>Mod menu settings</color>"))
            .Add(AxMenuCheckBox("Save mod menu settings on restart", Config._ShouldSaveSettings, Config.ShouldSaveSettings.Value));

        AxGetMainContainer((SPanelOptions)AxCreateSidePanel(MultiplayerPanel, false, "Multiplayer", Side.Right, 700, Color.black.WithAlpha(_panelAlpha), EBackground.None, false).Material(PanelBlur.GetForShade(_bgBlurAmount)).OverrideSorting(999).Active(false))
            .Add(AxDivider("<color=yellow>Host only</color>"))
            .Add(AxMenuInputText("Player limit (uncapped)", LabelPosition.Left, "e.g 10", MultiplayerSettings.PlayerLimitBuffer)
            .Add(AxMenuButtonText("<color=red>Set</color>", MultiplayerSettings.SetPlayerLimit, 40)));

        AxGetMainContainer((SPanelOptions)AxCreateSidePanel(EnemiesPanel, false, "Enemies/Characters", Side.Right, 700, Color.black.WithAlpha(_panelAlpha), EBackground.None, false).Material(PanelBlur.GetForShade(_bgBlurAmount)).OverrideSorting(999).Active(false))
            // Spawn enemies
            .Add(AxDivider("<color=yellow>Spawn enemy</color>"))
            .Add(AxMenuButtonText("Cannibal", Actors.SpawnActor, "cannibal", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Fat", Actors.SpawnActor, "fat", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Muddymale", Actors.SpawnActor, "muddymale", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Muddyfemale", Actors.SpawnActor, "muddyfemale", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Heavy", Actors.SpawnActor, "heavy", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Caterpillar", Actors.SpawnActor, "caterpillar", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Baby", Actors.SpawnActor, "baby", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Twins", Actors.SpawnActor, "twins", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Demon", Actors.SpawnActor, "demon", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Demonboss", Actors.SpawnActor, "demonboss", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Fingers", Actors.SpawnActor, "fingers", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Misspuffy", Actors.SpawnActor, "misspuffy", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Mrpuffy", Actors.SpawnActor, "mrpuffy", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Mrpuffton", Actors.SpawnActor, "mrpuffton", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Creepy virginia", Actors.SpawnActor, "creepyvirginia", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Armsy", Actors.SpawnActor, "armsy", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Frank", Actors.SpawnActor, "frank", 30).Margin(0, 10))
            .Add(AxMenuButtonText("FinalBoss", Actors.SpawnActor, "boss", 30).Margin(0, 10))
            // Spawn characters
            .Add(AxDivider("<color=yellow>Spawn characters</color>"))
            .Add(AxMenuButtonText("Kelvin", Actors.SpawnActor, "robby", 30).Margin(0, 10))
            .Add(AxMenuButtonText("Virginia", Actors.SpawnActor, "virginia", 30).Margin(0, 10))
            // Misc
            .Add(AxDivider("<color=yellow>Misc</color>"))
            .Add(AxMenuButton("Kill enemies", Actors.KillEnemies, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuButton("Kill animals", Actors.KillAnimals, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuButton("Burn enemies", Actors.BurnEnemies, null, EBackground.None).Margin(0, 10))
            .Add(AxMenuCheckBox("Freeze actors", Actors.FreezeActors));

        if (AxelModMenu.TryGetEmbeddedResourceBytes("map", out var mapBytes) && AxelModMenu.TryGetEmbeddedResourceBytes("playerdot", out var playerdotBytes))
        {
            Texture mapTex = ResourcesLoader.ByteToTex(mapBytes);
            Texture playerdotTex = ResourcesLoader.ByteToTex(playerdotBytes);

            var mapPanel = AxCreatePanel("InteractiveMap", false, new Vector2(720, 720), AnchorType.BottomLeft, Color.black, EBackground.None, true).BindVisibility(Teleport.ShowMap)
                - SImage.Dock(EDockType.Fill).Texture(mapTex).OnClick(Teleport.InteractiveMap);

            var coordsPanel = AxCreatePanel("CoordsPanel", false, new Vector2(45, 65), AnchorType.TopLeft, Color.black.WithAlpha(0.4f), EBackground.None).Vertical(2, "EC").Padding(2)
                - AxTextDynamic(PlayerSettings.PlayerPos.X, 16, TMPro.TextAlignmentOptions.Left).FontColor(new Color32(211, 31, 52, 255))
                - AxTextDynamic(PlayerSettings.PlayerPos.Y, 16, TMPro.TextAlignmentOptions.Left).FontColor(new Color32(211, 31, 52, 255))
                - AxTextDynamic(PlayerSettings.PlayerPos.Z, 16, TMPro.TextAlignmentOptions.Left).FontColor(new Color32(211, 31, 52, 255));
            mapPanel.Add(coordsPanel);

            MapDot = SImage.Texture(playerdotTex);
            MapDot.Pivot(0.5f, 0.5f);
            MapDot.Size(16, 16);
            mapPanel.Add(MapDot);
        }

        WorldEditPanel = (SPanelOptions)((SPanelOptions)AxCreatePanel("WorldEdit", false, new Vector2(700, 300), AnchorType.TopLeft, Color.black.WithAlpha(0.9f), EBackground.Sons).Vertical(4, "EC").PaddingVertical(10).PaddingHorizontal(25).BindVisibility(WorldEdit.IsWorldEditMode)
            - AxTextDynamic(WorldEdit.WorldEditInfo.PointedObject, 18, TMPro.TextAlignmentOptions.Left)
            - AxTextDynamic(WorldEdit.WorldEditInfo.ActiveObject, 18, TMPro.TextAlignmentOptions.Left)
            - AxTextDynamic(WorldEdit.WorldEditInfo.SnapToPlayer, 18, TMPro.TextAlignmentOptions.Left)
            - AxTextDynamic(WorldEdit.CurrRotationAxisString, 18, TMPro.TextAlignmentOptions.Left)
            - AxText("", 10, TMPro.TextAlignmentOptions.Left)
            - AxText("<color=#ffff66>Usage info:</color>", 22, TMPro.TextAlignmentOptions.Left)
            - AxText("Interaction key (E): select pointed object", 18, TMPro.TextAlignmentOptions.Left)
            - AxText("Right mouse button: release selected object", 18, TMPro.TextAlignmentOptions.Left)
            - AxText("Up/Down arrow keys: move object up and down (when snap to player is off)", 18, TMPro.TextAlignmentOptions.Left)
            - AxText("Q/R keys: rotate object on selected axis", 18, TMPro.TextAlignmentOptions.Left)
            - AxText("ScrollWheel: scale object up/down", 18, TMPro.TextAlignmentOptions.Left)
            - AxText("Capslock toggle: snap object to player", 18, TMPro.TextAlignmentOptions.Left)
            - AxText("Delete key: destroy selected object", 18, TMPro.TextAlignmentOptions.Left));
    }
}