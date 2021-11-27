﻿using LSR.Vehicles;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LosSantosRED.lsr.Interface;

public class SpawnTask
{
    private DispatchablePerson PersonType;
    private Agency Agency;
    private bool AddBlip;
    private Vector3 InitialPosition;
    private Vector3 StreetPosition;
    private float Heading;
    private VehicleExt Vehicle;
    private DispatchableVehicle VehicleType;
    private ISettingsProvideable Settings;
    private IWeapons Weapons;
    private INameProvideable Names;
    public SpawnTask(Agency agency, Vector3 initialPosition, Vector3 streetPosition, float heading, DispatchableVehicle vehicleType, DispatchablePerson officerType, bool addBlip, ISettingsProvideable settings, IWeapons weapons, INameProvideable names)
    {
        Agency = agency;
        PersonType = officerType;
        VehicleType = vehicleType;
        AddBlip = addBlip;
        InitialPosition = initialPosition;
        StreetPosition = streetPosition;
        Heading = heading;
        Settings = settings;
        Weapons = weapons;
        Names = names;
    }
    public List<PedExt> CreatedPeople { get; private set; } = new List<PedExt>();
    public List<VehicleExt> CreatedVehicles { get; private set; } = new List<VehicleExt>();
    private Vector3 Position
    {
        get
        {
            if(VehicleType == null)
            {
                return InitialPosition;
            }
            else if (VehicleType.IsHelicopter)
            {
                return InitialPosition + new Vector3(0f, 0f, 250f);
            }
            else if (VehicleType.IsBoat)
            {
                return InitialPosition;
            }
            else
            {
                return StreetPosition;
            }
        }
    }
    public void AttemptSpawn()
    {
        if (Agency != null)
        {
            if (VehicleType != null)
            {
                Vehicle = CreateVehicle();
                if (Vehicle != null && Vehicle.Vehicle.Exists())
                {
                    if (PersonType != null)
                    {
                        PedExt Person = CreatePerson();
                        if (Person != null && Person.Pedestrian.Exists() && Vehicle != null && Vehicle.Vehicle.Exists())
                        {
                            Person.Pedestrian.WarpIntoVehicle(Vehicle.Vehicle, -1);
                            //NativeFunction.Natives.TASK_VEHICLE_DRIVE_WANDER(Person.Pedestrian, Person.Pedestrian.CurrentVehicle, 15f, (int)VehicleDrivingFlags.Normal, 10f);//temp here for ems and fire
                            int OccupantsToAdd = RandomItems.MyRand.Next(VehicleType.MinOccupants, VehicleType.MaxOccupants + 1) - 1;
                            //EntryPoint.WriteToConsole($"SpawnTask: Created {VehicleType.ModelName} {VehicleType.MinOccupants}-{VehicleType.MaxOccupants} Driver: {Person.Pedestrian.Handle} Trying to Add {OccupantsToAdd}", 5);
                            for (int OccupantIndex = 1; OccupantIndex <= OccupantsToAdd; OccupantIndex++)
                            {
                                PedExt Passenger = CreatePerson();
                                if (Passenger != null && Passenger.Pedestrian.Exists() && Vehicle != null && Vehicle.Vehicle.Exists())
                                {   
                                    int SeatToAssign = OccupantIndex - 1;
                                    //EntryPoint.WriteToConsole($"SpawnTask: Adding Passenger To {VehicleType.ModelName} Passenger: {Passenger.Pedestrian.Handle} Seat: {SeatToAssign}", 5);
                                    Passenger.Pedestrian.WarpIntoVehicle(Vehicle.Vehicle, SeatToAssign);
                                }
                                else
                                {
                                    EntryPoint.WriteToConsole($"SpawnTask: Adding Passenger To {VehicleType.ModelName} Failed", 5);
                                }
                            }
                        }
                        else
                        {
                            if (Vehicle != null && Vehicle.Vehicle.Exists())
                            {
                                Vehicle.Vehicle.Delete();
                                EntryPoint.PersistentVehiclesDeleted++;
                                //EntryPoint.WriteToConsole("Failed to complete spawn, deleting");
                            }
                        }
                    }
                }
            }
            else if(PersonType != null)
            {
                CreatePerson();
            }

        }
    }
    private PedExt CreatePerson()
    {
        Ped ped = new Ped(PersonType.ModelName, new Vector3(Position.X, Position.Y, Position.Z + 1f), Heading);
        EntryPoint.SpawnedEntities.Add(ped);
        GameFiber.Yield();
        if (ped.Exists())
        {
            EntryPoint.WriteToConsole($"SPAWN TASK: CREATED PED {ped.Handle}",2);
            ped.RandomizeVariation();
            if (VehicleType != null && VehicleType.IsMotorcycle)
            {
                ped.GiveHelmet(false, HelmetTypes.PoliceMotorcycleHelmet, 4096);
                NativeFunction.CallByName<uint>("SET_PED_COMPONENT_VARIATION", ped, 4, 0, 0, 0);
            }
            else
            {
                NativeFunction.CallByName<uint>("SET_PED_COMPONENT_VARIATION", ped, 4, 1, 0, 0);
            }
            if (PersonType.RequiredVariation != null)
            {
                PersonType.RequiredVariation.ReplacePedComponentVariation(ped);
            }
            GameFiber.Yield();
            if(!ped.Exists())
            {
                return null;
            }
            ped.IsPersistent = true;
            EntryPoint.PersistentPedsCreated++;
            if (AddBlip && ped.Exists())
            {
                Blip myBlip = ped.AttachBlip();
                myBlip.Color = Agency.Color;
                myBlip.Scale = 0.6f;
            }
            PedExt Person = null;
            if (Agency.ResponseType == ResponseType.LawEnforcement)
            {
                NativeFunction.CallByName<bool>("SET_PED_AS_COP", ped, true);
                Cop PrimaryCop = new Cop(ped,Settings, ped.Health, Agency, true, null, Weapons, Names.GetRandomName(ped.IsMale));
                PrimaryCop.IssueWeapons(Weapons);
                Person = PrimaryCop;
            }
            else if (Agency.ResponseType == ResponseType.EMS)
            {
                EMT PrimaryEmt = new EMT(ped,Settings, ped.Health, Agency, true, null, Weapons, Names.GetRandomName(ped.IsMale));
                Person = PrimaryEmt;
            }
            else if (Agency.ResponseType == ResponseType.Fire)
            {
                Firefighter PrimaryFirefighter = new Firefighter(ped,Settings, ped.Health, Agency, true, null,Weapons,Names.GetRandomName(ped.IsMale));
                Person = PrimaryFirefighter;
            }
            CreatedPeople.Add(Person);
            return Person;
        }
        return null;
    }
    private VehicleExt CreateVehicle()
    {
        //EntryPoint.WriteToConsole($"Attempting to spawn {VehicleType.ModelName}",3);
        Vehicle copcar = new Vehicle(VehicleType.ModelName, Position, Heading);
        EntryPoint.SpawnedEntities.Add(copcar);
        GameFiber.Yield();
        if (copcar.Exists())
        {
            EntryPoint.WriteToConsole($"SPAWN TASK: CREATED VEHICLE {copcar.Handle}", 2);
            if (!VehicleType.IsHelicopter && !VehicleType.IsBoat)
            { 
                NativeFunction.Natives.SET_VEHICLE_ON_GROUND_PROPERLY<bool>(copcar, 5.0f);
            }
            VehicleExt CopVehicle = new VehicleExt(copcar, Settings);
            if (copcar.Exists())
            {
                CopVehicle.WasModSpawned = true;
                copcar.IsPersistent = true;
                EntryPoint.PersistentVehiclesCreated++;
                CopVehicle.UpdateLivery(Agency);
                CopVehicle.UpgradePerformance();
                CreatedVehicles.Add(CopVehicle);
                GameFiber.Yield();
                return CopVehicle;
            }
        }
        return null;
    }
}
