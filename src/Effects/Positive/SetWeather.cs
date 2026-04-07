using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrowdControl {

    public class SetWeather : Effect {
        public override string Code => EffectCode.SetWeather;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;
            
            var items = command.code.Split('_');
            var itemname = string.Join("_", items.Skip(1).ToArray());

            itemname = itemname.Replace("-", "_");

            EffectStatus specialWeatherStatus;
            if (TryApplySpecialCondition(currentMap, itemname, command, out specialWeatherStatus)) {
                return specialWeatherStatus;
            }

            Verse.WeatherDef weather = DefDatabase<WeatherDef>.GetNamedSilentFail(itemname);
            if (weather == null) {
                return EffectStatus.Failure;
            }

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

        private bool TryApplySpecialCondition(Map currentMap, string itemname, EffectCommand command, out EffectStatus status) {
            status = EffectStatus.Failure;

            GameConditionDef conditionDef = null;
            bool supportedCondition = true;

            switch (itemname) {
                case "VolcanicWinter":
                case "ToxicFallout":
                    conditionDef = DefDatabase<GameConditionDef>.GetNamedSilentFail(itemname);
                    break;
                case "GrayPall":
                case "BloodRain":
                    if (!ModsConfig.AnomalyActive) {
                        status = MissingRequiredDlc(command, "Anomaly");
                        return true;
                    }
                    conditionDef = DefDatabase<GameConditionDef>.GetNamedSilentFail(itemname);
                    break;
                default:
                    supportedCondition = false;
                    break;
            }

            if (!supportedCondition) {
                return false;
            }

            if (conditionDef == null) {
                return true;
            }

            GameConditionManager gameConditionManager = currentMap.GameConditionManager;
            if (gameConditionManager.ConditionIsActive(conditionDef)) {
                status = EffectStatus.Retry;
                return true;
            }

            int duration = GetConditionDurationTicks(command, itemname);
            GameCondition gameCondition = GameConditionMaker.MakeCondition(conditionDef, duration);
            gameConditionManager.RegisterCondition(gameCondition);

            string label = conditionDef.label ?? itemname;
            SendCardNotification("Weather Changed", $"{command.viewerName} triggered {label}.", LetterDefOf.NeutralEvent, command.viewerName);
            status = EffectStatus.Success;
            return true;
        }

        private static int GetConditionDurationTicks(EffectCommand command, string itemname) {
            if (command.duration > 0) {
                return Mathf.Max(1, Mathf.RoundToInt(command.duration / 1000f * 60f));
            }

            switch (itemname) {
                case "ToxicFallout":
                case "VolcanicWinter":
                    IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamedSilentFail(itemname);
                    if (incidentDef != null) {
                        return Mathf.RoundToInt(incidentDef.durationDays.RandomInRange * 60000f);
                    }
                    break;
                case "BloodRain":
                    return Mathf.RoundToInt(0.5f * 60000f);
                case "GrayPall":
                    return Mathf.RoundToInt(60000f);
            }

            return Mathf.RoundToInt(60000f);
        }
    }
}
