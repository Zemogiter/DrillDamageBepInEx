using System;
using System.Collections;
using System.Linq;
using Nautilus.Commands;
using UnityEngine;
using UWE;
using System.Text;
using System.Threading.Tasks;
using static HandReticle;

namespace DrillDamage
{
    internal class CommandHandler : MonoBehaviour
    {
        public void Awake()
        {
            DevConsole.RegisterConsoleCommand(this, "drillableGallery");
        }

        public static void DrillableGallery(NotificationCenter.Notification n)
        {
            if(Plugin.Options.debugmode == true)
            {
                TechType[] techTypes = new TechType[15];
                techTypes[0] = TechType.DrillableAluminiumOxide;
                techTypes[1] = TechType.DrillableCopper;
                techTypes[2] = TechType.DrillableDiamond;
                techTypes[3] = TechType.DrillableGold;
                techTypes[4] = TechType.DrillableKyanite;
                techTypes[5] = TechType.DrillableLead;
                techTypes[6] = TechType.DrillableLithium;
                techTypes[7] = TechType.DrillableMagnetite;
                techTypes[8] = TechType.DrillableMercury;
                techTypes[9] = TechType.DrillableNickel;
                techTypes[10] = TechType.DrillableQuartz;
                techTypes[11] = TechType.DrillableSalt;
                techTypes[12] = TechType.DrillableSilver;
                techTypes[13] = TechType.DrillableSulphur;
                techTypes[14] = TechType.DrillableTitanium;
                techTypes[15] = TechType.DrillableUranium;

                foreach (TechType techType in techTypes)
                {
                    ErrorMessage.AddMessage($"Spawning drillableGallery with {techTypes.Length} entities.");
                    DevConsole.SendConsoleCommand($"item {techType.AsString(true)}");
                }
            }
            else
            {
                ErrorMessage.AddDebug("Debug Mode is not enabled. Press F5 (default) to enter BepInEx configuration and go to " + Plugin.MODNAME + " section");
            } 
        }
    }
}
