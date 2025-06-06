﻿using RimWorld;
using Verse;
using HugsLib.Settings;
using System;

namespace CrowdControl {

    public class HarvestBountyEffect : TimedEffect {
        public override string Code => EffectCode.HarvestBounty;

        private float defaultCropYieldFactor = 1;

        private int Duration;
        private SettingHandle<float> Factor;
        public override void RegisterSettings(ModSettingsPack Settings) {
            this.RegisterBaseSetting(Settings);
            Duration = 300;
            Factor = Settings.GetHandle<float>(
                settingName: $"Settings.{Code}.Size", title: "Settings.Factor.Title".Translate(), description: "Settings.Factor.Description".Translate(),
                defaultValue: 3);
        }

        public override EffectStatus Execute(EffectCommand command) {
            Map currentMap;
            if (command.duration > 0) Duration = command.duration / 1000;


            bool hasMap = ModService.Instance.TryGetColonyMap(out currentMap);
            if (hasMap == false)
                return EffectStatus.Failure;

            if (IsActive == false) {
                StartEffect();
                SendCardNotification(LetterDefOf.PositiveEvent, command.viewerName);
                return EffectStatus.Success;
            }
            return EffectStatus.Failure;
        }

        public override void Tick() {
            double elapsed = (DateTime.Now - startTime).TotalSeconds;
            if (IsActive && elapsed >= Duration) {
                FinishEffect();
            }
        }

        private void StartEffect() {
            defaultCropYieldFactor = Find.Storyteller.difficulty.cropYieldFactor;
            startTime = DateTime.Now;
            Find.Storyteller.difficulty.cropYieldFactor = Factor;
            IsActive = true;
            ModService.Instance.EffectManager.AddTimedEffect(this);
        }

        private void FinishEffect() {
            Find.Storyteller.difficulty.cropYieldFactor = defaultCropYieldFactor;
            IsActive = false;
            string customLabel = string.Format("Notification.Terminated".Translate(), $"{Code}.Title".Translate());
            SendCardNotification(label: customLabel, description: "", notificationType: LetterDefOf.NeutralEvent);

        }
    }
}