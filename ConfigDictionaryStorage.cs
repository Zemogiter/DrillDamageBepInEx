using System.Collections.Generic;

namespace DrillDamage
{
    public static class ConfigDictionaryStorage
    {
        //Old Dictionary, using BepinEx config
        /*public static Dictionary<TechType, int> ConfigDictionary = new()
        {
              { TechType.Salt, Plugin.ConfigDrillableSaltDamage.Value },
              { TechType.Quartz, Plugin.ConfigDrillableQuartzDamage.Value },
              { TechType.Copper, Plugin.ConfigDrillableCopperDamage.Value },
              { TechType.Titanium, Plugin.ConfigDrillableTitaniumDamage.Value },
              { TechType.Lead, Plugin.ConfigDrillableLeadDamage.Value },
              { TechType.Silver, Plugin.ConfigDrillableSilverDamage.Value },
              { TechType.Diamond, Plugin.ConfigDrillableDiamondDamage.Value },
              { TechType.Gold, Plugin.ConfigDrillableGoldDamage.Value },
              { TechType.Magnetite, Plugin.ConfigDrillableMagnetiteDamage.Value },
              { TechType.Lithium, Plugin.ConfigDrillableLithiumDamage.Value },
              { TechType.MercuryOre, Plugin.ConfigDrillableMercuryDamage.Value }, //cut content, can be restored via other mods so it's included
              { TechType.UraniniteCrystal, Plugin.ConfigDrillableUraniumDamage.Value },
              { TechType.AluminumOxide, Plugin.ConfigDrillableAluminiumOxideDamage.Value },
              { TechType.Nickel, Plugin.ConfigDrillableNickelDamage.Value },
              { TechType.Sulphur, Plugin.ConfigDrillableSulphurDamage.Value },
              { TechType.Kyanite, Plugin.ConfigDrillableKyaniteDamage.Value }
        };*/
        //Curret Dictionary implementation, using Nautilus-powered in-game config
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
              { TechType.Kyanite, Plugin.Options.additionaldamagekyanite }
        };
    }
}
