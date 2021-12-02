﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EMSSettings
{
    public bool ManageDispatching { get; set; } = true;
    public bool ManageTasking { get; set; } = true;
    public bool ShowSpawnedBlips { get; set; } = false;
    public EMSSettings()
    {
        #if DEBUG
            //ShowSpawnedBlips =  true;
            ManageDispatching = false;
            ManageTasking = false;
        #else
            ShowSpawnedBlips = false;
        #endif
    }
}