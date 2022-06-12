﻿using ExtensionsMethods;
using LosSantosRED.lsr.Interface;
using LosSantosRED.lsr.Player.Activity;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;

namespace LosSantosRED.lsr.Player
{
    public class DrinkingActivity : DynamicActivity
    {
        private Rage.Object Bottle;
        private string PlayingAnim;
        private string PlayingDict;
        private DrinkingData Data;
        private bool IsAttachedToHand;
        private bool IsCancelled;
        private IIntoxicatable Player;
        private ISettingsProvideable Settings;
        private IIntoxicants Intoxicants;
        private Intoxicant CurrentIntoxicant;
        private bool hasGainedHP = false;
        private uint GameTimeLastGivenHealth;
        private int HealthGiven;
        private int TimesDrank;

        public DrinkingActivity(IIntoxicatable consumable, ISettingsProvideable settings) : base()
        {
            Player = consumable;
            Settings = settings;
        }
        public DrinkingActivity(IIntoxicatable consumable, ISettingsProvideable settings, ModItem modItem, IIntoxicants intoxicants) : base()
        {
            Player = consumable;
            Settings = settings;
            ModItem = modItem;
            Intoxicants = intoxicants;
        }
        public override ModItem ModItem { get; set; }
        public override string DebugString => $"Intox {Player.IsIntoxicated} Consum: {Player.IsPerformingActivity} I: {Player.IntoxicatedIntensity}";
        public override bool CanPause { get; set; } = false;
        public override bool CanCancel { get; set; } = true;
        public override void Cancel()
        {
            IsCancelled = true;
            Player.IsPerformingActivity = false;
            Player.Intoxication.StopIngesting(CurrentIntoxicant);
        }
        public override void Pause()
        {
            Cancel();//for now it just cancels
        }
        public override bool IsPaused() => false;
        public override void Continue()
        {
        }
        public override void Start()
        {
            Setup();
            GameFiber ScenarioWatcher = GameFiber.StartNew(delegate
            {
                Enter();
            }, "DrinkingWatcher");
        }
        private void AttachBottleToHand()
        {
            CreateBottle();
            if (Bottle.Exists() && !IsAttachedToHand)
            {
                Bottle.AttachTo(Player.Character, NativeFunction.CallByName<int>("GET_PED_BONE_INDEX", Player.Character, Data.HandBoneID), Data.HandOffset, Data.HandRotator);
                IsAttachedToHand = true;
                Player.AttachedProp = Bottle;
            }
        }
        private void CreateBottle()
        {
            if (!Bottle.Exists() && Data.PropModelName != "")
            {
                try 
                {
                    Bottle = new Rage.Object(Data.PropModelName, Player.Character.GetOffsetPositionUp(50f));
                }
                catch (Exception e)
                {
                    Game.DisplayNotification($"Could Not Spawn Prop {Data.PropModelName}");
                }
                if (Bottle.Exists())
                {
                    //Bottle.IsGravityDisabled = false;
                }
                else
                {
                    IsCancelled = true;
                }
            }
        }
        private void Enter()
        {
            Player.SetUnarmed();
            AttachBottleToHand();
            Player.IsPerformingActivity = true;
            PlayingDict = Data.AnimEnterDictionary;
            PlayingAnim = Data.AnimEnter;
            NativeFunction.CallByName<uint>("TASK_PLAY_ANIM", Player.Character, Data.AnimEnterDictionary, Data.AnimEnter, 1.0f, -1.0f, -1, 50, 0, false, false, false);//-1
            Idle();
        }
        private void Exit()
        {
            if (Bottle.Exists())
            {
                Bottle.Detach();
            }
            NativeFunction.Natives.CLEAR_PED_SECONDARY_TASK(Player.Character);
            Player.IsPerformingActivity = false;
            Player.Intoxication.StopIngesting(CurrentIntoxicant);

            if(ModItem?.CleanupItemImmediately == false)
            {
                GameFiber.Sleep(5000);
            }
            if (Bottle.Exists())
            {
                Bottle.Delete();
            }
        }
        private void Idle()
        {
            //if (Player.CanPerformActivities && !IsCancelled)
            //{
            //    Player.ButtonPrompts.AddPrompt("DrinkingActivity", "Stop Drinking", "StopDrink", Settings.SettingsManager.KeySettings.InteractCancel, 999);
            //}
            uint GameTimeBetweenDrinks = 1100;
            uint GameTimeLastChangedIdle = Game.GameTime;
            while (Player.CanPerformActivities && !IsCancelled)
            {
                Player.SetUnarmed();
                float AnimationTime = NativeFunction.CallByName<float>("GET_ENTITY_ANIM_CURRENT_TIME", Player.Character, PlayingDict, PlayingAnim);
                if (AnimationTime >= 1.0f)
                {
                    if (TimesDrank >= 8 && (HealthGiven == ModItem.HealthChangeAmount || Player.Character.Health == Player.Character.MaxHealth))
                    {
                        IsCancelled = true;
                    }
                    else if(Game.GameTime - GameTimeLastChangedIdle >= GameTimeBetweenDrinks)
                    {
                        TimesDrank++;
                        PlayingDict = Data.AnimIdleDictionary;
                        PlayingAnim = Data.AnimIdle.PickRandom();
                        NativeFunction.CallByName<uint>("TASK_PLAY_ANIM", Player.Character, PlayingDict, PlayingAnim, 1.0f, -1.0f, -1, 50, 0, false, false, false);
                        EntryPoint.WriteToConsole($"New Drinking Idle {PlayingAnim} TimesDrank {TimesDrank} HealthGiven {HealthGiven}", 5);
                    }
                }
                //if (Player.ButtonPrompts.IsPressed("StopDrink"))
                //{
                //    Player.ButtonPrompts.RemovePrompts("DrinkingActivity");
                //    IsCancelled = true;
                //}
                UpdateHealthGain();
                GameFiber.Yield();
            }
            //Player.ButtonPrompts.RemovePrompts("DrinkingActivity");
            Exit();
        }
        private void UpdateHealthGain()
        {
            if (Game.GameTime - GameTimeLastGivenHealth >= 1000)
            {
                if (ModItem.ChangesHealth)
                {
                    if(ModItem.HealthChangeAmount > 0 && HealthGiven < ModItem.HealthChangeAmount)
                    {
                        HealthGiven++;
                        Player.ChangeHealth(1);
                    }
                    else if (ModItem.HealthChangeAmount < 0 && HealthGiven > ModItem.HealthChangeAmount)
                    {
                        HealthGiven--;
                        Player.ChangeHealth(-1);
                    }
                }
                GameTimeLastGivenHealth = Game.GameTime;
            }
        }
        private void Setup()
        {
            List<string> AnimIdle;
            string AnimEnter;
            string AnimEnterDictionary;
            string AnimExit;
            string AnimExitDictionary;
            string AnimIdleDictionary;
            int HandBoneID;
            Vector3 HandOffset = Vector3.Zero;
            Rotator HandRotator = Rotator.Zero;
            string PropModel = "";


            bool isBottle = false;

            if (ModItem != null && ModItem.Name.ToLower().Contains("bottle"))
            {
                isBottle = true;
            }
            EntryPoint.WriteToConsole($"Drinking Start isBottle {isBottle}");



            HandBoneID = 18905;
            HandOffset = new Vector3(0.12f, -0.07f, 0.07f);
            HandRotator = new Rotator(-110.0f, 14.0f, 1.0f);



            if (ModItem != null && ModItem.ModelItem != null)
            {
                PropModel = ModItem.ModelItem.ModelName;
                HandBoneID = ModItem.ModelItem.AttachBoneIndex;
                HandOffset = ModItem.ModelItem.AttachOffset;
                HandRotator = ModItem.ModelItem.AttachRotation;
            }






            if (Player.IsInVehicle)
            {
                if (Player.IsDriver)
                {
                    if (isBottle)
                    {
                        AnimEnterDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ds@base";
                        AnimEnter = "enter";
                        AnimExitDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ds@base";
                        AnimExit = "exit";
                        AnimIdleDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ds@base";
                        AnimIdle = new List<string>() { "idle_a" };
                    }
                    else
                    {
                        AnimEnterDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ds@base";
                        AnimEnter = "enter";
                        AnimExitDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ds@base";
                        AnimExit = "exit";
                        AnimIdleDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ds@base";
                        AnimIdle = new List<string>() { "idle_a" };
                    }
                }
                else
                {
                    if (isBottle)
                    {
                        AnimEnterDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ps@base";
                        AnimEnter = "enter";
                        AnimExitDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ps@base";
                        AnimExit = "exit";
                        AnimIdleDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ps@base";
                        AnimIdle = new List<string>() { "idle_a" };
                    }
                    else
                    {
                        AnimEnterDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ps@base";
                        AnimEnter = "enter";
                        AnimExitDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ps@base";
                        AnimExit = "exit";
                        AnimIdleDictionary = "amb@code_human_in_car_mp_actions@drink_bottle@std@ps@base";
                        AnimIdle = new List<string>() { "idle_a" };
                    }
                }
            }
            else
            {
                if (isBottle)
                {
                    AnimEnterDictionary = "mp_player_intdrink";
                    AnimEnter = "intro_bottle";
                    AnimExitDictionary = "mp_player_intdrink";
                    AnimExit = "outro_bottle";
                    AnimIdleDictionary = "mp_player_intdrink";
                    AnimIdle = new List<string>() { "loop_bottle" };
                }
                else
                {
                    AnimEnterDictionary = "mp_player_intdrink";
                    AnimEnter = "intro";
                    AnimExitDictionary = "mp_player_intdrink";
                    AnimExit = "outro";
                    AnimIdleDictionary = "mp_player_intdrink";
                    AnimIdle = new List<string>() { "loop" };
                }
            }

            if (ModItem != null && ModItem.IsIntoxicating)
            {
                CurrentIntoxicant = Intoxicants.Get(ModItem.IntoxicantName);
                Player.Intoxication.StartIngesting(CurrentIntoxicant);
            }
            AnimationDictionary.RequestAnimationDictionay(AnimIdleDictionary);
            AnimationDictionary.RequestAnimationDictionay(AnimEnterDictionary);
            AnimationDictionary.RequestAnimationDictionay(AnimExitDictionary);
            Data = new DrinkingData(AnimEnter, AnimEnterDictionary, AnimExit, AnimExitDictionary, AnimIdle, AnimIdleDictionary, HandBoneID, HandOffset, HandRotator, PropModel);
        }
        //private void Setup()
        //{
        //    List<string> AnimIdle;
        //    string AnimEnter;
        //    string AnimEnterDictionary;
        //    string AnimExit;
        //    string AnimExitDictionary;
        //    string AnimIdleDictionary;
        //    int HandBoneID;
        //    Vector3 HandOffset;
        //    Rotator HandRotator;
        //    string PropModel = "";
        //    if (Player.ModelName.ToLower() == "player_zero" || Player.ModelName.ToLower() == "player_one" || Player.ModelName.ToLower() == "player_two" || Player.IsMale)
        //    {
        //        AnimEnterDictionary = "amb@world_human_drinking@beer@male@enter";
        //        AnimEnter = "enter";
        //        AnimExitDictionary = "amb@world_human_drinking@beer@male@exit";
        //        AnimExit = "exit";
        //        AnimIdleDictionary = "amb@world_human_drinking@beer@male@idle_a";
        //        AnimIdle = new List<string>() { "idle_a", "Idle_b", "Idle_c" };
        //        HandBoneID = 57005;
        //        HandOffset = new Vector3(0.12f, 0.0f, -0.06f);
        //        HandRotator = new Rotator(-77.0f, 23.0f, 0.0f);


