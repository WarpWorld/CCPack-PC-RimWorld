using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace CrowdControl {

    public class PowerOutageEffect : Effect {
        public override string Code => EffectCode.PowerOutage;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting((Settings));
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 1);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 3);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IncidentDef incidentDef = IncidentDefOf.SolarFlare;
            int durationInDays = ModService.Instance.Random.Next(MinCount, MaxCount);
            int duration = Mathf.RoundToInt(durationInDays * 60000f);

            GameConditionManager gameConditionManager = currentMap.GameConditionManager;
            GameConditionDef conditionDef = GameConditionDefOf.SolarFlare;

            if (gameConditionManager.ConditionIsActive(conditionDef) == false) {
                GameCondition gameCondition = GameConditionMaker.MakeCondition(incidentDef.gameCondition, duration);
                gameConditionManager.RegisterCondition(gameCondition);

                SendCardNotification(notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName);
                return EffectStatus.Success;
            }
            return EffectStatus.Failure;
        }
    }
}