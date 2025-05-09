﻿using ExtensionsMethods;
using LosSantosRED.lsr.Interface;
using LosSantosRED.lsr.Player.Activity;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LosSantosRED.lsr.Player
{
    public class IngestActivityNew : DynamicActivity
    {
        private Intoxicant CurrentIntoxicant;
        private EatingData Data;
        private bool hasGainedHP = false;
        private IIntoxicants Intoxicants;
        private bool IsAttachedToHand;
        private bool IsCancelled;
        private Rage.Object Item;
        private IActionable Player;
        private string PlayingAnim;
        private string PlayingDict;
        private ISettingsProvideable Settings;
        private IngestItem IngestItem;
        private ConsumableRefresher ConsumableItemNeedGain;
        private AnimationWatcher AnimationWatcher;
        private bool IsPlayingPillTakeAnimation;

        public IngestActivityNew(IActionable consumable, ISettingsProvideable settings, IngestItem modItem, IIntoxicants intoxicants) : base()
        {
            Player = consumable;
            Settings = settings;
            ModItem = modItem;
            IngestItem = modItem;
            Intoxicants = intoxicants;
        }
        public override string DebugString => $"";
        public override ModItem ModItem { get; set; }
        public override bool CanPause { get; set; } = false;
        public override bool CanCancel { get; set; } = true;
        public override bool IsUpperBodyOnly { get; set; } = true;
        public override string PausePrompt { get; set; } = "Pause Taking Pills";
        public override string CancelPrompt { get; set; } = "Stop Taking Pills";
        public override string ContinuePrompt { get; set; } = "Continue Taking Pills";
        public override void Cancel()
        {
            IsCancelled = true;
            Player.ActivityManager.IsPerformingActivity = false;
            Player.ActivityManager.IsUsingIllegalItem = false;
            Player.Intoxication.StopIngesting(CurrentIntoxicant);
        }
        public override void Continue()
        {
        }
        public override void Pause()
        {
            Cancel();//for now it just cancels
        }
        public override bool IsPaused() => false;
        public override void Start()
        {
            Setup();
            GameFiber ScenarioWatcher = GameFiber.StartNew(delegate
            {
                try
                {
                    Enter();
                }
                catch (Exception ex)
                {
                    EntryPoint.WriteToConsole(ex.Message + " " + ex.StackTrace, 0);
                    EntryPoint.ModController.CrashUnload();
                }
            }, "DrinkingWatcher");
        }
        public override bool CanPerform(IActionable player)
        {
            if (player.ActivityManager.CanPerformActivitiesMiddle)
            {
                return true;
            }
            Game.DisplayHelp($"Cannot Start Activity: {ModItem?.Name}");
            return false;
        }
        private void Enter()
        {
            Player.WeaponEquipment.SetUnarmed();
            AttachItemToHand();
            CancelPrompt = $"Stop Taking {IngestItem.Name}";
            Player.ActivityManager.IsPerformingActivity = true;
            if (ModItem.IsPublicUseIllegal)
            {
                Player.ActivityManager.IsUsingIllegalItem = true;
            }

            //Idle();
            PlayBase();
        }
        private void PlayBase()
        {
            while (Player.ActivityManager.CanPerformActivitiesMiddle && !IsCancelled)
            {
                DisableControls();
                if (!IsPlayingPillTakeAnimation && Player.ButtonPrompts.IsPressed("IngestTakePill"))
                {
                    TakePillLoop();
                }
                if(IsPlayingPillTakeAnimation)
                {
                    Player.ButtonPrompts.RemovePrompts("IngestActivity");
                }
                else
                {
                    Player.ButtonPrompts.AddPrompt("IngestActivity", $"Take {IngestItem?.Name} ({CountOfRemainingItems()})", "IngestTakePill", GameControl.Attack, 10);
                }
                if(!HasMoreOfItem())
                {
                    IsCancelled = true;
                }
                GameFiber.Yield();
            }
            Exit();
        }
        private bool HasMoreOfItem()
        {
            return CountOfRemainingItems() > 0;
        }
        private int CountOfRemainingItems()
        {
            InventoryItem inventoryItem = Player.Inventory.ItemsList.Where(x => x.ModItem != null && x.ModItem.Name.ToLower() == IngestItem.Name.ToLower()).FirstOrDefault();
            if(inventoryItem == null)
            {
                return 0;
            }
            return inventoryItem.Amount;
        }
        private void TakePillLoop()
        {
            Player.ButtonPrompts.RemovePrompts("IngestActivity");
            IsPlayingPillTakeAnimation = true;

            StartNewIdleAnimation();
            ConsumableItemNeedGain = new ConsumableRefresher(Player, IngestItem, Settings);
            while (Player.ActivityManager.CanPerformActivitiesMiddle && !IsCancelled)
            {
                DisableControls();
                Player.WeaponEquipment.SetUnarmed();
                float AnimationTime = NativeFunction.CallByName<float>("GET_ENTITY_ANIM_CURRENT_TIME", Player.Character, PlayingDict, PlayingAnim);
                if (AnimationTime >= 0.35f)
                {
                    ConsumableItemNeedGain.FullyConsume();
                    Player.Inventory.Use(IngestItem);
                    if (CurrentIntoxicant != null)
                    {
                        Player.Intoxication.Ingest(CurrentIntoxicant);
                    }
                    break;
                }
                bool isAnimRunning = AnimationWatcher.IsAnimationRunning(AnimationTime);
                if (!isAnimRunning)
                {
                    IsCancelled = true;
                }
                GameFiber.Yield();
            }
            NativeFunction.Natives.CLEAR_PED_SECONDARY_TASK(Player.Character);
            IsPlayingPillTakeAnimation = false;
        }
        private void Exit()
        {
            Player.ButtonPrompts.RemovePrompts("IngestActivity");
            if (Item.Exists())
            {
                Item.Detach();
            }
            NativeFunction.Natives.CLEAR_PED_SECONDARY_TASK(Player.Character);
            Player.ActivityManager.IsPerformingActivity = false;
            Player.ActivityManager.IsUsingIllegalItem = false;
            if (CurrentIntoxicant != null && !CurrentIntoxicant.ContinuesWithoutCurrentUse)
            {
                //EntryPoint.WriteToConsole("IngestActivity Exit, Stopping ingestion");
                Player.Intoxication.StopIngesting(CurrentIntoxicant);
            }
            GameFiber.Sleep(5000);
            if (Item.Exists())
            {
                Item.Delete();
            }
        }
        private void AttachItemToHand()
        {
            CreateItem();
            if (Item.Exists() && !IsAttachedToHand)
            {
                EntryPoint.WriteToConsole($"{Data.HandBoneName} {Data.HandOffset} {Data.HandRotator} BoneIndex:{NativeFunction.CallByName<int>("GET_ENTITY_BONE_INDEX_BY_NAME", Player.Character, Data.HandBoneName)}");
                Item.AttachTo(Player.Character, NativeFunction.CallByName<int>("GET_ENTITY_BONE_INDEX_BY_NAME", Player.Character, Data.HandBoneName), Data.HandOffset, Data.HandRotator);
                IsAttachedToHand = true;
                Player.AttachedProp.Add(Item);
            }
        }


        private void DisableControls()
        {
            Game.DisableControlAction(0, GameControl.Attack, true);// false);
            Game.DisableControlAction(0, GameControl.Attack2, true);// false);
            Game.DisableControlAction(0, GameControl.MeleeAttack1, true);// false);
            Game.DisableControlAction(0, GameControl.MeleeAttack2, true);// false);
            Game.DisableControlAction(0, GameControl.Detonate, true);// false);


            Game.DisableControlAction(0, GameControl.Aim, true);// false);
            Game.DisableControlAction(0, GameControl.VehicleAim, true);// false);
            Game.DisableControlAction(0, GameControl.AccurateAim, true);// false);
            Game.DisableControlAction(0, GameControl.VehiclePassengerAim, true);// false);
        }


        private void CreateItem()
        {
            if (!Item.Exists() && Data.PropModelName != "")
            {
                try
                {
                    Item = new Rage.Object(Data.PropModelName, Player.Character.GetOffsetPositionUp(50f));
                }
                catch (Exception ex)
                {
                    //EntryPoint.WriteToConsoleTestLong($"Error Spawning Model {ex.Message} {ex.StackTrace}");
                }
                if (Item.Exists())
                {
                    Item.IsGravityDisabled = false;
                }
                //else
                //{
                //    IsCancelled = true;
                //}
            }
        }
        private void StartNewIdleAnimation()
        {
            AnimationWatcher.Reset();
            PlayingDict = Data.AnimIdleDictionary;
            PlayingAnim = Data.AnimIdle.PickRandom();
            NativeFunction.CallByName<uint>("TASK_PLAY_ANIM", Player.Character, PlayingDict, PlayingAnim, 1.0f, -1.0f, -1, 50, 0, false, false, false);
        }
        private void Setup()
        {
            List<string> AnimIdle;
            string AnimEnter = "";
            string AnimEnterDictionary = "";
            string AnimExit = "";
            string AnimExitDictionary = "";
            string AnimIdleDictionary;
            string PropModel = "";
            string HandBoneName = "BONETAG_R_PH_HAND";
            Vector3 HandOffset = Vector3.Zero;
            Rotator HandRotator = Rotator.Zero;
            if (ModItem != null && ModItem.ModelItem != null)
            {
                PropModel = ModItem.ModelItem.ModelName;
                PropAttachment pa = ModItem.ModelItem.Attachments.FirstOrDefault(x => x.Name == "RightHand" && (x.Gender == "U" || x.Gender == Player.Gender));
                if (pa != null)
                {
                    HandOffset = pa.Attachment;
                    HandRotator = pa.Rotation;
                    HandBoneName = pa.BoneName;
                }

                EntryPoint.WriteToConsole($"HandOffset:{HandOffset} HandRotator:{HandRotator} HandBoneName:{HandBoneName}");
            }
            AnimIdleDictionary = "mp_suicide";
            AnimIdle = new List<string>() { "pill" };
            if (IngestItem != null && IngestItem.IsIntoxicating && IngestItem.Intoxicant != null)
            {
                CurrentIntoxicant = IngestItem.Intoxicant;
                Player.Intoxication.StartIngesting(CurrentIntoxicant);
            }

            AnimationDictionary.RequestAnimationDictionay(AnimIdleDictionary);
            Data = new EatingData("", "", AnimEnter, AnimEnterDictionary, AnimExit, AnimExitDictionary, AnimIdle, AnimIdleDictionary, HandBoneName, HandOffset, HandRotator, PropModel);
            AnimationWatcher = new AnimationWatcher();
        }
    }



}