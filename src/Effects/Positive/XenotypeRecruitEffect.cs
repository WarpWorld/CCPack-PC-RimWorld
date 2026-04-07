using RimWorld;
using Verse;
using System;

namespace CrowdControl {

    public class XenotypeRecruitEffect : Effect {
        public override string Code => EffectCode.XenotypeRecruit;

        public override EffectStatus Execute(EffectCommand command) {
            if (!ModsConfig.BiotechActive) {
                return MissingRequiredDlc(command, "Biotech");
            }

            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false) {
                return EffectStatus.Failure;
            }

            var items = command.code.Split('_');
            if (items.Length < 2) {
                return EffectStatus.Failure;
            }

            string xenotypeName = ResolveXenotypeName(items[1]);
            XenotypeDef xenotype = DefDatabase<XenotypeDef>.GetNamedSilentFail(xenotypeName);
            if (xenotype == null) {
                return EffectStatus.Failure;
            }

            IntVec3 spawnLocation;
            if (!ModService.Instance.TryFindRandomRoadEntryCell(currentMap, out spawnLocation)) {
                if (!ModService.Instance.TryFindRandomEntryCell(currentMap, out spawnLocation)) {
                    spawnLocation = CellFinder.RandomClosewalkCellNear(currentMap.Center, currentMap, 10, cell => cell.Standable(currentMap));
                }
            }

            Ideo primaryIdeo = ModsConfig.IdeologyActive ? Faction.OfPlayer?.ideos?.PrimaryIdeo : null;
            PawnGenerationRequest request = new PawnGenerationRequest(
                PawnKindDefOf.Colonist,
                Faction.OfPlayer,
                PawnGenerationContext.NonPlayer,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: true,
                colonistRelationChanceFactor: 20f,
                fixedIdeo: primaryIdeo,
                forcedXenotype: xenotype);

            Pawn newRecruit = PawnGenerator.GeneratePawn(request);

            if (primaryIdeo != null && newRecruit.ideo != null) {
                newRecruit.ideo.SetIdeo(primaryIdeo);
            }

            if (ModService.Instance.ShouldDisplayViewerName && String.IsNullOrEmpty(command.viewerName) == false) {
                NameTriple name = newRecruit.Name as NameTriple;
                if (name != null) {
                    newRecruit.Name = new NameTriple(name.First, command.viewerName, name.Last);
                }
            }

            GenSpawn.Spawn(newRecruit, spawnLocation, currentMap, WipeMode.Vanish);
            SendCardNotification(newRecruit, LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }

        private static string ResolveXenotypeName(string xenotypeName) {
            switch (xenotypeName) {
                case "Sanguophages":
                    return "Sanguophage";
                case "Neandrathals":
                case "Neanderthals":
                    return "Neanderthal";
                case "Yakkakin":
                    return "Yttakin";
                default:
                    return xenotypeName;
            }
        }
    }
}
