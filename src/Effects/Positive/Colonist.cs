using RimWorld;
using System.Collections.Generic;
using Verse;

namespace CrowdControl {

    public class Colonist : Effect {
        public override string Code => EffectCode.Colonist;

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            var items = command.code.Split('_');
            var itemname = items[1];

            itemname = itemname.Replace("-", "_");

            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();
            colonists.RemoveWhere(p => p.Dead == true);
            if (colonists.Count > 0) {
                Pawn pawn = colonists.RandomElement();

                switch (itemname)
                {
                    case "dropclothes":
                        if (pawn.apparel.WornApparel.Count < 1) return EffectStatus.Retry;
                        pawn.apparel.DropAll(pawn.Position);
                        SendCardNotification(currentMap, pawn.Position, "Clothing Dropped", $"{command.viewerName} made {pawn.Name} drop their clothes.", LetterDefOf.NegativeEvent, command.viewerName);
                        return EffectStatus.Success;
                    case "destroyclothes":
                        if (pawn.apparel.WornApparel.Count < 1) return EffectStatus.Retry;
                        pawn.apparel.DestroyAll();
                        SendCardNotification(currentMap, pawn.Position, "Clothing Destroyed", $"{command.viewerName} made {pawn.Name} destroy their clothes.", LetterDefOf.NegativeEvent, command.viewerName);
                        return EffectStatus.Success;

                    case "dropequip":
                        if (!pawn.equipment.HasAnything()) return EffectStatus.Retry;
                        SendCardNotification(currentMap, pawn.Position, "Equipment Dropped", $"{command.viewerName} made {pawn.Name} drop their equipment.", LetterDefOf.NegativeEvent, command.viewerName);
                        pawn.equipment.DropAllEquipment(pawn.Position);
                        return EffectStatus.Success;
                    case "destroyequip":
                        if (!pawn.equipment.HasAnything()) return EffectStatus.Retry;
                        pawn.equipment.DestroyAllEquipment();
                        SendCardNotification(currentMap, pawn.Position, "Equipment Destroyed", $"{command.viewerName} made {pawn.Name} destroy their equipment.", LetterDefOf.NegativeEvent, command.viewerName);
                        return EffectStatus.Success;

                    case "destroybed":
                        if (pawn.CurrentBed()==null) return EffectStatus.Retry;
                        pawn.CurrentBed().Destroy();
                        SendCardNotification(currentMap, pawn.Position, "Bed Destroyed", $"{command.viewerName} made {pawn.Name} destroy their bed.", LetterDefOf.NegativeEvent, command.viewerName);
                        return EffectStatus.Success;

                    case "injure":
                        Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Cut, pawn, null);
                        hediff.Severity = 1;
                        pawn.health.AddHediff(hediff, null, null, null);
                        SendCardNotification(currentMap, pawn.Position, "Colonist Injured", $"{command.viewerName} injured {pawn.Name}.", LetterDefOf.NegativeEvent, command.viewerName);
                        return EffectStatus.Success;
                    case "reminjure":
                        for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
                        {
                            if (pawn.health.hediffSet.hediffs[i] is Hediff_Injury)
                            {
                                pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs[i]);
                                SendCardNotification(currentMap, pawn.Position, "Colonist Injury Removed", $"{command.viewerName} removed an injury from {pawn.Name}.", LetterDefOf.PositiveEvent, command.viewerName);
                                return EffectStatus.Success;
                            }
                        }
                        return EffectStatus.Retry;
                    case "reset":
                        if(pawn.health.hediffSet.hediffs.Count < 1) return EffectStatus.Retry;
                        pawn.health.RemoveAllHediffs();
                        SendCardNotification(currentMap, pawn.Position, "Colonist Statuses Removed", $"{command.viewerName} removed statuses from {pawn.Name}.", LetterDefOf.PositiveEvent, command.viewerName);
                        return EffectStatus.Success;

                    case "kill":
                        if (pawn.health.ShouldBeDead()) return EffectStatus.Retry;
                        pawn.Kill(null);
                        SendCardNotification(currentMap, pawn.Position, "Colonist Killed", $"{command.viewerName} Killed {pawn.Name}.", LetterDefOf.NegativeEvent, command.viewerName);
                        return EffectStatus.Success;

                    case "skillup":
                        {
                            SkillDef target = null;
                            switch (items[2])
                            {
                                case "animals":
                                    target = SkillDefOf.Animals;
                                    break;
                                case "artistic":
                                    target = SkillDefOf.Artistic;
                                    break;
                                case "construction":
                                    target = SkillDefOf.Construction;
                                    break;
                                case "cooking":
                                    target = SkillDefOf.Cooking;
                                    break;
                                case "crafting":
                                    target = SkillDefOf.Crafting;
                                    break;
                                case "medical":
                                    target = SkillDefOf.Medicine;
                                    break;
                                case "melee":
                                    target = SkillDefOf.Melee;
                                    break;
                                case "mining":
                                    target = SkillDefOf.Mining;
                                    break;
                                case "intellectual":
                                    target = SkillDefOf.Intellectual;
                                    break;
                                case "plants":
                                    target = SkillDefOf.Plants;
                                    break;
                                case "shooting":
                                    target = SkillDefOf.Shooting;
                                    break;
                                case "social":
                                    target = SkillDefOf.Social;
                                    break;
                            }

                            SkillRecord skill = pawn.skills.GetSkill(target);

                            if (skill.levelInt >= 20 || skill.TotallyDisabled) return EffectStatus.Retry;
                            skill.Learn(skill.XpRequiredForLevelUp, true, true);

                            SendCardNotification(currentMap, pawn.Position, "Colonist Skill Up", $"{command.viewerName} increased {pawn.Name}'s {items[2]}.", LetterDefOf.PositiveEvent, command.viewerName);

                            return EffectStatus.Success;
                        }
                    case "skilldown":
                        {
                            SkillDef target = null;
                            switch (items[2])
                            {
                                case "animals":
                                    target = SkillDefOf.Animals;
                                    break;
                                case "artistic":
                                    target = SkillDefOf.Artistic;
                                    break;
                                case "construction":
                                    target = SkillDefOf.Construction;
                                    break;
                                case "cooking":
                                    target = SkillDefOf.Cooking;
                                    break;
                                case "crafting":
                                    target = SkillDefOf.Crafting;
                                    break;
                                case "medical":
                                    target = SkillDefOf.Medicine;
                                    break;
                                case "melee":
                                    target = SkillDefOf.Melee;
                                    break;
                                case "mining":
                                    target = SkillDefOf.Mining;
                                    break;
                                case "intellectual":
                                    target = SkillDefOf.Intellectual;
                                    break;
                                case "plants":
                                    target = SkillDefOf.Plants;
                                    break;
                                case "shooting":
                                    target = SkillDefOf.Shooting;
                                    break;
                                case "social":
                                    target = SkillDefOf.Social;
                                    break;
                            }

                            SkillRecord skill = pawn.skills.GetSkill(target);

                            if (skill.levelInt <= 0 || skill.TotallyDisabled) return EffectStatus.Retry;
                            skill.Learn(-1 * (skill.xpSinceLastLevel + 1), true, true);
                            skill.levelInt--;
                            skill.xpSinceLastLevel = 0;

                            SendCardNotification(currentMap, pawn.Position, "Colonist Skill Down", $"{command.viewerName} decreased {pawn.Name}'s {items[2]}.", LetterDefOf.NegativeEvent, command.viewerName);

                            return EffectStatus.Success;
                        }
                }


                return EffectStatus.Retry;
            }
            return EffectStatus.Failure;
        }
    }
}