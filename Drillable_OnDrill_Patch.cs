using BepInEx.Configuration;
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

            matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 5f));
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

        public static float GetDamageForDrillable(Drillable drillable)
        {
            var exosuitLogSource = new ManualLogSource("DrillDamage - Seamoth");
            BepInEx.Logging.Logger.Sources.Add(exosuitLogSource);
            float newDamage;
            if (Plugin.ConfigVariableModeEnabled.Value == false)
            {
                newDamage = Plugin.ConfigAdditionalDamage.Value;
            }
            else
            {
                TechType key = drillable.GetDominantResourceType();
                exosuitLogSource.LogInfo("The techType is = " + key);
                var valueGet = ConfigDictionaryStorage.ConfigDictionary.TryGetValue(key, out int value);
                exosuitLogSource.LogInfo("Value is = " + value);
                newDamage = value;
            }
            return newDamage;
        }
    }
}
