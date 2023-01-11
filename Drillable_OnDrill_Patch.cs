using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace DrillDamage;

public static class Drillable_OnDrill_Patch
{
    [HarmonyPrefix]
    public static bool OnDrill(object __instance, Vector3 position)
    {
        try
        {
            Plugin.Log("DrillDamage.OnDrill " + __instance.GetType().FullName, 3);
            Traverse traverse = null;
            string fullName = __instance.GetType().FullName;
            traverse = ((fullName == null || !(fullName == "SeamothArms.SeamothDrillable")) ? Traverse.Create(__instance).Field("health") : Traverse.Create(__instance).Field("drillable").Field("health"));
            float num = 0f;
            for (int i = 0; i < traverse.GetValue<float[]>().Length; i++)
            {
                num += traverse.GetValue<float[]>()[i];
            }
            if (num > 0f)
            {
                int num2 = FindClosestMesh(__instance, position);
                traverse.GetValue<float[]>()[num2] = Mathf.Max(1f, traverse.GetValue<float[]>()[num2] - (int)Plugin.ConfigAdditionalDamage.BoxedValue);
            }
        }
        catch (Exception ex)
        {
            Plugin.Log(ex.Message + "\r\n" + ex.StackTrace, 0);
            throw;
        }
        return true;
    }

    private static int FindClosestMesh(object __instance, Vector3 position)
    {
        Vector3 zero = Vector3.zero;
        return (int)__instance.GetType().InvokeMember("FindClosestMesh", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, __instance, new object[2] { position, zero });
    }
}
