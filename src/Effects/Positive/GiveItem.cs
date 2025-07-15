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

            var donationItems = DefDatabase<ThingDef>.AllDefs?.Where(t => t.defName == itemname);
 
            List<Thing> spawnItems = new List<Thing>();

            ThingDef itemDef = donationItems.RandomElement();
            var thing = ModService.Instance.CreateItem(itemDef);
            thing.stackCount = qty;
            spawnItems.Add(thing);

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
