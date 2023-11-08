﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

public class ActivitySettings : ISettingsDefaultable
{

    [Description("Will teleport to the sitting entry point instead of walking. Useful when there are objects in the way like a large table you dont want to hit.")]
    public bool TeleportWhenSitting { get; set; }
    [Description("Allow the player to start converstaions with random peds.")]
    public bool AllowPedConversations { get; set; }
    [Description("Allow the player to loot dead or unconscious peds.")]
    public bool AllowPedInspecting { get; set; }
    [Description("Allow the player to hold up peds.")]
    public bool AllowPedHoldUps { get; set; }
    [Description("Maximum distance in meters that you can hold up a ped.")]
    public float HoldUpDistance { get; set; }
    [Description("Allow the player to drag dead or unconscious peds.")]
    public bool AllowDraggingOtherPeds { get; set; }
    [Description("Plays an animation the dragged ped.")]
    public bool PlayDraggingPedAnimation { get; set; }
    [Description("Allow the player to hold a ped hostage at gunpoint.")]
    public bool AllowGrabbingPeds { get; set; }
    [Description("Allow the player to start random scenarios around the world.")]
    public bool AllowStartingScenarios { get; set; }
    [Description("Allow the player to go into crouch mode.")]
    public bool AllowPlayerCrouching { get; set; }
    [Description("Change player movement when in crouch mode.")]
    public bool CrouchingAdjustsMovementSpeed { get; set; }
    [Description("Amount of override player movement when crouching.")]
    public float CrouchMovementSpeedOverride { get; set; }
    [Description("Set a cinematic camera when sitting")]
    public bool UseAltCameraWhenSitting { get; set; }
    [Description("Force sitting when close to a seat")]
    public bool ForceSitWhenClose { get; set; }
    [Description("Time (in ms) before force sit kicks in")]
    public uint ForceSitTimeOut { get; set; }
    [Description("Distance (in meters) before force sit kicks in")]
    public float ForceSitDistance { get; set; }
    [Description("Distance (in meters) to slide when sitting")]
    public float SittingSlideDistance { get; set; }
    [Description("Will add an exclamation point to the prompt if the ped wants to buy an item you have")]
    public bool ShowInPromptWhenPedsWantToBuyItemsYouHave { get; set; }
    [Description("Extended DIstance added when changing plates")]
    public float PlateTheftFloat { get; set; }
    [Description("Time Between sips when drinking")]
    public uint DrinkTimeBetween { get; set; }
    [Description("Does the base animation play when drinking")]
    public bool DrinkStartsBase { get; set; }
    [Description("Show body armor on player when using the freemode characters. NOT FULLY IMPLEMENTED")]
    public bool DisplayBodyArmor { get; set; }
    [Description("NOT FULLY IMPLEMENTED")]
    public int BodyArmorDefaultDrawableID { get; set; }
    [Description("NOT FULLY IMPLEMENTED")]
    public int BodyArmorDefaultTextureID { get; set; }
    [Description("Grab Attach Extra X Distance")]
    public float GrabAttachX { get;  set; }
    [Description("Grab Attach Extra Y Distance")]
    public float GrabAttachY { get; set; }
    [Description("Grab Attach Extra Z Distance")]
    public float GrabAttachZ { get; set; }
    [Description("Human Shield Attach Extra X Distance")]
    public float HumanShieldAttachX { get; set; }
    [Description("Human Shield Attach Extra Y Distance")]
    public float HumanShieldAttachY { get; set; }
    [Description("Human Shield Attach Extra Z Distance")]
    public float HumanShieldAttachZ { get; set; }




    [OnDeserialized()]
    private void SetValuesOnDeserialized(StreamingContext context)
    {
        SetDefault();
    }

    public ActivitySettings()
    {
        SetDefault();
    }
    public void SetDefault()
    {
        TeleportWhenSitting = false;
        AllowPedConversations = true;
        AllowPedInspecting = true;
        AllowPedHoldUps = true;
        AllowDraggingOtherPeds = true;
        AllowGrabbingPeds = true;
        AllowStartingScenarios = false;
        AllowPlayerCrouching = true;
        PlayDraggingPedAnimation = true;
        CrouchingAdjustsMovementSpeed = true;
        CrouchMovementSpeedOverride = 5.0f;
        UseAltCameraWhenSitting = false;
        ForceSitWhenClose = true;
        ForceSitTimeOut = 3000;
        ForceSitDistance = 0.7f;
        SittingSlideDistance = 0.5f;//0.1f
        ShowInPromptWhenPedsWantToBuyItemsYouHave = true;
        HoldUpDistance = 15f;
        PlateTheftFloat = 1.0f;
        DrinkTimeBetween = 0;
        DrinkStartsBase = false;
        DisplayBodyArmor = false;
        BodyArmorDefaultDrawableID = 11;
        BodyArmorDefaultTextureID = 1;

//#if DEBUG
//        DisplayBodyArmor = true;
//#endif  


        GrabAttachX = -0.31f;
        GrabAttachY = 0.35f;
        GrabAttachZ = 0.04f;


        HumanShieldAttachX = -0.31f;
        HumanShieldAttachY = 0.12f;
        HumanShieldAttachZ = 0.04f;

    }
}