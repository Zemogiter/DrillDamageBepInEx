using BepInEx.Configuration;
using DrillDamage;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using System;

[HarmonyPatch(typeof(Drillable), nameof(Drillable.OnDrill))]
internal class Drillable_Transpiler_Patch
{
    [HarmonyTranspiler]
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> cins)
    {
        var matcher = new CodeMatcher(cins);

        matcher.MatchForward(false, new CodeMatch(OpCodes.Ldc_R4, 5f));
        if (matcher.IsValid)
        {
            matcher.SetAndAdvance(OpCodes.Ldarg_0, null);
            matcher.Insert(new CodeInstruction(OpCodes.Call, typeof(Drillable_Transpiler_Patch).GetMethod(nameof(GetDamageForDrillable))));
            return matcher.InstructionEnumeration();
        }

        Console.WriteLine($"Failed to find matching instructions for {nameof(Drillable_Transpiler_Patch)}. Either another mod has already transpiled this or the games code has changed.");
        return cins;
    }

    private static Dictionary<TechType, ConfigEntry<int>> ConfigDictionary = new(){
        { TechType.DrillableSalt, Plugin.ConfigDrillableSaltDamage },
        { TechType.DrillableQuartz, Plugin.ConfigDrillableQuartzDamage },
        { TechType.DrillableCopper, Plugin.ConfigDrillableCopperDamage },
        { TechType.DrillableTitanium, Plugin.ConfigDrillableTitaniumDamage },
        { TechType.DrillableLead, Plugin.ConfigDrillableLeadDamage },
        { TechType.DrillableSilver, Plugin.ConfigDrillableSilverDamage },
        { TechType.DrillableDiamond, Plugin.ConfigDrillableDiamondDamage },
        { TechType.DrillableGold, Plugin.ConfigDrillableGoldDamage },
        { TechType.DrillableMagnetite, Plugin.ConfigDrillableMagnetiteDamage },
        { TechType.DrillableLithium, Plugin.ConfigDrillableLithiumDamage },
        { TechType.DrillableMercury, Plugin.ConfigDrillableMercuryDamage },
        { TechType.DrillableUranium, Plugin.ConfigDrillableUraniumDamage },
        { TechType.DrillableAluminiumOxide, Plugin.ConfigDrillableAluminiumOxideDamage },
        { TechType.DrillableNickel, Plugin.ConfigDrillableNickelDamage },
        { TechType.DrillableSulphur, Plugin.ConfigDrillableSulphurDamage },
        { TechType.DrillableKyanite, Plugin.ConfigDrillableKyaniteDamage }
    };

    public static float GetDamageForDrillable(Drillable drillable)
    {
        float newDamage;
        if (Plugin.ConfigVariableModeEnabled.Value == false)
        {
            newDamage = Plugin.ConfigAdditionalDamage.Value;
        }
        else
        {
            TechType key = drillable.GetDominantResourceType();
            Plugin.Log("The techType is = " + key, 2);
            var valueGet = ConfigDictionary.TryGetValue(key, out var config);
            Plugin.Log("Value is = " + config.Value, 2);
            newDamage = config.Value;
        }
        return newDamage;
    }
}
