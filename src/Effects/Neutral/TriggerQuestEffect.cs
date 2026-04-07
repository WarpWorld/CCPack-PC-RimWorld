using RimWorld;
using RimWorld.QuestGen;
using System.Linq;
using Verse;

namespace CrowdControl {

    public class TriggerQuestEffect : Effect {
        public override string Code => EffectCode.TriggerQuest;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false) {
                return EffectStatus.Failure;
            }

            var items = command.code.Split('_');
            if (items.Length < 2) {
                return EffectStatus.Failure;
            }

            string questName = string.Join("_", items.Skip(1).ToArray());
            if (!ModsConfig.AnomalyActive && IsAnomalyOnlyQuest(questName)) {
                return MissingRequiredDlc(command, "Anomaly");
            }

            if (questName == "TransportPodCrash" || questName == "ResourcePodCrash") {
                return TryTriggerResourcePodDrop(currentMap, command.viewerName);
            }

            QuestScriptDef questDef = DefDatabase<QuestScriptDef>.GetNamedSilentFail(questName);
            if (questDef == null) {
                return EffectStatus.Failure;
            }

            Slate slate = new Slate();
            slate.Set("points", StorytellerUtility.DefaultThreatPointsNow(currentMap));
            slate.Set("map", currentMap);
            slate.Set("discoveryMethod", "Crowd Control");

            if (!questDef.CanRun(slate, currentMap)) {
                return EffectStatus.Failure;
            }

            Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(questDef, slate);
            if (!quest.hidden && questDef.sendAvailableLetter) {
                QuestUtility.SendLetterQuestAvailable(quest, "Crowd Control");
            }

            SendCardNotification(currentMap, currentMap.Center, LetterDefOf.NeutralEvent, command.viewerName);
            return EffectStatus.Success;
        }

        private static bool IsAnomalyOnlyQuest(string questName) {
            switch (questName) {
                case "CreepJoinerArrival":
                case "MonolithMigration":
                    return true;
                default:
                    return false;
            }
        }

        private EffectStatus TryTriggerResourcePodDrop(Map currentMap, string viewerName) {
            if (ThingSetMakerDefOf.ResourcePod?.root == null) {
                return EffectStatus.Failure;
            }

            IntVec3 dropSpot = DropCellFinder.RandomDropSpot(currentMap);
            DropPodUtility.DropThingsNear(dropSpot, currentMap, ThingSetMakerDefOf.ResourcePod.root.Generate(), 110, canInstaDropDuringInit: false, leaveSlag: true);

            SendCardNotification(currentMap, dropSpot, LetterDefOf.PositiveEvent, viewerName);
            return EffectStatus.Success;
        }
    }
}
