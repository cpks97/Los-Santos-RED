﻿using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;


public class VehicleRace
{
    private uint GameTimeStartedCountdown;
    private List<Blip> SpawnedBlips = new List<Blip>();
    private uint GameTimeStartedRace;
    private uint GameTimeFinishedRace;
    private uint GameTimePlayerFinishedRace;
    private bool HasWinner;
    private int BetAmount;
    private bool IsPinkSlipRace;
    private bool IsPlayerWinner;
    private IRaceable Player;
    private AIVehicleRacer AIWinner;
    private List<VehicleRacer> Finishers = new List<VehicleRacer>();
    public VehicleRace(string name, List<VehicleRaceCheckpoint> raceCheckpoints, List<VehicleRaceStartingPosition> vehicleRaceStartingPositions)
    {
        Name = name;
        RaceCheckpoints = raceCheckpoints;
        VehicleRaceStartingPositions = vehicleRaceStartingPositions;
    }
    public VehicleRace()
    {
    }

    public string Name { get; set; }

    public bool IsCountdownEnabled { get; set; } = true;
    public bool AreRacerBlipsEnabled { get; set; } = true;
    public List<VehicleRaceCheckpoint> RaceCheckpoints { get; set; }
    public List<VehicleRaceStartingPosition> VehicleRaceStartingPositions { get; set; }

    public bool HasRaceStarted => GameTimeStartedRace > 0;

