﻿using System;

namespace CrowdControl {
    public class EffectCode {
        private EffectCode(string value) { Value = value; }

        public string Value { get; private set; }

        public static EffectCode Test { get { return new EffectCode("test"); } }
        public static EffectCode Colonist { get { return new EffectCode("colonist"); } }
        public static EffectCode GiveItem { get { return new EffectCode("giveitem"); } }
        public static EffectCode TakeItem { get { return new EffectCode("takeitem"); } }
        public static EffectCode SetWeather { get { return new EffectCode("setweather"); } }
        public static EffectCode AnimalSelfTame { get { return new EffectCode("animalselftame"); } }
        public static EffectCode InspireColony { get { return new EffectCode("inspirecolony"); } }
        public static EffectCode HarvestBounty { get { return new EffectCode("harvestbounty"); } }
        public static EffectCode HealingGrace { get { return new EffectCode("healinggrace"); } }
        public static EffectCode NewRecruit { get { return new EffectCode("newrecruit"); } }
        public static EffectCode RandomGift { get { return new EffectCode("randomgift"); } }
        public static EffectCode SuperGift { get { return new EffectCode("supergift"); } }
        public static EffectCode ResearchBreakthrough { get { return new EffectCode("researchbreakthrough"); } }
        public static EffectCode ResurrectColonist { get { return new EffectCode("resurrectcolonist"); } }
        public static EffectCode MoodBoost { get { return new EffectCode("moodboost"); } }
        public static EffectCode CreateHats { get { return new EffectCode("createhats"); } }

        public static EffectCode AnimalStampede { get { return new EffectCode("animalstampede"); } }
        public static EffectCode MeteoriteLanding { get { return new EffectCode("meteoritelanding"); } }
        public static EffectCode CatDogRain { get { return new EffectCode("catdograin"); } }
        public static EffectCode RandomQuest { get { return new EffectCode("randomquest"); } }
        public static EffectCode TradeCaravan { get { return new EffectCode("tradecaravan"); } }
        public static EffectCode LimbReplacement { get { return new EffectCode("limbreplacement"); } }
        public static EffectCode HunterBecomesHunted { get { return new EffectCode("hunterbecomeshunted"); } }

        public static EffectCode DestroyHats { get { return new EffectCode("destroyhats"); } }
        public static EffectCode Infestation { get { return new EffectCode("infestation"); } }
        public static EffectCode MentalBreak { get { return new EffectCode("mentalbreak"); } }
        public static EffectCode OrbitalBarrage { get { return new EffectCode("orbitalbarrage"); } }
        public static EffectCode Outbreak { get { return new EffectCode("outbreak"); } }
        public static EffectCode Tornado { get { return new EffectCode("tornado"); } }
        public static EffectCode Wildfire { get { return new EffectCode("wildfire"); } }
        public static EffectCode WildmanHorde { get { return new EffectCode("wildmanhorde"); } }
        public static EffectCode FoulFood { get { return new EffectCode("foulfood"); } }
        public static EffectCode PowerOutage { get { return new EffectCode("poweroutage"); } }

        public override string ToString() {
            return Value;
        }

        public static implicit operator String(EffectCode category) { 
            return category.Value; 
        }

    }
}
