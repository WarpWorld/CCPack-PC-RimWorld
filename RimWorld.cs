using ConnectorLib.SimpleTCP;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.RimWorld;

public class RimWorld : SimpleTCPPack<SimpleTCPServerConnector>
{
    public override string Host => "0.0.0.0";

    public override ushort Port => 43384;

    public override ISimpleTCPPack.MessageFormatType MessageFormat => ISimpleTCPPack.MessageFormatType.CrowdControlLegacy;

    public RimWorld(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new("RimWorld", "RimWorld", "PC", ConnectorType.SimpleTCPServerConnector);

    public override EffectList Effects { get; } = new Effect[]
    {
        new("test", "test"),

        new("Give 50 Simple Meals", "giveitem_MealSimple_50") { Category = "Give Items" },
        new("Give 50 Fine Meals", "giveitem_MealFine_50") { Category = "Give Items" },
        new("Give 50 Lavish Meals", "giveitem_MealLavish_50") { Category = "Give Items" },

        new("Give 50 Beer", "giveitem_Beer_50") { Category = "Give Items" },
        new("Give 100 Potatoes", "giveitem_RawPotatoes_100") { Category = "Give Items" },
        new("Give 100 Corn", "giveitem_RawCorn_100") { Category = "Give Items" },
        new("Give 100 Berries", "giveitem_RawBerries_100") { Category = "Give Items" },
        new("Give 100 Cow Meat", "giveitem_Meat-Cow_100") { Category = "Give Items" },

        new("Give 50 Logs", "giveitem_WoodLog_50") { Category = "Give Items" },
        new("Give 50 Steel", "giveitem_Steel_50") { Category = "Give Items" },
        new("Give 50 Plasteel", "giveitem_Plasteel_50") { Category = "Give Items" },
        new("Give 50 Gold", "giveitem_Gold_50") { Category = "Give Items" },
        new("Give 50 Cloth", "giveitem_Cloth_50") { Category = "Give Items" },
        new("Give 50 Leather", "giveitem_Leather-Plain_50") { Category = "Give Items" },
        new("Give 50 Granite", "giveitem_Granite_50") { Category = "Give Items" },
        new("Give 50 Wool", "giveitem_WoolSheep_50") { Category = "Give Items" },
        new("Give 25 Medicine", "giveitem_MedicineIndustrial_25") { Category = "Give Items" },

        new("Set Weather to Clear", "setweather_Clear") { Category = "Weather" },                
        new("Set Weather to Rain", "setweather_Rain") { Category = "Weather" },
        new("Set Weather to Fog", "setweather_Fog") { Category = "Weather" },
        new("Set Weather to Snow", "setweather_SnowGentle") { Category = "Weather" },
        new("Set Weather to Blizzard", "setweather_SnowHard") { Category = "Weather" },
        new("Set Weather to Thunderstorm", "setweather_RainyThunderstorm") { Category = "Weather" },        
      
        new("Give Bolt Rifle", "giveitem_Gun-BoltActionRifle_1") { Category = "Give Equipment" },
        new("Give Shotgun", "giveitem_Gun-PumpShotgun_1") { Category = "Give Equipment" },
        new("Give Minigun", "giveitem_Gun-Minigun_1") { Category = "Give Equipment" },
        new("Give Knife", "giveitem_MeleeWeapon-Club_1") { Category = "Give Equipment" },
        new("Give Club", "giveitem_MeleeWeapon-Knife_1") { Category = "Give Equipment" },
        new("Give Grenades", "giveitem_Weapon-GrenadeFrag_5") { Category = "Give Equipment" },
        new("Give Helmet", "giveitem_Apparel-SimpleHelmet_1") { Category = "Give Equipment" },
        new("Give Parka", "giveitem_Apparel-Parka_1") { Category = "Give Equipment" },
        new("Give Flak Vest", "giveitem_Apparel-FlakVest_1") { Category = "Give Equipment" },

        new("Foul Food", "foulfood") { Category = "Take Items" },
        new("Take Random Material", "takeitem_material") { Category = "Take Items" },
        new("Take Random Equipment", "takeitem_equipment") { Category = "Take Items" },

        new("Random Colonist Drop Clothes", "colonist_dropclothes") { Category = "Colonists"},
        new("Random Colonist Destroy Clothes", "colonist_destroyclothes") { Category = "Colonists"},
        new("Random Colonist Drop Equipment", "colonist_dropequip") { Category = "Colonists"},
        new("Random Colonist Destroy Equipment", "colonist_destroyequip") { Category = "Colonists"},
        new("Random Colonist Destroy Bed", "colonist_destroybed") { Category = "Colonists"},
        new("Injure Random Colonist", "colonist_injure") { Category = "Colonists"},
        new("Heal Random Colonist", "colonist_reminjure") { Category = "Colonists"},
        new("Remove Random Colonist Statuses", "colonist_reset") { Category = "Colonists"},
        new("Kill Random Colonist", "colonist_kill") { Category = "Colonists"},
        new("Resurrect Random Colonist", "resurrectcolonist") { Category = "Colonists" },

        new("Random Colonist Animals Up","colonist_skillup_animals") { Category = "Skills"},
        new("Random Colonist Artistic Up","colonist_skillup_artistic") { Category = "Skills"},
        new("Random Colonist Construction Up","colonist_skillup_construction") { Category = "Skills"},
        new("Random Colonist Cooking Up","colonist_skillup_cooking") { Category = "Skills"},
        new("Random Colonist Crafting Up","colonist_skillup_crafting") { Category = "Skills"},
        new("Random Colonist Medical Up","colonist_skillup_medical") { Category = "Skills"},
        new("Random Colonist Melee Up","colonist_skillup_melee") { Category = "Skills"},
        new("Random Colonist Mining Up","colonist_skillup_mining") { Category = "Skills"},
        new("Random Colonist Intellectual Up","colonist_skillup_intellectual") { Category = "Skills"},
        new("Random Colonist Plants Up","colonist_skillup_plants") { Category = "Skills"},
        new("Random Colonist Shooting Up","colonist_skillup_shooting") { Category = "Skills"},
        new("Random Colonist Social Up","colonist_skillup_social") { Category = "Skills"},

        new("Random Colonist Animals Down","colonist_skilldown_animals") { Category = "Skills"},
        new("Random Colonist Artistic Down","colonist_skilldown_artistic") { Category = "Skills"},
        new("Random Colonist Construction Down","colonist_skilldown_construction") { Category = "Skills"},
        new("Random Colonist Cooking Down","colonist_skilldown_cooking") { Category = "Skills"},
        new("Random Colonist Crafting Down","colonist_skilldown_crafting") { Category = "Skills"},
        new("Random Colonist Medical Down","colonist_skilldown_medical") { Category = "Skills"},
        new("Random Colonist Melee Down","colonist_skilldown_melee") { Category = "Skills"},
        new("Random Colonist Mining Down","colonist_skilldown_mining") { Category = "Skills"},
        new("Random Colonist Intellectual Down","colonist_skilldown_intellectual") { Category = "Skills"},
        new("Random Colonist Plants Down","colonist_skilldown_plants") { Category = "Skills"},
        new("Random Colonist Shooting Down","colonist_skilldown_shooting") { Category = "Skills"},
        new("Random Colonist Social Down","colonist_skilldown_social") { Category = "Skills"},


        //new Effect("Positive Effects","positive", ItemKind.Folder),
        new("Animal Self Tame", "animalselftame") { Category = "Positive" },
        new("Colony Inspiration", "inspirecolony") { Category = "Positive" },
        new("Harvest Bountry", "harvestbounty") { Category = "Positive", Duration = TimeSpan.FromSeconds(300) },
        new("Healing Grace", "healinggrace") { Category = "Positive" },
        new("New Recruit", "newrecruit") { Category = "Positive" },
        new("Random Gift", "randomgift") { Category = "Positive" },
        new("Super Gift", "supergift") { Category = "Positive" },
        new("Research Breakthrough", "researchbreakthrough") { Category = "Positive" },
        new("Mood Boost", "moodboost") { Category = "Positive" },
        new("Create Hats", "createhats") { Category = "Positive" },

        //new Effect("Neutral Effects","neutral", ItemKind.Folder),
        new("Animal Stampede", "animalstampede") { Category = "Neutral" },
        new("Meteorite Crash Landing", "meteoritelanding") { Category = "Neutral" },
        new("Raining Cats and Dogs", "catdograin") { Category = "Neutral" },
        new("Random Quest", "randomquest") { Category = "Neutral" },
        new("Trade Caravan", "tradecaravan") { Category = "Neutral" },
        new("Limb Replacement", "limbreplacement") { Category = "Neutral" },
        new("Hunter Becomes the Hunted", "hunterbecomeshunted") { Category = "Neutral" },

        //new Effect("Negative Effects","negative", ItemKind.Folder),
        new("Destroy Hats", "destroyhats") { Category = "Negative" },
        new("Infestation", "infestation") { Category = "Negative" },
        new("Mental Break", "mentalbreak") { Category = "Negative" },
        new("Orbital Barrage", "orbitalbarrage") { Category = "Negative" },
        new("Outbreak", "outbreak") { Category = "Negative" },
        new("Tornado", "tornado") { Category = "Negative" },
        new("Wildfire", "wildfire") { Category = "Negative" },
        new("Wildman Horde", "wildmanhorde") { Category = "Negative" },
        new("Power Outage", "poweroutage") { Category = "Negative" }
    };
}