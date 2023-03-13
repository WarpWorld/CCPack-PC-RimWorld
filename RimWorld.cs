using System;
using CrowdControl.Common;
using CrowdControl.Games.Packs;
using ConnectorType = CrowdControl.Common.ConnectorType;

public class RimWorld : SimpleTCPPack
{
    public override string Host => "0.0.0.0";
    public override ushort Port => 43384;

    public RimWorld(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new(160, "RimWorld", "RimWorld", "PC", ConnectorType.SimpleTCPConnector);

    public override EffectList Effects { get; } = new Effect[]
    {
        //new Effect("Positive Effects","positive", ItemKind.Folder),
        new Effect("Animal Self Tame", "animalselftame") { Category = "Positive" },
        new Effect("Colony Inspiration", "inspirecolony") { Category = "Positive" },
        new Effect("Harvest Bountry", "harvestbounty") { Category = "Positive", Duration = TimeSpan.FromSeconds(300) },
        new Effect("Healing Grace", "healinggrace") { Category = "Positive" },
        new Effect("New Recruit", "newrecruit") { Category = "Positive" },
        new Effect("Random Gift", "randomgift") { Category = "Positive" },
        new Effect("Super Gift", "supergift") { Category = "Positive" },
        new Effect("Research Breakthrough", "researchbreakthrough") { Category = "Positive" },
        new Effect("Resurrect Colonist", "resurrectcolonist") { Category = "Positive" },
        new Effect("Mood Boost", "moodboost") { Category = "Positive" },
        new Effect("Create Hats", "createhats") { Category = "Positive" },

        //new Effect("Neutral Effects","neutral", ItemKind.Folder),
        new Effect("Animal Stampede", "animalstampede") { Category = "Neutral" },
        new Effect("Meteorite Crash Landing", "meteoritelanding") { Category = "Neutral" },
        new Effect("Raining Cats and Dogs", "catdograin") { Category = "Neutral" },
        new Effect("Random Quest", "randomquest") { Category = "Neutral" },
        new Effect("Trade Caravan", "tradecaravan") { Category = "Neutral" },
        new Effect("Limb Replacement", "limbreplacement") { Category = "Neutral" },
        new Effect("Hunter Becomes the Hunted", "hunterbecomeshunted") { Category = "Neutral" },

        //new Effect("Negative Effects","negative", ItemKind.Folder),
        new Effect("Destroy Hats", "destroyhats") { Category = "Negative" },
        new Effect("Infestation", "infestation") { Category = "Negative" },
        new Effect("Mental Break", "mentalbreak") { Category = "Negative" },
        new Effect("Orbital Barrage", "orbitalbarrage") { Category = "Negative" },
        new Effect("Outbreak", "outbreak") { Category = "Negative" },
        new Effect("Tornado", "tornado") { Category = "Negative" },
        new Effect("Wildfire", "wildfire") { Category = "Negative" },
        new Effect("Wildman Horde", "wildmanhorde") { Category = "Negative" },
        new Effect("Foul Food", "foulfood") { Category = "Negative" },
        new Effect("Power Outage", "poweroutage") { Category = "Negative" }
    };
}
