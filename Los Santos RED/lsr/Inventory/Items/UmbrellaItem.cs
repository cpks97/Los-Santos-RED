﻿using LosSantosRED.lsr.Interface;
using LosSantosRED.lsr.Player;
using System;
using System.Xml.Serialization;

[Serializable()]
public class UmbrellaItem : ModItem
{   
    public UmbrellaItem()
    {
        ToolType = ToolTypes.Umbrella;
    }
    public UmbrellaItem(string name, string description) : base(name, description, ItemType.Tools)
    {
        ToolType = ToolTypes.Umbrella;
    }
    public UmbrellaItem(string name) : base(name, ItemType.Tools)
    {
        ToolType = ToolTypes.Umbrella;
    }
    public override bool UseItem(IActionable actionable, ISettingsProvideable settings, IEntityProvideable world, ICameraControllable cameraControllable, IIntoxicants intoxicants)
    {
        EntryPoint.WriteToConsole("I AM IN Umbrella ACTIVITY!!!!!!!!!!");
        if (actionable.IsOnFoot && !actionable.ActivityManager.IsResting && actionable.ActivityManager.CanUseItemsBase)
        {
            actionable.ActivityManager.StartUpperBodyActivity(new UmbrellaActivity(actionable, this));
            return true;
        }
        return false;
    }
}

