using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Logging;

namespace DrillDamage
{
    [HarmonyPatch(typeof(Drillable), "OnDrill")]
    public class Drillable_Transpiler_Patch
    {
        [HarmonyTranspiler]
        [HarmonyDebug]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins)
        {
            var errorLogSource = new ManualLogSource("DrillDamage - Error");
            BepInEx.Logging.Logger.Sources.Add(errorLogSource);
            CodeMatcher matcher = new(cins);
            matcher.MatchForward(true, new CodeMatch(OpCodes.Ldc_R4, 5f));
            if (matcher.IsValid)
            {
                matcher.SetAndAdvance(OpCodes.Ldarg_0, null);
                matcher.ThrowIfInvalid("ArgumentOutOfRangeException, you need to check the code. Details: " + matcher.IsValid + matcher.Opcode + matcher.Operand);
                matcher.Insert(new CodeInstruction(OpCodes.Call, typeof(Drillable_Transpiler_Patch).GetMethod(nameof(GetDamageForDrillable))));
                return matcher.InstructionEnumeration();
            }
            else
            {
                errorLogSource.LogError($"Failed to find matching instructions. Either another mod has already transpiled this or the game's/Unity's code has changed.");
            }
            return cins;
            
        }

        public static float GetDamageForDrillable(Drillable drillable)
        {
            var exosuitLogSource = new ManualLogSource("DrillDamage - Prawn Suit");
            BepInEx.Logging.Logger.Sources.Add(exosuitLogSource);
            float newDamage;
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
    }
}
