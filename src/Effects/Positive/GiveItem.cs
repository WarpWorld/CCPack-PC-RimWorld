using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Collections.Generic;
using System.Linq;

namespace CrowdControl {

    public class GiveItem : Effect {
        public override string Code => EffectCode.GiveItem;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            var items = command.code.Split('_');
            var itemname = items[1];

            itemname = itemname.Replace("-", "_");

            var qty = int.Parse(items[2]);

            /*
            Verse.Log.Message($"Melee Weapons");
            var foodItems = DefDatabase<ThingDef>.AllDefs.Where(t => t.IsMeleeWeapon).ToList();/////////
            foreach (ThingDef item in foodItems)/////////
            {
                Verse.Log.Message($"{item.defName}");/////////
            }
            return EffectStatus.Success;
            */


            IEnumerable<ThingDef> donationItems;
            ThingDef itemDef;

            switch (itemname) {
                case "randfood":
                    donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.IsNutritionGivingIngestible && !t.defName.StartsWith("Corpse_") && !t.defName.StartsWith("Plant_Tree"));
                    //Verse.Log.Message($"num options: {donationItems.Count()}");
                    itemDef = donationItems.RandomElement();
                    //Verse.Log.Message($"chosen: {itemDef}");
                    itemname = itemDef.defName;
                    break;
                case "randapparel":
                    donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.IsApparel && (t.defName.StartsWith("Apparel")));
                    //Verse.Log.Message($"num options: {donationItems.Count()}");
                    itemDef = donationItems.RandomElement();
                    //Verse.Log.Message($"chosen: {itemDef}");
                    itemname = itemDef.defName;
                    break;
                case "randranged":
                    donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.IsRangedWeapon && (t.defName.StartsWith("Gun") || t.defName.StartsWith("Bow")));
                    //Verse.Log.Message($"num options: {donationItems.Count()}");
                    itemDef = donationItems.RandomElement();
                    //Verse.Log.Message($"chosen: {itemDef}");
                    itemname = itemDef.defName;
                    break;
                case "randmelee":
                    donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.IsMeleeWeapon && (t.defName.StartsWith("MeleeWeapon")));
                    //Verse.Log.Message($"num options: {donationItems.Count()}");
                    itemDef = donationItems.RandomElement();
                    //Verse.Log.Message($"chosen: {itemDef}");
                    itemname = itemDef.defName;
                    break;
                default:
                    donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.defName == itemname);
                    itemDef = donationItems.RandomElement();
                    break;
            }
            List<Thing> spawnItems = new List<Thing>();

            
            var thing = ModService.Instance.CreateItem(itemDef);
            thing.stackCount = qty;
            spawnItems.Add(thing);

            /*
            if(itemname == "Gun_BoltActionRifle")
            {
                donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.defName == "Bullet_BoltActionRifle");

                itemDef = donationItems.RandomElement();
                thing = ModService.Instance.CreateItem(itemDef);
                thing.stackCount = 25;
                spawnItems.Add(thing);
            }

            if(itemname == "Gun_PumpShotgun")
            {
                donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.defName == "Bullet_Shotgun");

                itemDef = donationItems.RandomElement();
                thing = ModService.Instance.CreateItem(itemDef);
                thing.stackCount = 25;
                spawnItems.Add(thing);
            }

            if(itemname == "Gun_Minigun")
            {
                donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.defName == "Bullet_Minigun");

                itemDef = donationItems.RandomElement();
                thing = ModService.Instance.CreateItem(itemDef);
                thing.stackCount = 100;
                spawnItems.Add(thing);
            }
            */

            IntVec3 location = DropCellFinder.TradeDropSpot(currentMap);
            DropPodUtility.DropThingsNear(location, currentMap, spawnItems);

            SendCardNotification(currentMap, location, "Received Items", $"{command.viewerName} sent you {qty} {itemname}.", LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }

        private static bool IsDonationItem(ThingDef thingDef) {
            return ((thingDef.BaseMarketValue > 0 && thingDef.BaseMarketValue <= 310.5f) &&
                (thingDef.category == ThingCategory.Item || thingDef.category == ThingCategory.Building) &&
                (thingDef.category != ThingCategory.Pawn) &&
                (thingDef.IsApparel || thingDef.IsWeapon || thingDef.IsDrug || thingDef.IsIngestible || thingDef.IsMetal || thingDef.IsMedicine || thingDef.IsArt) &&
                (thingDef.IsCorpse == false));
        }
    }
}
