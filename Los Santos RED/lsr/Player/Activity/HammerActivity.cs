﻿using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Drawing;
using System.Linq;

namespace LosSantosRED.lsr.Player
{
    public class HammerActivity : DynamicActivity
    {
        private bool IsCancelled;
        private IActionable Player;
        private ISettingsProvideable Settings;
        private HammerItem HammerItem;
        private MeleeWeaponAlias meleeWeaponAlias;

        public HammerActivity(IActionable player, ISettingsProvideable settings, ICameraControllable cameraControllable, HammerItem shovelItem) : base()
        {
            Player = player;
            ModItem = shovelItem;
            Settings = settings;
            HammerItem = shovelItem;
        }

        public override ModItem ModItem { get; set; }
        public override string DebugString => "";
        public override bool CanPause { get; set; } = false;
        public override bool CanCancel { get; set; } = true;
        public override string PausePrompt { get; set; } = "Pause Hammer";
        public override string CancelPrompt { get; set; } = "Put Away Hammer";
        public override string ContinuePrompt { get; set; } = "Continue Hammer";
        public override void Cancel()
        {
            Dispose();
        }
        public override void Pause()
        {

        }
        public override bool IsPaused() => false;
        public override void Continue()
        {

        }
        public override void Start()
        {
            EntryPoint.WriteToConsole($"Hammer Start", 5);
            GameFiber ShovelWatcher = GameFiber.StartNew(delegate
            {
                Setup();
                meleeWeaponAlias = new MeleeWeaponAlias(Player, Settings, HammerItem, 1317494643);
                meleeWeaponAlias.Start();
                while (Player.ActivityManager.CanPerformActivities && !IsCancelled)
                {
                    meleeWeaponAlias.Update();
                    if (meleeWeaponAlias.IsCancelled)
                    {
                        meleeWeaponAlias.Dispose();
                        break;
                    }
                    GameFiber.Yield();
                }
                Dispose();
            }, "HammerActivity");
        }
        private void Setup()
        {

        }
        private void Dispose()
        {
            EntryPoint.WriteToConsole("HAMMER ACTIVITY END");
            IsCancelled = true;
            Player.ActivityManager.IsPerformingActivity = false;
            meleeWeaponAlias?.Dispose();
        }
    }
}