    [XmlIgnore]
    public bool IsActive { get; set; }
    [XmlIgnore]
    public List<VehicleRacer> VehicleRacers
    {
        get
        {
            List<VehicleRacer> toReturn = new List<VehicleRacer>();
            toReturn.AddRange(AIVehicleRacers);
            toReturn.Add(PlayerRacer);
            return toReturn;
        }
    }
    [XmlIgnore]
    public List<AIVehicleRacer> AIVehicleRacers { get; set; }
    [XmlIgnore]
    public PlayerVehicleRacer PlayerRacer { get; set; }
    public void Setup(List<AIVehicleRacer> aivehicleRacers, PlayerVehicleRacer playerRacer, int betAmount, bool isForPinks)
    {
        VehicleRaceCheckpoint finishCheckpoint = RaceCheckpoints.OrderByDescending(x => x.Order).FirstOrDefault();
        if(finishCheckpoint != null)
        {
            finishCheckpoint.IsFinish = true;
        }
        AIVehicleRacers = aivehicleRacers;
        PlayerRacer = playerRacer;
        BetAmount = betAmount;
        IsPinkSlipRace = isForPinks;
    }
    public void Start(ITargetable targetable, IEntityProvideable World, ISettingsProvideable Settings, IRaceable player)
    {
        Player = player;
        GameFiber raceGameFiber = GameFiber.StartNew(delegate
        {
            ResetItems();
            IsActive = true;
            foreach (VehicleRacer vre in VehicleRacers)
            {
                vre.SetupRace(this);
            }
            RacePrelimiary();
            foreach (AIVehicleRacer vre in AIVehicleRacers)
            {
                vre.AssignTask(this, targetable, World, Settings);
            }
            foreach (VehicleRacer vre in VehicleRacers)
            {
                vre.SetRaceStart(this);
            }
            while (IsActive)
            {
                foreach (VehicleRacer vre in VehicleRacers)
                {
                    vre.Update(this);
                }
                if (!EntryPoint.ModController.IsRunning)
                {
                    IsActive = false;
                }
                EntryPoint.WriteToConsole($"GameTimePlayerFinishedRace {GameTimePlayerFinishedRace} IsActive {IsActive}");
                if (GameTimePlayerFinishedRace > 0 && Game.GameTime - GameTimePlayerFinishedRace >= 10000)
                {
                    IsActive = false;
                }
                GameFiber.Yield();
            }
            Dispose();
        }, "RaceGameFiber");
    }
    private void ResetItems()
    {
        GameTimeStartedRace = 0;
        GameTimeFinishedRace = 0;
        HasWinner = false;
        GameTimeStartedCountdown = 0;
        IsPlayerWinner = false;
        GameTimePlayerFinishedRace = 0;
        AIWinner = null;
    }
    public void Dispose()
    {
        PlayerRacer.Dispose();
        foreach (AIVehicleRacer vre in AIVehicleRacers)
        {
            vre.Dispose();     
        }
    }
    public void AddBlip(Blip checkpointBlip)
    {
        SpawnedBlips.Add(checkpointBlip);
    }
    private void RacePrelimiary()
    {
        if(IsCountdownEnabled)
        {
            DoCountdown();
        }     
        GameTimeStartedRace = Game.GameTime;
        PlayerRacer.ShowMessage("GO!");
    }
    private void DoCountdown()
    {
       //EntryPoint.WriteToConsole($"COUNTDOWN START {Game.GameTime - GameTimeStartedCountdown}");
        GameTimeStartedCountdown = Game.GameTime;
        string toShow = "";
        string showing = "";
        while (Game.GameTime - GameTimeStartedCountdown <= 3000)
        {
            NativeHelper.DisablePlayerMovementControl();
            if (Game.GameTime - GameTimeStartedCountdown < 1000)
            {
                toShow = "3";
            }
            else if (Game.GameTime - GameTimeStartedCountdown < 2000)
            {
                toShow = "2";
            }
            else if (Game.GameTime - GameTimeStartedCountdown < 3000)
            {
                toShow = "1";
            }
            if(toShow != showing)
            {
                PlayerRacer.ShowMessage(toShow);
                showing = toShow;
            }
            GameFiber.Yield();
        }
        //EntryPoint.WriteToConsole("COUNTDOWN END");
    }
    public void OnRacerFinishedRace(VehicleRacer vehicleRacer)
    {
        if(GameTimeFinishedRace == 0)
        {
            GameTimeFinishedRace = Game.GameTime;
        }
        Finishers.Add(vehicleRacer);
        int finalPosition = Finishers.Count();
        if (PlayerRacer.IsPlayer && finalPosition == 1)
        {
            IsPlayerWinner = true;
        }
        vehicleRacer.OnFinishedRace(finalPosition, this);    
        EntryPoint.WriteToConsole($" OnRacerFinishedRace {vehicleRacer.RacerName} {finalPosition} IsPlayerWinner:{IsPlayerWinner}");
        if (finalPosition == 1 && BetAmount > 0)
        {
            vehicleRacer.HandleWinningBet(BetAmount * VehicleRacers.Count());
        }
        if(!PlayerRacer.HasFinishedRace && finalPosition == 1)
        {
            PlayerRacer.ShowMessage($"Winner: {vehicleRacer.RacerName}", vehicleRacer.GetTotalTimeAsString());
        }
    }
    public void OnPlayerFinishedRace()
    {
        EntryPoint.WriteToConsole($"OnPlayerFinishedRace IsPlayerWinner: {IsPlayerWinner} GameTimePlayerFinishedRace{GameTimePlayerFinishedRace}");
        GameTimePlayerFinishedRace = Game.GameTime;
        if (IsPinkSlipRace)
        {
            HandlePinkSlips();
        }
        if(BetAmount > 0)
        {
            HandleBetting();
        }
    }
    private void HandleBetting()
    {
        int winAmount = VehicleRacers.Count() * BetAmount;
        if (IsPlayerWinner)
        {   
            Player.BankAccounts.GiveMoney(winAmount, false);
            EntryPoint.WriteToConsole($"Player is winner giving {winAmount}");
        }
    }
    public void AddTrackToMenu(MenuPool menuPool, UIMenu raceMenu)
    {
        UIMenu vehicleRaceMenuItem = menuPool.AddSubMenu(raceMenu, Name);
        vehicleRaceMenuItem.RemoveBanner();
        UIMenuItem startRace = new UIMenuItem(Name);
        startRace.Activated += (sender, selectedItem) =>
        {
            sender.Visible = false;
        };
        vehicleRaceMenuItem.AddItem(startRace);
    }
    public void Cancel(IRaceable player)
    {
        ReturnBet(player);
        IsActive = false;
        Game.DisplaySubtitle("You have cancelled the race");
    }
    private void ReturnBet(IRaceable player)
    {
        if(BetAmount > 0)
        {
            player.BankAccounts.GiveMoney(BetAmount, false);
        }
    }
    public void Forfeit()
    {
        IsActive = false;
        Game.DisplaySubtitle("You have forfeited the race");
    }
    public void HandlePinkSlips()
    {
        //Game.FadeScreenOut(1000, true);
        if(IsPlayerWinner)
        {
            foreach(AIVehicleRacer airacer in AIVehicleRacers)
            {
                if(airacer.PedExt != null && airacer.PedExt.Pedestrian.Exists() && airacer.PedExt.Pedestrian.CurrentVehicle.Exists())
                {
                    airacer.PedExt.Pedestrian.ClearLastVehicle();
                    airacer.PedExt.AssignedVehicle = null;
                    unsafe
                    {
                        int lol = 0;
                        NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                        NativeFunction.CallByName<uint>("TASK_VEHICLE_TEMP_ACTION", 0, airacer.PedExt.Pedestrian.CurrentVehicle, 27, 10000);
                        NativeFunction.CallByName<bool>("TASK_LEAVE_VEHICLE", 0, airacer.PedExt.Pedestrian.CurrentVehicle, airacer.PedExt.DefaultEnterExitFlag);// 256);
                        NativeFunction.CallByName<uint>("TASK_WANDER_STANDARD", 0, 0,0);
                        NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, false);
                        NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                        NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", airacer.PedExt.Pedestrian, lol);
                        NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
                    }

                    //NativeFunction.Natives.TASK_WANDER_STANDARD(PedGeneral.Pedestrian, 0, 0);
                }
                if (airacer.VehicleExt != null)
                {
                    Player.VehicleOwnership.TakeOwnershipOfVehicle(airacer.VehicleExt, false);
                }
            }
        }
        else
        {
            if (Player.Character.CurrentVehicle.Exists())
            {
                unsafe
                {
                    int lol = 0;
                    NativeFunction.CallByName<bool>("OPEN_SEQUENCE_TASK", &lol);
                    NativeFunction.CallByName<uint>("TASK_VEHICLE_TEMP_ACTION", 0, Player.Character.CurrentVehicle, 27, 3000);
                    NativeFunction.CallByName<bool>("TASK_LEAVE_VEHICLE", 0, Player.Character.CurrentVehicle, 0);// 256);
                    NativeFunction.CallByName<bool>("SET_SEQUENCE_TO_REPEAT", lol, false);
                    NativeFunction.CallByName<bool>("CLOSE_SEQUENCE_TASK", lol);
                    NativeFunction.CallByName<bool>("TASK_PERFORM_SEQUENCE", Player.Character, lol);
                    NativeFunction.CallByName<bool>("CLEAR_SEQUENCE_TASK", &lol);
                }
            }
            Player.VehicleOwnership.RemoveOwnershipOfVehicle(Player.CurrentVehicle);
        }
        //Game.FadeScreenIn(1000, false);
    }
}

