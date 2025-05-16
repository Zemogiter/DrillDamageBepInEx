using HarmonyLib;
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

            if (seamothDrillable != null) // if SeamothDrillable component is available on resource, processing the drill damage value, else do nothing
            {
                PropertyInfo drillDamage = seamothDrillable.GetType().GetProperty("drillDamage");

                //regular
                if (Config.Instance.affectsseamotharms == true && Config.Instance.variablemode == false)
                {
                    drillDamage.SetValue(seamothDrillable, Config.Instance.additionaldamageseamotharms);
                }
                //vanilla
                else if (Config.Instance.affectsseamotharms == true && Config.Instance.variablemode == false && Config.Instance.additionaldamageseamotharms == 1)
                {
                    drillDamage.SetValue(seamothDrillable, 1);
                }
                //variable mode enabled
                else if (Config.Instance.affectsseamotharms == true && Config.Instance.variablemode == true) 
                {
                    TechType key = __instance.GetDominantResourceType();
                    if (Config.Instance.debugmode == true)
                    {
                        seamothLogSource.LogInfo("The techType is = " + key);
                    }
                    var valueGet = Config.Instance.drillableOreList.TryGetValue(key.AsString(), out int value);
                    if (Config.Instance.debugmode == true)
                    {
                        seamothLogSource.LogInfo("Was the value obtained? " + valueGet + ". If so, the value is = " + value);
                    }
                    drillDamage.SetValue(seamothDrillable, value);
                }
            }
        }
    }
}
