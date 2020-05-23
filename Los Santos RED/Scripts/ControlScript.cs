﻿using ExtensionsMethods;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Extensions = ExtensionsMethods.Extensions;

public static class ControlScript
{
    private static Police.PoliceState HandsUpPreviousPoliceState;
    public static bool IsRunning { get; set; }
    public static void Initialize()
    {
        IsRunning = true;
    }
    public static void Dispose()
    {
        IsRunning = false;
    }
    public static void Tick()
    {
        if (IsRunning)
        {
            if (Game.IsKeyDownRightNow(General.MySettings.KeyBinding.SurrenderKey) && !Game.IsShiftKeyDownRightNow && !Game.IsControlKeyDownRightNow && !Game.LocalPlayer.IsFreeAiming && (!Game.LocalPlayer.Character.IsInAnyVehicle(false) || Game.LocalPlayer.Character.CurrentVehicle.Speed < 2.5f))
            {
                if (!PlayerState.HandsAreUp && !PlayerState.IsBusted)
                {
                    General.SetPedUnarmed(Game.LocalPlayer.Character, false);
                    HandsUpPreviousPoliceState = Police.CurrentPoliceState;
                    Surrendering.RaiseHands();
                    if (Game.LocalPlayer.Character.IsInAnyVehicle(false) && Game.LocalPlayer.Character.CurrentVehicle.Speed <= 10f)
                        Game.LocalPlayer.Character.CurrentVehicle.IsDriveable = false;
                }
            }
            else
            {
                if (PlayerState.HandsAreUp && !PlayerState.IsBusted)
                {
                    PlayerState.HandsAreUp = false; // You put your hands down
                    Police.CurrentPoliceState = HandsUpPreviousPoliceState;
                    Surrendering.LowerHands();
                    if (Game.LocalPlayer.Character.IsInAnyVehicle(false))
                        Game.LocalPlayer.Character.CurrentVehicle.IsDriveable = true;
                }
            }

            if (Game.IsKeyDownRightNow(General.MySettings.KeyBinding.SurrenderKey) && Game.IsShiftKeyDownRightNow && !Game.LocalPlayer.Character.IsInAnyVehicle(false))
            {
                Surrendering.CommitSuicide(Game.LocalPlayer.Character);
            }

            if (Game.IsKeyDownRightNow(General.MySettings.KeyBinding.SurrenderKey) && Game.IsControlKeyDownRightNow)
            {
                PlayerHealth.BandagePed(Game.LocalPlayer.Character);
            }
        }
    }
}