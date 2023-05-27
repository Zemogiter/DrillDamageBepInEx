using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using UnityEngine;
using BepInEx.Logging;

namespace DrillDamage
{
    [HarmonyPatch(typeof(Drillable), nameof(Drillable.OnDrill))]
    internal class Drillable_Transpiler_Patch
    {
        
        [HarmonyTranspiler]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins)
        {
            var errorLogSource = new ManualLogSource("DrillDamage - Error");
            BepInEx.Logging.Logger.Sources.Add(errorLogSource);
            var matcher = new CodeMatcher(cins);

            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldc_R4, 5f));
            if (matcher.IsValid)
            {
                matcher.SetAndAdvance(OpCodes.Ldarg_0, null);
                matcher.Insert(new CodeInstruction(OpCodes.Call, typeof(Drillable_Transpiler_Patch).GetMethod(nameof(GetDamageForDrillable))));
                return matcher.InstructionEnumeration();
            }
            else
            {
                errorLogSource.LogError($"Failed to find matching instructions for {nameof(Drillable_Transpiler_Patch)}. Either another mod has already transpiled this or the games code has changed.");
            }
            return cins;
        }

        public static int GetDamageForDrillable(Drillable drillable)
        {
            var exosuitLogSource = new ManualLogSource("DrillDamage - Seamoth");
            BepInEx.Logging.Logger.Sources.Add(exosuitLogSource);
            int newDamage;
            //regular
            if (Plugin.Options.variablemode == false && Plugin.Options.additionaldamage > 1)
            {
                newDamage = Plugin.Options.additionaldamage;
            }
            //vanilla
            else if (Plugin.Options.variablemode == false && Plugin.Options.additionaldamage == 1)
            {
                newDamage = 1;
            }
            //variable mode enabled
            else
            {
                TechType key = drillable.GetDominantResourceType();
                if (Plugin.Options.variablemode == true)
                {
                    exosuitLogSource.LogInfo("The techType is = " + key);
                }
                var valueGet = ConfigDictionaryStorage.ConfigDictionary.TryGetValue(key, out int value);
                if (Plugin.Options.debugmode == true)
                {
                    exosuitLogSource.LogInfo("Was the value obtained? " + valueGet + " Value is = " + value);
                }
                newDamage = value;
            }
            return newDamage;
        }

        /*public static int GetDamageForDrillable(Drillable drillable)
        {
            var exosuitLogSource = new ManualLogSource("DrillDamage - Seamoth");
            BepInEx.Logging.Logger.Sources.Add(exosuitLogSource);
            int newDamage;
            //regular
            if (Plugin.ConfigVariableModeEnabled.Value == false && Plugin.ConfigAdditionalDamage.Value > 0)
            {
                newDamage = Plugin.ConfigAdditionalDamage.Value;
            }
            //vanilla
            else if (Plugin.ConfigVariableModeEnabled.Value == false && Plugin.ConfigAdditionalDamage.Value == 0)
            {
                newDamage = 1;
            }
            //variable mode enabled
            else
            {
                TechType key = drillable.GetDominantResourceType();
                if (Plugin.ConfigDebugMode.Value == true)
                {
                    exosuitLogSource.LogInfo("The techType is = " + key);
                }
                var valueGet = ConfigDictionaryStorage.ConfigDictionary.TryGetValue(key, out int value);
                if (Plugin.ConfigDebugMode.Value == true)
                {
                    exosuitLogSource.LogInfo("Was the value obtained? " + valueGet + " Value is = " + value);
                }
                newDamage = value;
            }
            return newDamage;
        }*/
    }
}
