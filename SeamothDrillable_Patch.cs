﻿using HarmonyLib;
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

                //regular
                if (Plugin.Options.affectsseamotharms == true && Plugin.Options.variablemode == false)
                {
                    drillDamage.SetValue(seamothDrillable, Plugin.Options.additionaldamageseamotharms);
                }
                //vanilla
                else if (Plugin.Options.affectsseamotharms == true && Plugin.Options.variablemode == false && Plugin.Options.additionaldamageseamotharms == 1)
                {
                    drillDamage.SetValue(seamothDrillable, 1);
                }
                //variable mode enabled
                else if (Plugin.Options.affectsseamotharms == true && Plugin.Options.variablemode == true) 
                {
                    TechType key = __instance.GetDominantResourceType();
                    if (Plugin.Options.debugmode == true)
                    {
                        seamothLogSource.LogInfo("The techType is = " + key);
                    }
                    bool valueGet = ConfigDictionaryStorage.ConfigDictionary.TryGetValue(key, out int value);
                    if (Plugin.Options.debugmode == true)
                    {
                        seamothLogSource.LogInfo("Was the value obtained? " + valueGet + " Value is = " + value);
                    }
                    drillDamage.SetValue(seamothDrillable, value);
                }
            }
        }
    }
}
