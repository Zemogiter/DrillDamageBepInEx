using System;
using HarmonyLib;
using UnityEngine;

namespace DrillDamage;

public class LiveMixin_TakeDamage_Patch
{
    private static bool calling;

    [HarmonyPrefix]
    public static bool TakeDamage(LiveMixin __instance, DamageType type)
    {
        try
        {
            if (calling)
            {
                return true;
            }
            Plugin.Log("DrillDamage.LiveMixin.TakeDamage", 3);
            if (type != DamageType.Drill || !(bool)Plugin.ConfigAffectCreatures.BoxedValue)
            {
                return true;
            }
            Plugin.Log("DrillDamage.LiveMixin.TakeDamage Damage", 3);
            if (__instance.health > 0f)
            {
                calling = true;
                __instance.TakeDamage((int)Plugin.ConfigAdditionalDamage.BoxedValue, default, type);
                calling = false;
            }
        }
        catch (Exception ex)
        {
            Plugin.Log(ex.Message + "\r\n" + ex.StackTrace, 0);
            throw;
        }
        return true;
    }
}
