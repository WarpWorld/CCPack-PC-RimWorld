using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Collections.Generic;
using System.Linq;

namespace CrowdControl {

    public class SetWeather : Effect {
        public override string Code => EffectCode.SetWeather;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;
            
            var items = command.code.Split('_');
            var itemname = items[1];

            itemname = itemname.Replace("-", "_");

            Verse.WeatherDef weather;

            weather = WeatherDef.Named(itemname);

            if (currentMap.weatherManager.curWeather == weather) return EffectStatus.Retry;

            currentMap.weatherManager.TransitionTo(weather);

            SendCardNotification("Weather Changed", $"{command.viewerName} changed the weather to {itemname}.", LetterDefOf.NeutralEvent, command.viewerName);
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
