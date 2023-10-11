using System;
using CrowdControl.Common;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.RimWorld;

public class RimWorld : SimpleTCPPack
{
    public override string Host => "0.0.0.0";

    public override ushort Port => 43384;

    public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

    public RimWorld(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new(160, "RimWorld", "RimWorld", "PC", ConnectorType.SimpleTCPConnector);

    public override EffectList Effects { get; } = new Effect[]
    {
        //new Effect("Positive Effects","positive", ItemKind.Folder),
        new("Animal Self Tame", "animalselftame") { Category = "Positive" },
        new("Colony Inspiration", "inspirecolony") { Category = "Positive" },
        new("Harvest Bountry", "harvestbounty") { Category = "Positive", Duration = TimeSpan.FromSeconds(300) },
        new("Healing Grace", "healinggrace") { Category = "Positive" },
        new("New Recruit", "newrecruit") { Category = "Positive" },
        new("Random Gift", "randomgift") { Category = "Positive" },
        new("Super Gift", "supergift") { Category = "Positive" },
        new("Research Breakthrough", "researchbreakthrough") { Category = "Positive" },
        new("Resurrect Colonist", "resurrectcolonist") { Category = "Positive" },
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
        new("Foul Food", "foulfood") { Category = "Negative" },
        new("Power Outage", "poweroutage") { Category = "Negative" }
    };
}