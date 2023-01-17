using HarmonyLib;
using static VFXParticlesPool;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;

namespace DrillDamage
{
    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("Start")]
    internal class SeamothDrillable_Patch
    {
        [HarmonyPostfix]
        public static void Patch(Drillable __instance)
        {
            var seamothLogSource = new ManualLogSource("DrillDamage - Seamoth");
            BepInEx.Logging.Logger.Sources.Add(seamothLogSource);
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

                    TechType key = __instance.GetDominantResourceType();
                    seamothLogSource.LogInfo("The techType is = " + key);
                    var valueGet = ConfigDictionaryStorage.ConfigDictionary.TryGetValue(key, out int value);
                    seamothLogSource.LogInfo("Value is = " + value);
                    drillDamage.SetValue(seamothDrillable, value);
                }
            }
        }
    }
}
