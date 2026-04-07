using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CrowdControl {

    public class ColonyWideMoodBoostEffect : Effect {
        public override string Code => EffectCode.ColonyWideMoodBoost;

        public override EffectStatus Execute(EffectCommand command) {
            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder()
                .Where(pawn => pawn != null && pawn.IsColonist && !pawn.Dead && pawn.needs?.mood != null)
                .Distinct()
                .ToList();

            if (colonists.Count == 0) {
                return EffectStatus.Failure;
            }

            ThoughtDef def = DefDatabase<ThoughtDef>.GetNamedSilentFail("MoodBoost");
            if (def == null) {
                return EffectStatus.Failure;
            }

            foreach (Pawn pawn in colonists) {
                Thought_Memory moodBoostThought = (Thought_Memory)ThoughtMaker.MakeThought(def);
                pawn.needs.mood.thoughts.memories.TryGainMemory(moodBoostThought, null);
            }

            SendCardNotification(colonists.Select(pawn => (Thing)pawn).ToList(), LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }
    }
}
