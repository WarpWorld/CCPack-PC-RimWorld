using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CrowdControl {

    public class InstantGrowCropsEffect : Effect {
        public override string Code => EffectCode.InstantGrowCrops;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false) {
                return EffectStatus.Failure;
            }

            List<Plant> crops = currentMap.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
                .OfType<Plant>()
                .Where(plant => plant != null
                    && !plant.Destroyed
                    && plant.sown
                    && plant.def?.plant != null
                    && plant.def.plant.Harvestable
                    && !plant.HarvestableNow)
                .ToList();

            if (crops.Count == 0) {
                return EffectStatus.Retry;
            }

            foreach (Plant crop in crops) {
                crop.Growth = 1f;
            }

            SendCardNotification(crops.Select(crop => (Thing)crop).ToList(), LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }
    }
}
