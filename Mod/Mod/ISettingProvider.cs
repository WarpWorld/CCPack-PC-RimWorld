using HugsLib.Settings;

namespace CrowdControl {
    public interface ISettingProvider {
        void RegisterSettings(ModSettingsPack Settings);
    }
}