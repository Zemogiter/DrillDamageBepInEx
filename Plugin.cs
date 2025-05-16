using System;
using System.Reflection;
using HarmonyLib;
using BepInEx;
using BepInEx.Configuration;
using Nautilus.Handlers;

namespace DrillDamage
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        internal const string
            MODNAME = "DrillDamage",
            AUTHOR = "russleeiv",
            GUID = "russleeiv.subnautica.drilldamage",
            VERSION = "1.3.5.0";
        #endregion

        //public static Config Options { get; set; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void Awake()
        {
            Logger.LogInfo("DrillDamage - Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            DrillDamage.Config.Register();
            //var harmony = new Harmony(GUID);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo("DrillDamage - Finished patching");
        }
    }
}
