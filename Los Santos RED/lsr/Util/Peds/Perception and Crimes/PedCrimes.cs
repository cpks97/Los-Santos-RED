﻿using LosSantosRED.lsr.Interface;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensionsMethods;
using LSR.Vehicles;

public class PedCrimes
{
    private bool IsShootingCheckerActive = false;
    private PedExt PedExt;
    private List<Crime> CrimesViolating = new List<Crime>();
    private List<Crime> CrimesObserved = new List<Crime>();

    private List<WitnessedCrime> OtherPedCrimesObserved = new List<WitnessedCrime>();

    private ICrimes Crimes;
    private bool IsShooting = false;
    private ISettingsProvideable Settings;
    private uint GameTimeLastWitnessedCivilianCrime;
    private IWeapons Weapons;
    private uint GameTimeLastCommittedCrime;
    private uint GameTimeLastCommittedGTA;
    private bool ShouldCheck => PedExt.PedGroup != null && PedExt.PedGroup?.InternalName != "SECURITY_GUARD" && PedExt.PedGroup?.InternalName != "PRIVATE_SECURITY" && PedExt.PedGroup?.InternalName != "FIREMAN" && PedExt.PedGroup?.InternalName != "MEDIC";
    public PedCrimes(PedExt pedExt, ICrimes crimes, ISettingsProvideable settings, IWeapons weapons)
    {
        PedExt = pedExt;
        Crimes = crimes;
        Settings = settings;
        Weapons = weapons;
    }
    public int WantedLevel { get; set; } = 0;
    public bool IsWanted => WantedLevel > 0;
    public bool IsNotWanted => WantedLevel == 0;
    public bool IsDeadlyChase => CrimesObserved.Any(x => x.ResultsInLethalForce);
    public int CurrentlyViolatingWantedLevel => CrimesViolating.Any() ? CrimesViolating.Max(x => x.ResultingWantedLevel) : 0;
    public string CurrentlyViolatingWantedLevelReason => CrimesViolating.OrderBy(x=> x.Priority).FirstOrDefault()?.Name;
    public List<Crime> CrimesCurrentlyViolating => CrimesViolating;
    public bool IsCurrentlyViolatingAnyCrimes => CrimesViolating.Any();
    public bool IsCurrentlyViolatingAnyCivilianReportableCrimes => CrimesViolating.Any(x=> x.CanBeReportedByCivilians);
    public List<Crime> CrimesObservedViolating => CrimesObserved;

    public List<WitnessedCrime> OtherCrimesWitnessed => OtherPedCrimesObserved;

