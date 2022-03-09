﻿using LosSantosRED.lsr.Player;
using LSR.Vehicles;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface IInteractionable
    {
        bool IsHoldingUp { get; set; }
        WeaponCategory CurrentWeaponCategory { get; }
        bool IsConversing { get; set; }
        Ped Character { get; }
        PedExt CurrentTargetedPed { get; }
        bool IsAliveAndFree { get; }
        List<ButtonPrompt> ButtonPrompts { get; }
        bool IsInVehicle { get; }
        bool CanConverse { get; }
        int Money { get; }
        bool IsPerformingActivity { get; }
        bool IsTransacting { get; set; }
       // bool IsConductingIllicitTransaction { get; set; }
        bool IsSitting { get; }
        bool CharacterModelIsFreeMode { get; }
        string FreeModeVoice { get; }
        GameLocation CurrentShop { get; set; }
        bool IsMoveControlPressed { get; }
        bool CanPerformActivities { get; }
        bool IsDealingDrugs { get; set; }
        bool IsDealingIllegalGuns { get; set; }

        void SetAngeredCop();
        void GiveMoney(int v);
        void TakeOwnershipOfVehicle(VehicleExt myNewCar, bool showNotification);
        void ConsumeItem(ModItem toAdd);
        GangRelationships GangRelationships { get; }
        Inventory Inventory { get; }
        CellPhone CellPhone { get; }
        bool IsCarJacking { get; set; }

        void SetUnarmed();
    }
}
