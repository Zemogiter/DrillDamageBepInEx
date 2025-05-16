using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DrillDamage
{
    [HarmonyPatch(typeof(Drillable))]
    [HarmonyPatch("Start")]
    internal class VFDrillArmPatch
    {
        [HarmonyPostfix]
        public static void Patch(Drillable __instance)
        {
            var VFLogSource = new ManualLogSource("DrillDamage - VFArmDrill");
            Logger.Sources.Add(VFLogSource);
            var vfDrillArm = __instance.gameObject.GetComponent("VFDrillArm");

            if (vfDrillArm != null) // if vfDrillArm component is available on resource, processing the drill damage value, else do nothing
            {
                PropertyInfo drillDamage = vfDrillArm.GetType().GetProperty("drillDamage");

                //regular
                if (Config.Instance.affectsvehicleframeworkdrills == true && Config.Instance.variablemode == false)
                {
                    drillDamage.SetValue(vfDrillArm, Config.Instance.additionaldamageseamotharms);
                }
                //vanilla
                else if (Config.Instance.affectsvehicleframeworkdrills == true && Config.Instance.variablemode == false && Config.Instance.additionaldamageseamotharms == 1)
                {
                    drillDamage.SetValue(vfDrillArm, 1);
                }
                //variable mode enabled
                else if (Config.Instance.affectsvehicleframeworkdrills == true && Config.Instance.variablemode == true)
                {
                    TechType key = __instance.GetDominantResourceType();
                    if (Config.Instance.debugmode == true)
                    {
                        VFLogSource.LogInfo("The techType is = " + key);
                    }
                    var valueGet = Config.Instance.drillableOreList.TryGetValue(key.AsString(), out int value);
                    if (Config.Instance.debugmode == true)
                    {
                        VFLogSource.LogInfo("Was the value obtained? " + valueGet + ". If so, the value is = " + value);
                    }
                    drillDamage.SetValue(vfDrillArm, value);
                }
            }
        }
    }
}