    public void Reset()
    {
        CrimesViolating.Clear();
        CrimesObserved.Clear();
        IsShootingCheckerActive = false;
        WantedLevel = 0;

    }
    public void OnPedSeenByPolice()
    {
        int WantedLevelToAssign = 0;
        foreach (Crime crime in CrimesViolating)
        {
            if (crime.ResultingWantedLevel > WantedLevelToAssign)
            {
                WantedLevelToAssign = crime.ResultingWantedLevel;
            }
            AddObserved(crime);
        }
        if (WantedLevelToAssign > WantedLevel)
        {
            WantedLevel = WantedLevelToAssign;
        }
    }
    public void OnPedHeardByPolice()
    {
        int WantedLevelToAssign = 0;
        foreach(Crime crime in CrimesViolating.Where(x=> x.CanReportBySound))
        {
            if(crime.ResultingWantedLevel > WantedLevelToAssign)
            {
                WantedLevelToAssign = crime.ResultingWantedLevel;
            }
            AddObserved(crime);
        }
        if (WantedLevelToAssign > WantedLevel)
        {
            WantedLevel = WantedLevelToAssign;
        }  
    }
    public void ShootingChecker()
    {
        if (!IsShootingCheckerActive)
        {
            GameFiber.StartNew(delegate
            {
                IsShootingCheckerActive = true;
                EntryPoint.WriteToConsole($"        Ped {PedExt.Pedestrian.Handle} IsShootingCheckerActive {IsShootingCheckerActive}", 5);
                uint GameTimeLastShot = 0;
                while (PedExt.Pedestrian.Exists() && IsShootingCheckerActive)// && CarryingWeapon && IsShootingCheckerActive && ObservedWantedLevel < 3)
                {
                    if (PedExt.Pedestrian.IsShooting)
                    {
                        IsShooting = true;
                        GameTimeLastShot = Game.GameTime;
                    }
                    else if (Game.GameTime - GameTimeLastShot >= 5000)
                    {
                        IsShooting = false;
                    }
                    GameFiber.Yield();
                }
                IsShootingCheckerActive = false;
            }, "Ped Shooting Checker");
        }
    }
    public void Update(IEntityProvideable world, IPoliceRespondable player)
    {
        if (ShouldCheck && Settings.SettingsManager.CivilianSettings.CheckCivilianCrimes)
        {
            ResetViolations();
            if (!PedExt.IsArrested)
            {
                CheckCrimes(world, player);
                if (WantedLevel > 0)
                {
                    OnPedSeenByPolice();
                }
                else
                {
                    CheckPoliceSight(world);
                }
                if (player.IsNotWanted && Settings.SettingsManager.CivilianSettings.AllowCivilinsToCallPoliceOnOtherCivilians)
                {
                    CheckCrimeReaction(world, player);
                }
            }
        }
    }
    private void CheckPoliceSight(IEntityProvideable world)
    {
        foreach (Cop cop in world.PoliceList)
        {
            if (cop.Pedestrian.Exists())
            {
                float DistanceTo = cop.Pedestrian.DistanceTo2D(PedExt.Pedestrian.Position);
                if (DistanceTo <= 0.1f)
                {
                    DistanceTo = 999f;
                }
                if(DistanceTo <= 10f)//right next to them = they can see ALL!
                {
                    OnPedSeenByPolice();
                    OnPedHeardByPolice();
                    return;
                }
                if (DistanceTo <= Settings.SettingsManager.PoliceSettings.SightDistance && IsThisPedInFrontOf(cop.Pedestrian) && !cop.Pedestrian.IsDead && NativeFunction.CallByName<bool>("HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT", cop.Pedestrian, PedExt.Pedestrian))//55f
                {
                    OnPedSeenByPolice();
                    OnPedHeardByPolice();
                    return;
                }
                if (DistanceTo <= Settings.SettingsManager.PoliceSettings.GunshotHearingDistance)
                {
                    OnPedHeardByPolice();
                }
            }
        }
    }
    private bool IsThisPedInFrontOf(Ped ToCheck)
    {
        float Result = GetDotVectorResult(ToCheck, PedExt.Pedestrian);
        if (Result > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private float GetDotVectorResult(Entity source, Entity target)
    {
        if (source.Exists() && target.Exists())
        {
            Vector3 dir = (target.Position - source.Position).ToNormalized();
            return Vector3.Dot(dir, source.ForwardVector);
        }
        else return -1.0f;
    }
    private void CheckCrimeReaction(IEntityProvideable world, IPoliceRespondable playerToCheck)
    {
        if (IsNotWanted && !IsCurrentlyViolatingAnyCrimes && PedExt.WillCallPolice)
        {
            OtherPedCrimesObserved.RemoveAll(x => x.Perpetrator != null && x.Perpetrator.Pedestrian.Exists() && x.Perpetrator.Pedestrian.IsDead);
            foreach (PedExt criminal in world.CivilianList.Where(x => x.Pedestrian.Exists() && x.IsCurrentlyViolatingAnyCivilianReportableCrimes && x.Pedestrian.IsAlive))
            {
                if (!PedExt.Pedestrian.Exists())
                {
                    break;
                }
                else
                {

                    float distanceToCriminal = PedExt.Pedestrian.DistanceTo2D(criminal.Pedestrian);
                    uint VehicleWitnessed = 0;
                    uint WeaponWitnessed = 0;
                    Vector3 LocationWitnessed = criminal.Pedestrian.Position;


                    VehicleExt fullVehicle = null;
                    WeaponInformation fullWeapon = null;

                    if (distanceToCriminal <= 60f)
                    {
                        Vehicle tryingToEnter = criminal.Pedestrian.VehicleTryingToEnter;
                        if (criminal.Pedestrian.IsInAnyVehicle(false) && criminal.Pedestrian.CurrentVehicle.Exists())
                        {
                            VehicleWitnessed = criminal.Pedestrian.CurrentVehicle.Handle;
                        }
                        else if (tryingToEnter.Exists())
                        {
                            VehicleWitnessed = tryingToEnter.Handle;
                        }
                        uint currentWeapon;
                        NativeFunction.Natives.GET_CURRENT_PED_WEAPON<bool>(criminal.Pedestrian, out currentWeapon, true);
                        if (currentWeapon != 2725352035 && currentWeapon != 0)
                        {
                            WeaponWitnessed = currentWeapon;
                        }

                        fullVehicle = world.GetVehicleExt(VehicleWitnessed);
                        fullWeapon = Weapons.GetWeapon((ulong)WeaponWitnessed);

                    }
                    else
                    {
                        VehicleWitnessed = 0;
                        WeaponWitnessed = 0;
                        LocationWitnessed = Vector3.Zero;
                        fullVehicle = null;
                        fullWeapon = null;
                    }
                    if (distanceToCriminal <= 60f && criminal.Pedestrian.IsThisPedInFrontOf(PedExt.Pedestrian))
                    {
                        foreach (Crime crime in criminal.CrimesCurrentlyViolating.Where(x => x.CanBeReportedByCivilians))
                        {
                            AddOtherPedObserved(crime, criminal, fullVehicle, fullWeapon, LocationWitnessed);
                            GameTimeLastWitnessedCivilianCrime = Game.GameTime;
                        }
                    }
                    else if (distanceToCriminal <= 100f)
                    {
                        foreach (Crime crime in criminal.CrimesCurrentlyViolating.Where(x => x.CanBeReportedByCivilians && x.CanReportBySound))
                        {
                            AddOtherPedObserved(crime, criminal, fullVehicle, fullWeapon, LocationWitnessed);
                            GameTimeLastWitnessedCivilianCrime = Game.GameTime;
                        }
                    }
                }
            }
        }
    }
    private void CheckCrimes(IEntityProvideable world, IPoliceRespondable player)
    {
        if (!PedExt.IsArrested)
        {
            bool isVisiblyArmed = IsVisiblyArmed();
            if (isVisiblyArmed)
            {
                if (!PedExt.IsInVehicle)
                {
                    AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "BrandishingWeapon"));
                }
            }
            if (isVisiblyArmed)
            {
                ShootingChecker();
                if (IsShooting)
                {
                    AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "FiringWeapon"));
                    if (world.PoliceList.Any(x => x.DistanceToPlayer <= 60f))//maybe store and do the actual one?
                    {
                        AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "FiringWeaponNearPolice"));
                    }
                }
            }
            else
            {
                IsShootingCheckerActive = false;
            }
            //foreach(PedExt civie in world.CivilianList)
            //{
            //    if (civie.Pedestrian.Exists() && civie.Pedestrian.IsDead && civie.WasKilledBy(PedExt.Pedestrian) && civie.Pedestrian.DistanceTo2D(civie.Pedestrian) <= Settings.SettingsManager.PlayerSettings.Violations_MurderDistance)
            //    {
            //        AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "KillingCivilians"));
            //        return;
            //    }
            //}

            if (PedExt.Pedestrian.IsInCombat || PedExt.Pedestrian.IsInMeleeCombat)
            {
                AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "AssaultingCivilians"));
                if (isVisiblyArmed)
                {
                    AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "AssaultingWithDeadlyWeapon"));
                }
                foreach (Cop cop in world.PoliceList)
                {
                    if (cop.Pedestrian.Exists())
                    {
                        if (PedExt.Pedestrian.CombatTarget.Exists() && PedExt.Pedestrian.CombatTarget.Handle == cop.Pedestrian.Handle)
                        {
                            AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "HurtingPolice"));
                            break;
                        }
                    }
                }
            }
            if (PedExt.Pedestrian.IsJacking)
            {
                AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "GrandTheftAuto"));
                GameTimeLastCommittedGTA = Game.GameTime;
            }
            if (Game.GameTime - GameTimeLastCommittedGTA <= 90000 && PedExt.IsInVehicle && PedExt.IsDriver)
            {
                AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "DrivingStolenVehicle"));
            }
            if (CrimesViolating.Any())
            {
                GameTimeLastCommittedCrime = Game.GameTime;
            }
            if (!PedExt.IsArrested && player.Investigation.IsActive && Game.GameTime - GameTimeLastCommittedCrime <= 25000 && PedExt.Pedestrian.DistanceTo2D(player.Investigation.Position) <= player.Investigation.Distance)
            {
                AddViolating(Crimes?.CrimeList.FirstOrDefault(x => x.ID == "SuspiciousActivity"));
            }
        }
    }
    private bool IsVisiblyArmed()
    {
        WeaponDescriptor CurrentWeapon = PedExt.Pedestrian.Inventory.EquippedWeapon;
        if (CurrentWeapon == null)
        {
            return false;
        }
        else if (CurrentWeapon.Hash == (WeaponHash)2725352035
            || CurrentWeapon.Hash == (WeaponHash)966099553
            || CurrentWeapon.Hash == (WeaponHash)0x787F0BB//weapon_snowball
            || CurrentWeapon.Hash == (WeaponHash)0x060EC506//weapon_fireextinguisher
            || CurrentWeapon.Hash == (WeaponHash)0x34A67B97//weapon_petrolcan
            || CurrentWeapon.Hash == (WeaponHash)0xBA536372//weapon_hazardcan
            || CurrentWeapon.Hash == (WeaponHash)0x8BB05FD7//weapon_flashlight
            || CurrentWeapon.Hash == (WeaponHash)0x23C9F95C)//weapon_ball
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void AddViolating(Crime crime)
    {
        if (crime != null && crime.Enabled && !CrimesViolating.Any(x=> x.ID == crime.ID))
        {
            CrimesViolating.Add(crime);
        }
    }
    private void AddObserved(Crime crime)
    {
        if (crime != null && crime.Enabled && !CrimesObserved.Any(x => x.ID == crime.ID))
        {
            CrimesObserved.Add(crime);
        }
    }
    private void AddOtherPedObserved(Crime crime, PedExt perpetrator, VehicleExt vehicle, WeaponInformation weapon, Vector3 location)
    {
        if (crime != null && crime.Enabled)
        {
            WitnessedCrime ExistingEvent = OtherPedCrimesObserved.FirstOrDefault(x => x.Crime?.ID == crime.ID && x.Perpetrator.Handle == perpetrator.Handle);
            if (ExistingEvent == null)
            {
                OtherPedCrimesObserved.Add(new WitnessedCrime(crime, perpetrator, vehicle, weapon, location));
            }
            else 
            {
                ExistingEvent.UpdateWitnessed(vehicle, weapon, location);
            }
        }
    }
    private void ResetViolations()
    {
        CrimesViolating.Clear();
    }
}