        //        AnimEnterDictionary = "amb@world_human_drinking@coffee@male@enter";
        //        AnimEnter = "enter";
        //        AnimExitDictionary = "amb@world_human_drinking@coffee@male@exit";
        //        AnimExit = "exit";
        //        AnimIdleDictionary = "amb@world_human_drinking@coffee@male@idle_a";
        //        AnimIdle = new List<string>() { "idle_a", "Idle_b", "Idle_c" };

        //    }
        //    else
        //    {
        //        AnimEnterDictionary = "amb@world_human_drinking@beer@female@enter";
        //        AnimEnter = "enter";
        //        AnimExitDictionary = "amb@world_human_drinking@beer@female@exit";
        //        AnimExit = "exit";
        //        AnimIdleDictionary = "amb@world_human_drinking@beer@female@idle_a";
        //        AnimIdle = new List<string>() { "idle_a", "Idle_b", "Idle_c" };
        //        HandBoneID = 57005;
        //        HandOffset = new Vector3(0.12f, 0.0f, -0.06f);
        //        HandRotator = new Rotator(-77.0f, 23.0f, 0.0f);


        //        AnimEnterDictionary = "amb@world_human_drinking@coffee@female@enter";
        //        AnimEnter = "enter";
        //        AnimExitDictionary = "amb@world_human_drinking@coffee@female@exit";
        //        AnimExit = "exit";
        //        AnimIdleDictionary = "amb@world_human_drinking@coffee@female@idle_a";
        //        AnimIdle = new List<string>() { "idle_a", "Idle_b", "Idle_c" };
        //    }





