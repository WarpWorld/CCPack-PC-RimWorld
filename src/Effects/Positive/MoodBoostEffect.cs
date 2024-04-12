using RimWorld;
using System.Collections.Generic;
using Verse;

namespace CrowdControl {

    public class MoodBoostEffect : Effect {
        public override string Code => EffectCode.MoodBoost;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();
            if (colonists.Count > 0) {
                Pawn pawn = colonists.RandomElement();
                if (pawn.Dead == false) {
                    ThoughtDef def = DefDatabase<ThoughtDef>.GetNamed("MoodBoost");
                    if (def != null && pawn != null && pawn.IsColonist && !pawn.health.Dead) {
                        Thought_Memory moodBoostThought = (Thought_Memory)ThoughtMaker.MakeThought(def);
                        pawn.needs.mood.thoughts.memories.TryGainMemory(moodBoostThought, null);

                        SendCardNotification(lookAtThings: new List<Thing> { pawn }, notificationType: LetterDefOf.PositiveEvent, triggeredBy: command.viewerName);
                        return EffectStatus.Success;
                    }
                }
            }
            return EffectStatus.Failure;
        }
    }
}