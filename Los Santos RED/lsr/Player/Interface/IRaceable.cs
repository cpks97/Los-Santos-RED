﻿using LSR.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosSantosRED.lsr.Interface
{
    public interface IRaceable
    {
        VehicleExt CurrentVehicle { get; }
        GPSManager GPSManager { get; }

        string PlayerName { get; }
        bool IsSetDisabledControls { get; set; }
        RacingManager RacingManager { get; }
    }
}
