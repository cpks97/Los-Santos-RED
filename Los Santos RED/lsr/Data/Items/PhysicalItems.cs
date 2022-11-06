﻿using ExtensionsMethods;
using LosSantosRED.lsr.Helper;
using LosSantosRED.lsr.Interface;
using Rage;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PhysicalItems : IPropItems
{
    private readonly string ConfigFileName = "Plugins\\LosSantosRED\\PhysicalItems.xml";
    private List<PhysicalItem> PhysicalItemsList;

    public PhysicalItems()
    {

    }

    public List<PhysicalItem> Items => PhysicalItemsList;
    public PhysicalItem Get(string ID)
    {
        return PhysicalItemsList.FirstOrDefault(x => x.ID == ID);
    }
    public PhysicalItem GetRandomItem()
    {
        return PhysicalItemsList.Where(x => x.Type != ePhysicalItemType.Vehicle && x.Type != ePhysicalItemType.Weapon && x.Type != ePhysicalItemType.Ped).PickRandom();
    }
    public void ReadConfig()
    {
        DirectoryInfo LSRDirectory = new DirectoryInfo("Plugins\\LosSantosRED");
        FileInfo ConfigFile = LSRDirectory.GetFiles("PhysicalItems*.xml").OrderByDescending(x => x.Name).FirstOrDefault();
        if (ConfigFile != null)
        {
            EntryPoint.WriteToConsole($"Loaded Physical Items config: {ConfigFile.FullName}", 0);
            PhysicalItemsList = Serialization.DeserializeParams<PhysicalItem>(ConfigFile.FullName);
        }
        else if (File.Exists(ConfigFileName))
        {
            EntryPoint.WriteToConsole($"Loaded Physical Items config  {ConfigFileName}", 0);
            PhysicalItemsList = Serialization.DeserializeParams<PhysicalItem>(ConfigFileName);
        }
        else
        {
            EntryPoint.WriteToConsole($"No Physical Items config found, creating default", 0);
            DefaultConfig();
        }
    }
    private void DefaultConfig()
    {
        PhysicalItemsList = new List<PhysicalItem> { };
        DefaultConfig_Drinks();
        DefaultConfig_Food();
        DefaultConfig_Drugs();
        DefaultConfig_Tools();
        DefaultConfig_Vehicles();
        DefaultConfig_Weapons();
        Serialization.SerializeParams(PhysicalItemsList, ConfigFileName);
    }

    private void DefaultConfig_Drinks()
    {
        PhysicalItemsList.AddRange(new List<PhysicalItem> {
            //Drinks
            //Bottles
            new PhysicalItem("ba_prop_club_water_bottle", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.05f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("h4_prop_battle_waterbottle_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.05f), new Rotator(0.0f, 0.0f, 0.0f))}),

            new PhysicalItem("prop_energy_drink", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),

            new PhysicalItem("prop_amb_beer_bottle", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_beer_am", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beer_bar", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beer_blr", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beer_jakey", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beer_logger", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beer_patriot", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beer_pride", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beer_stz", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),
            new PhysicalItem("prop_beerdusche", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.15f), new Rotator(0.0f, 0.0f, 0.0f))}),

            new PhysicalItem("prop_cs_beer_bot_40oz", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.05f), new Rotator(0.0f, 0.0f, 0.0f)) }),

            new PhysicalItem("h4_prop_h4_t_bottle_02a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("h4_prop_h4_t_bottle_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0.0f, 0.0f, 0.0f)) }),

            new PhysicalItem("ng_proc_sodacan_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.1f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("ng_proc_sodacan_01b", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.1f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_orang_can_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),

            new PhysicalItem("ng_proc_sodacup_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.2f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("ng_proc_sodacup_01b", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.2f), new Rotator(0.0f, 0.0f, 0.0f)) }),

            new PhysicalItem("h4_prop_h4_can_beer_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.1f), new Rotator(0.0f, 0.0f, 0.0f)) }),

            new PhysicalItem("p_ing_coffeecup_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("p_ing_coffeecup_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),

            new PhysicalItem("ng_proc_sodacup_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.2f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("ng_proc_sodacup_01b", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.2f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("ng_proc_sodacup_01c", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, -0.2f), new Rotator(0.0f, 0.0f, 0.0f)) }),
        });

        //
    }
    private void DefaultConfig_Drugs()
    {
        PhysicalItemsList.AddRange(new List<PhysicalItem>
        {
            //Cigarettes/Cigars
            new PhysicalItem("ng_proc_cigarette01a", new List<PropAttachment>() {
                new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0f, 0f)),
                new PropAttachment("Head", "BONETAG_HEAD", new Vector3(-0.007f, 0.13f, 0.01f),new Rotator(0.0f, -175f, 91f)) {Gender = "M" },


                new PropAttachment("Head", "BONETAG_HEAD", new Vector3(-0.02f, 0.1f, 0.01f),new Rotator(0f, 0f, -80f)) {Gender = "F" },}),//looks good


                //Packages
                new PhysicalItem("v_ret_ml_cigs", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.03f, 0.0f),new Rotator(0.49f, 79f, 79f)) }),
                new PhysicalItem("v_ret_ml_cigs2", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.03f, 0.0f),new Rotator(0.49f, 79f, 79f)) }),
                new PhysicalItem("v_ret_ml_cigs3", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.03f, 0.0f),new Rotator(0.49f, 79f, 79f)) }),
                new PhysicalItem("v_ret_ml_cigs4", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.03f, 0.0f),new Rotator(0.49f, 79f, 79f)) }),
                new PhysicalItem("v_ret_ml_cigs5", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.03f, 0.0f),new Rotator(0.49f, 79f, 79f)) }),
                new PhysicalItem("v_ret_ml_cigs6", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.03f, 0.0f),new Rotator(0.49f, 79f, 79f)) }),
                new PhysicalItem("p_cigar_pack_02_s", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.17f, 0.02f, 0.0f),new Rotator(0.0f, -78f, 0f)) }),


            new PhysicalItem("prop_cigar_02", new List<PropAttachment>() { 
                new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(-0.02f, 0.0f, 0.0f), new Rotator(0.0f, 180f, 0f)) {Gender = "M" },
                //new PropAttachment("Head", "BONETAG_HEAD", new Vector3(-0.007f, 0.13f, 0.01f),new Rotator(0.0f, -175f, 91f)) {Gender = "M" },//doesnt look so good on franklin
                new PropAttachment("Head", "BONETAG_HEAD", new Vector3(-0.023f,0.087f,0.014f), new Rotator(50f, 0f, 90f)) { Gender = "M" },//a little close in for franklin

                new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(-0.015f,0.117f,0.01f),new Rotator(90f, 90f, 0f)) { Gender = "F" },
                new PropAttachment("Head", "BONETAG_HEAD", new Vector3(-0.023f,0.087f,0.014f), new Rotator(50f, 0f, 90f)) { Gender = "F" }
            }),//looksgood besides player mouth attach
            
            //Other Drugs
            new PhysicalItem("p_cs_joint_01", new List<PropAttachment>() {
                new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0f, 0f)),
                new PropAttachment("Head", "BONETAG_HEAD", new Vector3(-0.007f, 0.13f, 0.01f),new Rotator(0.0f, -175f, 91f)) {Gender = "M" },
                new PropAttachment("Head", "BONETAG_HEAD", new Vector3(-0.02f, 0.1f, 0.01f),new Rotator(0f, 0f, -80f)) {Gender = "F" },}),//looks good

            new PhysicalItem("prop_weed_bottle", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0f, 0f)) }),

            //new PhysicalItem("prop_cs_pills", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.12f, 0.03f, 0.0f),new Rotator(-76f, 0f, 0f)) }),
            new PhysicalItem("prop_cs_pills", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0f, 0f)) }),

            new PhysicalItem("sf_prop_sf_bag_weed_01a", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0f, 0f)) }),

            //new PhysicalItem("ba_prop_battle_sniffing_pipe", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.11f, 0.0f, -0.02f),new Rotator(-179f, 72f, -28f)) }),
            new PhysicalItem("ba_prop_battle_sniffing_pipe", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0f, 0f, 0f)) }),

            new PhysicalItem("prop_meth_bag_01"),


            //new PhysicalItem("prop_cs_crackpipe", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.07f, 0.02f),new Rotator(-119f, 47f, 0f)) }),
            new PhysicalItem("prop_cs_crackpipe", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),

            //new PhysicalItem("prop_syringe_01", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.16f, 0.02f, -0.07f),new Rotator(-170f, -148f, -36f)) }),//inject
            new PhysicalItem("prop_syringe_01", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),//inject

            //new PhysicalItem("prop_cs_meth_pipe", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.14f, 0.05f, -0.01f),new Rotator(-119f, 0f, 0f)) }),//Doesnt attach right
            new PhysicalItem("prop_cs_meth_pipe", new List<PropAttachment>() { new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0f, 0f, 0f)) }),//Doesnt attach right

        });
    }
    private void DefaultConfig_Food()
    {
        //all of this is no longer attached right for some reason....
        PhysicalItemsList.AddRange(new List<PhysicalItem>
        {
            //Generic Food
            new PhysicalItem("prop_cs_hotdog_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(178.0f, 28.0f, 0.0f)) }),
            new PhysicalItem("prop_cs_burger_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0.0f, 28.0f, 0.0f)) }),
            new PhysicalItem("prop_donut_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-15.0f, 17.0f, 0.0f)) }),
            new PhysicalItem("p_amb_bagel_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-15.0f, 17.0f, 0.0f)) }),
            new PhysicalItem("prop_food_chips", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("ng_proc_food_nana1a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("ng_proc_food_ornge1a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("ng_proc_food_aple1a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("prop_sandwich_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(178.0f, 28.0f, 0.0f)) }),
            new PhysicalItem("v_res_tt_pizzaplate", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("prop_pizza_box_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("prop_pizza_box_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("v_ret_ml_chips1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("v_ret_ml_chips2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("v_ret_ml_chips3", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("v_ret_ml_chips4", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("prop_choc_ego", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(25f, -11f, -95f)) }),
            new PhysicalItem("prop_candy_pqs", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-178f, -169f, 169f)) }),
            new PhysicalItem("prop_choc_pq", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-178f, -169f, 79f)) }),
            new PhysicalItem("prop_choc_meto", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(169f, 170f, 76f)) }),
            new PhysicalItem("prop_food_bs_burg1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("prop_food_bs_tray_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_food_bs_burger2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("prop_food_bs_tray_03", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_food_bs_tray_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_food_bs_chips", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(-77.0f, 23.0f, 0.0f)) }),
            new PhysicalItem("prop_food_bag1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_food_bs_burger2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(178.0f, 28.0f, 0.0f)) }),
            new PhysicalItem("prop_food_cb_tray_03", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_food_cb_tray_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_food_burg3", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(178.0f, 28.0f, 0.0f)) }),
            new PhysicalItem("prop_food_burg2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)) }),
            new PhysicalItem("prop_food_burg1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0.0f, 28.0f, 0.0f)) }),
            new PhysicalItem("prop_ff_noodle_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            new PhysicalItem("prop_ff_noodle_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),


            //new PhysicalItem("prop_cs_hotdog_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.14f, -0.02f, -0.04f),new Rotator(178.0f, 28.0f, 0.0f)) }),
            //new PhysicalItem("prop_cs_burger_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.16f, 0.01f, -0.04f),new Rotator(0.0f, 28.0f, 0.0f)) }),
            //new PhysicalItem("prop_donut_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.15f, 0.01f, -0.03f),new Rotator(-15.0f, 17.0f, 0.0f)) }),
            //new PhysicalItem("p_amb_bagel_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.15f, 0.01f, -0.03f),new Rotator(-15.0f, 17.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_chips", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("ng_proc_food_nana1a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("ng_proc_food_ornge1a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("ng_proc_food_aple1a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("prop_sandwich_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.14f, -0.02f, -0.04f),new Rotator(178.0f, 28.0f, 0.0f)) }),
            //new PhysicalItem("v_res_tt_pizzaplate", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("prop_pizza_box_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("prop_pizza_box_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("v_ret_ml_chips1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("v_ret_ml_chips2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("v_ret_ml_chips3", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("v_ret_ml_chips4", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("prop_choc_ego", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.13f, 0.05f, -0.02f),new Rotator(25f, -11f, -95f)) }),
            //new PhysicalItem("prop_candy_pqs", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.16f, 0.01f, -0.02f),new Rotator(-178f, -169f, 169f)) }),
            //new PhysicalItem("prop_choc_pq", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.02f, -0.02f),new Rotator(-178f, -169f, 79f)) }),
            //new PhysicalItem("prop_choc_meto", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.03f, -0.02f),new Rotator(169f, 170f, 76f)) }),
            //new PhysicalItem("prop_food_bs_burg1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_bs_tray_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_bs_burger2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_bs_tray_03", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_bs_tray_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_bs_chips", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, 0.0f, -0.06f),new Rotator(-77.0f, 23.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_bag1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_bs_burger2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.14f, -0.02f, -0.04f),new Rotator(178.0f, 28.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_cb_tray_03", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_cb_tray_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_burg3", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.14f, -0.02f, -0.04f),new Rotator(178.0f, 28.0f, 0.0f)) }),
            //new PhysicalItem("prop_food_burg2", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.14f, 0.01f, -0.06f),new Rotator(0f, 0f, 0f)) }),
            //new PhysicalItem("prop_food_burg1", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.16f, 0.01f, -0.04f),new Rotator(0.0f, 28.0f, 0.0f)) }),
            //new PhysicalItem("prop_ff_noodle_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
            //new PhysicalItem("prop_ff_noodle_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),
        });
    }
    private void DefaultConfig_Tools()
    {
        PhysicalItemsList.AddRange(new List<PhysicalItem>
        {
            //Generic Tools
            new PhysicalItem("prop_tool_screwdvr01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.1170f, 0.0610f, 0.0150f), new Rotator(-47.199f, 166.62f, -19.9f)) }),
            new PhysicalItem("prop_tool_drill", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.1170f, 0.0610f, 0.0150f), new Rotator(-47.199f, 166.62f, -19.9f)) }),
            new PhysicalItem("prop_tool_pliers", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.1170f, 0.0610f, 0.0150f), new Rotator(-47.199f, 166.62f, -19.9f)) }),
            new PhysicalItem("prop_tool_shovel", 
            new List<PropAttachment>() { 
                
            new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.005f, 0.006f, -0.048f), new Rotator(3f, -183f, 0f))
            ,new PropAttachment("RightHandWeapon", "BONETAG_R_PH_HAND", new Vector3(-0.03f,-0.277f,-0.062f),new Rotator(20f, -101f, 81f)) 
            
            }
            
            
            
            
            ) { IsLarge = true },

            
            new PhysicalItem("prop_binoc_01", new List<PropAttachment>() {

                new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), Rotator.Zero),
                new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.128f, 0.015f, 0.087f), new Rotator(-21f, -249f, -6f)),
                new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.1f, -0.1f, 0.0f), Rotator.Zero),





            }),

            new PhysicalItem("prop_bong_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f),new Rotator(0.0f, 0.0f, 0.0f)) }),


            new PhysicalItem("p_cs_lighter_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.13f, 0.02f, 0.02f), new Rotator(-93f, 40f, 0f)) }),
            new PhysicalItem("p_cs_lighter_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.13f, 0.02f, 0.02f), new Rotator(-93f, 40f, 0f)) }),
            new PhysicalItem("v_res_tt_lighter", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.13f, 0.02f, 0.02f), new Rotator(-93f, 40f, 0f)) }),
            new PhysicalItem("ex_prop_exec_lighter_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.13f, 0.02f, 0.02f), new Rotator(-93f, 40f, 0f)) }),
            new PhysicalItem("lux_prop_lighter_luxe", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.13f, 0.02f, 0.02f), new Rotator(-93f, 40f, 0f)) }),

            new PhysicalItem("p_amb_brolly_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(-0.01f, 0.01f, 0.05f), new Rotator(0f, -40f, 0f)) }) { IsLarge = true },//blue umbrella
            new PhysicalItem("p_amb_brolly_01_s", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(-0.01f, 0.01f, 0.05f), new Rotator(0f, -40f, 0f)) }) { IsLarge = true },//black umbrellal

            new PhysicalItem("gr_prop_gr_tape_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)) }),//flint duct tape
            new PhysicalItem("gr_prop_gr_sdriver_01", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)) }),//flint flathead screwdriver
            new PhysicalItem("gr_prop_gr_sdriver_02", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)) }),//flint multi bit screwdriver
            new PhysicalItem("gr_prop_gr_hammer_01", new List<PropAttachment>() {
                new PropAttachment("RightHand", "BONETAG_R_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)),
                new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)),
                new PropAttachment("RightHandWeapon","BONETAG_R_PH_HAND",new Vector3(0.02f,0f,-0.03f),new Rotator(0f, 0f, 0f)),
            
            }) { IsLarge = true },//flint rubber hammer







            new PhysicalItem("gr_prop_gr_driver_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)) }),//power metal impact driver
            new PhysicalItem("gr_prop_gr_drill_01a", new List<PropAttachment>() { new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.0f, 0.0f), new Rotator(0f, 0f, 0f)) }),//power metal cordless drill

            new PhysicalItem("prop_cs_police_torch", new List<PropAttachment>() { ///police maglite
                new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0f, 0.002f, 0.002f), new Rotator(-180f, -130f, -100f)),
                new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.0f, -0.12f, 0.0f), Rotator.Zero),//new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.0f, -0.07f, 0.0f), Rotator.Zero),//new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.0f, -0.05f, 0.0f), Rotator.Zero),
                new PropAttachment("EmissiveExtraDistance", "BONETAG_L_PH_HAND", new Vector3(-0.12f, -0.2f, 0.0f), Rotator.Zero),
                new PropAttachment("FrontRotation", "BONETAG_L_PH_HAND", new Vector3(90f, -1.0f, -1.0f), Rotator.Zero),
            }),

            new PhysicalItem("prop_tool_torch", new List<PropAttachment>() {//flint tools handle flashlight
                new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.12f, -0.02f, -0.08f), new Rotator(0f, 0f, -100f)),
                new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.1f, 0.35f, 0.0f), Rotator.Zero),
                new PropAttachment("EmissiveExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.0f, 0.1f, 0.0f), Rotator.Zero),
                new PropAttachment("FrontRotation", "BONETAG_L_PH_HAND", new Vector3(-90f, 1.0f, 1.0f), Rotator.Zero),
            }),

            new PhysicalItem("prop_phone_ing", new List<PropAttachment>() { 
                new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.04f,-0.05f,-0.01f), new Rotator(-20f, -290f, -60f)),
                new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.01f, 0.1f, 0.0f), Rotator.Zero),
                new PropAttachment("EmissiveExtraDistance", "BONETAG_L_PH_HAND", new Vector3(-0.05f, 0.2f, -0.1f), Rotator.Zero),
                new PropAttachment("FrontRotation", "BONETAG_L_PH_HAND", new Vector3(0f, 0.0f, 0.0f), Rotator.Zero),
            }),


            new PhysicalItem("prop_phone_ing_02", new List<PropAttachment>() {
                new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.04f,-0.05f,-0.01f), new Rotator(-20f, -290f, -60f)),
                new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.01f, 0.1f, 0.0f), Rotator.Zero),
                new PropAttachment("EmissiveExtraDistance", "BONETAG_L_PH_HAND", new Vector3(-0.05f, 0.2f, -0.1f), Rotator.Zero),
                new PropAttachment("FrontRotation", "BONETAG_L_PH_HAND", new Vector3(0f, 0.0f, 0.0f), Rotator.Zero),
            }),

            new PhysicalItem("prop_phone_ing_03", new List<PropAttachment>() {
                new PropAttachment("LeftHand", "BONETAG_L_PH_HAND", new Vector3(0.04f,-0.05f,-0.01f), new Rotator(-20f, -290f, -60f)),
                new PropAttachment("ExtraDistance", "BONETAG_L_PH_HAND", new Vector3(0.01f, 0.1f, 0.0f), Rotator.Zero),
                new PropAttachment("EmissiveExtraDistance", "BONETAG_L_PH_HAND", new Vector3(-0.05f, 0.2f, -0.1f), Rotator.Zero),
                new PropAttachment("FrontRotation", "BONETAG_L_PH_HAND", new Vector3(0f, 0.0f, 0.0f), Rotator.Zero),
            }),




        });
    }
    private void DefaultConfig_Weapons()
    {
        PhysicalItemsList.AddRange(new List<PhysicalItem>
        {
            new PhysicalItem("weapon_bat",0x958a4a8f,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_crowbar",0x84bd7bfd,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_golfclub",0x440e4788,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_hammer",0x4e875f73,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_hatchet",0xf9dcbf2d,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_knuckle",0xd8df3c3c,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_knife",0x99b507ea,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_machete",0xdd5df8d9,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_switchblade",0xdfe37640,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_nightstick",0x678b81b1,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_wrench",0x19044ee0,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_poolcue",0x94117305,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_pistol",0x1b06d571,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_pistol_mk2",0xbfe256d4,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_combatpistol",0x5ef9fec4,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_pistol50",0x99aeeb3b,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_ceramicpistol",0x2b5ef5ec,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_appistol",0x22d8fe39,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_heavypistol",0xd205520e,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_revolver",0xc1b3c3d1,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_revolver_mk2",0xcb96392f,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_snspistol",0xbfd21232,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_snspistol_mk2",0x88374054,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_stungun",0x45cd9cf3,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_vintagepistol",0x83839c4,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_sawnoffshotgun",0x7846a318,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_pumpshotgun",0x1d073a89,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_pumpshotgun_mk2",0x555af99a,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_assaultshotgun",0xe284c527,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_bullpupshotgun",0x9d61e50f,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_heavyshotgun",0x3aabbbaa,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_dbshotgun",0xef951fbb,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_autoshotgun",0x12e82d3d,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_combatshotgun",0x5a96ba4,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_microsmg",0x13532244,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_smg",0x2be6766b,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_smg_mk2",0x78a97cd0,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_assaultsmg",0xefe7e2df,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_combatpdw",0x0a3d4d34,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_machinepistol",0xdb1aa450,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_minismg",0xbd248b55,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_assaultrifle",0xbfefff6d,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_assaultrifle_mk2",0x394f415c,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_carbinerifle",0x83bf0278,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_carbinerifle_mk2",0xfad1f1c9,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_advancedrifle",0xaf113f99,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_specialcarbine",0xc0a3098d,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_specialcarbine_mk2",0x969c3d67,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_bullpuprifle",0x7f229f94,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_bullpuprifle_mk2",0x84d6fafd,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_compactrifle",0x624fe830,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_militaryrifle",0x9d1f17e6,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_heavyrifle",0xc78d71b4,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_mg",0x9d07f764,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_combatmg",0x7fd62962,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_combatmg_mk2",0xdbbd7280,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_gusenberg",0x61012683,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_sniperrifle",0x05fc3c11,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_heavysniper",0x0c472fe2,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_heavysniper_mk2",0xa914799,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_marksmanrifle",0xc734385a,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_marksmanrifle_mk2",0x6a6c02e0,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_rpg",0xb1ca77b1,ePhysicalItemType.Weapon) { IsLarge = true },
            new PhysicalItem("weapon_grenadelauncher",0xa284510b,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_grenade",0x93e220bd,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_molotov",0x24b17070,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_bzgas",0xa0973d5e,ePhysicalItemType.Weapon),
            new PhysicalItem("weapon_smokegrenade",0xbfe256d4,ePhysicalItemType.Weapon),
        });
    }
    private void DefaultConfig_Vehicles()
    {
        PhysicalItemsList.AddRange(new List<PhysicalItem>
        {
            new PhysicalItem("alpha",ePhysicalItemType.Vehicle),
            new PhysicalItem("btype",ePhysicalItemType.Vehicle),
            new PhysicalItem("btype2",ePhysicalItemType.Vehicle),
            new PhysicalItem("btype3",ePhysicalItemType.Vehicle),
            new PhysicalItem("buccaneer",ePhysicalItemType.Vehicle),
            new PhysicalItem("buccaneer2",ePhysicalItemType.Vehicle),
            new PhysicalItem("cavalcade",ePhysicalItemType.Vehicle),
            new PhysicalItem("cavalcade2",ePhysicalItemType.Vehicle),
            new PhysicalItem("emperor",ePhysicalItemType.Vehicle),
            new PhysicalItem("emperor2",ePhysicalItemType.Vehicle),
            new PhysicalItem("emperor3",ePhysicalItemType.Vehicle),
            new PhysicalItem("hermes",ePhysicalItemType.Vehicle),
            new PhysicalItem("lurcher",ePhysicalItemType.Vehicle),
            new PhysicalItem("manana",ePhysicalItemType.Vehicle),
            new PhysicalItem("manana2",ePhysicalItemType.Vehicle),
            new PhysicalItem("primo",ePhysicalItemType.Vehicle),
            new PhysicalItem("primo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("virgo",ePhysicalItemType.Vehicle),
            new PhysicalItem("vstr",ePhysicalItemType.Vehicle),
            new PhysicalItem("washington",ePhysicalItemType.Vehicle),
            new PhysicalItem("elegy",ePhysicalItemType.Vehicle),
            new PhysicalItem("elegy2",ePhysicalItemType.Vehicle),
            new PhysicalItem("euros",ePhysicalItemType.Vehicle),
            new PhysicalItem("hellion",ePhysicalItemType.Vehicle),
            new PhysicalItem("le7b",ePhysicalItemType.Vehicle),
            new PhysicalItem("remus",ePhysicalItemType.Vehicle),
            new PhysicalItem("s80",ePhysicalItemType.Vehicle),
            new PhysicalItem("savestra",ePhysicalItemType.Vehicle),
            new PhysicalItem("zr350",ePhysicalItemType.Vehicle),
            new PhysicalItem("zr380",ePhysicalItemType.Vehicle),
            new PhysicalItem("zr3802",ePhysicalItemType.Vehicle),
            new PhysicalItem("zr3803",ePhysicalItemType.Vehicle),
            new PhysicalItem("bruiser",ePhysicalItemType.Vehicle),
            new PhysicalItem("bruiser2",ePhysicalItemType.Vehicle),
            new PhysicalItem("bruiser3",ePhysicalItemType.Vehicle),
            new PhysicalItem("dubsta",ePhysicalItemType.Vehicle),
            new PhysicalItem("dubsta2",ePhysicalItemType.Vehicle),
            new PhysicalItem("dubsta3",ePhysicalItemType.Vehicle),
            new PhysicalItem("feltzer2",ePhysicalItemType.Vehicle),
            new PhysicalItem("feltzer3",ePhysicalItemType.Vehicle),
            new PhysicalItem("glendale",ePhysicalItemType.Vehicle),
            new PhysicalItem("glendale2",ePhysicalItemType.Vehicle),
            new PhysicalItem("limo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("openwheel1",ePhysicalItemType.Vehicle),
            new PhysicalItem("panto",ePhysicalItemType.Vehicle),
            new PhysicalItem("schafter2",ePhysicalItemType.Vehicle),
            new PhysicalItem("schafter3",ePhysicalItemType.Vehicle),
            new PhysicalItem("schafter4",ePhysicalItemType.Vehicle),
            new PhysicalItem("schafter5",ePhysicalItemType.Vehicle),
            new PhysicalItem("schafter6",ePhysicalItemType.Vehicle),
            new PhysicalItem("schwarzer",ePhysicalItemType.Vehicle),
            new PhysicalItem("serrano",ePhysicalItemType.Vehicle),
            new PhysicalItem("surano",ePhysicalItemType.Vehicle),
            new PhysicalItem("xls",ePhysicalItemType.Vehicle),
            new PhysicalItem("xls2",ePhysicalItemType.Vehicle),
            new PhysicalItem("krieger",ePhysicalItemType.Vehicle),
            new PhysicalItem("schlagen",ePhysicalItemType.Vehicle),
            new PhysicalItem("streiter",ePhysicalItemType.Vehicle),
            new PhysicalItem("terbyte",ePhysicalItemType.Vehicle),
            new PhysicalItem("bfinjection",ePhysicalItemType.Vehicle),
            new PhysicalItem("bifta",ePhysicalItemType.Vehicle),
            new PhysicalItem("club",ePhysicalItemType.Vehicle),
            new PhysicalItem("dune",ePhysicalItemType.Vehicle),
            new PhysicalItem("dune3",ePhysicalItemType.Vehicle),
            new PhysicalItem("raptor",ePhysicalItemType.Vehicle),
            new PhysicalItem("surfer",ePhysicalItemType.Vehicle),
            new PhysicalItem("surfer2",ePhysicalItemType.Vehicle),
            new PhysicalItem("weevil",ePhysicalItemType.Vehicle),
            new PhysicalItem("prairie",ePhysicalItemType.Vehicle),
            new PhysicalItem("banshee",ePhysicalItemType.Vehicle),
            new PhysicalItem("banshee2",ePhysicalItemType.Vehicle),
            new PhysicalItem("bison",ePhysicalItemType.Vehicle),
            new PhysicalItem("bison2",ePhysicalItemType.Vehicle),
            new PhysicalItem("bison3",ePhysicalItemType.Vehicle),
            new PhysicalItem("buffalo",ePhysicalItemType.Vehicle),
            new PhysicalItem("buffalo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("buffalo3",ePhysicalItemType.Vehicle),
            new PhysicalItem("dloader",ePhysicalItemType.Vehicle),
            new PhysicalItem("gauntlet",ePhysicalItemType.Vehicle),
            new PhysicalItem("gauntlet2",ePhysicalItemType.Vehicle),
            new PhysicalItem("gauntlet3",ePhysicalItemType.Vehicle),
            new PhysicalItem("gauntlet4",ePhysicalItemType.Vehicle),
            new PhysicalItem("gauntlet5",ePhysicalItemType.Vehicle),
            new PhysicalItem("gresley",ePhysicalItemType.Vehicle),
            new PhysicalItem("halftrack",ePhysicalItemType.Vehicle),
            new PhysicalItem("monster3",ePhysicalItemType.Vehicle),
            new PhysicalItem("monster4",ePhysicalItemType.Vehicle),
            new PhysicalItem("monster5",ePhysicalItemType.Vehicle),
            new PhysicalItem("paradise",ePhysicalItemType.Vehicle),
            new PhysicalItem("ratloader2",ePhysicalItemType.Vehicle),
            new PhysicalItem("rumpo",ePhysicalItemType.Vehicle),
            new PhysicalItem("rumpo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("rumpo3",ePhysicalItemType.Vehicle),
            new PhysicalItem("verlierer2",ePhysicalItemType.Vehicle),
            new PhysicalItem("youga",ePhysicalItemType.Vehicle),
            new PhysicalItem("youga2",ePhysicalItemType.Vehicle),
            new PhysicalItem("youga3",ePhysicalItemType.Vehicle),
            new PhysicalItem("boxville",ePhysicalItemType.Vehicle),
            new PhysicalItem("boxville3",ePhysicalItemType.Vehicle),
            new PhysicalItem("boxville4",ePhysicalItemType.Vehicle),
            new PhysicalItem("camper",ePhysicalItemType.Vehicle),
            new PhysicalItem("pony",ePhysicalItemType.Vehicle),
            new PhysicalItem("pony2",ePhysicalItemType.Vehicle),
            new PhysicalItem("stockade",ePhysicalItemType.Vehicle),
            new PhysicalItem("stockade3",ePhysicalItemType.Vehicle),
            new PhysicalItem("tiptruck",ePhysicalItemType.Vehicle),
            new PhysicalItem("bodhi2",ePhysicalItemType.Vehicle),
            new PhysicalItem("crusader",ePhysicalItemType.Vehicle),
            new PhysicalItem("freecrawler",ePhysicalItemType.Vehicle),
            new PhysicalItem("kalahari",ePhysicalItemType.Vehicle),
            new PhysicalItem("kamacho",ePhysicalItemType.Vehicle),
            new PhysicalItem("mesa",ePhysicalItemType.Vehicle),
            new PhysicalItem("mesa2",ePhysicalItemType.Vehicle),
            new PhysicalItem("mesa3",ePhysicalItemType.Vehicle),
            new PhysicalItem("seminole",ePhysicalItemType.Vehicle),
            new PhysicalItem("seminole2",ePhysicalItemType.Vehicle),
            new PhysicalItem("romero",ePhysicalItemType.Vehicle),
            new PhysicalItem("fugitive",ePhysicalItemType.Vehicle),
            new PhysicalItem("marshall",ePhysicalItemType.Vehicle),
            new PhysicalItem("picador",ePhysicalItemType.Vehicle),
            new PhysicalItem("surge",ePhysicalItemType.Vehicle),
            new PhysicalItem("taipan",ePhysicalItemType.Vehicle),
            new PhysicalItem("brawler",ePhysicalItemType.Vehicle),
            new PhysicalItem("cyclone",ePhysicalItemType.Vehicle),
            new PhysicalItem("raiden",ePhysicalItemType.Vehicle),
            new PhysicalItem("voltic",ePhysicalItemType.Vehicle),
            new PhysicalItem("voltic2",ePhysicalItemType.Vehicle),
            new PhysicalItem("asea",ePhysicalItemType.Vehicle),
            new PhysicalItem("asea2",ePhysicalItemType.Vehicle),
            new PhysicalItem("brutus",ePhysicalItemType.Vehicle),
            new PhysicalItem("brutus2",ePhysicalItemType.Vehicle),
            new PhysicalItem("brutus3",ePhysicalItemType.Vehicle),
            new PhysicalItem("burrito",ePhysicalItemType.Vehicle),
            new PhysicalItem("burrito2",ePhysicalItemType.Vehicle),
            new PhysicalItem("burrito3",ePhysicalItemType.Vehicle),
            new PhysicalItem("burrito4",ePhysicalItemType.Vehicle),
            new PhysicalItem("burrito5",ePhysicalItemType.Vehicle),
            new PhysicalItem("gburrito",ePhysicalItemType.Vehicle),
            new PhysicalItem("gburrito2",ePhysicalItemType.Vehicle),
            new PhysicalItem("granger",ePhysicalItemType.Vehicle),
            new PhysicalItem("hotring",ePhysicalItemType.Vehicle),
            new PhysicalItem("impaler",ePhysicalItemType.Vehicle),
            new PhysicalItem("impaler2",ePhysicalItemType.Vehicle),
            new PhysicalItem("impaler3",ePhysicalItemType.Vehicle),
            new PhysicalItem("impaler4",ePhysicalItemType.Vehicle),
            new PhysicalItem("lguard",ePhysicalItemType.Vehicle),
            new PhysicalItem("mamba",ePhysicalItemType.Vehicle),
            new PhysicalItem("moonbeam",ePhysicalItemType.Vehicle),
            new PhysicalItem("moonbeam2",ePhysicalItemType.Vehicle),
            new PhysicalItem("openwheel2",ePhysicalItemType.Vehicle),
            new PhysicalItem("premier",ePhysicalItemType.Vehicle),
            new PhysicalItem("rancherxl",ePhysicalItemType.Vehicle),
            new PhysicalItem("rancherxl2",ePhysicalItemType.Vehicle),
            new PhysicalItem("rhapsody",ePhysicalItemType.Vehicle),
            new PhysicalItem("sabregt",ePhysicalItemType.Vehicle),
            new PhysicalItem("sabregt2",ePhysicalItemType.Vehicle),
            new PhysicalItem("scramjet",ePhysicalItemType.Vehicle),
            new PhysicalItem("stalion",ePhysicalItemType.Vehicle),
            new PhysicalItem("stalion2",ePhysicalItemType.Vehicle),
            new PhysicalItem("tampa",ePhysicalItemType.Vehicle),
            new PhysicalItem("tampa2",ePhysicalItemType.Vehicle),
            new PhysicalItem("tampa3",ePhysicalItemType.Vehicle),
            new PhysicalItem("tornado",ePhysicalItemType.Vehicle),
            new PhysicalItem("tornado2",ePhysicalItemType.Vehicle),
            new PhysicalItem("tornado3",ePhysicalItemType.Vehicle),
            new PhysicalItem("tornado4",ePhysicalItemType.Vehicle),
            new PhysicalItem("tornado5",ePhysicalItemType.Vehicle),
            new PhysicalItem("tornado6",ePhysicalItemType.Vehicle),
            new PhysicalItem("tulip",ePhysicalItemType.Vehicle),
            new PhysicalItem("vamos",ePhysicalItemType.Vehicle),
            new PhysicalItem("vigero",ePhysicalItemType.Vehicle),
            new PhysicalItem("voodoo",ePhysicalItemType.Vehicle),
            new PhysicalItem("voodoo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("yosemite",ePhysicalItemType.Vehicle),
            new PhysicalItem("yosemite2",ePhysicalItemType.Vehicle),
            new PhysicalItem("yosemite3",ePhysicalItemType.Vehicle),
            new PhysicalItem("exemplar",ePhysicalItemType.Vehicle),
            new PhysicalItem("jb700",ePhysicalItemType.Vehicle),
            new PhysicalItem("jb7002",ePhysicalItemType.Vehicle),
            new PhysicalItem("massacro",ePhysicalItemType.Vehicle),
            new PhysicalItem("massacro2",ePhysicalItemType.Vehicle),
            new PhysicalItem("rapidgt",ePhysicalItemType.Vehicle),
            new PhysicalItem("rapidgt2",ePhysicalItemType.Vehicle),
            new PhysicalItem("rapidgt3",ePhysicalItemType.Vehicle),
            new PhysicalItem("seven70",ePhysicalItemType.Vehicle),
            new PhysicalItem("specter",ePhysicalItemType.Vehicle),
            new PhysicalItem("specter2",ePhysicalItemType.Vehicle),
            new PhysicalItem("vagner",ePhysicalItemType.Vehicle),
            new PhysicalItem("akuma",ePhysicalItemType.Vehicle),
            new PhysicalItem("blista",ePhysicalItemType.Vehicle),
            new PhysicalItem("blista2",ePhysicalItemType.Vehicle),
            new PhysicalItem("blista3",ePhysicalItemType.Vehicle),
            new PhysicalItem("double",ePhysicalItemType.Vehicle),
            new PhysicalItem("enduro",ePhysicalItemType.Vehicle),
            new PhysicalItem("jester",ePhysicalItemType.Vehicle),
            new PhysicalItem("jester2",ePhysicalItemType.Vehicle),
            new PhysicalItem("jester3",ePhysicalItemType.Vehicle),
            new PhysicalItem("jester4",ePhysicalItemType.Vehicle),
            new PhysicalItem("kanjo",ePhysicalItemType.Vehicle),
            new PhysicalItem("rt3000",ePhysicalItemType.Vehicle),
            new PhysicalItem("sugoi",ePhysicalItemType.Vehicle),
            new PhysicalItem("thrust",ePhysicalItemType.Vehicle),
            new PhysicalItem("verus",ePhysicalItemType.Vehicle),
            new PhysicalItem("veto",ePhysicalItemType.Vehicle),
            new PhysicalItem("veto2",ePhysicalItemType.Vehicle),
            new PhysicalItem("vindicator",ePhysicalItemType.Vehicle),
            new PhysicalItem("landstalker",ePhysicalItemType.Vehicle),
            new PhysicalItem("landstalker2",ePhysicalItemType.Vehicle),
            new PhysicalItem("regina",ePhysicalItemType.Vehicle),
            new PhysicalItem("stretch",ePhysicalItemType.Vehicle),
            new PhysicalItem("virgo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("virgo3",ePhysicalItemType.Vehicle),
            new PhysicalItem("habanero",ePhysicalItemType.Vehicle),
            new PhysicalItem("sheava",ePhysicalItemType.Vehicle),
            new PhysicalItem("vectre",ePhysicalItemType.Vehicle),
            new PhysicalItem("cog55",ePhysicalItemType.Vehicle),
            new PhysicalItem("cog552",ePhysicalItemType.Vehicle),
            new PhysicalItem("cogcabrio",ePhysicalItemType.Vehicle),
            new PhysicalItem("cognoscenti",ePhysicalItemType.Vehicle),
            new PhysicalItem("cognoscenti2",ePhysicalItemType.Vehicle),
            new PhysicalItem("huntley",ePhysicalItemType.Vehicle),
            new PhysicalItem("paragon",ePhysicalItemType.Vehicle),
            new PhysicalItem("paragon2",ePhysicalItemType.Vehicle),
            new PhysicalItem("stafford",ePhysicalItemType.Vehicle),
            new PhysicalItem("superd",ePhysicalItemType.Vehicle),
            new PhysicalItem("windsor",ePhysicalItemType.Vehicle),
            new PhysicalItem("windsor2",ePhysicalItemType.Vehicle),
            new PhysicalItem("fq2",ePhysicalItemType.Vehicle),
            new PhysicalItem("baller",ePhysicalItemType.Vehicle),
            new PhysicalItem("baller2",ePhysicalItemType.Vehicle),
            new PhysicalItem("baller3",ePhysicalItemType.Vehicle),
            new PhysicalItem("baller4",ePhysicalItemType.Vehicle),
            new PhysicalItem("baller5",ePhysicalItemType.Vehicle),
            new PhysicalItem("baller6",ePhysicalItemType.Vehicle),
            new PhysicalItem("bestiagts",ePhysicalItemType.Vehicle),
            new PhysicalItem("brioso",ePhysicalItemType.Vehicle),
            new PhysicalItem("brioso2",ePhysicalItemType.Vehicle),
            new PhysicalItem("carbonizzare",ePhysicalItemType.Vehicle),
            new PhysicalItem("cheetah",ePhysicalItemType.Vehicle),
            new PhysicalItem("cheetah2",ePhysicalItemType.Vehicle),
            new PhysicalItem("furia",ePhysicalItemType.Vehicle),
            new PhysicalItem("gt500",ePhysicalItemType.Vehicle),
            new PhysicalItem("italigto",ePhysicalItemType.Vehicle),
            new PhysicalItem("italirsx",ePhysicalItemType.Vehicle),
            new PhysicalItem("prototipo",ePhysicalItemType.Vehicle),
            new PhysicalItem("stinger",ePhysicalItemType.Vehicle),
            new PhysicalItem("stingergt",ePhysicalItemType.Vehicle),
            new PhysicalItem("turismo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("turismor",ePhysicalItemType.Vehicle),
            new PhysicalItem("visione",ePhysicalItemType.Vehicle),
            new PhysicalItem("khamelion",ePhysicalItemType.Vehicle),
            new PhysicalItem("ruston",ePhysicalItemType.Vehicle),
            new PhysicalItem("barracks2",ePhysicalItemType.Vehicle),
            new PhysicalItem("biff",ePhysicalItemType.Vehicle),
            new PhysicalItem("bulldozer",ePhysicalItemType.Vehicle),
            new PhysicalItem("cutter",ePhysicalItemType.Vehicle),
            new PhysicalItem("dump",ePhysicalItemType.Vehicle),
            new PhysicalItem("forklift",ePhysicalItemType.Vehicle),
            new PhysicalItem("insurgent",ePhysicalItemType.Vehicle),
            new PhysicalItem("insurgent2",ePhysicalItemType.Vehicle),
            new PhysicalItem("insurgent3",ePhysicalItemType.Vehicle),
            new PhysicalItem("menacer",ePhysicalItemType.Vehicle),
            new PhysicalItem("mixer",ePhysicalItemType.Vehicle),
            new PhysicalItem("mixer2",ePhysicalItemType.Vehicle),
            new PhysicalItem("nightshark",ePhysicalItemType.Vehicle),
            new PhysicalItem("scarab",ePhysicalItemType.Vehicle),
            new PhysicalItem("scarab2",ePhysicalItemType.Vehicle),
            new PhysicalItem("scarab3",ePhysicalItemType.Vehicle),
            new PhysicalItem("deluxo",ePhysicalItemType.Vehicle),
            new PhysicalItem("dukes",ePhysicalItemType.Vehicle),
            new PhysicalItem("dukes2",ePhysicalItemType.Vehicle),
            new PhysicalItem("dukes3",ePhysicalItemType.Vehicle),
            new PhysicalItem("nightshade",ePhysicalItemType.Vehicle),
            new PhysicalItem("phoenix",ePhysicalItemType.Vehicle),
            new PhysicalItem("ruiner",ePhysicalItemType.Vehicle),
            new PhysicalItem("ruiner2",ePhysicalItemType.Vehicle),
            new PhysicalItem("ruiner3",ePhysicalItemType.Vehicle),
            new PhysicalItem("coquette",ePhysicalItemType.Vehicle),
            new PhysicalItem("coquette2",ePhysicalItemType.Vehicle),
            new PhysicalItem("coquette3",ePhysicalItemType.Vehicle),
            new PhysicalItem("coquette4",ePhysicalItemType.Vehicle),
            new PhysicalItem("hauler",ePhysicalItemType.Vehicle),
            new PhysicalItem("hauler2",ePhysicalItemType.Vehicle),
            new PhysicalItem("phantom",ePhysicalItemType.Vehicle),
            new PhysicalItem("phantom2",ePhysicalItemType.Vehicle),
            new PhysicalItem("phantom3",ePhysicalItemType.Vehicle),
            new PhysicalItem("rubble",ePhysicalItemType.Vehicle),
            new PhysicalItem("asterope",ePhysicalItemType.Vehicle),
            new PhysicalItem("bjxl",ePhysicalItemType.Vehicle),
            new PhysicalItem("calico",ePhysicalItemType.Vehicle),
            new PhysicalItem("dilettante",ePhysicalItemType.Vehicle),
            new PhysicalItem("dilettante2",ePhysicalItemType.Vehicle),
            new PhysicalItem("everon",ePhysicalItemType.Vehicle),
            new PhysicalItem("futo",ePhysicalItemType.Vehicle),
            new PhysicalItem("futo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("intruder",ePhysicalItemType.Vehicle),
            new PhysicalItem("kuruma",ePhysicalItemType.Vehicle),
            new PhysicalItem("kuruma2",ePhysicalItemType.Vehicle),
            new PhysicalItem("previon",ePhysicalItemType.Vehicle),
            new PhysicalItem("rebel",ePhysicalItemType.Vehicle),
            new PhysicalItem("rebel2",ePhysicalItemType.Vehicle),
            new PhysicalItem("sultan",ePhysicalItemType.Vehicle),
            new PhysicalItem("sultan2",ePhysicalItemType.Vehicle),
            new PhysicalItem("sultan3",ePhysicalItemType.Vehicle),
            new PhysicalItem("sultanrs",ePhysicalItemType.Vehicle),
            new PhysicalItem("technical",ePhysicalItemType.Vehicle),
            new PhysicalItem("technical3",ePhysicalItemType.Vehicle),
            new PhysicalItem("z190",ePhysicalItemType.Vehicle),
            new PhysicalItem("casco",ePhysicalItemType.Vehicle),
            new PhysicalItem("felon",ePhysicalItemType.Vehicle),
            new PhysicalItem("felon2",ePhysicalItemType.Vehicle),
            new PhysicalItem("furoregt",ePhysicalItemType.Vehicle),
            new PhysicalItem("michelli",ePhysicalItemType.Vehicle),
            new PhysicalItem("pigalle",ePhysicalItemType.Vehicle),
            new PhysicalItem("tropos",ePhysicalItemType.Vehicle),
            new PhysicalItem("komoda",ePhysicalItemType.Vehicle),
            new PhysicalItem("novak",ePhysicalItemType.Vehicle),
            new PhysicalItem("tigon",ePhysicalItemType.Vehicle),
            new PhysicalItem("viseris",ePhysicalItemType.Vehicle),
            new PhysicalItem("avarus",ePhysicalItemType.Vehicle),
            new PhysicalItem("hexer",ePhysicalItemType.Vehicle),
            new PhysicalItem("innovation",ePhysicalItemType.Vehicle),
            new PhysicalItem("sanctus",ePhysicalItemType.Vehicle),
            new PhysicalItem("manchez",ePhysicalItemType.Vehicle),
            new PhysicalItem("manchez2",ePhysicalItemType.Vehicle),
            new PhysicalItem("mule",ePhysicalItemType.Vehicle),
            new PhysicalItem("mule2",ePhysicalItemType.Vehicle),
            new PhysicalItem("mule3",ePhysicalItemType.Vehicle),
            new PhysicalItem("mule4",ePhysicalItemType.Vehicle),
            new PhysicalItem("penumbra",ePhysicalItemType.Vehicle),
            new PhysicalItem("penumbra2",ePhysicalItemType.Vehicle),
            new PhysicalItem("sanchez",ePhysicalItemType.Vehicle),
            new PhysicalItem("sanchez2",ePhysicalItemType.Vehicle),
            new PhysicalItem("patriot",ePhysicalItemType.Vehicle),
            new PhysicalItem("patriot2",ePhysicalItemType.Vehicle),
            new PhysicalItem("squaddie",ePhysicalItemType.Vehicle),
            new PhysicalItem("asbo",ePhysicalItemType.Vehicle),
            new PhysicalItem("vagrant",ePhysicalItemType.Vehicle),
            new PhysicalItem("brickade",ePhysicalItemType.Vehicle),
            new PhysicalItem("cerberus",ePhysicalItemType.Vehicle),
            new PhysicalItem("cerberus2",ePhysicalItemType.Vehicle),
            new PhysicalItem("cerberus3",ePhysicalItemType.Vehicle),
            new PhysicalItem("firetruk",ePhysicalItemType.Vehicle),
            new PhysicalItem("flatbed",ePhysicalItemType.Vehicle),
            new PhysicalItem("packer",ePhysicalItemType.Vehicle),
            new PhysicalItem("pounder",ePhysicalItemType.Vehicle),
            new PhysicalItem("pounder2",ePhysicalItemType.Vehicle),
            new PhysicalItem("rallytruck",ePhysicalItemType.Vehicle),
            new PhysicalItem("wastelander",ePhysicalItemType.Vehicle),
            new PhysicalItem("bf400",ePhysicalItemType.Vehicle),
            new PhysicalItem("blazer",ePhysicalItemType.Vehicle),
            new PhysicalItem("blazer2",ePhysicalItemType.Vehicle),
            new PhysicalItem("blazer3",ePhysicalItemType.Vehicle),
            new PhysicalItem("blazer4",ePhysicalItemType.Vehicle),
            new PhysicalItem("carbonrs",ePhysicalItemType.Vehicle),
            new PhysicalItem("chimera",ePhysicalItemType.Vehicle),
            new PhysicalItem("outlaw",ePhysicalItemType.Vehicle),
            new PhysicalItem("shotaro",ePhysicalItemType.Vehicle),
            new PhysicalItem("stryder",ePhysicalItemType.Vehicle),
            new PhysicalItem("drafter",ePhysicalItemType.Vehicle),
            new PhysicalItem("ninef",ePhysicalItemType.Vehicle),
            new PhysicalItem("ninef2",ePhysicalItemType.Vehicle),
            new PhysicalItem("omnis",ePhysicalItemType.Vehicle),
            new PhysicalItem("rocoto",ePhysicalItemType.Vehicle),
            new PhysicalItem("tailgater",ePhysicalItemType.Vehicle),
            new PhysicalItem("tailgater2",ePhysicalItemType.Vehicle),
            new PhysicalItem("ardent",ePhysicalItemType.Vehicle),
            new PhysicalItem("f620",ePhysicalItemType.Vehicle),
            new PhysicalItem("formula2",ePhysicalItemType.Vehicle),
            new PhysicalItem("jackal",ePhysicalItemType.Vehicle),
            new PhysicalItem("jugular",ePhysicalItemType.Vehicle),
            new PhysicalItem("locust",ePhysicalItemType.Vehicle),
            new PhysicalItem("lynx",ePhysicalItemType.Vehicle),
            new PhysicalItem("pariah",ePhysicalItemType.Vehicle),
            new PhysicalItem("penetrator",ePhysicalItemType.Vehicle),
            new PhysicalItem("swinger",ePhysicalItemType.Vehicle),
            new PhysicalItem("xa21",ePhysicalItemType.Vehicle),
            new PhysicalItem("autarch",ePhysicalItemType.Vehicle),
            new PhysicalItem("entity2",ePhysicalItemType.Vehicle),
            new PhysicalItem("entityxf",ePhysicalItemType.Vehicle),
            new PhysicalItem("imorgon",ePhysicalItemType.Vehicle),
            new PhysicalItem("tyrant",ePhysicalItemType.Vehicle),
            new PhysicalItem("bati",ePhysicalItemType.Vehicle),
            new PhysicalItem("bati2",ePhysicalItemType.Vehicle),
            new PhysicalItem("esskey",ePhysicalItemType.Vehicle),
            new PhysicalItem("faggio",ePhysicalItemType.Vehicle),
            new PhysicalItem("faggio2",ePhysicalItemType.Vehicle),
            new PhysicalItem("faggio3",ePhysicalItemType.Vehicle),
            new PhysicalItem("fcr",ePhysicalItemType.Vehicle),
            new PhysicalItem("fcr2",ePhysicalItemType.Vehicle),
            new PhysicalItem("infernus",ePhysicalItemType.Vehicle),
            new PhysicalItem("infernus2",ePhysicalItemType.Vehicle),
            new PhysicalItem("monroe",ePhysicalItemType.Vehicle),
            new PhysicalItem("oppressor",ePhysicalItemType.Vehicle),
            new PhysicalItem("oppressor2",ePhysicalItemType.Vehicle),
            new PhysicalItem("osiris",ePhysicalItemType.Vehicle),
            new PhysicalItem("reaper",ePhysicalItemType.Vehicle),
            new PhysicalItem("ruffian",ePhysicalItemType.Vehicle),
            new PhysicalItem("tempesta",ePhysicalItemType.Vehicle),
            new PhysicalItem("tezeract",ePhysicalItemType.Vehicle),
            new PhysicalItem("torero",ePhysicalItemType.Vehicle),
            new PhysicalItem("toros",ePhysicalItemType.Vehicle),
            new PhysicalItem("vacca",ePhysicalItemType.Vehicle),
            new PhysicalItem("vortex",ePhysicalItemType.Vehicle),
            new PhysicalItem("zentorno",ePhysicalItemType.Vehicle),
            new PhysicalItem("zorrusso",ePhysicalItemType.Vehicle),
            new PhysicalItem("comet2",ePhysicalItemType.Vehicle),
            new PhysicalItem("comet3",ePhysicalItemType.Vehicle),
            new PhysicalItem("comet4",ePhysicalItemType.Vehicle),
            new PhysicalItem("comet5",ePhysicalItemType.Vehicle),
            new PhysicalItem("comet6",ePhysicalItemType.Vehicle),
            new PhysicalItem("growler",ePhysicalItemType.Vehicle),
            new PhysicalItem("neon",ePhysicalItemType.Vehicle),
            new PhysicalItem("pfister811",ePhysicalItemType.Vehicle),
            new PhysicalItem("deveste",ePhysicalItemType.Vehicle),
            new PhysicalItem("diablous",ePhysicalItemType.Vehicle),
            new PhysicalItem("diablous2",ePhysicalItemType.Vehicle),
            new PhysicalItem("lectro",ePhysicalItemType.Vehicle),
            new PhysicalItem("nemesis",ePhysicalItemType.Vehicle),
            new PhysicalItem("emerus",ePhysicalItemType.Vehicle),
            new PhysicalItem("formula",ePhysicalItemType.Vehicle),
            new PhysicalItem("gp1",ePhysicalItemType.Vehicle),
            new PhysicalItem("italigtb",ePhysicalItemType.Vehicle),
            new PhysicalItem("italigtb2",ePhysicalItemType.Vehicle),
            new PhysicalItem("t20",ePhysicalItemType.Vehicle),
            new PhysicalItem("tyrus",ePhysicalItemType.Vehicle),
            new PhysicalItem("cheburek",ePhysicalItemType.Vehicle),
            new PhysicalItem("deviant",ePhysicalItemType.Vehicle),
            new PhysicalItem("fusilade",ePhysicalItemType.Vehicle),
            new PhysicalItem("defiler",ePhysicalItemType.Vehicle),
            new PhysicalItem("hakuchou",ePhysicalItemType.Vehicle),
            new PhysicalItem("hakuchou2",ePhysicalItemType.Vehicle),
            new PhysicalItem("pcj",ePhysicalItemType.Vehicle),
            new PhysicalItem("vader",ePhysicalItemType.Vehicle),
            new PhysicalItem("tractor2",ePhysicalItemType.Vehicle),
            new PhysicalItem("tractor3",ePhysicalItemType.Vehicle),
            new PhysicalItem("adder",ePhysicalItemType.Vehicle),
            new PhysicalItem("nero",ePhysicalItemType.Vehicle),
            new PhysicalItem("nero2",ePhysicalItemType.Vehicle),
            new PhysicalItem("thrax",ePhysicalItemType.Vehicle),
            new PhysicalItem("ztype",ePhysicalItemType.Vehicle),
            new PhysicalItem("oracle",ePhysicalItemType.Vehicle),
            new PhysicalItem("oracle2",ePhysicalItemType.Vehicle),
            new PhysicalItem("revolter",ePhysicalItemType.Vehicle),
            new PhysicalItem("sc1",ePhysicalItemType.Vehicle),
            new PhysicalItem("sentinel",ePhysicalItemType.Vehicle),
            new PhysicalItem("sentinel2",ePhysicalItemType.Vehicle),
            new PhysicalItem("sentinel3",ePhysicalItemType.Vehicle),
            new PhysicalItem("zion",ePhysicalItemType.Vehicle),
            new PhysicalItem("zion2",ePhysicalItemType.Vehicle),
            new PhysicalItem("zion3",ePhysicalItemType.Vehicle),
            new PhysicalItem("cypher",ePhysicalItemType.Vehicle),
            new PhysicalItem("rebla",ePhysicalItemType.Vehicle),
            new PhysicalItem("benson",ePhysicalItemType.Vehicle),
            new PhysicalItem("blade",ePhysicalItemType.Vehicle),
            new PhysicalItem("bobcatxl",ePhysicalItemType.Vehicle),
            new PhysicalItem("bullet",ePhysicalItemType.Vehicle),
            new PhysicalItem("caracara",ePhysicalItemType.Vehicle),
            new PhysicalItem("caracara2",ePhysicalItemType.Vehicle),
            new PhysicalItem("chino",ePhysicalItemType.Vehicle),
            new PhysicalItem("chino2",ePhysicalItemType.Vehicle),
            new PhysicalItem("clique",ePhysicalItemType.Vehicle),
            new PhysicalItem("contender",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator2",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator3",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator4",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator5",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator6",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator7",ePhysicalItemType.Vehicle),
            new PhysicalItem("dominator8",ePhysicalItemType.Vehicle),
            new PhysicalItem("ellie",ePhysicalItemType.Vehicle),
            new PhysicalItem("flashgt",ePhysicalItemType.Vehicle),
            new PhysicalItem("fmj",ePhysicalItemType.Vehicle),
            new PhysicalItem("gb200",ePhysicalItemType.Vehicle),
            new PhysicalItem("guardian",ePhysicalItemType.Vehicle),
            new PhysicalItem("hotknife",ePhysicalItemType.Vehicle),
            new PhysicalItem("hustler",ePhysicalItemType.Vehicle),
            new PhysicalItem("imperator",ePhysicalItemType.Vehicle),
            new PhysicalItem("imperator2",ePhysicalItemType.Vehicle),
            new PhysicalItem("imperator3",ePhysicalItemType.Vehicle),
            new PhysicalItem("minivan",ePhysicalItemType.Vehicle),
            new PhysicalItem("minivan2",ePhysicalItemType.Vehicle),
            new PhysicalItem("monster",ePhysicalItemType.Vehicle),
            new PhysicalItem("peyote",ePhysicalItemType.Vehicle),
            new PhysicalItem("peyote2",ePhysicalItemType.Vehicle),
            new PhysicalItem("peyote3",ePhysicalItemType.Vehicle),
            new PhysicalItem("radi",ePhysicalItemType.Vehicle),
            new PhysicalItem("retinue",ePhysicalItemType.Vehicle),
            new PhysicalItem("retinue2",ePhysicalItemType.Vehicle),
            new PhysicalItem("riata",ePhysicalItemType.Vehicle),
            new PhysicalItem("sadler",ePhysicalItemType.Vehicle),
            new PhysicalItem("sadler2",ePhysicalItemType.Vehicle),
            new PhysicalItem("sandking",ePhysicalItemType.Vehicle),
            new PhysicalItem("sandking2",ePhysicalItemType.Vehicle),
            new PhysicalItem("slamtruck",ePhysicalItemType.Vehicle),
            new PhysicalItem("slamvan",ePhysicalItemType.Vehicle),
            new PhysicalItem("slamvan2",ePhysicalItemType.Vehicle),
            new PhysicalItem("slamvan3",ePhysicalItemType.Vehicle),
            new PhysicalItem("slamvan4",ePhysicalItemType.Vehicle),
            new PhysicalItem("slamvan5",ePhysicalItemType.Vehicle),
            new PhysicalItem("slamvan6",ePhysicalItemType.Vehicle),
            new PhysicalItem("speedo",ePhysicalItemType.Vehicle),
            new PhysicalItem("speedo2",ePhysicalItemType.Vehicle),
            new PhysicalItem("speedo4",ePhysicalItemType.Vehicle),
            new PhysicalItem("stanier",ePhysicalItemType.Vehicle),
            new PhysicalItem("trophytruck",ePhysicalItemType.Vehicle),
            new PhysicalItem("trophytruck2",ePhysicalItemType.Vehicle),
            new PhysicalItem("winky",ePhysicalItemType.Vehicle),
            new PhysicalItem("fagaloa",ePhysicalItemType.Vehicle),
            new PhysicalItem("ingot",ePhysicalItemType.Vehicle),
            new PhysicalItem("nebula",ePhysicalItemType.Vehicle),
            new PhysicalItem("warrener",ePhysicalItemType.Vehicle),
            new PhysicalItem("warrener2",ePhysicalItemType.Vehicle),
            new PhysicalItem("neo",ePhysicalItemType.Vehicle),
            new PhysicalItem("dynasty",ePhysicalItemType.Vehicle),
            new PhysicalItem("issi2",ePhysicalItemType.Vehicle),
            new PhysicalItem("issi3",ePhysicalItemType.Vehicle),
            new PhysicalItem("issi4",ePhysicalItemType.Vehicle),
            new PhysicalItem("issi5",ePhysicalItemType.Vehicle),
            new PhysicalItem("issi6",ePhysicalItemType.Vehicle),
            new PhysicalItem("issi7",ePhysicalItemType.Vehicle),
            new PhysicalItem("bagger",ePhysicalItemType.Vehicle),
            new PhysicalItem("cliffhanger",ePhysicalItemType.Vehicle),
            new PhysicalItem("daemon",ePhysicalItemType.Vehicle),
            new PhysicalItem("daemon2",ePhysicalItemType.Vehicle),
            new PhysicalItem("deathbike",ePhysicalItemType.Vehicle),
            new PhysicalItem("deathbike2",ePhysicalItemType.Vehicle),
            new PhysicalItem("deathbike3",ePhysicalItemType.Vehicle),
            new PhysicalItem("gargoyle",ePhysicalItemType.Vehicle),
            new PhysicalItem("nightblade",ePhysicalItemType.Vehicle),
            new PhysicalItem("ratbike",ePhysicalItemType.Vehicle),
            new PhysicalItem("rrocket",ePhysicalItemType.Vehicle),
            new PhysicalItem("sovereign",ePhysicalItemType.Vehicle),
            new PhysicalItem("wolfsbane",ePhysicalItemType.Vehicle),
            new PhysicalItem("zombiea",ePhysicalItemType.Vehicle),
            new PhysicalItem("zombieb",ePhysicalItemType.Vehicle),
            new PhysicalItem("faction",ePhysicalItemType.Vehicle),
            new PhysicalItem("faction2",ePhysicalItemType.Vehicle),
            new PhysicalItem("faction3",ePhysicalItemType.Vehicle),
            new PhysicalItem("journey",ePhysicalItemType.Vehicle),
            new PhysicalItem("stratum",ePhysicalItemType.Vehicle),
            new PhysicalItem("supervolito",ePhysicalItemType.Vehicle),
            new PhysicalItem("supervolito2",ePhysicalItemType.Vehicle),
            new PhysicalItem("swift",ePhysicalItemType.Vehicle),
            new PhysicalItem("swift2",ePhysicalItemType.Vehicle),
            new PhysicalItem("volatus",ePhysicalItemType.Vehicle),
            new PhysicalItem("thruster",ePhysicalItemType.Vehicle),
            new PhysicalItem("havok",ePhysicalItemType.Vehicle),
            new PhysicalItem("alphaz1",ePhysicalItemType.Vehicle),
            new PhysicalItem("howard",ePhysicalItemType.Vehicle),
            new PhysicalItem("luxor",ePhysicalItemType.Vehicle),
            new PhysicalItem("luxor2",ePhysicalItemType.Vehicle),
            new PhysicalItem("miljet",ePhysicalItemType.Vehicle),
            new PhysicalItem("nimbus",ePhysicalItemType.Vehicle),
            new PhysicalItem("pyro",ePhysicalItemType.Vehicle),
            new PhysicalItem("shamal",ePhysicalItemType.Vehicle),
            new PhysicalItem("vestra",ePhysicalItemType.Vehicle),
            new PhysicalItem("avenger",ePhysicalItemType.Vehicle),
            new PhysicalItem("avenger2",ePhysicalItemType.Vehicle),
            new PhysicalItem("dodo",ePhysicalItemType.Vehicle),
            new PhysicalItem("hydra",ePhysicalItemType.Vehicle),
            new PhysicalItem("mogul",ePhysicalItemType.Vehicle),
            new PhysicalItem("tula",ePhysicalItemType.Vehicle),
            new PhysicalItem("microlight",ePhysicalItemType.Vehicle),
            new PhysicalItem("besra",ePhysicalItemType.Vehicle),
            new PhysicalItem("rogue",ePhysicalItemType.Vehicle),
            new PhysicalItem("seabreeze",ePhysicalItemType.Vehicle),
            new PhysicalItem("marquis",ePhysicalItemType.Vehicle),
            new PhysicalItem("toro",ePhysicalItemType.Vehicle),
            new PhysicalItem("toro2",ePhysicalItemType.Vehicle),
            new PhysicalItem("dinghy",ePhysicalItemType.Vehicle),
            new PhysicalItem("dinghy2",ePhysicalItemType.Vehicle),
            new PhysicalItem("dinghy3",ePhysicalItemType.Vehicle),
            new PhysicalItem("dinghy4",ePhysicalItemType.Vehicle),
            new PhysicalItem("dinghy5",ePhysicalItemType.Vehicle),
            new PhysicalItem("speeder",ePhysicalItemType.Vehicle),
            new PhysicalItem("speeder2",ePhysicalItemType.Vehicle),
            new PhysicalItem("jetmax",ePhysicalItemType.Vehicle),
            new PhysicalItem("longfin",ePhysicalItemType.Vehicle),
            new PhysicalItem("squalo",ePhysicalItemType.Vehicle),
            new PhysicalItem("suntrap",ePhysicalItemType.Vehicle),
            new PhysicalItem("tropic",ePhysicalItemType.Vehicle),
            new PhysicalItem("tropic2",ePhysicalItemType.Vehicle),
            new PhysicalItem("seashark",ePhysicalItemType.Vehicle),
            new PhysicalItem("seashark2",ePhysicalItemType.Vehicle),
            new PhysicalItem("seashark3",ePhysicalItemType.Vehicle),
        });
    }

}