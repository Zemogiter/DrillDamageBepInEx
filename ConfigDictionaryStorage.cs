using System.Collections.Generic;

namespace DrillDamage
{
    public static class ConfigDictionaryStorage
    {
        public static Dictionary<TechType, int> ConfigDictionary = new()
        {
              { TechType.Salt, Plugin.Options.additionaldamagesalt },
              { TechType.Quartz, Plugin.Options.additionaldamagequartz },
              { TechType.Copper, Plugin.Options.additionaldamagecopper },
              { TechType.Titanium, Plugin.Options.additionaldamagetitanium },
              { TechType.Lead, Plugin.Options.additionaldamagelead },
              { TechType.Silver, Plugin.Options.additionaldamagesilver },
              { TechType.Diamond, Plugin.Options.additionaldamagediamond },
              { TechType.Gold, Plugin.Options.additionaldamagegold },
              { TechType.Magnetite, Plugin.Options.additionaldamagemagnetite },
              { TechType.Lithium, Plugin.Options.additionaldamagelithium },
              { TechType.MercuryOre, Plugin.Options.additionaldamagemercury }, //cut content, can be restored via other mods so it's included
              { TechType.UraniniteCrystal, Plugin.Options.additionaldamageuranium },
              { TechType.AluminumOxide, Plugin.Options.additionaldamagealuminiumoxide },
              { TechType.Nickel, Plugin.Options.additionaldamagenickiel },
              { TechType.Sulphur, Plugin.Options.additionaldamagesulphur },
              { TechType.Kyanite, Plugin.Options.additionaldamagekyanite },
              { TechType.PrecursorIonCrystal, Plugin.Options.additionaldamageion }
        };
    }
}
