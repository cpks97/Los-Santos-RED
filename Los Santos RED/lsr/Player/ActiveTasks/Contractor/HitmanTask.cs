using ExtensionsMethods;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using LSR.Vehicles;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LosSantosRED.lsr.Player.ActiveTasks
{
    public class HitmanTask : IPlayerTask
    {
        private ITaskAssignable Player;
        private ITimeReportable Time;
        private IGangs Gangs;
        private PlayerTasks PlayerTasks;
        private IPlacesOfInterest PlacesOfInterest;
        private List<DeadDrop> ActiveDrops = new List<DeadDrop>();
        private ISettingsProvideable Settings;
        private IEntityProvideable World;
        private ICrimes Crimes;
        private IShopMenus ShopMenus;
        private PlayerTask CurrentTask;
        private int MoneyToRecieve;
        private DeadDrop myDrop;
        private IWeapons Weapons;
        private INameProvideable Names;
        private bool TargetIsMale;
        private string TargetName;
        private bool TargetIsAtHome;
        private GameLocation TargetLocation;
        private Vector3 TargetSpawnPosition;
        private float TargetSpawnHeading;
        private readonly List<string> FemaleTargetPossibleModels = new List<string>() { "a_f_o_ktown_01", "a_f_o_soucent_01","a_f_o_genstreet_01", "a_f_o_soucent_02", "a_f_y_bevhills_01", "a_f_y_bevhills_02","a_f_y_business_01", "a_f_y_business_02", "a_f_y_business_03", "a_f_y_business_04",
            "a_f_y_genhot_01", "a_f_y_fitness_01", "a_f_m_business_02", "a_f_y_clubcust_01", "a_f_y_femaleagent","a_f_y_eastsa_03","a_f_y_hiker_01","a_f_y_hipster_01","a_f_y_hipster_04","a_f_y_skater_01","a_f_y_soucent_03","a_f_y_tennis_01","a_f_y_vinewood_01","a_f_y_tourist_02" };

        private readonly List<string> MaleTargetPossibleModels = new List<string>() { "a_m_m_afriamer_01", "a_m_m_beach_01","a_m_m_bevhills_01", "a_m_m_bevhills_02", "a_m_m_business_01", "a_m_m_fatlatin_01","a_m_m_genfat_01", "a_m_m_malibu_01", "a_m_m_ktown_01", "a_m_m_mexcntry_01",
            "a_m_m_soucent_01", "a_m_m_soucent_02", "a_m_m_tourist_01", "a_m_y_bevhills_01", "a_m_y_bevhills_02","a_m_y_beachvesp_01","a_m_y_business_02","a_m_y_business_01","a_m_y_business_03","a_m_y_clubcust_01","a_m_y_genstreet_01","a_m_y_genstreet_02","a_m_y_hipster_01","a_m_y_hipster_03","a_m_y_ktown_02","a_m_y_polynesian_01","a_m_y_soucent_02" };

        private bool HasSpawnPosition => TargetSpawnPosition != Vector3.Zero;
        private int SpawnPositionCellX;
        private int SpawnPositionCellY;
        private bool IsTargetSpawned;
        private int GameTimeToWaitBeforeComplications;
        private string TargetModel;

        private PedExt Target;
        private PedVariation TargetVariation;
        private bool HasAddedComplications;
        private bool WillAddComplications;
        private object pedHeadshotHandle;
        private bool TargetIsCustomer;
        private bool WillFlee;
        private bool WillFight;
        private ShopMenu TargetShopMenu;
        private WeaponInformation TargetWeapon;
        private ContractorContact Contact;

        private bool IsPlayerFarFromTarget => Target != null && Target.Pedestrian.Exists() && Target.Pedestrian.DistanceTo2D(Player.Character) >= 850f;
        private bool IsPlayerNearTargetSpawn => SpawnPositionCellX != -1 && SpawnPositionCellY != -1 && NativeHelper.IsNearby(EntryPoint.FocusCellX, EntryPoint.FocusCellY, SpawnPositionCellX, SpawnPositionCellY, 6);
        public HitmanTask(ITaskAssignable player, ITimeReportable time, IGangs gangs, PlayerTasks playerTasks, IPlacesOfInterest placesOfInterest, List<DeadDrop> activeDrops, ISettingsProvideable settings, IEntityProvideable world,
            ICrimes crimes, INameProvideable names, IWeapons weapons, IShopMenus shopMenus, ContractorContact contractorContact)
        {
            Player = player;
            Time = time;
            Gangs = gangs;
            PlayerTasks = playerTasks;
            PlacesOfInterest = placesOfInterest;
            ActiveDrops = activeDrops;
            Settings = settings;
            World = world;
            Crimes = crimes;
            Names = names;
            Weapons = weapons;
            ShopMenus = shopMenus;
            Contact = contractorContact;
        }
        public void Setup()
        {

        }
        public void Dispose()
        {
            if (Target != null && Target.Pedestrian.Exists())
            {
                Target.DeleteBlip();
                Target.Pedestrian.IsPersistent = false;
                Target.Pedestrian.Delete();
            }
            if (TargetLocation != null)
            {
                TargetLocation.IsPlayerInterestedInLocation = false;
            }
        }
        public void Start(ContractorContact contact)
        {
            Contact = contact;
            if (Contact == null)
            {
                return;
            }
            if (PlayerTasks.CanStartNewTask(Contact.Name))
            {
                GetPedInformation();
                if (HasSpawnPosition)
                {
                    GetPayment();
                    SendInitialInstructionsMessage();
                    AddTask();
                    GameFiber PayoffFiber = GameFiber.StartNew(delegate
                    {
                        try
                        {
                            Loop();
                            FinishTask();
                        }
                        catch (Exception ex)
                        {
                            EntryPoint.WriteToConsole(ex.Message + " " + ex.StackTrace, 0);
                            EntryPoint.ModController.CrashUnload();
                        }
                    }, "PayoffFiber");
                }
                else
                {
                    SendTaskAbortMessage();
                }
            }
        }
        private void GetPedInformation()
        {
            TargetIsMale = RandomItems.RandomPercent(60);
            TargetName = Names.GetRandomName(TargetIsMale);
            TargetIsAtHome = RandomItems.RandomPercent(30);
            if (TargetIsAtHome)
            {
                TargetLocation = PlacesOfInterest.PossibleLocations.Residences.Where(x => !x.IsOwnedOrRented && x.IsCorrectMap(World.IsMPMapLoaded) && x.IsSameState(Player.CurrentLocation?.CurrentZone?.GameState)).PickRandom();
            }
            else
            {
                TargetLocation = PlacesOfInterest.PossibleLocations.TargetTaskLocations().Where(x => x.IsCorrectMap(World.IsMPMapLoaded) && x.IsSameState(Player.CurrentLocation?.CurrentZone?.GameState)).PickRandom();
            }

            TargetVariation = null;
            if (TargetIsMale)
            {
                TargetModel = MaleTargetPossibleModels.Where(x => Player.ModelName.ToLower() != x.ToLower()).PickRandom();
            }
            else
            {
                TargetModel = FemaleTargetPossibleModels.Where(x => Player.ModelName.ToLower() != x.ToLower()).PickRandom();
            }
            if (TargetLocation != null)
            {
                TargetSpawnPosition = TargetLocation.EntrancePosition;
                TargetSpawnHeading = TargetLocation.EntranceHeading;
                SpawnPositionCellX = (int)(TargetSpawnPosition.X / EntryPoint.CellSize);
                SpawnPositionCellY = (int)(TargetSpawnPosition.Y / EntryPoint.CellSize);
            }
            else
            {
                TargetSpawnPosition = Vector3.Zero;
                SpawnPositionCellX = -1;
                SpawnPositionCellY = -1;
            }
        }
        private void Loop()
        {
            while (true)
            {
                if (CurrentTask == null || !CurrentTask.IsActive)
                {
                    //EntryPoint.WriteToConsoleTestLong($"Task Inactive for {Contact.Name}");
                    break;
                }
                if (!IsTargetSpawned && IsPlayerNearTargetSpawn)
                {
                    IsTargetSpawned = SpawnTarget();
                }
                if (IsTargetSpawned && IsPlayerFarFromTarget)
                {
                    DespawnTarget();
                    if (Target.HasSeenPlayerCommitCrime)
                    {
                        //EntryPoint.WriteToConsoleTestLong("Hitman job TARGET FLED");
                        Game.DisplayHelp($"{Contact.Name} The target fled");
                        break;
                    }
                }
                else if (IsTargetSpawned && Target != null && Target.Pedestrian.Exists() && Target.Pedestrian.IsDead)
                {
                    Target.Pedestrian.IsPersistent = false;
                    Target.DeleteBlip();
                    //EntryPoint.WriteToConsoleTestLong("Hitman Job TARGET WAS KILLED");
                    CurrentTask.OnReadyForPayment(true);
                    break;
                }
                if (IsTargetSpawned && Target != null && !Target.Pedestrian.Exists())//somehow it got removed, set it as despawned
                {
                    DespawnTarget();
                }
                GameFiber.Sleep(1000);
            }
        }
        private void FinishTask()
        {
            if (TargetLocation != null)
            {
                TargetLocation.IsPlayerInterestedInLocation = false;
            }
            if (CurrentTask != null && CurrentTask.IsActive && CurrentTask.IsReadyForPayment)
            {
                //GameFiber.Sleep(RandomItems.GetRandomNumberInt(5000, 10000));

                StartDeadDropPayment();//sets u teh whole dead drop thingamajic
            }
            else if (CurrentTask != null && CurrentTask.IsActive)
            {
                //GameFiber.Sleep(RandomItems.GetRandomNumberInt(5000, 10000));
                SetFailed();
            }
            else
            {
                Dispose();
            }
        }
        private void SetCompleted()
        {
            //EntryPoint.WriteToConsoleTestLong("Hitman Job COMPLETED");

            PlayerTasks.CompleteTask(Contact, true);

            SendCompletedMessage();
        }
        private void SetFailed()
        {
            //EntryPoint.WriteToConsoleTestLong("Hitman Job FAILED");
            SendFailMessage();
            PlayerTasks.FailTask(Contact);
        }
        private void StartDeadDropPayment()
        {
            myDrop = PlacesOfInterest.GetUsableDeadDrop(World.IsMPMapLoaded, Player.CurrentLocation);
            if (myDrop != null)
            {
                myDrop.SetupDrop(MoneyToRecieve, false);
                ActiveDrops.Add(myDrop);
                SendDeadDropStartMessage();
                while (true)
                {
                    if (CurrentTask == null || !CurrentTask.IsActive)
                    {
                        //EntryPoint.WriteToConsoleTestLong($"Task Inactive for {Contact.Name}");
                        break;
                    }
                    if (myDrop.InteractionComplete)
                    {
                        //EntryPoint.WriteToConsoleTestLong($"Picked up money for Hitman task for {Contact.Name}");
                        Game.DisplayHelp($"{Contact.Name} Money Picked Up");
                        break;
                    }
                    GameFiber.Sleep(1000);
                }
                if (CurrentTask != null && CurrentTask.IsActive && CurrentTask.IsReadyForPayment)
                {
                    PlayerTasks.CompleteTask(Contact, true);
                }
                myDrop?.Reset();
                myDrop?.Deactivate(true);
            }
            else
            {

                PlayerTasks.CompleteTask(Contact, true);
                SendQuickPaymentMessage();
            }
        }
        private void AddTask()
        {
            //EntryPoint.WriteToConsoleTestLong($"You are hired to kill a target!");
            PlayerTasks.AddTask(Contact, 0, 2000, 0, -500, 7, "Hit Contract");
            CurrentTask = PlayerTasks.GetTask(Contact.Name);
            IsTargetSpawned = false;
            GameTimeToWaitBeforeComplications = RandomItems.GetRandomNumberInt(3000, 10000);
            HasAddedComplications = false;
            WillAddComplications = RandomItems.RandomPercent(Settings.SettingsManager.TaskSettings.ContractorHitmanTaskComplicationsPercentage);
            WillFlee = false;
            WillFight = false;
            if (WillAddComplications)
            {
                if (RandomItems.RandomPercent(50))
                {
                    WillFlee = true;
                }
                else
                {
                    WillFight = true;
                }
            }
            TargetWeapon = null;
            if (RandomItems.RandomPercent(40))
            {
                Weapons.GetRandomRegularWeapon(WeaponCategory.Melee);
            }
            else
            {
                if (RandomItems.RandomPercent(50))
                {
                    Weapons.GetRandomRegularWeapon(WeaponCategory.Pistol);
                }
                else
                {
                    if (RandomItems.RandomPercent(50))
                    {
                        Weapons.GetRandomRegularWeapon(WeaponCategory.AR);
                    }
                    else
                    {
                        Weapons.GetRandomRegularWeapon(WeaponCategory.Shotgun);
                    }
                }
            }
            TargetShopMenu = null;
            TargetIsCustomer = RandomItems.RandomPercent(30f);
            if (TargetIsCustomer)
            {
                TargetShopMenu = ShopMenus.GetRandomDrugCustomerMenu();
            }

            if (TargetLocation != null)
            {
                TargetLocation.IsPlayerInterestedInLocation = true;
            }
        }
        private bool SpawnTarget()
        {
            if (TargetSpawnPosition != Vector3.Zero)
            {
                World.Pedestrians.CleanupAmbient();
                Ped ped = new Ped(TargetModel, TargetSpawnPosition, TargetSpawnHeading);
                GameFiber.Yield();
                NativeFunction.Natives.SET_MODEL_AS_NO_LONGER_NEEDED(Game.GetHashKey(TargetModel));
                if (ped.Exists())
                {
                    string GroupName = "Man";
                    if (!TargetIsMale)
                    {
                        GroupName = "Woman";
                    }
                    Target = new PedExt(ped, Settings, Crimes, Weapons, TargetName, GroupName, World);
                    if (Settings.SettingsManager.TaskSettings.ShowEntityBlips)
                    {
                        Target.AddBlip();
                    }
                    World.Pedestrians.AddEntity(Target);
                    Target.WasEverSetPersistent = true;
                    Target.CanBeAmbientTasked = true;
                    Target.CanBeTasked = true;
                    Target.WasModSpawned = true;
                    if (TargetVariation == null)
                    {
                        Target.Pedestrian.RandomizeVariation();
                        TargetVariation = NativeHelper.GetPedVariation(Target.Pedestrian);
                    }
                    else
                    {
                        TargetVariation.ApplyToPed(Target.Pedestrian);
                    }
                    pedHeadshotHandle = NativeFunction.Natives.REGISTER_PEDHEADSHOT<int>(ped);
                    if (TargetIsCustomer)
                    {
                        Target.SetupTransactionItems(TargetShopMenu, false);
                    }
                    if (WillAddComplications)
                    {
                        ped.RelationshipGroup = RelationshipGroup.HatesPlayer;
                        if (WillFlee)//flee
                        {
                            Target.WillCallPolice = true;
                            Target.WillCallPoliceIntense = true;
                            Target.WillFight = false;
                            Target.WillFightPolice = false;
                            Target.WillAlwaysFightPolice = false;
                            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, (int)eCombatAttributes.BF_AlwaysFlee, true);
                            NativeFunction.Natives.SET_PED_FLEE_ATTRIBUTES(ped, 2, true);
                            //EntryPoint.WriteToConsoleTestLong("HITMAN TASK, THE TARGET WITH FLEE FROM YOU");
                        }
                        else if (WillFight)
                        {
                            Target.WillFight = true;
                            Target.WillCallPolice = false;
                            Target.WillCallPoliceIntense = false;
                            Target.WillFightPolice = true;
                            Target.WillAlwaysFightPolice = true;
                            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, (int)eCombatAttributes.BF_AlwaysFight, true);
                            NativeFunction.Natives.SET_PED_COMBAT_ATTRIBUTES(ped, (int)eCombatAttributes.BF_CanFightArmedPedsWhenNotArmed, true);
                            NativeFunction.Natives.SET_PED_FLEE_ATTRIBUTES(ped, 0, false);

                            if (TargetWeapon != null)
                            {
                                NativeFunction.Natives.GIVE_WEAPON_TO_PED(ped, (uint)TargetWeapon.Hash, TargetWeapon.AmmoAmount, false, false);
                            }
                            //EntryPoint.WriteToConsoleTestLong("Hitman Contract, THE TARGET WITH FIGHT YOU");
                        }
                        //they either know and flee, or know and fight     
                    }
                    GameFiber.Sleep(1000);
                    SendTargetSpawnedMessage();
                    return true;
                }
            }
            return false;
        }
        private void SendTargetSpawnedMessage()
        {
            List<string> Replies;
            string LookingForItem = "";
            if (TargetIsCustomer)
            {
                MenuItem myMenuItem = Target.ShopMenu?.Items.Where(x => x.NumberOfItemsToPurchaseFromPlayer > 0 && x.IsIllicilt).PickRandom();
                if (myMenuItem != null)
                {
                    LookingForItem = myMenuItem.ModItemName;
                }
            }
            if (NativeFunction.Natives.IsPedheadshotReady<bool>(pedHeadshotHandle))
            {
                Replies = new List<string>() {
                    $"Picture of ~y~{TargetName}~s~ attached. I heard they were still around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"Sent you a picture of ~y~{TargetName}~s~. They should still be around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"~y~{TargetName}~s~. They are still around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"The name is ~y~{TargetName}~s~, pic attached. They are loitering around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"Remember, ~y~{TargetName}~s~ is the name. I also sent a picture. I got word they are still around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                     };
                string PickedReply = Replies.PickRandom();
                if (TargetIsCustomer && LookingForItem != "")
                {
                    List<string> ItemReplies = new List<string>() {
                    $" They are probably looking for ~p~{LookingForItem}~s~.",
                    $" They like ~p~{LookingForItem}~s~.",
                    $" Will be looking to buy ~p~{LookingForItem}~s~.",
                    $" They are interested in ~p~{LookingForItem}~s~.",
                    $" The target likes ~p~{LookingForItem}~s~.",
                     };
                    PickedReply += ItemReplies.PickRandom();
                }

                if (WillFight || WillFlee)
                {
                    PickedReply += " ~s~The target might have gotten wind, be careful.";
                }
                string str = NativeFunction.Natives.GET_PEDHEADSHOT_TXD_STRING<string>(pedHeadshotHandle);
                EntryPoint.WriteToConsole($"Contractor SENT PICTURE MESSAGE {str}");
                Player.CellPhone.AddCustomScheduledText(Contact, PickedReply, Time.CurrentDateTime, str, true);
            }
            else
            {
                Replies = new List<string>() {
                    $"~y~{TargetName}~s~. I heard they were still around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"~y~{TargetName}~s~. They should still be around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"~y~{TargetName}~s~. They are still around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"The name is ~y~{TargetName}~s~. They are loitering around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                    $"Remember, ~y~{TargetName}~s~ is the name. I got word they are still around ~p~{TargetLocation.Name} {TargetLocation.FullStreetAddress}~s~.",
                     };
                string PickedReply = Replies.PickRandom();
                if (TargetIsCustomer && LookingForItem != "")
                {
                    List<string> ItemReplies = new List<string>() {
                    $" They are probably looking for ~p~{LookingForItem}~s~.",
                    $" They like ~p~{LookingForItem}~s~.",
                    $" Will be looking to buy ~p~{LookingForItem}~s~.",
                    $" They are interested in ~p~{LookingForItem}~s~.",
                    $" The target likes ~p~{LookingForItem}~s~.",
                     };
                    PickedReply += ItemReplies.PickRandom();
                }

                if (WillFight || WillFlee)
                {
                    PickedReply += " ~s~The target might have gotten wind, be careful.";
                }
                EntryPoint.WriteToConsole("CONTRACTOR SENT REGULAR MESSAGE");
                Player.CellPhone.AddCustomScheduledText(Contact, PickedReply, Time.CurrentDateTime, null, true);
            }
        }
        private void DespawnTarget()
        {
            if (Target != null && Target.Pedestrian.Exists())
            {
                Target.DeleteBlip();
                Target.Pedestrian.Delete();
                //EntryPoint.WriteToConsoleTestLong("Contractor DESPAWNED TARGETS");
            }
            IsTargetSpawned = false;
        }
        private void GetPayment()
        {
            MoneyToRecieve = RandomItems.GetRandomNumberInt(Settings.SettingsManager.TaskSettings.ContractorHitmanTaskPaymentMin, Settings.SettingsManager.TaskSettings.ContractorHitmanTaskPaymentMax).Round(500);
            if (MoneyToRecieve <= 0)
            {
                MoneyToRecieve = 500;
            }
        }
        private void SendTaskAbortMessage()
        {
            List<string> Replies = new List<string>() {
                    "Nothing yet, I'll let you know",
                    "I've got nothing for you yet",
                    "Give me a few days",
                    "Not a lot to be done right now",
                    "We will let you know when you can do something for us",
                    "Check back later.",
                    };
            Player.CellPhone.AddPhoneResponse(Contact.Name, Replies.PickRandom());
        }
        private void SendInitialInstructionsMessage()
        {
            List<string> Replies;
            if (TargetIsAtHome)
            {
                Replies = new List<string>() {
                    $"Got a target that needs to disappear. Home address is ~p~{TargetLocation.FullStreetAddress}~s~. Name ~y~{TargetName}~s~. ${MoneyToRecieve}",
                    $"Get to the house at ~p~{TargetLocation.FullStreetAddress}~s~ and get rid of ~y~{TargetName}~s~. ${MoneyToRecieve} on complation",
                    $"We need to you shut this guy up before he squeals. He lives at ~p~{TargetLocation.FullStreetAddress}~s~. The name is ~y~{TargetName}~s~. Payment of ${MoneyToRecieve}",
                    $"~y~{TargetName}~s~ is living at ~p~{TargetLocation.FullStreetAddress}~s~. They should be home. You know what to do. ${MoneyToRecieve}",
                    $"Need you to make sure ~y~{TargetName}~s~ doesn't make it to the deposition, they live at ~p~{TargetLocation.FullStreetAddress}~s~. ${MoneyToRecieve}",
                     };
            }
            else
            {
                Replies = new List<string>() {
                    $"Got a target that needs to disappear. They hang around ~p~{TargetLocation.Name}~s~. Address is ~p~{TargetLocation.FullStreetAddress}~s~. Name ~y~{TargetName}~s~. ${MoneyToRecieve}",
                    $"Get to ~p~{TargetLocation.Name}~s~ on ~p~{TargetLocation.FullStreetAddress}~s~ and get rid of ~y~{TargetName}~s~. ${MoneyToRecieve} on complation",
                    $"We need to you shut this guy up before he squeals. He's at ~p~{TargetLocation.Name}~s~ ~p~{TargetLocation.FullStreetAddress}~s~. The name is ~y~{TargetName}~s~. Payment of ${MoneyToRecieve}",
                    $"~y~{TargetName}~s~ is at ~p~{TargetLocation.Name}~s~, address is ~p~{TargetLocation.FullStreetAddress}~s~. You know what to do. ${MoneyToRecieve}",
                    $"Need you to make sure ~y~{TargetName}~s~ doesn't make it to the deposition, they are currently at ~p~{TargetLocation.Name}~s~ on ~p~{TargetLocation.FullStreetAddress}~s~. ${MoneyToRecieve}",
                     };
            }

            Player.CellPhone.AddPhoneResponse(Contact.Name, Replies.PickRandom());
        }
        private void SendQuickPaymentMessage()
        {
            List<string> Replies = new List<string>() {
                            $"Seems like that thing we discussed is done? Sending you ${MoneyToRecieve}",
                            $"Word got around that you are done with that thing for us, sending your payment of ${MoneyToRecieve}",
                            $"Sending your payment of ${MoneyToRecieve}",
                            $"Sending ${MoneyToRecieve}",
                            $"Heard you were done. We owe you ${MoneyToRecieve}",
                            };
            Player.CellPhone.AddScheduledText(Contact, Replies.PickRandom(), 1, false);
        }
        private void SendDeadDropStartMessage()
        {
            List<string> Replies = new List<string>() {
                            $"Pickup your payment of ${MoneyToRecieve} from {myDrop.FullStreetAddress}, its {myDrop.Description}.",
                            $"Go get your payment of ${MoneyToRecieve} from {myDrop.Description}, address is {myDrop.FullStreetAddress}.",
                            };

            Player.CellPhone.AddScheduledText(Contact, Replies.PickRandom(), 1, false);
        }
        private void SendCompletedMessage()
        {
            List<string> Replies = new List<string>() {
                        $"Seems like that thing we discussed is done? Sending you ${MoneyToRecieve}",
                        $"Word got around that you are done with that thing for us, sending your payment of ${MoneyToRecieve}",
                        $"Sending your payment of ${MoneyToRecieve}",
                        $"Sending ${MoneyToRecieve}",
                        $"Heard you were done. We owe you ${MoneyToRecieve}",
                        };
            Player.CellPhone.AddScheduledText(Contact, Replies.PickRandom(), 1, false);
        }
        private void SendFailMessage()
        {
            List<string> Replies = new List<string>() {
                        $"You spooked them, they are gone. Thank for nothing.",
                        $"How could you let them get away?",
                        $"They weren't supposed to show up in court. Useless.",
                        $"How did you fuck this up so bad, they are squealing everything",
                        $"Since you fucked that up, they went right to the cops.",
                        };
            Player.CellPhone.AddScheduledText(Contact, Replies.PickRandom(), 1, false);
        }
    }

}
