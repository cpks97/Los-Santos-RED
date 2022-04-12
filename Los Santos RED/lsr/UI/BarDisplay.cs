﻿using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BarDisplay
{
    private IDisplayable Player;
    private ISettingsProvideable Settings;

    public BarDisplay(IDisplayable player, ISettingsProvideable settings)
    {
        Player = player;
        Settings = settings;
    }

    public void Draw(float bar1Percent, float bar2Percent, float bar3Percent)
    {
        if (Settings.SettingsManager.UISettings.ShowStaminaBar && NativeFunction.Natives.IS_HUD_COMPONENT_ACTIVE<bool>(0))
        {
            DrawBackground();
            DrawBar1(bar1Percent);
            DrawBar2(bar2Percent);
            DrawBar3(bar3Percent);



            //DrawSpeed();


        }
    }
    //private void DrawSpeed()
    //{
    //    float width = 0.025f;
    //    float height = 0.05f;


        
    //    NativeFunction.Natives.DRAW_RECT(Settings.SettingsManager.UISettings.SpeedLimitPositionY, Settings.SettingsManager.UISettings.SpeedLimitPositionX, width + 0.001f, height + 0.001f, 0, 0, 0, 255, false);
    //    NativeFunction.Natives.DRAW_RECT(Settings.SettingsManager.UISettings.SpeedLimitPositionY, Settings.SettingsManager.UISettings.SpeedLimitPositionX, width, height, 255, 255, 255, 255, false);


    //}
    private void DrawBar1(float percentage)
    {
        float BackWidth = 0.07f;
        float BackPosX = 0.015f + (BackWidth / 2);
        float PosY = 0.992f;
        float BackPosY = PosY;
        float BackHeight = 0.0075f;
        float FrontWidth = BackWidth * percentage;
        float FrontPosX = BackPosX;
        float FrontPosY = PosY;
        float FrontHeight = 0.0075f;
        FrontPosX = FrontPosX - ((BackWidth - FrontWidth) / 2);
        NativeFunction.Natives.DRAW_RECT(BackPosX, BackPosY, BackWidth, BackHeight, 142, 50, 50, 100, false);
        NativeFunction.Natives.DRAW_RECT(FrontPosX, FrontPosY, FrontWidth, FrontHeight, 181, 48, 48, 255, false);
    }
    private void DrawBar2(float percentage)
    {
        float BackWidth = 0.0335f;
        float BackPosX = 0.0867f + (BackWidth / 2);
        float PosY = 0.992f;
        float BackPosY = PosY;

        float BackHeight = 0.0075f;
        float FrontWidth = BackWidth * Player.IntoxicatedIntensityPercent;
        float FrontPosX = BackPosX;
        float FrontPosY = PosY;
        float FrontHeight = 0.0075f;
        FrontPosX = FrontPosX - ((BackWidth - FrontWidth) / 2);
        NativeFunction.Natives.DRAW_RECT(BackPosX, BackPosY, BackWidth, BackHeight, 72, 133, 164, 100, false);
        NativeFunction.Natives.DRAW_RECT(FrontPosX, FrontPosY, FrontWidth, FrontHeight, 72, 133, 164, 255, false);
    }
    private void DrawBar3(float percentage)
    {
        float BackWidth = 0.0335f;
        float BackPosX = 0.121875f + (BackWidth / 2);
        float PosY = 0.992f;//0.9925f;
        float BackPosY = PosY;
        float BackHeight = 0.0075f;
        float FrontWidth = BackWidth * percentage;// DisplayablePlayer.StaminaPercent;
        float FrontPosX = BackPosX;
        float FrontPosY = PosY;
        float FrontHeight = 0.0075f;
        FrontPosX = FrontPosX - ((BackWidth - FrontWidth) / 2);
        NativeFunction.Natives.DRAW_RECT(BackPosX, BackPosY, BackWidth, BackHeight, 202, 169, 66, 100, false);
        NativeFunction.Natives.DRAW_RECT(FrontPosX, FrontPosY, FrontWidth, FrontHeight, 202, 169, 66, 255, false);
    }
    private void DrawBackground()
    {
        float BackWidth = 0.14075f;//0.07f;
        float BackPosX = 0.015f + (BackWidth / 2);//0.08525f;
        float BackPosY = 0.992f;//0.9925f;
        float BackHeight = 0.015f;//0.0075f;
        NativeFunction.Natives.DRAW_RECT(BackPosX, BackPosY, BackWidth, BackHeight, 0, 0, 0, 125, false);
    }
}

