using RimWorld;
using Verse;
using System.Linq;
using Verse.AI;
using System;
using System.Collections.Generic;

namespace CrowdControl {

    public class HunterBecomesHuntedEffect : Effect {
        public override string Code => EffectCode.HunterBecomesHunted;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            List<Pawn> predators = currentMap.mapPawns.AllPawnsSpawned.Where(TargetIsPredator).ToList();
            List<Pawn> potentialPrey = currentMap.mapPawns.SpawnedColonyAnimals.ToList();

            if (predators.Count > 0 && potentialPrey.Count > 0) {
                Pawn predator = predators.RandomElement();
                Pawn prey = potentialPrey.RandomElement();

                bool alreadyHunting = predator.jobs.AllJobs().Any(a => a.def == JobDefOf.PredatorHunt);
                if (alreadyHunting == false) {
                    Job job = JobMaker.MakeJob(JobDefOf.PredatorHunt, new LocalTargetInfo(prey));
                    predator.jobs.StartJob(job);
                    SendCardNotification(lookAtThings: new List<Thing> { predator, prey }, notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName);
                    return EffectStatus.Success;
                }
            }
            return EffectStatus.Failure;
        }

        private bool TargetIsPredator(Pawn target) {
            return target.Faction != Faction.OfPlayer && 
                target.def.race.Animal == true && 
                target.health.Dead == false;
        }

    }
}
