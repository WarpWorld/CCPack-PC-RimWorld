using RimWorld;
using Verse;
using System.Linq;
using HugsLib.Settings;
using System;

namespace CrowdControl {

    public class FoulFoodEffect : Effect {
        public override string Code => EffectCode.FoulFood;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            var foodItems = DefDatabase<ThingDef>.AllDefs?.Where(IsFoodItem).ToList();
            var stockpileZones = currentMap.zoneManager.AllZones.OfType<Zone_Stockpile>().ToList();

            foreach (Zone_Stockpile zone in stockpileZones) {
                foreach (IntVec3 cell in zone.Cells) {
                    Thing item = currentMap.thingGrid.ThingAt(cell, ThingCategory.Item);
                    if (item != null && foodItems.Contains(item.def)) {
                        item.Destroy();
                        if (item is Corpse == false && String.IsNullOrEmpty(item.Label) == false) {
                            string labelText = String.Format($"{Code}.Letter".Translate(), item.Label);
                            SendCardNotification(label: labelText, description: $"{Code}.Description".Translate(), notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName);
                        }
                        else {
                            SendCardNotification(notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName);
                        }
                        return EffectStatus.Success;
                    }
                }
            }

            return EffectStatus.Failure;
        }

        private static bool IsFoodItem(ThingDef thingDef) {
            return (thingDef.IsNutritionGivingIngestible);
        }
    }
}