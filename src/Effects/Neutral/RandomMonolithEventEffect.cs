using RimWorld;
using Verse;

namespace CrowdControl {

    public class RandomMonolithEventEffect : Effect {
        public override string Code => EffectCode.RandomMonolithEvent;

        public override EffectStatus Execute(EffectCommand command) {
            if (!ModsConfig.AnomalyActive) {
                return MissingRequiredDlc(command, "Anomaly");
            }

            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false) {
                return EffectStatus.Failure;
            }

            int pick = ModService.Instance.Random.Next(0, 3);
            for (int i = 0; i < 3; i++) {
                switch ((pick + i) % 3) {
                    case 0:
                        if (TryTriggerQuest(currentMap, "MonolithMigration", command.viewerName) == EffectStatus.Success) {
                            return EffectStatus.Success;
                        }
                        break;
                    case 1:
                        if (TryTriggerIncident(currentMap, "DeathPall", command.viewerName) == EffectStatus.Success) {
                            return EffectStatus.Success;
                        }
                        break;
                    default:
                        if (TryTriggerCondition(currentMap, "GrayPall", command.viewerName) == EffectStatus.Success) {
                            return EffectStatus.Success;
                        }
                        break;
                }
            }

            return EffectStatus.Failure;
        }

        private EffectStatus TryTriggerQuest(Map map, string questName, string viewerName) {
            QuestScriptDef questDef = DefDatabase<QuestScriptDef>.GetNamedSilentFail(questName);
            if (questDef == null) {
                return EffectStatus.Failure;
            }

            RimWorld.QuestGen.Slate slate = new RimWorld.QuestGen.Slate();
            slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(map));
            slate.Set("map", map);
            slate.Set("discoveryMethod", "Crowd Control");
            if (!questDef.CanRun(slate, map)) {
                return EffectStatus.Failure;
            }

            Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(questDef, slate);
            if (!quest.hidden && questDef.sendAvailableLetter) {
                QuestUtility.SendLetterQuestAvailable(quest, "Crowd Control");
            }

            SendCardNotification(map, map.Center, LetterDefOf.NeutralEvent, viewerName);
            return EffectStatus.Success;
        }

        private EffectStatus TryTriggerIncident(Map map, string incidentName, string viewerName) {
            IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamedSilentFail(incidentName);
            if (incidentDef == null) {
                return EffectStatus.Failure;
            }

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(incidentDef.category, map);
            parms.target = map;
            if (!incidentDef.Worker.CanFireNow(parms) || !incidentDef.Worker.TryExecute(parms)) {
                return EffectStatus.Failure;
            }

            SendCardNotification(map, map.Center, LetterDefOf.NeutralEvent, viewerName);
            return EffectStatus.Success;
        }

        private EffectStatus TryTriggerCondition(Map map, string conditionName, string viewerName) {
            GameConditionDef conditionDef = DefDatabase<GameConditionDef>.GetNamedSilentFail(conditionName);
            if (conditionDef == null || map.GameConditionManager.ConditionIsActive(conditionDef)) {
                return EffectStatus.Failure;
            }

            GameCondition gameCondition = GameConditionMaker.MakeCondition(conditionDef, 60000);
            map.GameConditionManager.RegisterCondition(gameCondition);
            SendCardNotification(map, map.Center, LetterDefOf.NeutralEvent, viewerName);
            return EffectStatus.Success;
        }
    }
}
