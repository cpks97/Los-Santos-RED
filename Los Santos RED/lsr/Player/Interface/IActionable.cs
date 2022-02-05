﻿using LosSantosRED.lsr.Locations;
using LosSantosRED.lsr.Player;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface IActionable
    {
        bool IsDead { get; }
        bool IsBusted { get; }
        bool IsInVehicle { get; }
        bool IsPerformingActivity { get; set; }
        bool CanPerformActivities { get; }
        List<LicensePlate> SpareLicensePlates { get; }
        bool IsCop { get; }
        LocationData CurrentLocation { get; }
        Ped Character { get; }
        bool IsMale { get; }
        bool IsSitting { get; set; }
       Rage.Object AttachedProp { get; }
        bool IsMoveControlPressed { get; }
        string ModelName { get; }
        bool HasCurrentActivity { get; }
        bool CharacterModelIsFreeMode { get; }
        GestureData LastGesture { get; set; }
        bool IsMakingInsultingGesture { get; set; }
        void StartSittingDown(bool FindSittingProp, bool EnterForward);
        void CommitSuicide();
        void DisplayPlayerNotification();
        void GiveMoney(int v);
        void RemovePlate();
        void ChangePlate(int Index);
        void StopDynamicActivity();
        void ChangePlate(LicensePlate selectedItem);
        void TakeOwnershipOfNearestCar();
        void CallPolice();
        void SetUnarmed();
        void StartConsumingActivity(ModItem selectedStuff);
        void PauseDynamicActivity();
        void ContinueDynamicActivity();
        void Gesture(GestureData gestureData);
        void EnterVehicleAsPassenger();
        void ShuffleToNextSeat();
        void ForceErraticDriver();
        void ToggleBodyArmor(int Type);
        void SetBodyArmor(int value);
        void StartLayingDown(bool v);
        void StartHotwiring();
        GangRelationships GangRelationships { get; }
        Inventory Inventory { get; }
        void RemoveOwnershipOfNearestCar();
    }
}
