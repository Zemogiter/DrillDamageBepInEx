using System;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;

namespace DrillDamage
{
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
                liveMixerLog.LogInfo("DrillDamage.LiveMixin.TakeDamage");
                if (type != DamageType.Drill || !Plugin.Options.affectcreatures)
                {
                    return true;
                }
                liveMixerLog.LogInfo("DrillDamage.LiveMixin.TakeDamage Damage");
                if (__instance.health > 0f)
                {
                    calling = true;
                    //__instance.TakeDamage(Plugin.ConfigCreatureDamage.Value, default, type);
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
