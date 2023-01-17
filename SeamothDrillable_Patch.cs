using DrillDamage;
using HarmonyLib;
using static VFXParticlesPool;
using System.Collections.Generic;
using System.Reflection;

[HarmonyPatch(typeof(Drillable))]
[HarmonyPatch("Start")]
internal class SeamothDrillable_Patch
{
    [HarmonyPostfix]
    public void Patch(Drillable __instance)
    {
        var seamothDrillable = __instance.gameObject.GetComponent("SeamothDrillable");

        if (seamothDrillable != null) // if SeamothDrillable component is available on resource, processing the drill damage value else do nothing
        {
            PropertyInfo drillDamage = seamothDrillable.GetType().GetProperty("drillDamage");

            if (Plugin.ConfigAffectSeamothArms.Value == true && Plugin.ConfigVariableModeEnabled.Value == false)
            {
                drillDamage.SetValue(seamothDrillable, Plugin.ConfigAdditionalDamage);
            }
            else if (Plugin.ConfigAffectSeamothArms.Value == true && Plugin.ConfigVariableModeEnabled.Value == true)
            {
                Dictionary<TechType, int> ConfigDictionary = new()
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
                TechType key = __instance.GetDominantResourceType();
                Plugin.Log("The techType is = " + key, 2);
                var valueGet = ConfigDictionary.TryGetValue(key, out int value);
                Plugin.Log("Value is = " + value, 2);
                drillDamage.SetValue(seamothDrillable, value);
            }
        }
    }
}