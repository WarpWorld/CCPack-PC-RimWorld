using RimWorld;
using Verse;
using System.Linq;
using System;
using HugsLib.Settings;
using System.Collections.Generic;

namespace CrowdControl {

    public class CreateHatsEffect : Effect {
        public override string Code => EffectCode.CreateHats;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 2);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 5);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            var hatDefs = DefDatabase<ThingDef>.AllDefs.Where(IsHeadGearItem);
            hatDefs = hatDefs.OrderBy(def => def.techLevel).ToList();
            hatDefs = hatDefs.Take((hatDefs.Count() - 1) / 2);

            int spawnCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            List<Thing> spawnItems = new List<Thing>(spawnCount);

            foreach (var i in Enumerable.Range(0, spawnCount)) {
                ThingDef itemDef = hatDefs.RandomElement();
                spawnItems.Add(ModService.Instance.CreateItem(itemDef));
            }

            IntVec3 location = DropCellFinder.TradeDropSpot(currentMap);
            DropPodUtility.DropThingsNear(location, currentMap, spawnItems);

            SendCardNotification(currentMap, location, LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }

        private bool IsHeadGearItem(ThingDef def) {
            return (def.fileName == "Apparel_Headgear.xml");
        }
    }
}