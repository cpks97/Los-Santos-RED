﻿using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System.Collections.Generic;
using System.Linq;

public class ActionMenu : Menu
{
    private UIMenu Actions;
    private UIMenuListScrollerItem<LSR.Vehicles.LicensePlate> ChangePlate;
    private UIMenuItem Drink;
    private IActionable Player;
    private UIMenuItem RemovePlate;
    private UIMenuItem SitDown;
    private UIMenuItem Smoke;
    private UIMenuItem SmokePot;
    private UIMenuItem PauseConsuming;
    private UIMenuItem StopConsuming;
    private UIMenuItem Suicide;
    private ISettingsProvideable Settings;
    private UIMenuItem ContinueConsuming;
    private MenuPool MenuPool;
    private UIMenu GestureMenu;
    private List<GestureLookup> GestureLookups;
    public ActionMenu(MenuPool menuPool, UIMenu parentMenu, IActionable player, ISettingsProvideable settings)
    {
        Player = player;
        Settings = settings;
        MenuPool = menuPool;
        Actions = MenuPool.AddSubMenu(parentMenu, "Actions");
        Actions.SetBannerType(EntryPoint.LSRedColor);
        Actions.OnItemSelect += OnActionItemSelect;
        
        CreateActionsMenu();
    }
    public int SelectedPlateIndex { get; set; }
    public override void Hide()
    {
        Actions.Visible = false;
    }
    public override void Show()
    {
        Update();
        Actions.Visible = true;
    }
    public override void Toggle()
    {
        
        if (!Actions.Visible)
        {
            Actions.Visible = true;
        }
        else
        {
            Actions.Visible = false;
        }
    }
    public void Update()
    {
        //CreateActionsMenu();
        if (Player.CanPerformActivities)
        {
            Suicide.Enabled = true;
            ChangePlate.Enabled = true;
            RemovePlate.Enabled = true;

            if (Settings.SettingsManager.PlayerSettings.AllowConsumeWithoutInventory)
            {
                Drink.Enabled = true;
                Smoke.Enabled = true;
                SmokePot.Enabled = true;
            }
        }
        else
        {
            Suicide.Enabled = false;
            ChangePlate.Enabled = false;
            RemovePlate.Enabled = false;
            if (Settings.SettingsManager.PlayerSettings.AllowConsumeWithoutInventory)
            {
                Drink.Enabled = false;
                Smoke.Enabled = false;
                SmokePot.Enabled = false;
            }
        }
        if (Player.IsPerformingActivity)
        {
            StopConsuming.Enabled = true;
        }
        else
        {
            StopConsuming.Enabled = false;
        }
        ChangePlate.Items = Player.SpareLicensePlates;
    }
    private void CreateActionsMenu()
    {
        Setup();
        Actions.Clear();
        Suicide = new UIMenuItem("Suicide", "Commit Suicide");
        ChangePlate = new UIMenuListScrollerItem<LSR.Vehicles.LicensePlate>("Change Plate", "Change your license plate if you have spares.",Player.SpareLicensePlates);
        RemovePlate = new UIMenuItem("Remove Plate", "Remove the license plate.");
        SitDown = new UIMenuItem("Sit Down", "Face the nearest Seat.");
        if (Settings.SettingsManager.PlayerSettings.AllowConsumeWithoutInventory)
        {
            Drink = new UIMenuItem("Drink", "Start Drinking");
            Smoke = new UIMenuItem("Smoke", "Start Smoking");
            SmokePot = new UIMenuItem("Smoke Pot", "Start Smoking Pot");
        }

        GestureMenu = MenuPool.AddSubMenu(Actions, "Gesture");
        GestureMenu.SetBannerType(System.Drawing.Color.FromArgb(181, 48, 48));

        foreach(GestureLookup gl in GestureLookups)
        {
            GestureMenu.AddItem(new UIMenuItem(gl.Name, gl.Name));
        }
        GestureMenu.OnItemSelect += OnGestureItemSelect;

        ContinueConsuming = new UIMenuItem("Continue", "Continue Consuming Activity");
        PauseConsuming = new UIMenuItem("Pause", "Pause Consuming Activity");
        StopConsuming = new UIMenuItem("Stop", "Stop Consuming Activity");

        Actions.AddItem(Suicide);
        Actions.AddItem(ChangePlate);
        Actions.AddItem(RemovePlate);
        Actions.AddItem(SitDown);
        if (Settings.SettingsManager.PlayerSettings.AllowConsumeWithoutInventory)
        {
            Actions.AddItem(Drink);
            Actions.AddItem(Smoke);
            Actions.AddItem(SmokePot);
        }
        Actions.AddItem(ContinueConsuming);
        Actions.AddItem(PauseConsuming);
        Actions.AddItem(StopConsuming);

        
       // Actions.OnListChange += OnListChange;
    }
    private void OnActionItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == Suicide)
        {
            Player.CommitSuicide();
        }
        else if (selectedItem == ChangePlate)
        {
            Player.ChangePlate(ChangePlate.SelectedItem);
        }
        else if (selectedItem == RemovePlate)
        {
            Player.RemovePlate();
        }
        else if (selectedItem == Drink)
        {
            Player.StartDrinkingActivity();
        }
        else if (selectedItem == Smoke)
        {
            Player.StartSmoking();
        }
        else if (selectedItem == SmokePot)
        {
            Player.StartSmokingPot();
        }
        else if (selectedItem == SitDown)
        {
            Player.StartSittingDown();
        }
        else if (selectedItem == ContinueConsuming)
        {
            Player.ContinueDynamicActivity();
        }
        else if (selectedItem == PauseConsuming)
        {
            Player.PauseDynamicActivity();
        }
        else if (selectedItem == StopConsuming)
        {
            Player.StopDynamicActivity();
        }
        Actions.Visible = false;
        ChangePlate.Items = Player.SpareLicensePlates;
    }
    private void OnGestureItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        EntryPoint.WriteToConsole($"GestureMenu Select: {selectedItem.Text}", 5);
        GestureLookup gl = GestureLookups.FirstOrDefault(x=> x.Name == selectedItem.Text);
        if(gl != null)
        {
            Player.Gesture(gl.AnimationName);
            GestureMenu.Visible = false;
        }
    }
    private void Setup()
    {
        GestureLookups = new List<GestureLookup>()
        {
            new GestureLookup("Bring It On","gesture_bring_it_on"),
            new GestureLookup("Bye (Hard)","gesture_bye_hard"),
            new GestureLookup("Bye (Soft)","gesture_bye_soft"),
            new GestureLookup("Come Here (Hard)","gesture_come_here_hard"),
            new GestureLookup("Come Here (Soft)","gesture_come_here_soft"),
            new GestureLookup("Damn","gesture_damn"),
            new GestureLookup("Displeased","gesture_displeased"),
            new GestureLookup("Easy Now","gesture_easy_now"),
            new GestureLookup("Hand Down","gesture_hand_down"),
            new GestureLookup("Hand Left","gesture_hand_left"),
            new GestureLookup("Hand Right","gesture_hand_right"),
            new GestureLookup("Head No","gesture_head_no"),
            new GestureLookup("Hello","gesture_hello"),
            new GestureLookup("I Will","gesture_i_will"),
            new GestureLookup("Me","gesture_me"),
            new GestureLookup("Me (Hard)","gesture_me_hard"),
            new GestureLookup("No Way","gesture_no_way"),
            new GestureLookup("Nod No (Hard)","gesture_nod_no_hard"),
            new GestureLookup("Nod No (Soft)","gesture_nod_no_soft"),
            new GestureLookup("Nod Yes (Hard)","gesture_nod_yes_hard"),
            new GestureLookup("Nod Yes (Soft)","gesture_nod_yes_soft"),
            new GestureLookup("Pleased","gesture_pleased"),
            new GestureLookup("Point","gesture_point"),
            new GestureLookup("Shrug (Hard)","gesture_shrug_hard"),
            new GestureLookup("Shrug (Soft)","gesture_shrug_soft"),
            new GestureLookup("What (Hard)","gesture_what_hard"),
            new GestureLookup("What (Soft)","gesture_what_soft"),
            new GestureLookup("Why","gesture_why"),
            new GestureLookup("Why Left","gesture_why_left"),
            new GestureLookup("You (Hard)","gesture_you_hard"),
            new GestureLookup("You (Soft)","gesture_you_soft"),
            new GestureLookup("Its Mine","getsure_its_mine"),
        };
    }
    private class GestureLookup
    {
        public GestureLookup(string name, string animationName)
        {
            Name = name;
            AnimationName = animationName;
        }

        public string Name { get; set; }
        public string AnimationName { get; set; }
    }

}