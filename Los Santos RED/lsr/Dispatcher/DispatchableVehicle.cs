﻿using Rage;
using Rage.Native;
using System.Collections.Generic;

public class DispatchableVehicle
{
    public DispatchableVehicle()
    {
    }
    public DispatchableVehicle(string modelName, int ambientSpawnChance, int wantedSpawnChance)
    {
        ModelName = modelName;
        AmbientSpawnChance = ambientSpawnChance;
        WantedSpawnChance = wantedSpawnChance;
    }
    public int AmbientSpawnChance { get; set; } = 0;
    public bool IsBoat => NativeFunction.Natives.IS_THIS_MODEL_A_BOAT<bool>(Game.GetHashKey(ModelName));
    public bool IsCar => NativeFunction.Natives.IS_THIS_MODEL_A_CAR<bool>(Game.GetHashKey(ModelName));
    public bool IsHelicopter => NativeFunction.Natives.IS_THIS_MODEL_A_HELI<bool>(Game.GetHashKey(ModelName));
    public bool IsMotorcycle => NativeFunction.Natives.IS_THIS_MODEL_A_BIKE<bool>(Game.GetHashKey(ModelName));
    public int MaxOccupants { get; set; } = 2;
    public int MaxWantedLevelSpawn { get; set; } = 5;
    public int MinOccupants { get; set; } = 1;
    public int MinWantedLevelSpawn { get; set; } = 0;
    public string ModelName { get; set; }
    public List<int> RequiredLiveries { get; set; } = new List<int>();
    public List<string> RequiredPassengerModels { get; set; } = new List<string>();
    public int WantedSpawnChance { get; set; } = 0;
    public bool CanCurrentlySpawn(int WantedLevel) => CurrentSpawnChance(WantedLevel) > 0;
    public int CurrentSpawnChance(int WantedLevel)
    {
        if (WantedLevel > 0)
        {
            if (WantedLevel >= MinWantedLevelSpawn && WantedLevel <= MaxWantedLevelSpawn)
            {
                return WantedSpawnChance;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return AmbientSpawnChance;
        }
    }
}