using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using UnityEngine;
using BepInEx.Logging;
using static RootMotion.BipedNaming;
using System.Reflection;

namespace DrillDamage
{
    [HarmonyPatch(typeof(Drillable), nameof(Drillable.OnDrill))]
    public class Drillable_Transpiler_Patch
    {
        static FieldInfo f_someField = AccessTools.Field(typeof(Drillable), nameof(Drillable.drillDamage));
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);
            var errorLogSource = new ManualLogSource("DrillDamage - Error");
            BepInEx.Logging.Logger.Sources.Add(errorLogSource);
            var found = false;
            foreach (var instruction in instructions)
            {
                if (instruction.StoresField(f_someField))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Drillable_Transpiler_Patch).GetMethod(nameof(GetDamageForDrillable)));
                    found = true;
                }
                yield return instruction;
            }
            if (found is false)
                errorLogSource.LogError("Cannot find <Stdfld drillDamage> in Drillable");
        }
        /*[HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins)
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
        }*/

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