        //    //need left hand and into end exit?
        //    //amb@code_human_in_car_mp_actions@drink@std@ds@base enter
        //    //amb@code_human_in_car_mp_actions@drink_bottle@std@ps@base exit


        //    if (ModItem != null && ModItem.ModelItem != null)
        //    {
        //        PropModel = ModItem.ModelItem.ModelName;
        //        HandBoneID = ModItem.ModelItem.AttachBoneIndex;
        //        HandOffset = ModItem.ModelItem.AttachOffset;
        //        HandRotator = ModItem.ModelItem.AttachRotation;
        //    }


        //    //if (Player.IsSitting || Player.IsInVehicle)
        //    //{
        //    //    HandBoneID = 18905;
        //    //    AnimEnterDictionary = "amb@code_human_in_car_mp_actions@drink@std@ds@base";
        //    //    AnimEnter = "enter";
        //    //    AnimExitDictionary = "amb@code_human_in_car_mp_actions@drink@std@ds@base";
        //    //    AnimExit = "exit";
        //    //    AnimIdleDictionary = "amb@code_human_in_car_mp_actions@drink@std@ds@base";
        //    //    AnimIdle = new List<string>() { "idle_a" };
        //    //}


        //    if (ModItem != null && ModItem.IsIntoxicating)
        //    {
        //        CurrentIntoxicant = Intoxicants.Get(ModItem.IntoxicantName);
        //        Player.Intoxication.StartIngesting(CurrentIntoxicant);
        //    }
        //    AnimationDictionary.RequestAnimationDictionay(AnimIdleDictionary);
        //    AnimationDictionary.RequestAnimationDictionay(AnimEnterDictionary);
        //    AnimationDictionary.RequestAnimationDictionay(AnimExitDictionary);
        //    Data = new DrinkingData(AnimEnter, AnimEnterDictionary, AnimExit, AnimExitDictionary, AnimIdle, AnimIdleDictionary, HandBoneID, HandOffset, HandRotator, PropModel);
        //}
    }
}