using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Collections.Generic;
using System.Linq;

namespace CrowdControl {

    public class SuperGiftEffect : Effect {
        public override string Code => EffectCode.SuperGift;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 1);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 4);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            var donationItems = DefDatabase<ThingDef>.AllDefs?.Where(IsDonationItem);
            int spawnCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            List<Thing> spawnItems = new List<Thing>(spawnCount);

            foreach (var i in Enumerable.Range(0, spawnCount)) {
                ThingDef itemDef = donationItems.RandomElement();
                spawnItems.Add(ModService.Instance.CreateItem(itemDef));
            }

            IntVec3 location = DropCellFinder.TradeDropSpot(currentMap);
            DropPodUtility.DropThingsNear(location, currentMap, spawnItems);

            SendCardNotification(currentMap, location, LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }

        private static bool IsDonationItem(ThingDef thingDef) {
            return ((thingDef.BaseMarketValue >= 310) &&
                (thingDef.category == ThingCategory.Item || thingDef.category == ThingCategory.Building) &&
                (thingDef.category != ThingCategory.Pawn) &&
                (thingDef.IsApparel || thingDef.IsWeapon || thingDef.IsDrug || thingDef.IsIngestible || thingDef.IsMetal || thingDef.IsMedicine || thingDef.IsArt) &&
                (thingDef.IsCorpse == false));
        }
    }
}