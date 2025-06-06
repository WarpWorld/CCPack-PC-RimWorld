﻿using RimWorld;
using Verse;

namespace CrowdControl {
    public class MeteoriteLandingEffect : Effect {
        public override string Code => EffectCode.MeteoriteLanding;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, currentMap);
            IncidentWorker_MeteoriteImpact worker = new IncidentWorker_MeteoriteImpact();

            worker.def = IncidentDefOf.ShipChunkDrop;
            worker.def.label = "meteoritelanding.Label".Translate();
            worker.def.letterText = "meteoritelanding.Description".Translate();

            if (worker.TryExecute(parms) == false) {
                return EffectStatus.Failure;
            }

            SendCardNotification(triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}