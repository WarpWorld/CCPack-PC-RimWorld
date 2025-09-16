using System.Diagnostics.CodeAnalysis;
using ConnectorLib.SimpleTCP;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.RimWorld;

public class RimWorld : SimpleTCPPack<SimpleTCPServerConnector>
{
    public override string Host => "0.0.0.0";

    public override ushort Port => 43384;

    [SuppressMessage("PackMetadata", "CC1007:Message Format Property")]
    public override ISimpleTCPPack.MessageFormatType MessageFormat => ISimpleTCPPack.MessageFormatType.CrowdControlLegacy;

    public RimWorld(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new("RimWorld", "RimWorld", "PC", ConnectorType.SimpleTCPServerConnector);

    public override EffectList Effects { get; } = new Effect[]
    {
        //new("test", "test"),

        new("Give 50 Simple Meals", "giveitem_MealSimple_50") { Image = "givemeal", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 50 Fine Meals", "giveitem_MealFine_50") { Image = "givemeal", Category = "Give Items" , Description = "", Price = 50 },
        new("Give 50 Lavish Meals", "giveitem_MealLavish_50") { Image = "givemeal", Category = "Give Items" , Description = "", Price = 100 },

        new("Give 50 Beer", "giveitem_Beer_50") { Image = "givefood", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 100 Potatoes", "giveitem_RawPotatoes_100") { Image = "givefood", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 100 Corn", "giveitem_RawCorn_100") { Image = "givefood", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 100 Berries", "giveitem_RawBerries_100") { Image = "givefood", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 100 Cow Meat", "giveitem_Meat-Cow_100") { Image = "givefood", Category = "Give Items" , Description = "", Price = 25 },

        new("Give 50 Logs", "giveitem_WoodLog_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 15 },
        new("Give 50 Steel", "giveitem_Steel_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 15 },
        new("Give 50 Plasteel", "giveitem_Plasteel_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 50 Gold", "giveitem_Gold_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 50 Cloth", "giveitem_Cloth_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 15 },
        new("Give 50 Leather", "giveitem_Leather-Plain_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 15 },
        new("Give 50 Granite", "giveitem_Granite_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 15 },
        new("Give 50 Wool", "giveitem_WoolSheep_50") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 25 },
        new("Give 25 Medicine", "giveitem_MedicineIndustrial_25") { Image = "giveitem", Category = "Give Items" , Description = "", Price = 50 },

        new("Clear", "setweather_Clear") { Note = "Weather",Category = "Weather" , Description = "Change the weather!", Price = 100 },
        new("Rain", "setweather_Rain") { Category = "Weather" , Description = "Change the weather!", Price = 100 },
        new("Fog", "setweather_Fog") { Category = "Weather" , Description = "Change the weather!", Price = 100 },
        new("Snow", "setweather_SnowGentle") { Category = "Weather" , Description = "Change the weather!", Price = 100 },
        new("Blizzard", "setweather_SnowHard") { Category = "Weather" , Description = "Change the weather!", Price = 100 },
        new("Thunderstorm", "setweather_RainyThunderstorm") { Category = "Weather" , Description = "Change the weather!", Price = 100 },

        new("Give Bolt Rifle", "giveitem_Gun-BoltActionRifle_1") { Image = "giveweapon", Category = "Give Equipment" , Description = "Give the player a weapon!", Price = 15 },
        new("Give Shotgun", "giveitem_Gun-PumpShotgun_1") { Image = "giveweapon", Category = "Give Equipment" , Description = "Give the player a weapon!", Price = 25 },
        new("Give Minigun", "giveitem_Gun-Minigun_1") { Image = "giveweapon", Category = "Give Equipment" , Description = "Give the player a weapon!", Price = 50 },
        new("Give Knife", "giveitem_MeleeWeapon-Club_1") { Image = "giveweapon", Category = "Give Equipment" , Description = "Give the player a weapon!", Price = 15 },
        new("Give Club", "giveitem_MeleeWeapon-Knife_1") { Image = "giveweapon", Category = "Give Equipment" , Description = "Give the player a weapon!", Price = 10 },
        new("Give Grenades", "giveitem_Weapon-GrenadeFrag_5") { Image = "giveweapon", Category = "Give Equipment" , Description = "Give the player a weapon!", Price = 50 },
        new("Give Helmet", "giveitem_Apparel-SimpleHelmet_1") { Image = "givearmor", Category = "Give Equipment" , Description = "Give the player a helmet!", Price = 15 },
        new("Give Parka", "giveitem_Apparel-Parka_1") { Image = "givearmor", Category = "Give Equipment" , Description = "Give the player a parka!", Price = 15 },
        new("Give Flak Vest", "giveitem_Apparel-FlakVest_1") { Image = "givearmor", Category = "Give Equipment" , Description = "Give the player a vest!", Price = 15 },

        new("Foul Food", "foulfood") { Category = "Take Items" , Description = "A randomly selected food stack within the colony Stockpile Zone is destroyed!", Price = 25 },
        new("Take Random Material", "takeitem_material") { Category = "Take Items" , Description = "", Price = 50 },
        new("Take Random Equipment", "takeitem_equipment") { Category = "Take Items" , Description = "", Price = 50 },

        new("Drop Clothes", "colonist_dropclothes") { Category = "Colonists", Description = "Make a random colonist drop their clothes!", Price = 25 },
        new("Destroy Clothes", "colonist_destroyclothes") { Category = "Colonists", Description = "Destroy a random colonist clothes!", Price = 50 },
        new("Drop Equipment", "colonist_dropequip") { Category = "Colonists", Description = "Make a random colonist drop their equipment!", Price = 25 },
        new("Destroy Equipment", "colonist_destroyequip") { Category = "Colonists", Description = "Destroy a random colonist equipment!", Price = 50 },
        new("Destroy Bed", "colonist_destroybed") { Category = "Colonists", Description = "Destroy a random colonist bed!", Price = 50 },
        new("Injure", "colonist_injure") { Category = "Colonists", Description = "Cause an injury on a random colonist.", Price = 15 },
        new("Heal", "colonist_reminjure") { Category = "Colonists", Description = "Heal a random colonist!", Price = 15 },
        new("Remove Statuses", "colonist_reset") { Category = "Colonists", Description = "Remove a random colonist status effects!", Price = 15 },
        new("Kill", "colonist_kill") { Category = "Colonists", Description = "Murder a random colonist!", Price = 300 },
        new("Resurrect", "resurrectcolonist") { Category = "Colonists" , Description = "A random dead colonist is resurrected.", Price = 100 },

        new("Animals Up","colonist_skillup_animals") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15 },
        new("Artistic Up","colonist_skillup_artistic") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Construction Up","colonist_skillup_construction") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15 },
        new("Cooking Up","colonist_skillup_cooking") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Crafting Up","colonist_skillup_crafting") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Medical Up","colonist_skillup_medical") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Melee Up","colonist_skillup_melee") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Mining Up","colonist_skillup_mining") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Intellectual Up","colonist_skillup_intellectual") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Plants Up","colonist_skillup_plants") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Shooting Up","colonist_skillup_shooting") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },
        new("Social Up","colonist_skillup_social") { Image = "skillup", Category = "Skills", Description = "Increase a random colonist skill by 1!", Price = 15  },

        new("Animals Down","colonist_skilldown_animals") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15  },
        new("Artistic Down","colonist_skilldown_artistic") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Construction Down","colonist_skilldown_construction") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Cooking Down","colonist_skilldown_cooking") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Crafting Down","colonist_skilldown_crafting") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Medical Down","colonist_skilldown_medical") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Melee Down","colonist_skilldown_melee") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Mining Down","colonist_skilldown_mining") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Intellectual Down","colonist_skilldown_intellectual") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Plants Down","colonist_skilldown_plants") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Shooting Down","colonist_skilldown_shooting") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },
        new("Social Down","colonist_skilldown_social") { Image = "skilldown", Category = "Skills", Description = "Decrease a random colonist skill by 1!", Price = 15 },


        //new Effect("Positive Effects","positive", ItemKind.Folder),
        new("Animal Self Tame", "animalselftame") { Description = "A random tamable pawn within the current map will self-tame.", Price = 10 },
        new("Colony Inspiration", "inspirecolony") { Category = "Colonists" , Description = "Each eligible colonist will receive a random inspiration.", Price = 50 },
        new("Harvest Bountry", "harvestbounty") { Duration = TimeSpan.FromSeconds(300) , Description = "Crops harvested during this time yield additional products.", Price = 25 },
        new("Healing Grace", "healinggrace") { Category = "Colonists" , Description = "Removes any negative health effect for each colonist.", Price = 150 },
        new("New Recruit", "newrecruit") { Category = "Colonists" , Description = "A new recruit joins the colony. NOTE: If ShouldDisplayViewerName is set within the options, the new pawn will have the triggering viewer's name.", Price = 100 },
        new("Random Gift", "randomgift") { Image = "giveitem", Category = "Give Items" , Description = "Numerous randomly selected items are delivered somewhere in the colony via drop-ship.", Price = 50 },
        new("Super Gift", "supergift") { Image = "giveitem", Category = "Give Items" , Description = "Send the colonist an extra powerful gift!", Price = 500 },
        new("Research Breakthrough", "researchbreakthrough") { Description = "A random eligible research item is completed.", Price = 50 },
        new("Mood Boost", "moodboost") { Category = "Colonists" , Description = "Lifts the spirits of a random colonist increasing their sanity to full!", Price = 50 },
        new("Create Hats", "createhats") { Image = "giveweapon", Category = "Give Equipment" , Description = "Drop off 2-3 random hats for all colonists!", Price = 100 },

        //new Effect("Neutral Effects","neutral", ItemKind.Folder),
        new("Animal Stampede", "animalstampede") { Category = "Event" , Description = "A random herd of animals wanders through the colony.", Price = 50 },
        new("Meteorite Crash Landing", "meteoritelanding") { Category = "Event" , Description = "A meteorite containing various minerals crashes into the colony.", Price = 100 },
        new("Raining Cats and Dogs", "catdograin") { Category = "Event" , Description = "Cats and dogs rain from the sky.", Price = 15 },
        new("Random Quest", "randomquest") { Description = "A randomly selected quest is offered to the player.", Price = 50 },
        new("Trade Caravan", "tradecaravan") { Category = "Event" , Description = "A trade caravan visits the colony.", Price = 50 },
        new("Limb Replacement", "limbreplacement") { Category = "Colonists" , Description = "Replace a limb with either a bionic limb (Positive) or a wooden downgrade like a peg leg, or a hook (Negative)", Price = 50 },
        new("Hunter Becomes the Hunted", "hunterbecomeshunted") { Category = "Event" , Description = "Activates an event where a random wild animal is hunting for your pets!", Price = 150 },

        //new Effect("Negative Effects","negative", ItemKind.Folder),
        new("Destroy Hats", "destroyhats") { Category = "Colonists" , Description = "All currently equipped hats are destroyed.", Price = 250 },
        new("Infestation", "infestation") { Category = "Event" , Description = "An alien infestation spreads nearby to the colony.", Price = 350 },
        new("Mental Break", "mentalbreak") { Category = "Colonists" , Description = "Your strongest colonists receive a negative mood effect.", Price = 50 },
        new("Orbital Barrage", "orbitalbarrage") { Category = "Event" , Description = "A loitering enemy ship attacks the colony with an orbital barrage.", Price = 500 },
        new("Outbreak", "outbreak") { Category = "Colonists" , Description = "Several colonists receive an infectious disease.", Price = 250 },
        new("Tornado", "tornado") { Category = "Event" , Description = "A tornado appears somewhere within the colony.", Price = 500 },
        new("Wildfire", "wildfire") { Category = "Event" , Description = "Several wildfires break out within the colony.", Price = 150 },
        new("Wildman Horde", "wildmanhorde") { Category = "Event" , Description = "A group of Wildman raid the colony.", Price = 150 },
        new("Power Outage", "poweroutage") { Category = "Event", Description = "Electricity is lame, return to our tribal roots! Cause a power outage for a minute or two.", Price = 250 },
    };
}