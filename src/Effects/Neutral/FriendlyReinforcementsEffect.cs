using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace CrowdControl {

    public class FriendlyReinforcementsEffect : Effect {
        public override string Code => EffectCode.FriendlyReinforcements;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false) {
                return EffectStatus.Failure;
            }

            float points = Mathf.Max(300f, StorytellerUtility.DefaultThreatPointsNow(currentMap));

            IntVec3 spawnLocation;
            if (!ModService.Instance.TryFindRandomRoadEntryCell(currentMap, out spawnLocation)) {
                if (!ModService.Instance.TryFindRandomEntryCell(currentMap, out spawnLocation)) {
                    spawnLocation = CellFinder.RandomClosewalkCellNear(currentMap.Center, currentMap, 10, cell => cell.Standable(currentMap));
                }
            }

            // Prefer RimWorld's built-in allied military aid when an ally is available.
            Faction alliedFaction = Find.FactionManager.RandomAlliedFaction(allowHidden: true);
            if (alliedFaction != null) {
                IncidentParms aidParms = new IncidentParms();
                aidParms.target = currentMap;
                aidParms.faction = alliedFaction;
                aidParms.points = points;
                aidParms.spawnCenter = spawnLocation;
                aidParms.raidArrivalModeForQuickMilitaryAid = true;
                aidParms.biocodeApparelChance = 1f;
                aidParms.biocodeWeaponsChance = 1f;

                if (IncidentDefOf.RaidFriendly.Worker.CanFireNow(aidParms) && IncidentDefOf.RaidFriendly.Worker.TryExecute(aidParms)) {
                    SendCardNotification(currentMap, spawnLocation, LetterDefOf.PositiveEvent, command.viewerName);
                    return EffectStatus.Success;
                }
            }

            if (ModService.Instance.TryGetFriendlyCombatFaction(out Faction faction, points) == false) {
                return SetEffectStatus(EffectStatus.Failure, "No friendly faction was available to send reinforcements.");
            }

            points = Mathf.Max(points, faction.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.Combat) * 1.05f);

            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, currentMap);
            parms.target = currentMap;
            parms.faction = faction;
            parms.points = points;
            parms.spawnCenter = spawnLocation;

            List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(
                IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms, ensureCanGenerateAtLeastOnePawn: true),
                warnOnZeroResults: false).ToList();

            if (pawns.Count == 0) {
                return SetEffectStatus(EffectStatus.Failure, "Failed to generate any friendly reinforcements.");
            }

            IntVec3 fallbackLocation = CellFinder.RandomClosewalkCellNear(currentMap.Center, currentMap, 10, cell => cell.Standable(currentMap));
            foreach (Pawn pawn in pawns) {
                IntVec3 pawnSpawnLocation = CellFinder.RandomClosewalkCellNear(spawnLocation, currentMap, 6, cell => cell.Standable(currentMap));
                GenSpawn.Spawn(pawn, pawnSpawnLocation, currentMap, WipeMode.Vanish);
            }

            LordMaker.MakeNewLord(faction, new LordJob_AssistColony(faction, fallbackLocation), currentMap, pawns);
            SendCardNotification(currentMap, spawnLocation, LetterDefOf.PositiveEvent, command.viewerName);
            return EffectStatus.Success;
        }
    }
}
