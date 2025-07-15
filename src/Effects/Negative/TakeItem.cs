using RimWorld;
using Verse;
using System.Linq;
using HugsLib.Settings;
using System;
using System.Collections.Generic;

namespace CrowdControl {

    public class TakeItem : Effect {
        public override string Code => EffectCode.FoulFood;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            var items = command.code.Split('_');
            var itemname = items[1];

            itemname = itemname.Replace("-", "_");

            List<ThingDef> remItems = null;

            if(itemname == "material")   
                remItems = DefDatabase<ThingDef>.AllDefs?.Where(p => materials.Contains(p.defName)).ToList();
            if (itemname == "equipment")
                remItems = DefDatabase<ThingDef>.AllDefs?.Where(p => equipment.Contains(p.defName)).ToList();

            var stockpileZones = currentMap.zoneManager.AllZones.OfType<Zone_Stockpile>().ToList();

            if(remItems!=null)
            foreach (Zone_Stockpile zone in stockpileZones) {
                foreach (IntVec3 cell in zone.Cells) {
                    Thing item = currentMap.thingGrid.ThingAt(cell, ThingCategory.Item);
                    
                    if (item != null && remItems.Contains(item.def)) {
                        item.Destroy();

                        SendCardNotification("Item Destroyed", $"{command.viewerName} took your {item.def.defName}.", LetterDefOf.NegativeEvent, command.viewerName);

                        return EffectStatus.Success;
                    }
                }
            }

            return EffectStatus.Failure;
        }

        private static string[] materials = { "Silver", "Gold", "Steel", "Plasteel", "WoodLog", "Uranium", "Jade", "Cloth", "Synthread", "DevilstrandCloth", "Hyperweave", "WoolSheep", "WoolAlpaca", "WoolMegasloth", "WoolMuffalo", "WoolBison", "Leather_Plain", "Leather_Dog", "Leather_Wolf", "Leather_Panthera", "Leather_Camel", "Leather_Bluefur", "Leather_Bear", "Leather_GuineaPig", "Leather_Human", "Leather_Pig", "Leather_Light", "Leather_Bird", "Leather_Chinchilla", "Leather_Fox", "Leather_Lizard", "Leather_Elephant", "Leather_Heavy", "Leather_Rhinoceros", "Leather_Thrumbo", "Leather_Patch", "Sandstone", "SmoothedSandstone", "ChunkSandstone", "BlocksSandstone", "Granite", "SmoothedGranite", "ChunkGranite", "BlocksGranite", "Limestone", "SmoothedLimestone", "ChunkLimestone", "BlocksLimestone", "Slate", "SmoothedSlate", "ChunkSlate", "BlocksSlate", "Marble", "SmoothedMarble", "ChunkMarble", "BlocksMarble"  };
        private static string[] equipment = { "Apparel_ShieldBelt", "Apparel_CowboyHat", "Apparel_BowlerHat", "Apparel_TribalHeaddress", "Apparel_Tuque", "Apparel_WarMask", "Apparel_WarVeil", "Apparel_SimpleHelmet", "Apparel_AdvancedHelmet", "Apparel_PowerArmorHelmet", "Apparel_ArmorHelmetRecon", "Apparel_PsychicFoilHelmet", "Apparel_HatHood", "Apparel_ClothMask", "Apparel_SmokepopBelt", "Apparel_FirefoampopPack", "Apparel_PsychicShockLance", "Apparel_PsychicInsanityLance", "Apparel_TribalA", "Apparel_Parka", "Apparel_Pants", "Apparel_BasicShirt", "Apparel_CollarShirt", "Apparel_Duster", "Apparel_Jacket", "Apparel_PlateArmor", "Apparel_FlakVest", "Apparel_FlakPants", "Apparel_FlakJacket", "Apparel_PowerArmor", "Apparel_ArmorRecon", "Apparel_Robe", "PowerClaw", "MeleeWeapon_BreachAxe", "MeleeWeapon_Mace", "MeleeWeapon_Gladius", "MeleeWeapon_LongSword", "MeleeWeapon_Club", "MeleeWeapon_Knife", "MeleeWeapon_Ikwa", "MeleeWeapon_Spear", "Bullet_Revolver", "Gun_Revolver", "Gun_Autopistol", "Bullet_Autopistol", "Gun_MachinePistol", "Bullet_MachinePistol", "Gun_IncendiaryLauncher", "Bullet_IncendiaryLauncher", "Gun_SmokeLauncher", "Bullet_SmokeLauncher", "Gun_EmpLauncher", "Bullet_EMPLauncher", "Gun_BoltActionRifle", "Bullet_BoltActionRifle", "Gun_PumpShotgun", "Bullet_Shotgun", "Gun_ChainShotgun", "Gun_HeavySMG", "Bullet_HeavySMG", "Gun_LMG", "Bullet_LMG", "Gun_AssaultRifle", "Bullet_AssaultRifle", "Gun_SniperRifle", "Bullet_SniperRifle", "Gun_Minigun", "Bullet_Minigun", "Gun_TripleRocket", "Bullet_Rocket", "Gun_DoomsdayRocket", "Bullet_DoomsdayRocket", "Weapon_GrenadeFrag", "Proj_GrenadeFrag", "Weapon_GrenadeMolotov", "Proj_GrenadeMolotov", "Weapon_GrenadeEMP", "Proj_GrenadeEMP", "Bullet_ChargeBlasterHeavy", "Gun_ChargeBlasterHeavy", "Bullet_InfernoCannon", "Gun_InfernoCannon", "Gun_Needle", "Bullet_NeedleGun", "Bow_Short", "Arrow_Short", "Pila", "Pilum_Thrown", "Bow_Recurve", "Arrow_Recurve", "Bow_Great", "Arrow_Great", "Gun_ChargeRifle", "Bullet_ChargeRifle", "Gun_ChargeLance", "Bullet_ChargeLance", "OrbitalTargeterBombardment", "OrbitalTargeterPowerBeam", "TornadoGenerator", "Gun_ThumpCannon", "Bullet_ThumpCannon" };


    }
}