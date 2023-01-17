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
              { TechType.DrillableSalt, Plugin.ConfigDrillableSaltDamage.Value },
              { TechType.DrillableQuartz, Plugin.ConfigDrillableQuartzDamage.Value },
              { TechType.DrillableCopper, Plugin.ConfigDrillableCopperDamage.Value },
              { TechType.DrillableTitanium, Plugin.ConfigDrillableTitaniumDamage.Value },
              { TechType.DrillableLead, Plugin.ConfigDrillableLeadDamage.Value },
              { TechType.DrillableSilver, Plugin.ConfigDrillableSilverDamage.Value },
              { TechType.DrillableDiamond, Plugin.ConfigDrillableDiamondDamage.Value },
              { TechType.DrillableGold, Plugin.ConfigDrillableGoldDamage.Value },
              { TechType.DrillableMagnetite, Plugin.ConfigDrillableMagnetiteDamage.Value },
              { TechType.DrillableLithium, Plugin.ConfigDrillableLithiumDamage.Value },
              { TechType.DrillableMercury, Plugin.ConfigDrillableMercuryDamage.Value },
              { TechType.DrillableUranium, Plugin.ConfigDrillableUraniumDamage.Value },
              { TechType.DrillableAluminiumOxide, Plugin.ConfigDrillableAluminiumOxideDamage.Value },
              { TechType.DrillableNickel, Plugin.ConfigDrillableNickelDamage.Value },
              { TechType.DrillableSulphur, Plugin.ConfigDrillableSulphurDamage.Value },
              { TechType.DrillableKyanite, Plugin.ConfigDrillableKyaniteDamage.Value }
        };
    }
}
