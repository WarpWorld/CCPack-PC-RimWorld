using RimWorld;
using Verse;
using HugsLib.Settings;
using System.Collections.Generic;

namespace CrowdControl {

    public class WildfireEffect : Effect {
        public override string Code => EffectCode.Wildfire;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;
        private SettingHandle<float> Size;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting(Settings);
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 3);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 6);
            Size = Settings.GetHandle<float>(
                settingName: $"Settings.{Code}.Size", title: "Settings.Size.Title".Translate(), description: "Settings.Size.Description".Translate(),
                defaultValue: 10);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            int attemptCount = 0;
            int fireCount = ModService.Instance.Random.Next(MinCount, MaxCount) * (int) Size;
            List<TargetInfo> fireLocations = new List<TargetInfo>(fireCount);

            while (fireCount > 0 && attemptCount < 50) {
                IntVec3 nucleationPoint = CellFinder.RandomNotEdgeCell(30, currentMap);

                if (nucleationPoint.IsValid) {
                    for (int i = 0; i < Size; i++) {
                        IntVec3 fireLocation;
                        CellFinder.TryFindRandomCellNear(nucleationPoint, currentMap, (int)Size, vec3 => vec3.Standable(currentMap) && vec3.Roofed(currentMap) == false, out fireLocation);

                        if (fireLocation.IsValid) {
                            Fire fire = (Fire)ThingMaker.MakeThing(ThingDefOf.Fire, null);
                            fire.fireSize = Size;
                            GenSpawn.Spawn(fire, fireLocation, currentMap, Rot4.North, WipeMode.Vanish, false);
                            fireLocations.Add(new TargetInfo(fireLocation, currentMap));
                            fireCount--;
                        }
                    }
                }
                attemptCount++;
            }

            if (fireCount == 0) {
                SendCardNotification(lookAtThings: fireLocations, notificationType: LetterDefOf.ThreatSmall, triggeredBy: command.viewerName);
                return EffectStatus.Success;
            }
            return EffectStatus.Failure;
        }
    }
}
