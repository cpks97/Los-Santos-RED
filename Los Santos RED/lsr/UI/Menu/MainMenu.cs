﻿using LosSantosRED.lsr.Interface;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System.Linq;

public class MainMenu : Menu
{
    private UIMenuItem AboutMenu;
    private ActionMenu ActionMenu;
    private InventoryMenu InventoryMenu;
    private UIMenu Main;
    private PedSwapMenu PedSwapMenu;
    private ILocationInteractable Player;
    private UIMenuItem RemoveVehicleOwnership;
    private SaveMenu SaveMenu;
    private ISettingsProvideable Settings;
    private SettingsMenu SettingsMenu;
    private UIMenuItem ShowReportingMenu;
    private UIMenuItem TakeVehicleOwnership;
    private ITaskerable Tasker;
    private UI UI;
    private UIMenuItem UnloadMod;
    private MenuPool MenuPool;
    private UIMenu VehicleItems;
    private UIMenuItem ShowSimplePhoneMenu;
    private UIMenu GangItems;
    private UIMenuListScrollerItem<Gang> SetAsGangMember;
    private IGangs Gangs;
    private UIMenuItem LeaveGang;

    public MainMenu(MenuPool menuPool, IActionable actionablePlayer, ILocationInteractable player, ISaveable saveablePlayer, IGameSaves gameSaves, IWeapons weapons, IPedSwap pedswap, IEntityProvideable world, ISettingsProvideable settings, ITaskerable tasker, IInventoryable playerinventory, IModItems modItems, UI ui, IGangs gangs, ITimeControllable time, IPlacesOfInterest placesOfInterest, IDances dances, IGestures gestures)
    {
        MenuPool = menuPool;
        Player = player;
        Settings = settings;
        Tasker = tasker;
        Gangs = gangs;
        UI = ui;
        Main = new UIMenu("Los Santos RED", "Select an Option");
        Main.SetBannerType(EntryPoint.LSRedColor);
        menuPool.Add(Main);
        Main.OnItemSelect += OnItemSelect;
        Main.OnListChange += OnListChange;
        SettingsMenu = new SettingsMenu(menuPool, Main, Settings);
        SaveMenu = new SaveMenu(menuPool, Main, saveablePlayer, gameSaves, weapons, pedswap, playerinventory, Settings, world, gangs, time, placesOfInterest, modItems);
        PedSwapMenu = new PedSwapMenu(menuPool, Main, pedswap, gangs);
        ActionMenu = new ActionMenu(menuPool, Main, actionablePlayer, Settings, dances, gestures);
        InventoryMenu = new InventoryMenu(menuPool, Main, player, modItems, false);
        CreateMainMenu();
    }
    public override void Hide()
    {
        Main.Visible = false;
        ActionMenu.Hide();
        SaveMenu.Hide();
        InventoryMenu.Hide();
    }
    public override void Show()
    {
        if (!Main.Visible)
        {
            ActionMenu.Update();
            InventoryMenu.Update();
            Main.Visible = true;
            ActionMenu.Hide();
            InventoryMenu.Hide();
            SettingsMenu.Hide();
            SaveMenu.Hide();
            PedSwapMenu.Hide();
        }
    }
    public override void Toggle()
    {
        if (!Main.Visible)
        {
            ActionMenu.Update();
            InventoryMenu.Update();
            Main.Visible = true;
            ActionMenu.Hide();
            InventoryMenu.Hide();
            SettingsMenu.Hide();
            SaveMenu.Hide();
            PedSwapMenu.Hide();
        }
        else
        {
            Main.Visible = false;

            ActionMenu.Hide();
            InventoryMenu.Hide();
            SettingsMenu.Hide();
            SaveMenu.Hide();
            PedSwapMenu.Hide();
        }
    }
    private void CreateMainMenu()
    {
        AboutMenu = new UIMenuItem("About", "Shows some general information about the mod and its features. More to Come.");
        AboutMenu.RightBadge = UIMenuItem.BadgeStyle.Alert;
        ShowReportingMenu = new UIMenuItem("Player Information", "Show the player information menu. This pause menu has info about Owned Vehicles, Licenses, ~r~Gang Relationships~s~, and ~y~Locations~s~.");
        ShowReportingMenu.RightBadge = UIMenuItem.BadgeStyle.Lock;


        ShowSimplePhoneMenu = new UIMenuItem("Replies and Contacts", "Shows the phone replies, text messages, and contacts. Will allow you to call ~p~Contacts~s~ and lookup ~y~Locations~s~.");
        ShowSimplePhoneMenu.RightBadge = UIMenuItem.BadgeStyle.Alert;












        UnloadMod = new UIMenuItem("Unload Mod", "Unload mod and change back to vanilla. ~r~Load Game~s~ required at minimum, ~r~Restart~s~ for best results.");
        UnloadMod.RightBadge = UIMenuItem.BadgeStyle.Star;
        Main.AddItem(AboutMenu);
        Main.AddItem(ShowReportingMenu);
        Main.AddItem(ShowSimplePhoneMenu);

        VehicleItems = MenuPool.AddSubMenu(Main, "Vehicle Ownership");
        VehicleItems.SetBannerType(EntryPoint.LSRedColor);
        Main.MenuItems[Main.MenuItems.Count() - 1].Description = "Add or Remove ownership of nearby vehicles.";
        Main.MenuItems[Main.MenuItems.Count() - 1].RightBadge = UIMenuItem.BadgeStyle.Car;

        TakeVehicleOwnership = new UIMenuItem("Set Vehicle as Owned", "Set closest vehicle as owned by the mode. This will let you enter it freely and police/civilians will not react as if it is stolen when you enter.");
        TakeVehicleOwnership.RightBadge = UIMenuItem.BadgeStyle.Car;
        RemoveVehicleOwnership = new UIMenuItem("Remove Vehicle Onwership", "Set closest vehicle as not owned");
        RemoveVehicleOwnership.RightBadge = UIMenuItem.BadgeStyle.Car;

        VehicleItems.AddItem(TakeVehicleOwnership);
        VehicleItems.AddItem(RemoveVehicleOwnership);


        VehicleItems.OnItemSelect += OnItemSelect;



        GangItems = MenuPool.AddSubMenu(Main, "Gangs");
        GangItems.SetBannerType(EntryPoint.LSRedColor);
        Main.MenuItems[Main.MenuItems.Count() - 1].Description = "Join or Leave a Gang.";
        Main.MenuItems[Main.MenuItems.Count() - 1].RightBadge = UIMenuItem.BadgeStyle.Alert;

        SetAsGangMember = new UIMenuListScrollerItem<Gang>("Become Gang Member", "Become a gang member of the selected gang", Gangs.GetAllGangs());
        SetAsGangMember.Activated += (menu, item) =>
        {
            Player.GangRelationships.ResetGang(true);
            Player.GangRelationships.SetGang(SetAsGangMember.SelectedItem, true);
            menu.Visible = false;
        };
        GangItems.AddItem(SetAsGangMember);

        LeaveGang = new UIMenuItem("Leave Gang", "Leave your current gang");
        LeaveGang.Activated += (menu, item) =>
        {
            Player.GangRelationships.ResetGang(true);
            menu.Visible = false;
        };
        GangItems.AddItem(LeaveGang);




        Main.AddItem(UnloadMod);
    }
    private void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
    {
        if (selectedItem == ShowReportingMenu)
        {
            UI.TogglePlayerInfoMenu();
            Main.Visible = false;
        }
        else if (selectedItem == ShowSimplePhoneMenu)
        {
            UI.ToggleMessagesMenu();
            Main.Visible = false;
        }
        else if (selectedItem == UnloadMod)
        {
            EntryPoint.ModController.Dispose();
            Main.Visible = false;
        }
        else if (selectedItem == TakeVehicleOwnership)
        {
            Player.VehicleOwnership.TakeOwnershipOfNearestCar();
            Main.Visible = false;
        }
        else if (selectedItem == RemoveVehicleOwnership)
        {
            Player.VehicleOwnership.RemoveOwnershipOfNearestCar();
            Main.Visible = false;
        }
        else if (selectedItem == AboutMenu)
        {
            UI.ToggleAboutMenu();
            Main.Visible = false;
        }    
    }
    private void OnListChange(UIMenu sender, UIMenuListItem list, int index)
    {
    }
}