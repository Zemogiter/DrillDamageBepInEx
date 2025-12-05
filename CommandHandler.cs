using System;
using UnityEngine;

namespace DrillDamage
{
    internal class CommandHandler : MonoBehaviour
    {
        public void Awake()
        {
            DevConsole.RegisterConsoleCommand(this, "drillableGallery");
            DevConsole.RegisterConsoleCommand(this, "drilldamage_rebuild");
            DevConsole.RegisterConsoleCommand(this, "drilldamage_diag");
        }
        // Notification callback used to spawn a gallery of drillables when debug mode is enabled.
        public static void DrillableGallery(NotificationCenter.Notification n, TechType[] techTypes)
        {
            if (Config.Instance != null && Config.Instance.debugmode == true)
            {
                TechType[] spawnList = new TechType[]
                {
                    TechType.DrillableAluminiumOxide,
                    TechType.DrillableCopper,
                    TechType.DrillableDiamond,
                    TechType.DrillableGold,
                    TechType.DrillableKyanite,
                    TechType.DrillableLead,
                    TechType.DrillableLithium,
                    TechType.DrillableMagnetite,
                    TechType.DrillableMercury,
                    TechType.DrillableNickel,
                    TechType.DrillableQuartz,
                    TechType.DrillableSalt,
                    TechType.DrillableSilver,
                    TechType.DrillableSulphur,
                    TechType.DrillableTitanium,
                    TechType.DrillableUranium,
                    TechType.PrecursorIonCrystal
                };

                foreach (TechType techType in spawnList)
                {
                    ErrorMessage.AddMessage($"Spawning drillableGallery with {spawnList.Length} entities.");
                    DevConsole.SendConsoleCommand($"spawn {techType.AsString(true)}");
                }
            }
            else
            {
                ErrorMessage.AddDebug("Debug Mode is not enabled.");
            }
        }

        // console command: "drilldamage_rebuild" -- forces discovery now
        public void drilldamage_rebuild()
        {
            ErrorMessage.AddMessage("DrillDamage: forcing drillable discovery rebuild...");
            if (Config.Instance != null)
            {
                Config.RebuildDrillableListDeferred(force: true);
                ErrorMessage.AddMessage($"DrillDamage: rebuild triggered; found {Config.DrillableOreList?.Count ?? 0} entries.");
            }
            else
            {
                ErrorMessage.AddMessage("DrillDamage: config instance missing; cannot rebuild.");
            }
        }

        // console command: "drilldamage_diag" -- run discovery diagnostics and log candidates
        public void drilldamage_diag()
        {
            ErrorMessage.AddMessage("DrillDamage: running discovery diagnostics (log -> DrillDamage - Diagnostic).");
            try
            {
                Config.LogDiscoveryDiagnostics(300);
                ErrorMessage.AddMessage("DrillDamage: diagnostics completed. Check BepInEx log for 'DrillDamage - Diagnostic' entries.");
            }
            catch (Exception ex)
            {
                ErrorMessage.AddMessage($"DrillDamage: diagnostics failed: {ex.Message}");
            }
        }
    }
}
   