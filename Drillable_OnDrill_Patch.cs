using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using static UnityEngine.UI.DefaultControls;

namespace DrillDamage
{
    public class Drillable_OnDrill_Patch
    {
        private MeshRenderer[] renderers;
        public Drillable.ResourceType[] resources;
        [HarmonyPrefix]
        public bool OnDrill(object __instance, Vector3 position)
        {
            try
            {
                Plugin.Log("DrillDamage.OnDrill " + __instance.GetType().FullName, 3);
                Traverse traverse = null;
                string fullName = __instance.GetType().FullName;
                traverse = (fullName == null || !(fullName == "ModdedArmsHelper.API.SeamothDrillable")) ? Traverse.Create(__instance).Field("health") : Traverse.Create(__instance).Field("drillable").Field("health");
                float num = 0f;
                for (int i = 0; i < traverse.GetValue<float[]>().Length; i++)
                {
                    num += traverse.GetValue<float[]>()[i];
                }
                if (num > 0f)
                {
                    int num2 = FindClosestMesh(position, out Vector3 center);
                    traverse.GetValue<float[]>()[num2] = Mathf.Max(1f, traverse.GetValue<float[]>()[num2] - Plugin.ConfigAdditionalDamage.Value);
                }
                if (Plugin.ConfigVariableModeEnabled.Value == true)
                {
                    Dictionary<TechType, int> ConfigDictionary = new()
                    {
                        { TechType.DrillableSalt, Plugin.ConfigDrillableSaltDamage.Value },
                        { TechType.DrillableQuartz, Plugin.ConfigDrillableQuartzDamage.Value },
                        { TechType.DrillableCopper, Plugin.ConfigDrillableCopperDamage.Value },
                        { TechType.DrillableTitanium, Plugin.ConfigDrillableTitaniumDamage.Value },
                        { TechType.DrillableLead, Plugin.ConfigDrillableLeadDamage.Value },
                        { TechType.DrillableSilver, Plugin.ConfigDrillableSilverDamage.Value },
                        { TechType.DrillableDiamond, Plugin.ConfigDrillableDiamondDamage.Value },
                        { TechType.DrillableGold, Plugin.ConfigDrillableGoldDamage.Value },
                        { TechType.DrillableMagnetite, Plugin.ConfigDrillableMagnetiteDamage.Value },
                        { TechType.DrillableLithium, Plugin.ConfigDrillableLithiumDamage.Value },
                        { TechType.DrillableMercury, Plugin.ConfigDrillableMercuryDamage.Value },
                        { TechType.DrillableUranium, Plugin.ConfigDrillableUraniumDamage.Value },
                        { TechType.DrillableAluminiumOxide, Plugin.ConfigDrillableAluminiumOxideDamage.Value },
                        { TechType.DrillableNickel, Plugin.ConfigDrillableNickelDamage.Value },
                        { TechType.DrillableSulphur, Plugin.ConfigDrillableSulphurDamage.Value },
                        { TechType.DrillableKyanite, Plugin.ConfigDrillableKyaniteDamage.Value }
                    };
                    int num2 = FindClosestMesh(position, out Vector3 center);
                    Plugin.Log("Result of FindClosestMesh = " + num2, 2);
                    Drillable.ResourceType resourceType;
                    TechType key = resourceType.techType;
                    Plugin.Log("The techType is = " + key, 2);
                    var valueGet = ConfigDictionary.TryGetValue(key, out int value);
                    Plugin.Log("Value is = " + value, 2);
                    traverse.GetValue<float[]>()[num2] = Mathf.Max(1f, traverse.GetValue<float[]>()[num2] - value);

                }
            }
            catch (Exception ex)
            {
                Plugin.Log(ex.Message + "\r\n" + ex.StackTrace, 0);
                throw;
            }
            return true;
        }
        private int FindClosestMesh(Vector3 position, out Vector3 center)
        {
            int result = 0;
            float num = float.PositiveInfinity;
            center = Vector3.zero;

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].gameObject.activeInHierarchy)
                {
                    Bounds encapsulatedAABB = UWE.Utils.GetEncapsulatedAABB(renderers[i].gameObject, -1);
                    float sqrMagnitude = (encapsulatedAABB.center - position).sqrMagnitude;
                    if (sqrMagnitude < num)
                    {
                        num = sqrMagnitude;
                        result = i;
                        center = encapsulatedAABB.center;
                        if (sqrMagnitude <= 0.5f)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}
