using HugsLib;
using HugsLib.Settings;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace CrowdControl {
    public class RimWorldTV : ModBase {
        public override string ModIdentifier => "RimWorldCrowdControl";

        private ModService ModService;
        private const int CONNECTION_TICK_INTERVAL = 600;

        public RimWorldTV() {
            ModService = ModService.Instance;
            ModService.Logger = this.Logger;
        }

        public override void DefsLoaded() {
            RegisterModSettings();
        }

        public override void WorldLoaded() {
            EnsureConnection();
        }

        public override void MapLoaded(Map map) {
            EnsureConnection();
        }

        public override void Tick(int currentTick) {
            if (currentTick % CONNECTION_TICK_INTERVAL == 0) {
                EnsureConnection();
            }
        }

        public void RegisterModSettings() {
            ModService.RegisterSettings(Settings);

            // This is a little hacky, basically we make a copy of all setting handles prior to adding effect settings. 
            // Then use that list to determine what effects should be hidden until "Show Advanced Settings" is checked. 
            List<SettingHandle> systemSettings = new List<SettingHandle>(Settings.Handles);
            foreach (KeyValuePair<string, Effect> entry in ModService.EffectList) {
                Effect effect = entry.Value;
                effect.RegisterSettings(Settings);
            }
            Settings.Handles.ToList().ForEach(handle => { 
                if (systemSettings.Contains(handle) == false) {
                    handle.VisibilityPredicate = () => { return ModService.ShowAdvanced; };
                }
            });
        }

        private void EnsureConnection() {
            EffectManager effectManager = ModService.EffectManager;
            if (effectManager == null && Current.Game != null) {
                effectManager = Current.Game.GetComponent<EffectManager>();
            }

            if (effectManager != null) {
                effectManager.EnsureConnection();
            }
        }
    }
}
