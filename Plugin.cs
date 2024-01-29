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
            VERSION = "1.3.4.0";
        #endregion

        public static Config Options { get; } = OptionsPanelHandler.RegisterModOptions<Config>();

        public void Awake()
        {
            Logger.LogInfo("DrillDamage - Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            var harmony = new Harmony(GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo("DrillDamage - Finished patching");
        }
    }
}
