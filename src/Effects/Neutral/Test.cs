using RimWorld;
using Verse;
using System.Linq;
using HugsLib.Settings;
using System;

namespace CrowdControl {

    public class Test : Effect {
        public override string Code => EffectCode.FoulFood;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;


            var foodItems = DefDatabase<WeatherDef>.AllDefs?.ToList();
            foreach (WeatherDef thing in foodItems)
            {
                Verse.Log.Message($"{thing.defName}");
            }

            /*
            var foodItems = DefDatabase<ThingDef>.AllDefs?.Where(IsFoodItem).ToList();
            foreach (ThingDef thing in foodItems)
            {
                Verse.Log.Message($"{thing.defName}");
            }*/

            return EffectStatus.Success;
        }

        private static bool IsFoodItem(ThingDef thingDef) {
            return true;// (thingDef.IsNutritionGivingIngestible);
        }
    }
}