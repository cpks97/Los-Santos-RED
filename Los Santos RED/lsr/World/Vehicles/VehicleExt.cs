﻿using ExtensionsMethods;
using LosSantosRED.lsr;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LSR.Vehicles
{
    public class VehicleExt
    {
        private uint GameTimeEntered = 0;
        public Vehicle Vehicle { get; set; } = null;
        public Engine Engine { get; set; } //= new Engine();
        public Radio Radio { get; set; }// = new Radio();
        public Indicators Indicators { get; set; }// = new Indicators();
        public FuelTank FuelTank { get; set; }// = new FuelTank();   
        public Color DescriptionColor { get; set; }
        public LicensePlate CarPlate { get; set; }
        public LicensePlate OriginalLicensePlate { get; set; }
        public bool ManuallyRolledDriverWindowDown { get; set; }
        public Vector3 PositionOriginallyEntered { get; set; } = Vector3.Zero;
        public bool HasBeenDescribedByDispatch { get; set; }
        public bool WasAlarmed { get; set; }
        public bool WasJacked { get; set; }
        public bool IsStolen { get; set; }
        public bool WasReportedStolen { get; set; }
        public bool HasUpdatedPlateType { get; private set; }
        public bool NeedsToBeReportedStolen
        {
            get
            {
                if (!WasReportedStolen && Game.GameTime > GameTimeToReportStolen && GameTimeEntered > 0)
                    return true;
                else
                    return false;
            }
        }
        public uint GameTimeToReportStolen
        {
            get
            {
                if (WasAlarmed && GameTimeEntered > 0)
                    return GameTimeEntered + 100000;
                else if (GameTimeEntered > 0)
                    return GameTimeEntered + 600000;
                else
                    return 0;
            }
        }
        public bool ColorMatchesDescription
        {
            get
            {
                if (Vehicle.PrimaryColor == DescriptionColor)
                    return true;
                else
                    return false;
            }
        }
        public bool HasOriginalPlate
        {
            get
            {
                if (CarPlate.PlateNumber == OriginalLicensePlate.PlateNumber)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool CopsRecognizeAsStolen
        {
            get
            {
                if (!IsStolen)
                    return false;
                else
                {
                    if (CarPlate.IsWanted)
                        return true;
                    else if (WasReportedStolen && ColorMatchesDescription)
                        return true;
                    else
                        return false;
                }
            }
        }
        public bool CanUpdatePlate
        {
            get
            {
                VehicleClass CurrentClass = (VehicleClass)ClassInt();
                if (CurrentClass == VehicleClass.Compact)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.Coupe)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.Muscle)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.OffRoad)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.Sedan)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.Sport)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.SportClassic)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.Super)
                {
                    return true;
                }
                else if (CurrentClass == VehicleClass.SUV)
                {
                    return true;
                }
                return false;
            }
        }
        public bool HasBeenEnteredByPlayer
        {
            get
            {
                if(GameTimeEntered == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public VehicleLockStatus OriginalLockStatus { get; private set; }
        public bool OriginalEngineStatus { get; private set; }
        public VehicleExt(Vehicle _Vehicle, uint _GameTimeEntered, bool _WasJacked, bool _WasAlarmed, bool _IsStolen, LicensePlate _CarPlate)
        {
            Vehicle = _Vehicle;
            GameTimeEntered = _GameTimeEntered;
            WasJacked = _WasJacked;
            WasAlarmed = _WasAlarmed;
            IsStolen = _IsStolen;

            DescriptionColor = _Vehicle.PrimaryColor;
            CarPlate = _CarPlate;
            OriginalLicensePlate = _CarPlate;

            if (Vehicle.Exists())
                PositionOriginallyEntered = Vehicle.Position;
            else
                PositionOriginallyEntered = Game.LocalPlayer.Character.Position;

            _Vehicle.FuelLevel = RandomItems.MyRand.Next(25, 100);
            OriginalLockStatus = _Vehicle.LockStatus;
            OriginalEngineStatus = _Vehicle.IsEngineOn;
            _Vehicle.SetLock((VehicleLockStatus)7);

            Engine = new Engine(this);
            Radio = new Radio(this);
            Indicators = new Indicators(this);
            FuelTank = new FuelTank(this);
        }
        public VehicleExt(Vehicle _Vehicle)
        {
            Vehicle = _Vehicle;
            DescriptionColor = _Vehicle.PrimaryColor;   
            LicensePlate _CarPlate = new LicensePlate(_Vehicle.LicensePlate, (uint)_Vehicle.Handle, NativeFunction.CallByName<int>("GET_VEHICLE_NUMBER_PLATE_TEXT_INDEX", _Vehicle), false);
            CarPlate = _CarPlate;
            OriginalLicensePlate = _CarPlate;
            _Vehicle.FuelLevel = RandomItems.MyRand.Next(25, 100);
            OriginalLockStatus = _Vehicle.LockStatus;
            OriginalEngineStatus = _Vehicle.IsEngineOn;
            _Vehicle.SetLock((VehicleLockStatus)7);

            Engine = new Engine(this);
            Radio = new Radio(this);
            Indicators = new Indicators(this);
            FuelTank = new FuelTank(this);
        }
        public void SetAsEntered()
        {
            if (GameTimeEntered == 0)
                GameTimeEntered = Game.GameTime;
        }
        public Color VehicleColor()
        {
            if (Vehicle.Exists())
            {
                Color BaseColor = Extensions.GetBaseColor(DescriptionColor);
                return BaseColor;
            }
            else
            {
                return Color.White;
            }
        }
        public string MakeName()
        {
            if (Vehicle.Exists())
            {
                string MakeName;
                unsafe
                {
                    IntPtr ptr = NativeFunction.CallByHash<IntPtr>(0xF7AF4F159FF99F97, Vehicle.Model.Hash);
                    MakeName = Marshal.PtrToStringAnsi(ptr);
                }
                unsafe
                {
                    IntPtr ptr2 = NativeFunction.CallByHash<IntPtr>(0x7B5280EBA9840C72, MakeName);
                    MakeName = Marshal.PtrToStringAnsi(ptr2);
                }
                if (MakeName == "CARNOTFOUND" || MakeName == "NULL")
                    return "";
                else
                    return MakeName;
            }
            else
            {
                return "";
            }

        }
        public string ModelName()
        {
            if (Vehicle.Exists())
            {
                string ModelName;
                unsafe
                {
                    IntPtr ptr = NativeFunction.CallByName<IntPtr>("GET_DISPLAY_NAME_FROM_VEHICLE_MODEL", Vehicle.Model.Hash);
                    ModelName = Marshal.PtrToStringAnsi(ptr);
                }
                unsafe
                {
                    IntPtr ptr2 = NativeFunction.CallByHash<IntPtr>(0x7B5280EBA9840C72, ModelName);
                    ModelName = Marshal.PtrToStringAnsi(ptr2);
                }
                if (ModelName == "CARNOTFOUND" || ModelName == "NULL")
                    return "";
                else
                    return ModelName;
            }
            else
            {
                return "";
            }
        }
        public int ClassInt()
        {
            int ClassInt = NativeFunction.CallByName<int>("GET_VEHICLE_CLASS", Vehicle);
            return ClassInt;
        }
        public void UpdatePlate()//this might need to come out of here.... along with the two bools
        {
            HasUpdatedPlateType = true;
            PlateType CurrentType = Mod.DataMart.PlateTypes.GetPlateType(NativeFunction.CallByName<int>("GET_VEHICLE_NUMBER_PLATE_TEXT_INDEX", Vehicle));
            string CurrentPlateNumber = Vehicle.LicensePlate;
            if (RandomItems.RandomPercent(10) && CurrentType != null && CurrentType.CanOverwrite && CanUpdatePlate)
            {
                PlateType NewType = Mod.DataMart.PlateTypes.GetRandomPlateType();
                if (NewType != null)
                {
                    string NewPlateNumber = NewType.GenerateNewLicensePlateNumber();
                    if (NewPlateNumber != "")
                    {
                        Vehicle.LicensePlate = NewPlateNumber;
                        OriginalLicensePlate.PlateNumber = NewPlateNumber;
                        CarPlate.PlateNumber = NewPlateNumber;
                    }
                    NativeFunction.CallByName<int>("SET_VEHICLE_NUMBER_PLATE_TEXT_INDEX", Vehicle, NewType.Index);
                    OriginalLicensePlate.PlateType = NewType.Index;
                    CarPlate.PlateType = NewType.Index;
                }
            }
        }
        public void Update()
        {
            if (IsCar)
            {
                Engine.Update();
                Radio.Update();
                Indicators.Update();
                FuelTank.Update();
            }
        }
        public bool IsCar
        {
            get
            {
                return NativeFunction.CallByName<bool>("IS_THIS_MODEL_A_CAR", Vehicle.Model.Hash);
            }
        }
        //public bool SetLock(VehicleLockStatus DesiredStatus)
        //{
        //    if (Vehicle.LockStatus != (VehicleLockStatus)1 && Vehicle.LockStatus != (VehicleLockStatus)7)
        //    {
        //        return false;
        //    }
        //    if (Vehicle.HasDriver)
        //    {
        //        return false;
        //    }
        //    foreach (VehicleDoor myDoor in Vehicle.GetDoors())
        //    {
        //        if (!myDoor.IsValid() || myDoor.IsOpen)
        //        {
        //            return false;//invalid doors make the car not locked
        //        }
        //    }
        //    if (!NativeFunction.CallByName<bool>("ARE_ALL_VEHICLE_WINDOWS_INTACT", Vehicle))
        //    {
        //        return false;//broken windows == not locked
        //    }
        //    if (Vehicle.IsConvertible && Vehicle.ConvertibleRoofState == VehicleConvertibleRoofState.Lowered)
        //    {
        //        return false;
        //    }
        //    if (Vehicle.IsBike || Vehicle.IsPlane || Vehicle.IsHelicopter)
        //    {
        //        return false;
        //    }

        //    Vehicle.MustBeHotwired = true;
        //    Vehicle.LockStatus = DesiredStatus;//Locked for player
        //    return true;
        //}
    }
}