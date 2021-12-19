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
    public class EatingActivity : DynamicActivity
    {
        private Rage.Object Food;
        private string PlayingAnim;
        private string PlayingDict;
        private EatingData Data;
        private IntoxicatingEffect IntoxicatingEffect;
        private bool IsAttachedToHand;
        private bool IsCancelled;
        private IIntoxicatable Player;
        private ISettingsProvideable Settings;
       // private ModItem ModItem;
        private IIntoxicants Intoxicants;
        private Intoxicant CurrentIntoxicant;
        public EatingActivity(IIntoxicatable consumable, ISettingsProvideable settings, ModItem modItem, IIntoxicants intoxicants) : base()
        {
            Player = consumable;
            Settings = settings;
            ModItem = modItem;
            Intoxicants = intoxicants;
        }
        public override ModItem ModItem { get; set; }
        public override string DebugString => $"Intox {Player.IsIntoxicated} Consum: {Player.IsPerformingActivity} I: {Player.IntoxicatedIntensity}";
        public override void Cancel()
        {
            IsCancelled = true;
            Player.IsPerformingActivity = false;
            Player.StopIngesting(CurrentIntoxicant);
        }
        public override void Pause()
        {

        }
        public override void Continue()
        {
        }
        public override void Start()
        {
            EntryPoint.WriteToConsole("EatingActivity START", 5);
            Setup();
            GameFiber ScenarioWatcher = GameFiber.StartNew(delegate
            {
                Enter();
            }, "DrinkingWatcher");
        }
        private void AttachFoodToHand()
        {
            CreateFood();
            if (Food.Exists() && !IsAttachedToHand)
            {
                Food.AttachTo(Player.Character, NativeFunction.CallByName<int>("GET_PED_BONE_INDEX", Player.Character, Data.HandBoneID), Data.HandOffset, Data.HandRotator);
                IsAttachedToHand = true;
                Player.AttachedProp = Food;
            }
        }
        private void CreateFood()
        {
            if (!Food.Exists() && Data.PropModelName != "")
            {
                try
                {
                    //Vector3 position = Player.Character.GetOffsetPositionUp(50f);
                    //Model modelToCreate = new Model(Game.GetHashKey(Data.PropModelName));
                    //modelToCreate.LoadAndWait();
                    //Food = NativeFunction.Natives.CREATE_OBJECT<Rage.Object>(Game.GetHashKey(Data.PropModelName), position.X, position.Y, position.Z, 0f);
                    Food = new Rage.Object(Data.PropModelName, Player.Character.GetOffsetPositionUp(50f));
                }
                catch(Exception e)
                {
                    Game.DisplayNotification($"Could Not Spawn Prop {Data.PropModelName}");
                }
                if (Food.Exists())
                {
                    Food.IsGravityDisabled = false;
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
            AttachFoodToHand();
            Player.IsPerformingActivity = true;


            //PlayingDict = Data.AnimEnterDictionary;
            //PlayingAnim = Data.AnimEnter;
            //NativeFunction.CallByName<uint>("TASK_PLAY_ANIM", Player.Character, PlayingDict, PlayingAnim, 1.0f, -1.0f, -1, 50, 0, false, false, false);//-1
            //while (Player.CanPerformActivities && !IsCancelled)
            //{
            //    Player.SetUnarmed();
            //    float AnimationTime = NativeFunction.CallByName<float>("GET_ENTITY_ANIM_CURRENT_TIME", Player.Character, PlayingDict, PlayingAnim);
            //    if (AnimationTime >= 0.5f)
            //    {
            //        break;
            //    }
            //    GameFiber.Yield();
            //}
            Idle();
        }
        private void Exit()
        {
            if (Food.Exists())
            {
                Food.Detach();
            }
            //Player.Character.Tasks.Clear();



            // NativeFunction.Natives.CLEAR_PED_TASKS(Player.Character);
            

                NativeFunction.Natives.CLEAR_PED_SECONDARY_TASK(Player.Character);


            Player.IsPerformingActivity = false;
            Player.StopIngesting(CurrentIntoxicant);
            GameFiber.Sleep(5000);
            if (Food.Exists())
            {
                Food.Delete();
            }
        }
        private void Idle()
        {
            PlayingDict = Data.AnimIdleDictionary;
            PlayingAnim = Data.AnimIdle.PickRandom();
            NativeFunction.CallByName<uint>("TASK_PLAY_ANIM", Player.Character, PlayingDict, PlayingAnim, 1.0f, -1.0f, -1, 50, 0, false, false, false);//-1

            EntryPoint.WriteToConsole($"Eating Activity Playing {PlayingDict} {PlayingAnim}", 5);
            bool HasMadeNoise = false;
            while (Player.CanPerformActivities && !IsCancelled)
            {
                Player.SetUnarmed();
                float AnimationTime = NativeFunction.CallByName<float>("GET_ENTITY_ANIM_CURRENT_TIME", Player.Character, PlayingDict, PlayingAnim);
                if (AnimationTime >= 0.9f)
                {
                    if (Food.Exists())
                    {
                        Food.Delete();
                        if(Game.LocalPlayer.Character.Health < Game.LocalPlayer.Character.MaxHealth)
                        {
                            int ToAdd = 10;
                            if(Game.LocalPlayer.Character.MaxHealth - Game.LocalPlayer.Character.Health < 10)
                            {
                                ToAdd = Game.LocalPlayer.Character.MaxHealth - Game.LocalPlayer.Character.Health;
                            }
                            Player.Character.Health += ToAdd;
                        }
                    }
                }
                if (AnimationTime >= 1.0f)
                {
                    break;
                }
                if(!HasMadeNoise && AnimationTime >= 0.2)
                {
                    HasMadeNoise = true;
                    SayAvailableAmbient(Player.Character, new List<string>() { "GENERIC_EAT" }, false);
                }
                GameFiber.Yield();
            }
            Exit();
        }
        private void Setup()
        {
            List<string> AnimIdle;
            string AnimBase = "";
            string AnimBaseDictionary = "";
            string AnimEnter = "";
            string AnimEnterDictionary = "";
            string AnimExit = "";
            string AnimExitDictionary = "";
            string AnimIdleDictionary;
            int HandBoneID = 57005;
            Vector3 HandOffset = Vector3.Zero;
            Rotator HandRotator = Rotator.Zero;
            string PropModel = "";

            if (Player.ModelName.ToLower() == "player_zero" || Player.ModelName.ToLower() == "player_one" || Player.ModelName.ToLower() == "player_two" || Player.IsMale)
            {
                AnimIdleDictionary = "amb@code_human_wander_eating_donut@male@idle_a";
                AnimIdle = new List<string>() { "idle_a", "Idle_b", "Idle_c" };
                AnimBase = "base";
                AnimBaseDictionary = "amb@code_human_wander_eating_donut@male@base";
                AnimEnter = "static";
                AnimEnterDictionary = "amb@code_human_wander_eating_donut@male@base";

                if(Player.IsSitting || Player.IsInVehicle)
                {
                    AnimIdleDictionary = "amb@world_human_seat_wall_eating@male@both_hands@idle_a";
                    AnimIdle = new List<string>() {  "Idle_c" };
                }

            }
            else
            {
                AnimIdleDictionary = "amb@code_human_wander_eating_donut@female@idle_a";
                AnimIdle = new List<string>() { "idle_a", "Idle_b", "Idle_c" };
                AnimBase = "base";
                AnimBaseDictionary = "amb@code_human_wander_eating_donut@female@base";
                AnimEnter = "static";
                AnimEnterDictionary = "amb@code_human_wander_eating_donut@female@base";

                if (Player.IsSitting || Player.IsInVehicle)
                {
                    AnimIdleDictionary = "amb@world_human_seat_wall_eating@female@sandwich_right_hand@idle_a";
                    AnimIdle = new List<string>() { "idle_a" };
                }
            }
            if(ModItem != null && ModItem.ModelItem != null)
            {
                HandBoneID = ModItem.ModelItem.AttachBoneIndex;
                HandOffset = ModItem.ModelItem.AttachOffset;
                HandRotator = ModItem.ModelItem.AttachRotation;
                PropModel = ModItem.ModelItem.ModelName;
            }
            if (ModItem != null && ModItem.IsIntoxicating)
            {
                CurrentIntoxicant = Intoxicants.Get(ModItem.IntoxicantName);
                Player.StartIngesting(CurrentIntoxicant);
            }
            AnimationDictionary.RequestAnimationDictionay(AnimBaseDictionary);
            AnimationDictionary.RequestAnimationDictionay(AnimEnterDictionary);
            AnimationDictionary.RequestAnimationDictionay(AnimIdleDictionary);
            Data = new EatingData(AnimBase, AnimBaseDictionary, AnimEnter, AnimEnterDictionary, AnimExit, AnimExitDictionary, AnimIdle, AnimIdleDictionary, HandBoneID, HandOffset, HandRotator, PropModel);
        }
        private bool SayAvailableAmbient(Ped ToSpeak, List<string> Possibilities, bool WaitForComplete)
        {
            bool Spoke = false;
            foreach (string AmbientSpeech in Possibilities.OrderBy(x => RandomItems.MyRand.Next()))
            {
                ToSpeak.PlayAmbientSpeech(null, AmbientSpeech, 0, SpeechModifier.Force);
                GameFiber.Sleep(100);
                if (ToSpeak.IsAnySpeechPlaying)
                {
                    Spoke = true;
                }
                if (Spoke)
                {
                    break;
                }
            }
            GameFiber.Sleep(100);
            while (ToSpeak.IsAnySpeechPlaying && WaitForComplete)
            {
                Spoke = true;
                GameFiber.Yield();
            }
            if (!Spoke)
            {
                Game.DisplayNotification($"\"{Possibilities.FirstOrDefault()}\"");
            }
            return Spoke;
        }
    }
}