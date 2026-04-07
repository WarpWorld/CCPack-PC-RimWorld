using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CrowdControl {

    public class ResurrectAllColonistsEffect : Effect {
        public override string Code => EffectCode.ResurrectAllColonists;

        public override EffectStatus Execute(EffectCommand command) {
            List<Pawn> deadColonists = PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead
                .Where(colonist => colonist != null && colonist.Dead && colonist.IsColonist && colonist.Faction == Faction.OfPlayer)
                .ToList();

            if (deadColonists.Count == 0) {
                return EffectStatus.Failure;
            }

            List<Thing> resurrectedColonists = new List<Thing>();
            foreach (Pawn colonist in deadColonists) {
                ResurrectionUtility.TryResurrect(colonist);
                if (!colonist.health.Dead) {
                    resurrectedColonists.Add(colonist);
                }
            }

            if (resurrectedColonists.Count == 0) {
                return EffectStatus.Failure;
            }

            SendCardNotification(resurrectedColonists, LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }
    }
}
