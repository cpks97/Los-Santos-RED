﻿using LosSantosRED.lsr.Locations;
using LosSantosRED.lsr.Player;
using LSR.Vehicles;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface ICellPhoneable
    {
        BankAccounts BankAccounts { get; }
        Destinations Destinations { get; }
        LocationData CurrentLocation { get; }
        GangRelationships GangRelationships { get; }
        PlayerTasks PlayerTasks { get; }
        CellPhone CellPhone { get; }
        PoliceResponse PoliceResponse { get; }
        int WantedLevel { get; }
        bool IsWanted { get; }
        bool IsNotWanted { get; }
        VehicleExt CurrentVehicle { get; }
        string PlayerName { get; }
        Rage.Ped Character { get; }
        string ModelName { get; }
        Scanner Scanner { get; }
        Investigation Investigation { get; }
        Vector3 Position { get; }
        bool IsCop { get; }
        void SetWantedLevel(int v1, string v2, bool v3);
        void AddCrime(Crime toCallIn, bool v1, Vector3 placeSeen, VehicleExt vehicleSeen, WeaponInformation weaponSeen, bool v2, bool v3, bool v4);
    }
}
