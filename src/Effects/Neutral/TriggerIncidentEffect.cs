using RimWorld;
using System.Linq;
using Verse;

namespace CrowdControl {

    public class TriggerIncidentEffect : Effect {
        public override string Code => EffectCode.TriggerIncident;

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

            string incidentName = string.Join("_", items.Skip(1).ToArray());
            if (!ModsConfig.AnomalyActive && IsAnomalyOnlyIncident(incidentName)) {
                return MissingRequiredDlc(command, "Anomaly");
            }

            if (incidentName == "ResourcePodCrash") {
                return TryTriggerResourcePodDrop(currentMap, command.viewerName);
            }

            IncidentDef incidentDef = DefDatabase<IncidentDef>.GetNamedSilentFail(incidentName);
            if (incidentDef == null) {
                return EffectStatus.Failure;
            }

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(incidentDef.category, currentMap);
            parms.target = currentMap;
            if (!incidentDef.Worker.CanFireNow(parms) || !incidentDef.Worker.TryExecute(parms)) {
                return EffectStatus.Failure;
            }

            SendCardNotification(currentMap, currentMap.Center, LetterDefOf.NeutralEvent, command.viewerName);
            return EffectStatus.Success;
        }

        private static bool IsAnomalyOnlyIncident(string incidentName) {
            switch (incidentName) {
                case "VoidCuriosity":
                case "DeathPall":
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
