using RimWorld;
using Verse;
using System.Linq;

namespace CrowdControl {

    public class ResurrectColonistEffect : Effect {
        public override string Code => EffectCode.ResurrectColonist;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            Pawn toRessurect = Find.ColonistBar.GetColonistsInOrder()?.Where(colonist => colonist.Dead).RandomElement();
            if (toRessurect != null) {
                ResurrectionUtility.Resurrect(toRessurect);
                
                if (toRessurect.health.Dead == false) { 
                    SendCardNotification(toRessurect, LetterDefOf.PositiveEvent, command.viewerName);
                    return EffectStatus.Success;
                }
            }
            return EffectStatus.Failure;
        }
    }
}
