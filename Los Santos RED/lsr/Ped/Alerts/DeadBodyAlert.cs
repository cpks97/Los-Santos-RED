﻿using ExtensionsMethods;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Mod;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DeadBodyAlert : PedAlert
{
    public List<BodyWatcher> BodiesSeen { get; private set; } = new List<BodyWatcher>();
    public DeadBodyAlert(PedExt pedExt, ISettingsProvideable settings) : base(pedExt, settings, ePedAlertType.DeadBody)
    {
        Priority = 1;
        IsPositionAlert = false;
        IsPedAlert = true;
        TimeOutTime = Settings.SettingsManager.WorldSettings.DeadBodyAlertTimeout;
    }
    public override void Update(IPoliceRespondable policeRespondable, IEntityProvideable world)
    {
        if (!PedExt.PedAlertTypes.HasFlag(ePedAlertType.DeadBody))
        {
            return;
        }
        if (!PedExt.Pedestrian.Exists() || PedExt.IsUnconscious || PedExt.PlayerPerception.DistanceToTarget > 150f)// || world.TotalWantedLevel >= 3)
        {
            return;
        }
        foreach (PedExt deadBody in world.Pedestrians.DeadPeds.Where(x => !BodiesSeen.Any(y => y.PedBody?.Handle == x.Handle) && x.Pedestrian.Exists() && PedExt.Pedestrian.Exists()))
        {
            float distanceToBody = PedExt.Pedestrian.DistanceTo2D(deadBody.Pedestrian);
            bool CanSeeBody = false;
            if (!deadBody.WasKilledByPlayer && distanceToBody <= 15f)
            {
                CanSeeBody = true;
            }
            else if (distanceToBody <= 45f && world.TotalWantedLevel >= 3)
            {
                CanSeeBody = true;
            }
            else if (distanceToBody <= 45f && deadBody.Pedestrian.IsThisPedInFrontOf(PedExt.Pedestrian) && (deadBody.HasBeenSeenDead || NativeFunction.CallByName<bool>("HAS_ENTITY_CLEAR_LOS_TO_ENTITY_IN_FRONT", PedExt.Pedestrian, deadBody.Pedestrian)))
            {
                CanSeeBody = true;
                GameFiber.Yield();
            }
            if (CanSeeBody)
            {
                deadBody.SetWasSeenDead();
                deadBody.HasBeenSeenDead = true;
                PedExt.SetSeenDead(deadBody);
                BodiesSeen.Add(new BodyWatcher(deadBody));
                EntryPoint.WriteToConsole($"I AM PED {PedExt.Handle} AND I JUST SAW A DEAD BODY {deadBody.Handle}");
                if (deadBody.GeneratesBodyAlerts)
                {
                    AddAlert(deadBody);
                }
            }
        }
        BodiesSeen.RemoveAll(x => x.PedBody == null || !x.PedBody.Pedestrian.Exists());
        //BodiesSeen.ForEach(x => x.CheckReports());
        base.Update(policeRespondable, world);
        if(HasReachedPosition)
        {
            BodiesSeen.ForEach(x => x.DisableAlerts());
        }
    }
}

