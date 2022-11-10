using HugsLib.Settings;
using RimWorld;
using Verse;

namespace CrowdControl {

    public class TornadoEffect : Effect {
        public override string Code => EffectCode.Tornado;

        private SettingHandle<int> MinCount;
        private SettingHandle<int> MaxCount;

        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting(Settings);
            MinCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MinCount", title: "Settings.MinCount.Title".Translate(), description: "Settings.MinCount.Description".Translate(),
                defaultValue: 2);
            MaxCount = Settings.GetHandle<int>(
                settingName: $"Settings.{Code}.MaxCount", title: "Settings.MaxCount.Title".Translate(), description: "Settings.MaxCount.Description".Translate(),
                defaultValue: 4);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            int fireCount = ModService.Instance.Random.Next(MinCount, MaxCount);
            while (fireCount > 0) {
                IntVec3 spawnLocation = CellFinder.RandomNotEdgeCell(30, currentMap);
                bool wasSuccessful =  GenSpawn.Spawn(ThingDefOf.Tornado, spawnLocation, currentMap) != null;
                if (wasSuccessful) {
                    fireCount--;
                }
            }

            SendCardNotification(LetterDefOf.ThreatBig, triggeredBy: command.viewerName);
            return EffectStatus.Success;
        }
    }
}
