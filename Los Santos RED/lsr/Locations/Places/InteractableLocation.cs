﻿using ExtensionsMethods;
using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class InteractableLocation : BasicLocation
{
    private uint NotificationHandle;
    private readonly List<string> FallBackVendorModels = new List<string>() { "s_m_m_strvend_01", "s_m_m_linecook" };
    public virtual string ContactIcon { get; set; } = "CHAR_BLANK_ENTRY";
    public bool IsAnyMenuVisible => MenuPool.IsAnyMenuOpen();

    [XmlIgnore]
    public Merchant Merchant { get; set; }
    [XmlIgnore]
    public bool HasVendor => VendorPosition != Vector3.Zero;

    public Vector3 VendorPosition { get; set; } = Vector3.Zero;
    public float VendorHeading { get; set; } = 0f;
    public List<string> VendorModels { get; set; }
    public bool HasCustomCamera => CameraPosition != Vector3.Zero;
    public Vector3 CameraPosition { get; set; } = Vector3.Zero;
    public Vector3 CameraDirection { get; set; } = Vector3.Zero;
    public Rotator CameraRotation { get; set; }
    public string ContactName { get; set; } = "";
    public bool CanInteractWhenWanted { get; set; } = false;

    [XmlIgnore]
    public ShopMenu Menu { get; set; }
    public string MenuID { get; set; }



    [XmlIgnore]
    public bool CanInteract { get; set; } = true;
    [XmlIgnore]
    public UIMenu InteractionMenu { get; private set; }
    [XmlIgnore]
    public MenuPool MenuPool { get; private set; }

    public virtual string ButtonPromptText { get; set; }
    

    public InteractableLocation(Vector3 _EntrancePosition, float _EntranceHeading, string _Name, string _Description) : base(_EntrancePosition, _EntranceHeading, _Name, _Description)
    {
        ButtonPromptText = $"Interact with {_Name}";
    }
    public InteractableLocation() : base()
    {
    }
    public virtual void OnInteract(ILocationInteractable Player, IModItems modItems, IEntityProvideable world, ISettingsProvideable settings, IWeapons weapons, ITimeControllable time)
    {
        //CreateInteractionMenu();
        //InteractionMenu.Visible = true;
        //EntryPoint.WriteToConsole("InteractableLocation OnInteract 2");
    }

    public void CreateInteractionMenu()
    {
        MenuPool = new MenuPool();
        InteractionMenu = new UIMenu(Name, Description);
        if (HasBannerImage)
        {
            BannerImage = Game.CreateTextureFromFile($"Plugins\\LosSantosRED\\images\\{BannerImagePath}");
            InteractionMenu.SetBannerType(BannerImage);
            Game.RawFrameRender += (s, e) => MenuPool.DrawBanners(e.Graphics);
        }
        //InteractionMenu.OnItemSelect += OnItemSelect;
        MenuPool.Add(InteractionMenu);
        CanInteract = false;
    }
    public void DisposeInteractionMenu()
    {
        Game.RawFrameRender -= (s, e) => MenuPool.DrawBanners(e.Graphics);
        if (InteractionMenu != null)
        {
            InteractionMenu.Visible = false;
        }
        CanInteract = true;
    }
    public void ProcessInteractionMenu()
    {
        while (IsAnyMenuVisible)
        {
            MenuPool.ProcessMenus();
            GameFiber.Yield();
        }
    }
    public override void Setup(IInteriors interiors, ISettingsProvideable settings, ICrimes crimes, IWeapons weapons)
    {
        if (HasVendor)
        {
            CanInteract = false;
            SpawnVendor(settings, crimes, weapons);
            GameFiber.Yield();
        }
        base.Setup(interiors, settings, crimes, weapons);
    }

    public virtual void OnItemSold(ModItem modItem, MenuItem menuItem, int totalItems)
    {
        if (modItem != null)
        {
            int TextSound = NativeFunction.Natives.GET_SOUND_ID<int>();
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(TextSound, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", 0);
            Game.RemoveNotification(NotificationHandle);
            if (modItem.MeasurementName == "Item")
            {
                NotificationHandle = Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", Name, "~g~Sale", $"You have sold {totalItems} ~r~{modItem.Name}(s)~s~");
            }
            else
            {
                NotificationHandle = Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", Name, "~g~Sale", $"You have sold {totalItems} {modItem.MeasurementName}(s) of ~r~{modItem.Name}~s~");
            }
        }
    }
    public virtual void OnItemPurchased(ModItem modItem, MenuItem menuItem, int totalItems)
    {
        if (modItem != null)
        {
            int TextSound = NativeFunction.Natives.GET_SOUND_ID<int>();
            NativeFunction.Natives.PLAY_SOUND_FRONTEND(TextSound, "PURCHASE", "HUD_LIQUOR_STORE_SOUNDSET", 0);
            Game.RemoveNotification(NotificationHandle);
            if (modItem.MeasurementName == "Item")
            {
                NotificationHandle = Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", Name, "~g~Purchase", $"You have purchased {totalItems} ~r~{modItem.Name}(s)~s~");
            }
            else
            {
                NotificationHandle = Game.DisplayNotification("CHAR_BLANK_ENTRY", "CHAR_BLANK_ENTRY", Name, "~g~Purchase", $"You have purchased {totalItems} {modItem.MeasurementName}(s) of ~r~{modItem.Name}~s~");
            }
        }
    }

    public override void Dispose()
    {
        if (Merchant != null && Merchant.Pedestrian.Exists())
        {
            Merchant.Pedestrian.Delete();
        }
        base.Dispose();
    }
    private void SpawnVendor(ISettingsProvideable settings, ICrimes crimes, IWeapons weapons)
    {
        Ped ped;
        string ModelName;
        if (VendorModels != null && VendorModels.Any())
        {
            ModelName = VendorModels.PickRandom();
        }
        else
        {
            ModelName = FallBackVendorModels.PickRandom();
        }
        Model modelToCreate = new Model(Game.GetHashKey(ModelName));
        modelToCreate.LoadAndWait();
        ped = NativeFunction.Natives.CREATE_PED<Ped>(26, Game.GetHashKey(ModelName), VendorPosition.X, VendorPosition.Y, VendorPosition.Z + 1f, VendorHeading, false, false);
        GameFiber.Yield();
        if (ped.Exists())
        {
            ped.IsPersistent = true;//THIS IS ON FOR NOW!
            ped.RandomizeVariation();
            ped.Tasks.StandStill(-1);
            ped.KeepTasks = true;
            EntryPoint.SpawnedEntities.Add(ped);
            GameFiber.Yield();
            if (ped.Exists())
            {
                Merchant = new Merchant(ped, settings, false, false, false, "Vendor", crimes, weapons);
                Merchant.ShopMenu = Menu;
                Merchant.AssociatedStore = this;
                EntryPoint.WriteToConsole($"MERCHANT SPAWNED? Menu: {Menu == null} HANDLE {ped.Handle}");
            }
        }
    }


}

