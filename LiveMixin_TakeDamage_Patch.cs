using System;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;

namespace DrillDamage
{
    [HarmonyPatch(typeof(LiveMixin))]
    [HarmonyPatch("TakeDamage")]
    public class LiveMixin_TakeDamage_Patch
    {
        private static bool calling;

        [HarmonyPrefix]
        public static bool TakeDamage(LiveMixin __instance, DamageType type)
        {
            var liveMixerLog = new ManualLogSource("DrillDamage - Creatures");
            BepInEx.Logging.Logger.Sources.Add(liveMixerLog);
            try
            {
                if (calling)
                {
                    return true;
                }
                if (type != DamageType.Drill || Plugin.Options.affectcreatures == false) //vanila behaviour
                {
                    return true;
                }
                if (__instance.health > 0f) //mod behaviour
                {
                    calling = true;
                    __instance.TakeDamage(Plugin.Options.additionaldamagecreatures, default, type);
                    calling = false;
                }
            }
            catch (Exception ex)
            {
                liveMixerLog.LogError(ex.Message + "\r\n" + ex.StackTrace);
                throw;
            }
            return true;
        }
    }
}
