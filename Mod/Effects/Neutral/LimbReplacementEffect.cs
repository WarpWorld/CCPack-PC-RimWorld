using RimWorld;
using Verse;
using System.Linq;
using System.Collections.Generic;

namespace CrowdControl {

    public class LimbReplacementEffect : Effect {
        public override string Code => EffectCode.LimbReplacement;

        private readonly Dictionary<string, string> LookupTable = new Dictionary<string, string>() {
            { "InstallBionicArm", "InstallWoodenHand" },
            { "InstallBionicLeg", "InstallPegLeg" },
            { "InstallBionicTongue", "InstallDenture" },

            { "InstallWoodenHand", "InstallBionicArm" },
            { "InstallPegLeg", "InstallBionicLeg" },
            { "InstallDenture", "InstallBionicTongue" }
        };

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            Pawn targetColonist = Find.ColonistBar.GetColonistsInOrder().RandomElement();
            if (targetColonist == null || targetColonist.Dead)
                return EffectStatus.Failure;

            var target = LookupTable.RandomElement();
            var primary = target.Key;
            var secondary = target.Value;

            RecipeDef primarySurgeryDef = DefDatabase<RecipeDef>.GetNamed(primary);
            HediffDef primaryImplantDef = primarySurgeryDef.addsHediff;
            bool hasPrimary = targetColonist.health.hediffSet.HasHediff(primaryImplantDef);

            RecipeDef secondarySurgeryDef = DefDatabase<RecipeDef>.GetNamed(secondary);
            HediffDef secondaryImplantDef = secondarySurgeryDef.addsHediff;
            bool hasSecondry = targetColonist.health.hediffSet.HasHediff(secondaryImplantDef);

            RecipeDef surgeryDefToAdd = null;
            HediffDef implantDefToAdd = null;
            Hediff existingImplant = null;

            if (!hasSecondry && !hasPrimary) {
                surgeryDefToAdd = primarySurgeryDef;
                implantDefToAdd = primaryImplantDef;
            }
            // Already has primary implant. Remove primary, give secondary.
            else if (hasPrimary && !hasSecondry) {
                surgeryDefToAdd = secondarySurgeryDef;
                implantDefToAdd = secondaryImplantDef;
                existingImplant = targetColonist.health.hediffSet.GetFirstHediffOfDef(primaryImplantDef, false);
            }
            // Already has secondary implant. Remove secondary, give primary.
            else if (hasSecondry && !hasPrimary) {
                surgeryDefToAdd = primarySurgeryDef;
                implantDefToAdd = primaryImplantDef;
                existingImplant = targetColonist.health.hediffSet.GetFirstHediffOfDef(secondaryImplantDef, false);
            }
            else
                return EffectStatus.Failure;

            if (existingImplant != null) {
                targetColonist.health.RemoveHediff(existingImplant);
            }

            BodyPartDef targetBodyPart = surgeryDefToAdd.appliedOnFixedBodyParts.First();
            BodyPartRecord bodyPartRecord = targetColonist.RaceProps.body.GetPartsWithDef(targetBodyPart).FirstOrFallback(null);
            bool missingBodyPart = targetColonist.health.hediffSet.PartIsMissing(bodyPartRecord);
            if (missingBodyPart) {
                RecreateBodyPartChain(targetColonist, bodyPartRecord);
            }

            targetColonist.health.AddHediff(implantDefToAdd, bodyPartRecord, null, null);
            SendCardNotification(notificationType: LetterDefOf.NeutralEvent, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }

        public void RecreateBodyPartChain(Pawn targetPawn, BodyPartRecord targetPart) {
            // First is the target missing?
            Hediff missingPart = GetMissingPart(targetPawn, targetPart);
            if (missingPart != null) {
                targetPawn.health.RemoveHediff(missingPart);
            }

            // Second, does the target have missing nodes? 
            bool targetHasMissingParts = HasMissingParts(targetPawn, targetPart);
            if (targetHasMissingParts) {
                foreach (var part in targetPart.parts) {
                    Hediff missingPart2 = GetMissingPart(targetPawn, part);
                    if (missingPart2 != null)
                        targetPawn.health.RemoveHediff(missingPart2);
                }
            }

            // Third, does the target's parent missing, or have missing nodes?
            BodyPartRecord parent = targetPart.parent;
            if (parent != null) {
                bool parentIsMissing = GetMissingPart(targetPawn, parent) != null;
                bool parentHasMissingParts = HasMissingParts(targetPawn, parent);
                if (parentIsMissing) {
                    RecreateBodyPartChain(targetPawn, parent);
                }
                else if (parentHasMissingParts) {
                    foreach (var part in parent.parts) {
                        Hediff missingPart2 = GetMissingPart(targetPawn, part);
                        if (missingPart2 != null)
                            targetPawn.health.RemoveHediff(missingPart2);
                    }
                }
            }
        }

        private Hediff GetMissingPart(Pawn targetPawn, BodyPartRecord missingPart) {
            List<Hediff> parts = targetPawn.health.hediffSet.hediffs.Where(hediff => hediff.Part == missingPart && hediff is Hediff_MissingPart).ToList();
            return (parts.Count > 0) ? parts.First() : null;
        }

        private bool HasMissingParts(Pawn targetPawn, BodyPartRecord rootPart) {
            List<BodyPartRecord> parts = rootPart.parts.Where(part => GetMissingPart(targetPawn, part) != null).ToList();
            return (parts.Count > 0) ? true : false;
        }
    }
}
