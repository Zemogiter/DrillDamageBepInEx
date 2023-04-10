using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillDamage
{
    public static class ConfigDictionaryStorage
    {
        public static Dictionary<TechType, int> ConfigDictionary = new()
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
              { TechType.MercuryOre, Plugin.ConfigDrillableMercuryDamage.Value },
              { TechType.UraniniteCrystal, Plugin.ConfigDrillableUraniumDamage.Value },
              { TechType.AluminumOxide, Plugin.ConfigDrillableAluminiumOxideDamage.Value },
              { TechType.Nickel, Plugin.ConfigDrillableNickelDamage.Value },
              { TechType.Sulphur, Plugin.ConfigDrillableSulphurDamage.Value },
              { TechType.Kyanite, Plugin.ConfigDrillableKyaniteDamage.Value }
        };
    }
}
